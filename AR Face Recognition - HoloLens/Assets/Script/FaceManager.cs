using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System;
using SharpConfig;
using UnityEngine.UI;
using UnityEngine.Networking;


public class FaceManager : MonoBehaviour {

    // On considère la classe comme un Singleton
    public static FaceManager Instance { get; private set; }

    Vector3 camera_pos_;            // (x, y, z)
    Quaternion camera_rot_;         // Rotations autour des axes
    Resolution camera_resolution_;  // Résolution de la caméra

    // Assets utilisés
    public GameObject textPrefab;   // Contenu textuel
    public GameObject frame;        // Encadrement du visage
    public GameObject status;       // Texte d'état

    UnityEngine.XR.WSA.Input.GestureRecognizer gesture_recognizer_;
    UnityEngine.XR.WSA.WebCam.PhotoCapture photo_capture_ = null;

    bool _processing = false;

    // Lien de notre serveur Python ainsi que de l'API de requêtes.
    // Attention toutefois, il peut être nécessaire de changer l'adresse de Server0 !
    public string server0 = "http://192.168.137.1:18001/";
    public string server1 = "http://faceapi.us.to/";

    // Quand une photo est prise
    void OnPhotoCaptureCreated(UnityEngine.XR.WSA.WebCam.PhotoCapture captureObject)
    {
        photo_capture_ = captureObject;
        camera_resolution_ = UnityEngine.XR.WSA.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        UnityEngine.XR.WSA.WebCam.CameraParameters c = new UnityEngine.XR.WSA.WebCam.CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = camera_resolution_.width;
        c.cameraResolutionHeight = camera_resolution_.height;
        c.pixelFormat = UnityEngine.XR.WSA.WebCam.CapturePixelFormat.PNG;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    // Démarrage du mmode photo
    private void OnPhotoModeStarted(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Camera ready");
            photo_capture_.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
            _processing = false;
        }
    }

    // Capture de l'image
    void OnCapturedPhotoToMemory(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result, UnityEngine.XR.WSA.WebCam.PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            Debug.Log("photo captured");
            List<byte> imageBufferList = new List<byte>();
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

            var cameraToWorldMatrix = new Matrix4x4();
            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);

            camera_pos_ = cameraToWorldMatrix.MultiplyPoint3x4(new Vector3(0, 0, -1));
            camera_rot_ = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

            Matrix4x4 projectionMatrix;
            photoCaptureFrame.TryGetProjectionMatrix(Camera.main.nearClipPlane, Camera.main.farClipPlane, out projectionMatrix);
            Matrix4x4 pixelToCameraMatrix = projectionMatrix.inverse;

            status.GetComponent<TextMesh>().text = "photo captured, processing...";
            status.transform.position = camera_pos_;
            status.transform.rotation = camera_rot_;

            StartCoroutine(PostToServer(imageBufferList.ToArray(), cameraToWorldMatrix, pixelToCameraMatrix));
        }
        photo_capture_.StopPhotoModeAsync(OnStoppedPhotoMode);
    }


    void OnStoppedPhotoMode(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        photo_capture_.Dispose();
        photo_capture_ = null;
        _processing = false;
    }



    IEnumerator<object> PostToServer(byte[] imageData, Matrix4x4 cameraToWorldMatrix, Matrix4x4 pixelToCameraMatrix)
    {
        string emotions = "Emotions:\n";
        WWW www0 = new WWW(server0, imageData);
        yield return www0;
        string responseString0 = www0.text;
        Debug.Log(responseString0);
        JSONObject j0 = new JSONObject(responseString0);

        var existing = GameObject.FindGameObjectsWithTag("faceText");

        foreach (var go in existing)
            Destroy(go);

        existing = GameObject.FindGameObjectsWithTag("faceBounds");

        foreach (var go in existing)
            Destroy(go);

        if (j0.list.Count == 0)
        {
            status.GetComponent<TextMesh>().text = "Cannot find faces ...";
            yield break;
        }

        else
            status.SetActive(false);

        TextMesh txtMesh;

        foreach (var result in j0.list)
        {
            // Extraction des données reçues
            GameObject txtObject = (GameObject)Instantiate(textPrefab);
            txtMesh = txtObject.GetComponent<TextMesh>();
            float y = -(result["y"].f / camera_resolution_.height - .5f);
            float x =  result["x"].f / camera_resolution_.width - .5f;
            float w =  result["w"].f / camera_resolution_.width;
            float h =  result["h"].f / camera_resolution_.height;

            // Placement de l'objet
            GameObject faceBounds = (GameObject)Instantiate(frame);
            faceBounds.transform.position = cameraToWorldMatrix.MultiplyPoint3x4(pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(x + w / 2, y, 0)));
            faceBounds.transform.rotation = camera_rot_;
            Vector3 scale = pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(w, h, 0));
            scale.z = .1f;
            faceBounds.transform.localScale = scale / 3;
            faceBounds.tag = "faceBounds";
            Debug.Log(string.Format("{0},{1} translates to {2},{3}", x, y, faceBounds.transform.position.x, faceBounds.transform.position.y));

            Vector3 origin = cameraToWorldMatrix.MultiplyPoint3x4(pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(x + w + .1f, y, 0)));
            txtObject.transform.position = origin;
            txtObject.transform.rotation = camera_rot_;
            txtObject.tag = "faceText";

            emotions = result["emotion"].str;
    
            // Décommenter cette partie pour avoir des informations supplémentaires
            /*WWW www1 = new WWW(server1, imageData);
            yield return www1;
            string responseString1 = www1.text;
            Debug.Log(responseString1);
            JSONObject j1 = new JSONObject(responseString1);

            string infos = "";
            foreach (var result1 in j1.list)
            {
                infos = "INFOS\n";
                infos += "Gender: " + result["fpp"]["attributes"]["gender"]["value"].str + "\n";
                infos += "Age:    " + result["fpp"]["attributes"]["age"]["value"].f + "\n";
            }

            txtMesh.text = infos + emotions;*/
            txtMesh.text = emotions;
        }

    }


    // Use this for initialization
    void Start () {

        Instance = this;
        
        // Set up a GestureRecognizer to detect Select gestures.
        gesture_recognizer_ = new UnityEngine.XR.WSA.Input.GestureRecognizer();
        gesture_recognizer_.TappedEvent += (source, tapCount, ray) =>
        {
            if (!_processing)
            {
                _processing = true;
                status.GetComponent<TextMesh>().text = "Photo taken !";
                status.SetActive(true);
                UnityEngine.XR.WSA.WebCam.PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
            }
            else
            {
                status.GetComponent<TextMesh>().text = "Please wait...";
                status.SetActive(true);
            }
        };
        gesture_recognizer_.StartCapturingGestures();
        status.GetComponent<TextMesh>().text = "Photo taken !";
        _processing = true;

        UnityEngine.XR.WSA.WebCam.PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        // Décommenter la partie suivante pour une execution en temps réel.
        // InvokeRepeating("OnPhotoCaptureCreated", 5, 2);
    }
	

}
