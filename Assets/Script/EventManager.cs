using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;
using Firebase.RemoteConfig;
using Firebase;
using ByteBrewSDK;
using Firebase.Crashlytics;
using UnityEngine.SceneManagement;

#if UNITY_ANDROID
using Google;
#endif


#if UNITY_ANDROID
using Google.Play.Review;
#else
using UnityEngine.iOS;
#endif

[System.Serializable]
public class RemoteDefaultInfo
{
    public string key, value;
}
public class EventManager : MonoBehaviour
{
    static Firebase.FirebaseApp app;
    public static bool fireBaseInitDone;
    public RemoteDefaultInfo[] remoteDefault;
    public static EventManager instance;
    static DataManager dataController;

#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    private Coroutine _coroutine;
#else
    bool reviewRequested = false;
#endif

    public void RateAndReview()
    {
#if UNITY_IOS
        RequestReview();
#elif UNITY_ANDROID
        StartCoroutine(LaunchReview());
#endif
    }

#if UNITY_ANDROID
    IEnumerator InitReview(bool force = false)
    {
        if (_reviewManager == null) _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            if (force)
            {
                //   Debug.LogError("============= force init");
                DirectlyOpen();
            }
            yield break;
        }
        else
        {
            //  Debug.LogError("=========== error init: " + requestFlowOperation.GetResult().ToString());
        }

        _playReviewInfo = requestFlowOperation.GetResult();

        //    Debug.LogError("================== init review:" + _playReviewInfo.ToString());
    }

    IEnumerator LaunchReview()
    {
        if (_playReviewInfo == null)
        {
            if (_coroutine != null)
            {
                Debug.LogError("============= stop coroutine");
                StopCoroutine(_coroutine);
            }
            yield return StartCoroutine(InitReview(true));
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            DirectlyOpen();
            yield break;
        }
        else
        {
            Debug.LogError("=========== error lauch:" + launchFlowOperation.GetResult().ToString());
        }

        Debug.LogError("================== launch review:" + launchFlowOperation.GetResult().ToString());
    }

    void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }
#else
    void RequestReview()
    {
        if (reviewRequested == false)
        {
            bool popupShown = Device.RequestStoreReview();
            if (popupShown)
            {
                // The review popup was presented to the user, set "reviewRequested" to "true" to reflect that
                // Note: there's no way to check if the user actually gave a review for the app or cancelled the popup.
                reviewRequested = true;
                Debug.LogError("================== rate ios true");
            }
            else
            {
                // The review popup wasn't presented. Log a message and reset "reviewRequested" so you can revisit this in the future.
                Debug.Log("iOS version is too low or StoreKit framework was not linked.");
                reviewRequested = false;
                Debug.LogError("================== rate ios false");
            }
        }
    }
#endif

    void CreateByteBrew()
    {
#if UNITY_ANDROID
        ByteBrew.InitializeByteBrew();
#elif UNITY_IOS
// Call ByteBrew ATT Wrapper
ByteBrew.requestForAppTrackingTransparency((status) =>
{
    //Case 0: ATTrackingManagerAuthorizationStatusAuthorized
    //Case 1: ATTrackingManagerAuthorizationStatusDenied
    //Case 2: ATTrackingManagerAuthorizationStatusRestricted
    //Case 3: ATTrackingManagerAuthorizationStatusNotDetermined
    Debug.Log("ByteBrew Got a status of: " + status);
    ByteBrew.InitializeByteBrew();
});
#endif
        // Debug.LogError("==========get user id:" + ByteBrew.GetUserID());
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }



    private void Start()
    {
        dataController = DataManager.instance;

        CreateByteBrew();


        if (!fireBaseInitDone)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    app = FirebaseApp.DefaultInstance;
                    fireBaseInitDone = true;
                    InitRemoteConfig();

                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
        }

#if UNITY_ANDROID
        _coroutine = StartCoroutine(InitReview());

#endif

    }

    void InitRemoteConfig()
    {
        System.Collections.Generic.Dictionary<string, object> defaults =
  new System.Collections.Generic.Dictionary<string, object>();

        for (int i = 0; i < remoteDefault.Length; i++)
        {
            defaults.Add(remoteDefault[i].key, remoteDefault[i].value);
            //  Debug.LogError("=================remote value:" + remoteDefault[i].value);
        }
        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
          .ContinueWithOnMainThread(task =>
          {
              //   Debug.LogError("================ fetch start");
              FetchDataAsync();
          });
    }
    public Task FetchDataAsync()
    {
        System.Threading.Tasks.Task fetchTask =
       FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("============== not complete fetch");
            dataController.CallGDPR();

            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"================={nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            dataController.CallGDPR();
            return;
        }
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                  Debug.LogError($"====Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            });

        DataParamManager.timeDelayShowAds = float.Parse(FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[0].key).StringValue);
        DataParamManager.timeCollapsedBannerFL = float.Parse(FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[1].key).StringValue);
        dataController.GetData(FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[2].key).StringValue);
        DataParamManager.idNativeAds = FirebaseRemoteConfig.GetInstance(app).GetValue(remoteDefault[3].key).StringValue;

        Debug.LogError("============== oke GDPR" + DataParamManager.idNativeAds);

        dataController.CallGDPR();
      

    }



    public static void SUM_VIDEO_SHOW_SUCCESS_NAME(string value)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("rewardsucess_" + value + "_ss_" + dataController.SaveData().session);


            if (SceneManager.GetActiveScene().name == "Play")
            {
                FirebaseAnalytics.LogEvent("rewardGPlaylv_" + (DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
            }
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("rewardsucess", value);
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("rewardsucess", eventParameters);
            ByteBrew.TrackAdEvent(ByteBrewAdTypes.Reward, value);

            if (SceneManager.GetActiveScene().name == "Play")
            {
                Dictionary<string, string> eventParameters2 = new Dictionary<string, string>();
                eventParameters2.Add("rewardGPlaylv_", (DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1));
                eventParameters2.Add("session", dataController.SaveData().session.ToString());
                ByteBrew.NewCustomEvent("rewardGPlaylv_", eventParameters2);
            }

        }
        Debug.LogError("rewardsucess_" + value + "_ss_" + dataController.SaveData().session);
        Debug.LogError("rewardGPlaylv_" + (DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void SUM_VIDEO_SHOW_NAME(string value)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("rewarddisplay_" + value + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("rewarddisplay", value);
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("rewarddisplay", eventParameters);
        }
        Debug.LogError("rewarddisplay_" + value + "_ss_" + dataController.SaveData().session);
    }

    public static void SUM_OPENADS_ALL_GAME(string value)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("aoadisplay_" + value + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("aoadisplay", value);
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("aoadisplay", eventParameters);
        }
        Debug.LogError("aoadisplay_" + value + "_ss_" + dataController.SaveData().session);

    }
    public static void SUM_BANNER_ALL_GAME()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("showbanner");
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("showbanner", eventParameters);
            ByteBrew.TrackAdEvent(ByteBrewAdTypes.Banner, "");
        }
    }

    public static void SUM_INTER_ALL_GAME(string value)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("intershow_" + value + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("intershow", value);
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("intershow", eventParameters);
        }
        Debug.LogError("intershow_" + value + "_ss_" + dataController.SaveData().session);
    }

    public static void SUM_INTER_ALL_GAME_SUCCESS(string value)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("intersuccess_" + value + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("intersuccess", value);
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("intersuccess", eventParameters);
            ByteBrew.TrackAdEvent(ByteBrewAdTypes.Interstitial, value);
        }
        Debug.LogError("intersuccess_" + value + "_ss_" + dataController.SaveData().session);
    }
    static string storeName = "";
    public static void REMOVEADS(UnityEngine.Purchasing.Product product)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("removeads_" + DataParamManager.indexPackIAP + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
#if UNITY_IOS
            storeName = "Apple App Store";
#elif UNITY_ANDROID
            storeName = "Google Play Store";
#endif
            ByteBrew.TrackInAppPurchaseEvent(storeName, product.metadata.isoCurrencyCode, (float)product.metadata.localizedPrice, product.definition.id, "");
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("pack", DataParamManager.indexPackIAP.ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("removeads", eventParameters);
        }
        Debug.LogError("removeads_" + DataParamManager.indexPackIAP + "_ss_" + dataController.SaveData().session);
    }
    public static void BUYTICKET(UnityEngine.Purchasing.Product product)
    {
//        if (fireBaseInitDone)
//        {
//            FirebaseAnalytics.LogEvent("ticketpack_" + DataParamManager.indexPackIAP + "_ss_" + dataController.SaveData().session);
//        }
//        if (ByteBrew.IsByteBrewInitialized())
//        {
//#if UNITY_IOS
//            storeName = "Apple App Store";
//#elif UNITY_ANDROID
//            storeName = "Google Play Store";
//#endif
//            ByteBrew.TrackInAppPurchaseEvent(storeName, product.metadata.isoCurrencyCode, (float)product.metadata.localizedPrice, product.definition.id, "");
//            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
//            eventParameters.Add("pack", DataParamManager.indexPackIAP.ToString());
//            eventParameters.Add("session", dataController.SaveData().session.ToString());
//            ByteBrew.NewCustomEvent("ticketpack", eventParameters);
//        }
//        Debug.LogError("ticketpack_" + DataParamManager.indexPackIAP + "_ss_" + dataController.SaveData().session);
    }

    public static void FOLLOW_BUTTON_EVENT(string value, string nameScene)
    {
        if (fireBaseInitDone)
        {
            if (nameScene == "Menu")
            {
                FirebaseAnalytics.LogEvent("menuclick_" + value + "_level_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1));
                Debug.LogError("menuclick_" + value + "_level_" + dataController.GetCurrentLevel().indexLevel + 1 + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1));
            }

            else if (nameScene == "Play")
            {
                FirebaseAnalytics.LogEvent("playclick_" + value + "_level_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1));
                Debug.LogError("playclick_" + value + "_level_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1));
            }
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            if (nameScene == "Menu")
            {
                eventParameters.Add("menuclick", value);
            }
            else if (nameScene == "Play")
            {
                eventParameters.Add("playclick", value);
            }
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("pf", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            ByteBrew.NewCustomEvent("click", eventParameters);
        }


    }
    public static void EVENTTIMEPLAYING(int value)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("timess_" + value + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("time", value.ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("timess", eventParameters);
        }
        Debug.LogError("timess_" + value + "_ss_" + dataController.SaveData().session);
    }
    public static void EVENTTIMEPLAYINGMORE25MINUS()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("timessmoretwentyfive" + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("timessmoretwentyfive", eventParameters);
        }
        Debug.LogError("timessmoretwentyfive" + "_ss_" + dataController.SaveData().session);
    }

    static string GetPrice(decimal amount)
    {
        decimal val = decimal.Multiply(amount, 0.63m);
        return val.ToString();
    }

    public static void STARTLEVEL()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("startlevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("startlevel", eventParameters);
        }
        Debug.LogError("startlevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void REPLAYLEVEL()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("replaylevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("replaylevel", eventParameters);
        }
        Debug.LogError("replaylevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void DONELEVEL()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("donelevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("donelevel", eventParameters);
        }
        Debug.LogError("donelevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void LOSELEVEL()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("loselevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("loselevel", eventParameters);
        }
        Debug.LogError("loselevel_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void BOOSTERHINT()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("boosterhint_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("boosterhint", eventParameters);
        }
        Debug.LogError("boosterhint_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void BOOSTERSKIP()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("boosterskip_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("boosterskip", eventParameters);
        }
        Debug.LogError("boosterskip_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void BOOSTERADDTIME()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("boosteraddtime_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("boosteraddtime", eventParameters);
        }
        Debug.LogError("boosteraddtime_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void PAYSKIP()
    {
        //if (fireBaseInitDone)
        //{
        //    FirebaseAnalytics.LogEvent("payskip_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        //}
        //if (ByteBrew.IsByteBrewInitialized())
        //{
        //    Dictionary<string, string> eventParameters = new Dictionary<string, string>();
        //    eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
        //    eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
        //    eventParameters.Add("session", dataController.SaveData().session.ToString());
        //    ByteBrew.NewCustomEvent("payskip", eventParameters);
        //}
        //Debug.LogError("payskip_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void PAYADDTIME()
    {
        //if (fireBaseInitDone)
        //{
        //    FirebaseAnalytics.LogEvent("payaddtime_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        //}
        //if (ByteBrew.IsByteBrewInitialized())
        //{
        //    Dictionary<string, string> eventParameters = new Dictionary<string, string>();
        //    eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
        //    eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
        //    eventParameters.Add("session", dataController.SaveData().session.ToString());
        //    ByteBrew.NewCustomEvent("payaddtime", eventParameters);
        //}
        //Debug.LogError("payaddtime_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void BACKHOME()
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("backhome_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
            eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
            eventParameters.Add("session", dataController.SaveData().session.ToString());
            ByteBrew.NewCustomEvent("backhome", eventParameters);
        }
        Debug.LogError("backhome_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_ss_" + dataController.SaveData().session);
    }
    public static void CHECKDONESTEPLEVEL(string value)
    {
    //    if (fireBaseInitDone)
    //    {
    //        FirebaseAnalytics.LogEvent("donestep_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_" + value + "_ss_" + dataController.SaveData().session);
    //    }
    //    if (ByteBrew.IsByteBrewInitialized())
    //    {
    //        Dictionary<string, string> eventParameters = new Dictionary<string, string>();
    //        eventParameters.Add("step", value);
    //        eventParameters.Add("level", (dataController.GetCurrentLevel().indexLevel + 1).ToString());
    //        eventParameters.Add("prefab", (dataController.GetCurrentLevel().indexPrefab + 1).ToString());
    //        eventParameters.Add("session", dataController.SaveData().session.ToString());
    //        ByteBrew.NewCustomEvent("donestep", eventParameters);
    //    }
    //    Debug.LogError("donestep_" + (dataController.GetCurrentLevel().indexLevel + 1) + "_pf_" + (dataController.GetCurrentLevel().indexPrefab + 1) + "_" + value + "_ss_" + dataController.SaveData().session);
    }
    public static void CHECK_STEP_TUTORIAL(string value)
    {
        if (fireBaseInitDone)
        {
            FirebaseAnalytics.LogEvent("checksteptutorial_" + value);
        }
        if (ByteBrew.IsByteBrewInitialized())
        {
            Dictionary<string, string> eventParameters = new Dictionary<string, string>();
            eventParameters.Add("steptut", value);
            ByteBrew.NewCustomEvent("checksteptutorial", eventParameters);
            ByteBrew.TrackAdEvent(ByteBrewAdTypes.Interstitial, value);
        }
        Debug.LogError("checksteptutorial" + value);
    }
}
