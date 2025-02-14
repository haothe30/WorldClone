using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CheckResolution : MonoBehaviour
{
    float checkResolution;
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] bool calculate;
    // [SerializeField] SkeletonGraphic gateSA;
    private void Awake()
    {
        Resolution();
    }

    public void Resolution()
    {
        checkResolution = ((float)Screen.height / Screen.width); // man hinh ngang
        //Debug.LogError("==================== resolution:" + (float)Screen.width + ":" + Screen.height + ":" + checkResolution);
        if (checkResolution >= 2048f / 2732) // >= 0.7496... man hinh ipad 
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 1f;

            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 2f)*/;
                DataParamManager.percentDown = 2f;
                //Camera.main.orthographicSize = 7.6f;
            }
            // Debug.LogError("===========>= 641/855 || 3:4 || 2048/2732 || 628/837");
        }
        else if (checkResolution >= 480f / 800 && checkResolution < 2048f / 2732) // >= 0.6 && < 0.7496
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.5f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 3f)*/;
                DataParamManager.percentDown = 2f;
                //Camera.main.orthographicSize = 7.6f;
            }
            //    Debug.LogError("=========== 480/800");
        }
        else if (checkResolution >= 750f / 1334 && checkResolution < 480f / 800)  // >=0.5622 && < 0.6
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.5f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 3f)*/;
                DataParamManager.percentDown = 2f;
                //Camera.main.orthographicSize = 7.6f;
            }
            // Debug.LogError("===========750f / 1334 || 720/1280 || 1080/1920 || 1440 / 2560 || 9/16 || 481/855");
        }
        else if (checkResolution >= 1080f / 2160 && checkResolution < 750f / 1334) // >=0.5 && < 0.5622
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.1f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4f)*/;
                DataParamManager.percentDown = 2f;
                Camera.main.orthographicSize = 7.2f;
            }
            // Debug.LogError("=========== 1080:2160 || 9:18");
        }
        else if (checkResolution >= 1440f / 2960 && checkResolution < 1080f / 2160) //>= 0.4864 &&  < 0.5
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.1f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4f)*/;
                DataParamManager.percentDown = 2f;
                Camera.main.orthographicSize = 7.2f;
            }
            //  Debug.LogError("=========== 1440:2960");
        }
        else if (checkResolution >= 720f / 1520 && checkResolution < 1440f / 2960) //>= 0.4736 && < 0.4864
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.1f;
            if (calculate)
            {
                Camera.main.orthographicSize = 7.2f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4.5f)*/;
                DataParamManager.percentDown = 2f;
            }
            // Debug.LogError("=========== 720:1520");
        }
        else if (checkResolution >= 1125f / 2436 && checkResolution < 720f / 1520) // >= 0.461 && < 0.4736
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 5f)*/;
                DataParamManager.percentDown = 2f;
                Camera.main.orthographicSize = 7.2f;
            }
            //Debug.LogError("=========== IP 11 1125/2436");
        }
        else if (checkResolution >= 1080f / 2400 && checkResolution < 1125f / 2436) // >= 0.45 && < 0.461
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 5.5f)*/;
                DataParamManager.percentDown = 2f;
                Camera.main.orthographicSize = 8f;
            }
            //  Debug.LogError("=========== 1080:2400");
        }
        else if (checkResolution < 1080f / 2400) // < 0.45
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0f;
            if (calculate)
            {
                Camera.main.orthographicSize = 8.2f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 8f)*/;
                DataParamManager.percentDown = 2f;
            }
            //  Debug.LogError("=========== <= 1440:3400");
        }
        else
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.5f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4.5f)*/;
                DataParamManager.percentDown = 2f;
                Camera.main.orthographicSize = 7.2f;
            }
            // Debug.LogError("============ loai man hinh khac chua ro:" + (float)Screen.width + ":" + Screen.height + ":" + checkResolution);
        }
    }
}
