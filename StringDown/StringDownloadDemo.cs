
using System;
using System.Globalization;
using System.Security.Policy;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using VRC.SDK3.Image;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

//using UnityEditor;

public class StringDownloadDemo : UdonSharpBehaviour
{
    public VRCUrl[] stringUrls;


    void Start()
    {
        for (int i = 0; i < stringUrls.Length; i++)
        {
            //Debug.Log("String URL: " + stringUrls[i].Get());
            VRCStringDownloader.LoadUrl(stringUrls[i], (IUdonEventReceiver)this);
        }
    }

    private void Update()
    {
        ////_imageDownloader.DownloadImage(imageUrls[_loadedIndex], renderer.material, _udonEventReceiver, rgbInfo);
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    for (int i = 0; i < stringUrls.Length; i++)
        //    {
        //        //Debug.Log("String URL: " + stringUrls[i].Get());
        //        VRCStringDownloader.LoadUrl(stringUrls[i], (IUdonEventReceiver)this);
        //    }
        //}
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        
        Debug.Log("String Downloaded: " + result.Result);
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {

        Debug.Log("String Downloaded: " + result.Result);
    }
}
