using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI; 

public class llancer_connection : MonoBehaviour
{
    // Start is called before the first frame update
    max_class max = new max_class(); 
    public void ditBonjour()
    {
        Text txt = transform.Find("Text").GetComponent<Text>();
        txt.text = max.Yolo();
        max.ConnexionServer(); 
    }

    public void test()
    {
        //string path = max.PrendrePhoto();
        //max_class.RunAsync("Assets/photo/face.jpg").GetAwaiter().GetResult();
        // max_class.RunAsync("Assets/photo/face.jpg").GetAwaiter().GetResult(); 
        //Text txt = GameObject.Find("toto").GetComponent<Text>();
        //txt.text=max.sendInfo();
        //txt.text = max.sendInfo1();
        //max.sendInfo();
        //max_class.RunAsync("Assets/photo/max_photo.png").GetAwaiter().GetResult(); 

         StartCoroutine(max.Upload(max.PrendrePhoto2())); 
    }
}
