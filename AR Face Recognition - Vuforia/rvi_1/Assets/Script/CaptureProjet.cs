using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.Networking;
using Vuforia; 


public class CaptureProjet : MonoBehaviour
{
    // Start is called before the first frame update
    private max_class test;
    private string tmp;
    private Button lancer;
    private InputField ipchamp;
    private Text txtip;
    private Text saveIp;
    private bool record;
    private int compte_frame;
    private Button arret;
    private string[] affiche;
    private int x = 5;
    private int y = 10;
    private Rect mon_rec;
    public GameObject frame; 
    void Start()
    {
        //test = new max_class();    
        //StartCoroutine(Upload()); 
        lancer = GameObject.Find("buttonIp").GetComponent<Button>();
        ipchamp = GameObject.Find("champIp").GetComponent<InputField>();
        txtip = GameObject.Find("TextIp").GetComponent<Text>();
        saveIp = GameObject.Find("AdressIp").GetComponent<Text>();
        arret = GameObject.Find("Button").GetComponent<Button>(); 
        record = false;
        arret.gameObject.SetActive(false); 
        compte_frame = 0;
        //GameObject.Find("toto").SetActive(false); 
        lancer.onClick.AddListener(OnButton);
        arret.onClick.AddListener(OnArret); 
        

    }

    IEnumerator Upload()
    {
        byte[] fileBytes = File.ReadAllBytes("Assets/photo/max_photo.png");
        WWWForm yolo = new WWWForm();
        yolo.AddBinaryData("max_data", fileBytes);


        UnityWebRequest www = UnityWebRequest.Post("http://192.168.137.1:18001/emotions_rec", yolo);

        yield return  www.SendWebRequest();

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
            else {
                txt.text = System.Text.Encoding.UTF8.GetString(po);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        /*tmp = test.PrendrePhoto();
        test.RunAsync(tmp).GetAwaiter().GetResult();*/
        compte_frame++; 
        if(record==true)
        {
            if(compte_frame%100==0)
            {
                StartCoroutine(TakeScreenshotAndSave());
            }

           

        }
    }


    public void setCamera(float x,float y,float heigt,float wight)
    {
        GameObject faceBounds = (GameObject)Instantiate(frame);
        faceBounds.transform.position = new Vector2(x, y); 
        
       
    }

    private void OnButton()
    {
        if(ipchamp.text.Length!=0)
        {
            saveIp.text = ipchamp.text;
            txtip.gameObject.SetActive(false);
            ipchamp.gameObject.SetActive(false);
            lancer.gameObject.SetActive(false);
            arret.gameObject.SetActive(true);
            record = true; 
        }
    }

    private void OnArret()
    {
        if(record)
        {
            record = false;
            arret.transform.Find("Text").GetComponent<Text>().text="Lancer";
            Debug.Log("Arret du record"); 
        }
        else
        {
            record = true;
            arret.transform.Find("Text").GetComponent<Text>().text = "Arrêter";
            Debug.Log("Relance du  record "); 
        }
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        //Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "My img {0}.png"));
        StartCoroutine(sendImage(ss.EncodeToPNG()));
        // To avoid memory leaks
        Destroy(ss);
    }

    IEnumerator sendImage(Byte[] infobyte)
    {
        
        WWWForm yolo = new WWWForm();
        yolo.AddBinaryData("file", infobyte);


        string[] addresip = { "http://",saveIp.text,"/emotions_rec" };
        UnityWebRequest www = UnityWebRequest.Post(string.Concat(addresip), yolo);
        //UnityWebRequest www = UnityWebRequest.Post("http://faceapi.us.to/", yolo);

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
                string tmp = j["emotion"].str;
                //string[] separator = new string[] { "\n" };  
                //affiche = tmp.Split(separator, StringSplitOptions.None); 
                Debug.Log(j);
                //txt.text = j["emotion"].str;
                //txt.text = System.Text.Encoding.UTF8.GetString(po);
                //txt.text = string.Concat(affiche);
                tmp = tmp.Replace("\\n", "\r\n");
                tmp = tmp.Replace("\\t", " ");
                txt.text = tmp;
                txt.transform.position = new Vector2(j["x"].f,j["y"].f);
                mon_rec = new Rect(j["x"].f, j["y"].f, j["w"].f, j["h"].f); 
            }
        }

    }

    private void OnGUI()
    {
        if (record)
        {
            /* int x = 5;
             int y = 30;
             int with = 250;
             int height = 150; 
             for(int i = 0; i< affiche.Length; i++)
             {
                 GUI.Label(new Rect(x, y, with, height),affiche[i]);
                 y += 30; 
             }*/
            /*GUI.Label(new Rect(2, 10, 150, 100), "Press S for coucou");
            GUI.Label(new Rect(2, 30, 150, 100), "Press B for client");*/
            //GUI.Label(mon_rec,);



        }
    }



}
