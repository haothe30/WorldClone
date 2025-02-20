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
        Debug.LogError("==================== resolution:" + (float)Screen.width + ":" + Screen.height + ":" + checkResolution);
        if (checkResolution >= 2048f / 2732) // >= 0.7496... man hinh ipad 
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                    canvasScaler.matchWidthOrHeight = 0f;
                else
                    canvasScaler.matchWidthOrHeight = 0.35f;
            }

            if (calculate)
            {
                Camera.main.orthographicSize = 4.9f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 2f)*/;
                DataParamManager.percentDown = 2f;
                //Camera.main.orthographicSize = 7.6f;
            }
            Debug.LogError("===========>= 855/641 || 4:3 || 2732/2048 || 837/628");
        }
        else if (checkResolution >= 480f / 800 && checkResolution < 2048f / 2732) // >= 0.6 && < 0.7496
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                    canvasScaler.matchWidthOrHeight = 0f;

                else
                {
                    canvasScaler.matchWidthOrHeight = 0.5f;
                }
            }
            if (calculate)
            {
                Camera.main.orthographicSize = 4;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 3f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("=========== 800/480");
        }
        else if (checkResolution >= 750f / 1334 && checkResolution < 480f / 800)  // >=0.5622 && < 0.6
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.5f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 3f)*/;
                DataParamManager.percentDown = 2f;
                Camera.main.orthographicSize = 4f;
            }
            Debug.LogError("=========== 1334/750f || 1280/720 || 1920/1080 || 2560/1440 || 16/9 || 855/481");
        }
        else if (checkResolution >= 1080f / 2160 && checkResolution < 750f / 1334) // >=0.5 && < 0.5622
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                {
                    canvasScaler.matchWidthOrHeight = 1f;
                }
                else
                {
                    canvasScaler.matchWidthOrHeight = 0.55f;
                }
            }
            if (calculate)
            {
                Camera.main.orthographicSize = 4f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("=========== 2160:1080 || 18:9");
        }
        else if (checkResolution >= 1440f / 2960 && checkResolution < 1080f / 2160) //>= 0.4864 &&  < 0.5
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                {
                    canvasScaler.matchWidthOrHeight = 0.95f;
                }
                else
                {
                    canvasScaler.matchWidthOrHeight = 0.55f;
                }
            }
            if (calculate)
            {
                Camera.main.orthographicSize = 4f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("=========== 2960:1440");
        }
        else if (checkResolution >= 720f / 1520 && checkResolution < 1440f / 2960) //>= 0.4736 && < 0.4864
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                {
                    canvasScaler.matchWidthOrHeight = 0.35f;
                }
                else
                {
                    canvasScaler.matchWidthOrHeight = 0.55f;
                }
            }
            if (calculate)
            {
                Camera.main.orthographicSize = 4f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4.5f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("=========== 1520:720");
        }
        else if (checkResolution >= 1125f / 2436 && checkResolution < 720f / 1520) // >= 0.461 && < 0.4736
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                {
                    canvasScaler.matchWidthOrHeight = 0.35f;
                }
                else
                {
                    canvasScaler.matchWidthOrHeight = 0.55f;
                }
            }
            if (calculate)
            {
                Camera.main.orthographicSize = 3.9f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 5f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("===========  2436:1125");
        }
        else if (checkResolution >= 1080f / 2400 && checkResolution < 1125f / 2436) // >= 0.45 && < 0.461
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                    canvasScaler.matchWidthOrHeight = 0.4f;

                else
                    canvasScaler.matchWidthOrHeight = 0.6f;
            }
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 5.5f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("=========== 2400:1080");
        }
        else if (checkResolution < 1080f / 2400) // < 0.45
        {
            if (canvasScaler != null)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                    canvasScaler.matchWidthOrHeight = 1f;
                else
                    canvasScaler.matchWidthOrHeight = 0.6f;
            }
            if (calculate)
            {
                //Camera.main.orthographicSize = 8.2f;
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 8f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("=========== <= 3400:1440");
        }
        else
        {
            if (canvasScaler != null)
                canvasScaler.matchWidthOrHeight = 0.5f;
            if (calculate)
            {
                DataParamManager.percentTakePicture = Screen.width /*- (Screen.width / 4.5f)*/;
                DataParamManager.percentDown = 2f;
            }
            Debug.LogError("============ loai man hinh khac chua ro:" + (float)Screen.width + ":" + Screen.height + ":" + checkResolution);
        }
    }
}
