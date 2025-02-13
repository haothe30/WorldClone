using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif

using UnityEngine.SceneManagement;
using TMPro;

public class CheckUpdateGame : MonoBehaviour
{
    public static CheckUpdateGame instance;
#if UNITY_ANDROID
    private AppUpdateManager appUpdateManager;
#endif
    [SerializeField] string nameSceneNext;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartFunc();
    }
    public void StartFunc()
    {

#if !UNITY_EDITOR && !UNITY_IOS
            appUpdateManager = new AppUpdateManager();
            StartCoroutine(CheckForUpdate());
#else
             SceneManager.LoadScene(nameSceneNext);
#endif     
    }
#if UNITY_ANDROID
    IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            Debug.LogError("========= info update:" + appUpdateInfoResult.ToString());
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                Debug.LogError("===================== co ban cap nhat moi");
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: true);
                StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, appUpdateOptions));
            }
            else
            {
                Debug.LogError("===================== ko co ban cap nhat moi nen load vao scene tiep theo luon");
                SceneManager.LoadScene(nameSceneNext);
            }
        }
        else
        {
            Debug.LogError("===================== loi check update");
            SceneManager.LoadScene(nameSceneNext);
        }
    }
    IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfoOp_i, AppUpdateOptions appUpdateOptions_i)
    {
        // Creates an AppUpdateRequest that can be used to monitor the
        // requested in-app update flow.
        var startUpdateRequest = appUpdateManager.StartUpdate(
          // The result returned by PlayAsyncOperation.GetResult().
          appUpdateInfoOp_i,
          // The AppUpdateOptions created defining the requested in-app update
          // and its parameters.
          appUpdateOptions_i);
        yield return startUpdateRequest;

        Debug.LogError("============== check update complete");

        // If the update completes successfully, then the app restarts and this line
        // is never reached. If this line is reached, then handle the failure (for
        // example, by logging result.Error or by displaying a message to the user).
    }
#endif


}

