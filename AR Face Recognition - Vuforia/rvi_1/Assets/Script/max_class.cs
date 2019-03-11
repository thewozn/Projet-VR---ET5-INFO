using System.Collections;
using System.Collections.Generic;
using System;
using System.Net; 
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; 


public class max_class : MonoBehaviour
{
    // Start is called before the first frame update
    private TcpClient client;
    private NetworkStream mystream;
    private IPAddress ipadresse;
    private static readonly HttpClient client1 = new HttpClient();

    public string Yolo()
    {
        return "MAx Mbourra";
    }

    public void ConnexionServer()
    {
        try
        {
            //172.18.66.230
            //172.18.68.232
            //client = new TcpClient("192.168.1.40", 4444);
            client = new TcpClient("172.18.67.232", 4444);
            NetworkStream mystream = client.GetStream();
            Byte[] data = Encoding.ASCII.GetBytes("bonjour maman c'est max ");
            mystream.Write(data, 0, data.Length);
            Debug.Log("sent : 0");

            data = new Byte[1024];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = mystream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            GameObject button = GameObject.Find("Button");
           // Debug.Log("Received: {0}", responseData);



            mystream.Close();
            client.Close(); 
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString()); 
        }
        
        
    }

    public void cacheObjet()
    {
      
    }

    public String PrendrePhoto()
    {
        GameObject button = GameObject.Find("Button");
        //GameObject prendre = GameObject.Find("notification");
        GameObject toto = GameObject.Find("toto");
        button.SetActive(false);
       // prendre.SetActive(false);
        String sauvegarde = "Assets/photo/max_photo.png";
        ScreenCapture.CaptureScreenshot("Assets/photo/max_photo.png",ScreenCapture.StereoScreenCaptureMode.BothEyes);
        button.SetActive(true);
        toto.SetActive(true);
        return sauvegarde; 
        
    }

    public string PrendrePhoto2()
    {
        GameObject button = GameObject.Find("Button");
        //GameObject prendre = GameObject.Find("notification");
        GameObject toto = GameObject.Find("toto");
        button.SetActive(false);
        // prendre.SetActive(false);
        String sauvegarde = "max_photo_temp.png";
        ScreenCapture.CaptureScreenshot("max_photo_temp.png", ScreenCapture.StereoScreenCaptureMode.BothEyes);
        button.SetActive(true);
        toto.SetActive(true);
        return sauvegarde;
    }

    public static async Task  RunAsync(string nomphoto)
    {

        // Chargement et lecture du fichier (ce qu'il y aura probablement � adapter)

        Debug.Log("je rentre dans la fonction"); 
        byte[] fileBytes = File.ReadAllBytes(nomphoto);

        // Pour les application UWP, ce code peut th�oriquement marcher
        // L'id�e est de sauvegarder toutes les X frames une image et de la sauvegarder pour l'envoyer
        /* Windows.Storage.StorageFolder storageFolder = Windows.Storage.KnownFolders.CameraRoll;
         *  Windows.Storage.StorageFile file = await storageFolder.GetFileAsync("myfile"); 
         */

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://192.168.137.1:18001/");
        MultipartFormDataContent form = new MultipartFormDataContent();

        // Cr�ation du formulaire
        HttpContent content = new StringContent("file");
        form.Add(content, "file");
        content = new StreamContent(new MemoryStream(fileBytes));
        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "file",
            FileName = "face.jpg"
        };

        

        form.Add(content);
        Debug.Log("contenu ajouter"); 
        String info = null;
        try
        {
            var response = await client.PostAsync("emotions_rec", form);
            info = response.Content.ReadAsStringAsync().Result;
            //Emotion server_reponse = JsonUtility.FromJson<Emotion>(info);
            Debug.Log(info);
            //Console.WriteLine(info);
            //Console.WriteLine("document recu");
            Debug.Log("document recu "); 
            //TextMesh test = GameObject.Find("reponse_emotion").gameObject.GetComponent<TextMesh>();  
            //test.text="amour"; 
             
        }
        catch (Exception e)
        {
            //Console.WriteLine(e);
            Debug.Log(e); 
        }

    }

    public void sendInfo()
    {
        
        byte[] fileBytes = File.ReadAllBytes("Assets/photo/max_photo.png");
        WWWForm yolo = new WWWForm();
        yolo.AddBinaryData("max_data", fileBytes); 
        

        UnityWebRequest www = UnityWebRequest.Post("http://192.168.137.1:18001/emotions_rec",yolo);

        //yield return  
        www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            Text txt = GameObject.Find("toto").GetComponent<Text>();
            Byte[] po = www.downloadHandler.data;
            if(po.Length==0)
            {
                txt.text = "perdu"; 
            }
            else { txt.text = po.ToString(); }
        }

        
    }

   public string sendInfo1()
    {
        Debug.Log("Sending");
        string facerec_url = "http://192.168.1.10:18001/";
        byte[] image = new byte[1000];
        WWW www = new WWW(facerec_url, image);
       // yield return www;
        string responseString = www.text;
        Debug.Log(responseString);
        return responseString; 
    }


    public IEnumerator Upload(string filephoto)
    {
        byte[] fileBytes = File.ReadAllBytes(filephoto);
        WWWForm yolo = new WWWForm();
        yolo.AddBinaryData("file", fileBytes);

        string[] addresip = { "http://", GameObject.Find("AdressIp").GetComponent<Text>().text, ":18001/emotions_rec" };
        UnityWebRequest www = UnityWebRequest.Post(string.Concat(addresip), yolo);
        // UnityWebRequest www = UnityWebRequest.Post("http://192.168.137.1:18001/emotions_rec", yolo);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            Text txt = GameObject.Find("toto").GetComponent<Text>();
            Byte[] po = www.downloadHandler.data;
            if (po.Length == 0)
            {
                txt.text = "perdu";
            }
            else
            {
                JSONObject j = new JSONObject(System.Text.Encoding.UTF8.GetString(po));
                txt.text = j["emotion"].str; 
                //txt.text = System.Text.Encoding.UTF8.GetString(po);
            }
        }
    }





}
