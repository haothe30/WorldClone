using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Analytics;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{

    [System.Serializable]
    public class NativeAdsInfo
    {

        public GameObject panel;
        public RawImage adIcon;
        public RawImage adChoices;
        public Text adHeadlineText;
        public Text adCallToActionText;

        public bool loaded;
        public AdLoader adLoaderNative;
        public NativeAd nativeAd;

        public Texture2D adIconTexture, adChoicesTexture;

    }


    public static AdsManager instance;
    [SerializeField] bool testAds, testsui;
    [SerializeField] string appId;
    [SerializeField] string appIdIOS;

    [SerializeField] string bannerIdAndroid;
    [SerializeField] string interIdAndroid;
    [SerializeField] string videoIdAndroid;
    [SerializeField] string AOAAndroid;
    [SerializeField] string bannerAdapterIdAndroid;
    [SerializeField] string nativeId;

    [SerializeField] string bannerIdIOS;
    [SerializeField] string interIdIOS;
    [SerializeField] string videoIdIOS;
    [SerializeField] string AOAIOS;
    [SerializeField] string bannerAdapterIdIOS;
    [SerializeField] string nativeIdIOS;

    [SerializeField] string appIdAndroidTest, appIdIOSTest;
    [SerializeField] string bannerAdapterIdAndroidTest;
    [SerializeField] string bannerAdapterIdIOSTest;
    [SerializeField] string nativeIdAndroidTest;

    [SerializeField] NativeAdsInfo[] nativeAds;

    string bannerId;
    string bannerAdapterId;
    string interId;
    string videoId;
    string aoaId;
    string idNative;

    bool doneWatchAds = false;
    Action acreward;
    string nameEventVideo, nameEventInter, nameEventAOA;
    string appIdTemp;
    AppOpenAd appOpenAd;
    //  RewardedAd rewardAdmob;

    bool loadingReward = false;


    public void ActiveNativeAds(bool show, int index, Transform point)
    {
        if (show)
        {
            for (int i = 0; i < nativeAds.Length; i++)
            {
                nativeAds[i].panel.SetActive(false);
                nativeAds[i].panel.transform.parent = DataManager.instance.GetLoadingPanel().transform;
                nativeAds[i].panel.transform.localScale = Vector3.one;
                nativeAds[i].panel.transform.localPosition = Vector3.zero;
            }

            if (!DataManager.instance.SaveData().removeAds && !DataManager.instance.AnAds)
            {
                nativeAds[index].panel.SetActive(true);
                nativeAds[index].panel.transform.parent = point;
                nativeAds[index].panel.transform.localScale = Vector3.one;
                nativeAds[index].panel.transform.localPosition = Vector3.zero;
            }
            //  Debug.LogError("==================== Call display native");
        }
        else
        {
            for (int i = 0; i < nativeAds.Length; i++)
            {
                nativeAds[i].panel.SetActive(false);
                nativeAds[i].panel.transform.parent = DataManager.instance.GetLoadingPanel().transform;
                nativeAds[i].panel.transform.localScale = Vector3.one;
                nativeAds[i].panel.transform.localPosition = Vector3.zero;
            }

            // Debug.LogError("==================== Call hide native");
        }
    }
    public void RequestNativeAd0()
    {
#if UNITY_IOS
       idNative = nativeIdIOS;
#else

        if (testAds)
        {
            idNative = nativeIdAndroidTest;
        }
        else
        {
            if(string.IsNullOrEmpty(DataParamManager.idNativeAds))
            {
                idNative = nativeId;
            }    
            else
            {
                idNative = DataParamManager.idNativeAds;
            }
            Debug.LogError("============= id native ads:" + idNative);
        }

#endif
        nativeAds[0].loaded = false;

        if (nativeAds[0].nativeAd != null)
        {
            // Debug.LogError("=============Destroying native ad.");
            nativeAds[0].nativeAd.Destroy();
            nativeAds[0].nativeAd = null;
        }
        Debug.LogError("=============== id native:" + idNative);
        nativeAds[0].adLoaderNative = new AdLoader.Builder(idNative).ForNativeAd().Build();
        //   Debug.LogError("=============== Ad Loader Native:" + nativeAds[0].adLoaderNative);
        nativeAds[0].adLoaderNative.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        nativeAds[0].adLoaderNative.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        nativeAds[0].adLoaderNative.OnNativeAdImpression += this.HandleNativeImpression;
        nativeAds[0].adLoaderNative.LoadAd(new AdRequest());

        //  Debug.LogError("======================= Request Native Ads");
    }
    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        nativeAds[0].adIcon.texture = nativeAds[0].adIconTexture;
        nativeAds[0].adChoices.texture = nativeAds[0].adChoicesTexture;
        Debug.LogError("============= native load false");
    }
    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs e)
    {
        Debug.LogError("============= native load success 111:" + e.nativeAd);
        if (!nativeAds[0].loaded)
        {
            //    Debug.LogError("============= native load success 222:" + e.nativeAd);
            nativeAds[0].nativeAd = e.nativeAd;
            nativeAds[0].loaded = true;

            //if (nativeAds[0].nativeAd != null)
            //{
            if (nativeAds[0].nativeAd.GetIconTexture() != null)
            {
                nativeAds[0].adIcon.texture = nativeAds[0].nativeAd.GetIconTexture();
            }
            else
            {
                nativeAds[0].adIcon.texture = nativeAds[0].adIconTexture;
            }
            if (nativeAds[0].nativeAd.GetAdChoicesLogoTexture() != null)
            {
                nativeAds[0].adChoices.texture = nativeAds[0].nativeAd.GetAdChoicesLogoTexture();
            }
            else
            {
                nativeAds[0].adChoices.texture = nativeAds[0].adChoicesTexture;
            }

            nativeAds[0].adHeadlineText.text = nativeAds[0].nativeAd.GetHeadlineText();
            nativeAds[0].adCallToActionText.text = nativeAds[0].nativeAd.GetCallToActionText();
            //   Debug.LogError("============================= chay duoc toi day r");


            //if (nativeAd.RegisterIconImageGameObject(adIcon.gameObject))
            //{
            //    Debug.LogError("================= regis adicon success");
            //}
            //else
            //{
            //    Debug.LogError("================= regis adicon false");
            //}

            //if (nativeAd.RegisterAdChoicesLogoGameObject(adChoices.gameObject))
            //{
            //    Debug.LogError("================= regis adChoices success");
            //}
            //else
            //{
            //    Debug.LogError("================= regis adChoices false");
            //}

            //if (nativeAd.RegisterHeadlineTextGameObject(adHeadlineText.gameObject))
            //{
            //    Debug.LogError("================= regis adHeadline success");
            //}
            //else
            //{
            //    Debug.LogError("================= regis adHeadline false");
            //}

            //if (nativeAd.RegisterCallToActionGameObject(adCallToActionText.gameObject))
            //{
            //    Debug.LogError("================= regis adCallToAction success");
            //}
            //else
            //{
            //    Debug.LogError("================= regis adCallToAction false");
            //}
            nativeAds[0].nativeAd.RegisterIconImageGameObject(nativeAds[0].adIcon.gameObject);
            nativeAds[0].nativeAd.RegisterAdChoicesLogoGameObject(nativeAds[0].adChoices.gameObject);
            nativeAds[0].nativeAd.RegisterHeadlineTextGameObject(nativeAds[0].adHeadlineText.gameObject);
            nativeAds[0].nativeAd.RegisterCallToActionGameObject(nativeAds[0].adCallToActionText.gameObject);

            //}
            //else
            //{
            //    Debug.LogError("===================== native ads is null");
            //}
        }

    }


    private void HandleNativeImpression(object sender, EventArgs e)
    {
        //  Debug.LogError("============== ad native impression");
        StartCoroutine(DelayShowNativeAds());

    }
    IEnumerator DelayShowNativeAds()
    {
        yield return DataParamManager.TIMEREFRESHNATIVE();
        RequestNativeAd0();
    }



    public void Start()
    {
        SetPrivacy();
        //  IronSource.Agent.validateIntegration();
#if UNITY_ANDROID
        appIdTemp = appId;
        bannerId = bannerIdAndroid;
        interId = interIdAndroid;
        videoId = videoIdAndroid;
        aoaId = AOAAndroid;
        if (testAds)
        {
            appIdTemp = appIdAndroidTest;
            bannerAdapterId = bannerAdapterIdAndroidTest;
        }
        else
        {
            bannerAdapterId = bannerAdapterIdAndroid;
        }
#elif UNITY_IOS
        appIdTemp = appIdIOS;
        bannerId = bannerIdIOS;
        interId = interIdIOS;
        videoId = videoIdIOS;
        aoaId = AOAIOS;
        if (testAds)
        {
            appIdTemp = appIdIOSTest;
            bannerAdapterId = bannerAdapterIdIOSTest;
        }
        else
        {
            bannerAdapterId = bannerAdapterIdIOS;
        }
#endif
        InitAds();

        // IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
    }
    private void SetPrivacy()
    {
        //  Debug.Log("unity-script: Moloco and IronSource set privacy");

        // Arbitrary privacy settings
        const bool consent = true;
        const bool childDirected = false;
        const bool doNotSell = false;

        //// Setting privacy settings to IronSource
        //IronSource.Agent.setConsent(consent);
        //IronSource.Agent.setMetaData("is_child_directed", childDirected.ToString());
        //IronSource.Agent.setMetaData("do_not_sell", doNotSell.ToString());


        //if (testsui)
        //    IronSource.Agent.setMetaData("is_test_suite", "enable");

    }
    private void ImpressionDataReadyEvent(/*IronSourceImpressionData impressionData*/string message, MaxSdkBase.AdInfo adInfo)
    {
        if (EventManager.fireBaseInitDone)
        {
            if (adInfo == null) return;
            if (adInfo.Revenue < 0) return;

            Firebase.Analytics.Parameter[] adParameters = {
        new Firebase.Analytics.Parameter("ad_platform", "Applovin"),
        new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
        new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
        new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
        new Firebase.Analytics.Parameter("currency","USD"),
        new Firebase.Analytics.Parameter("value", adInfo.Revenue)
    };
            FirebaseAnalytics.LogEvent("ad_impression", adParameters);

            //      if (impressionData?.revenue != null)
            //      {
            //          Firebase.Analytics.Parameter[] AdParameters = {
            //  new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
            //  new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
            //  new Firebase.Analytics.Parameter("ad_unit_name", impressionData.instanceName),
            //  new Firebase.Analytics.Parameter("ad_format", impressionData.adUnit),
            //  new Firebase.Analytics.Parameter("currency","USD"),
            //  new Firebase.Analytics.Parameter("value", impressionData.revenue.Value)
            //};
            //          Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
            //      }
        }
    }

    public void InitAdsAfterGDPR()
    {
        DataParamManager.SetTimeCollapsedBanner();
        DataParamManager.SetTimeRefreshNative();
        if (DataManager.instance.AnAds || DataManager.instance.SaveData().removeAds /*|| DataManager.instance.SaveData().premium >= 0*/)
            return;
        MobileAds.Initialize(initStatus =>
        {
            RequestNativeAd0();
            //  InitAOAAds();
            InitBannerAdapter();
        });

        RequestConfiguration requestConfiguration = new RequestConfiguration();
        requestConfiguration.TestDeviceIds.Add("A08B0124DBCF2EB955AEC33E9238C996");
        MobileAds.SetRequestConfiguration(requestConfiguration);

        //if (DataParamManager.typeBanner == 1)
        //{
        //    RequestBannerAds();
        //}
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
    BannerView _bannerView;
    AdRequest adRequest;
    AdSize adaptiveSize;
    bool collapsibleBannerIsExpand = false;
    void CreateBannerView()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        _bannerView = new BannerView(bannerAdapterId, adaptiveSize, AdPosition.Top);

        // Register for ad events.
        _bannerView.OnBannerAdLoaded += OnBannerAdapterAdLoaded;
        _bannerView.OnBannerAdLoadFailed += OnBannerAdapterAdLoadFailed;
        _bannerView.OnAdFullScreenContentClosed += OnBannerAdapterAdClose;
        _bannerView.OnAdFullScreenContentOpened += OnBannerAdapterAdOpened;
        _bannerView.OnAdPaid += OnBannerCollapsedPaid;
        //  Debug.LogError("=========== create banner view");
    }


    private void OnBannerCollapsedPaid(AdValue adValue)
    {
        if (adValue == null) return;
        double value = adValue.Value * 0.000001f;

        if (EventManager.fireBaseInitDone)
        {
            Parameter[] adParameters = {
        new Parameter("ad_source", "admob"),
        new Parameter("ad_format", "collapsible_banner"),
        new Parameter("currency","USD"),
        new Parameter("value", value)
    };
            FirebaseAnalytics.LogEvent("ad_impression", adParameters);
        }
    }

    private void OnBannerAdapterAdOpened()
    {
        collapsibleBannerIsExpand = true;
        //   Debug.LogError("================= opened adapter banner");
    }

    public void OnBannerAdapterAdClose()
    {
        collapsibleBannerIsExpand = false;

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            StartCoroutine(DelayAutoSpawnBannerAdaptive());
        }
        //   Debug.LogError("================= close adapter banner");
    }

    IEnumerator DelayAutoSpawnBannerAdaptive()
    {
        yield return DataParamManager.TIMECOLLAPSEDBANNER();
        AutoShowBannerAdaptive();
    }

    void InitBannerAdapter()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        adRequest = new AdRequest();
        // adRequest.Extras.Add("collapsible", "top");
        //  adRequest.Extras.Add("collapsible_request_id", Guid.NewGuid().ToString());
        // Load a banner ad.
        _bannerView.LoadAd(adRequest);
        //   Debug.LogError("=========== init collapsed banner");
    }

    public void AutoShowBannerAdaptive()
    {
        if (DataManager.instance.SaveData().removeAds || DataManager.instance.AnAds)
            return;
        if (collapsibleBannerIsExpand)
            return;
        if (_bannerView != null)
        {
            if (SceneManager.GetActiveScene().name == "Menu" || (SceneManager.GetActiveScene().name == "Play" && DataParamManager.state == DataParamManager.STATEGAMEPLAY.RESULT))
            {

                adRequest = new AdRequest();
                adRequest.Extras.Add("collapsible", "top");
                _bannerView.LoadAd(adRequest);
                //  Debug.LogError("================= Auto Load Show Adapter Banner");
            }
        }
        else
        {
            //  Debug.LogError("================= Cannot Auto Load Show Adapter Banner");
        }


    }
    public bool GetCollapsibleBannerIsExpand
    {
        get { return collapsibleBannerIsExpand; }
        set { collapsibleBannerIsExpand = value; }
    }
    private void OnBannerAdapterAdLoadFailed(LoadAdError obj)
    {
        //  Debug.LogError("================= banner adapter load false:" + obj.ToString());
    }

    private void OnBannerAdapterAdLoaded()
    {
        if (SceneManager.GetActiveScene().name == "Loading")
        {
            //   Debug.LogError("========== load xog banner collapsed roi nhung ơ scene loading nen an di");
            HideBannerAds();
        }
        else
        {
            ShowBannerAds();
            //   Debug.LogError("================= banner collapsed load success");
        }
    }

    public void InitAds()
    {
        if (DataManager.instance.AnAds)
            return;

        MaxSdk.SetSdkKey(appIdTemp);

        //   MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitAOAAds();
            //  InitBanner();
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += ImpressionDataReadyEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += ImpressionDataReadyEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += ImpressionDataReadyEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += ImpressionDataReadyEvent;

            if (testsui)
                MaxSdk.ShowCreativeDebugger();
        };


        // Debug.LogError("=========== init ads");
        //IronSourceRewardedVideoEvents.onAdOpenedEvent += OnRewardDisplayEvent;
        //IronSourceRewardedVideoEvents.onAdClosedEvent += OnRewardedAdDismissedEvent;
        //IronSourceRewardedVideoEvents.onAdAvailableEvent += OnRewardedAdLoadedEvent;
        //IronSourceRewardedVideoEvents.onAdUnavailableEvent += OnRewardedAdFailedEvent;
        //IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnRewardedAdFailedToDisplayEvent;
        //IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewardedAdReceivedRewardEvent;
        //IronSourceInterstitialEvents.onAdReadyEvent += OnInterstitialLoadedEvent;
        //IronSourceInterstitialEvents.onAdLoadFailedEvent += OnInterstitialFailedEvent;
        //IronSourceInterstitialEvents.onAdOpenedEvent += OnInterstitialDisplayEvent;
        //IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialFailedToDisplayEvent;
        //IronSourceInterstitialEvents.onAdClosedEvent += OnInterstitialDismissedEvent;
        //IronSourceBannerEvents.onAdLoadedEvent += BannerAdLoadedEvent;
        //IronSourceBannerEvents.onAdLoadFailedEvent += BannerAdLoadedEventFalse;

        //IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
        //IronSource.Agent.init(appIdTemp);
    }
    private void SdkInitializationCompletedEvent()
    {
        RequestInterAds();
        RequestVideoAds();
        // RequestBannerAds();
        //if (testsui)
        //    IronSource.Agent.launchTestSuite();
        Debug.LogError("================ init ads success");

    }
    void InitAOAAds()
    {
        MaxSdk.LoadAppOpenAd(aoaId);

        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
        MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadSuccess;
        MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenLoadFalse;


        //if (appOpenAd != null)
        //{
        //    appOpenAd.Destroy();
        //    appOpenAd = null;
        //}
        //// Debug.LogError("============== AOA id:" + aoaId);
        //AdRequest request = new AdRequest();
        //AppOpenAd.Load(aoaId, request, (AppOpenAd ad, LoadAdError error) =>
        // {
        //     // if error is not null, the load request failed.
        //     if (error != null || ad == null)
        //     {
        //         //Debug.LogError("===========app open ad failed to load an ad " +
        //         //   "with error : " + error);
        //         return;
        //     }

        //     //  Debug.LogError("===========App open ad loaded with response : "
        //     //  + ad.GetResponseInfo());

        //     appOpenAd = ad;
        //     ad.OnAdFullScreenContentClosed += OnAppOpenDismissedEvent;
        //     ad.OnAdPaid += OnAppOpenPaid;
        // });

    }

    private void OnAppOpenPaid(AdValue adValue)
    {
        if (adValue == null) return;
        double value = adValue.Value * 0.000001f;
        if (EventManager.fireBaseInitDone)
        {
            Parameter[] adParameters = {
        new Parameter("ad_source", "admob"),
        new Parameter("ad_format", "aoa"),
        new Parameter("currency","USD"),
        new Parameter("value", value)
    };
            FirebaseAnalytics.LogEvent("ad_impression", adParameters);
        }

    }


    private void OnAppOpenLoadFalse(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        loadingAOA = false;
        Debug.LogError("========== AOA load false");
    }

    private void OnAppOpenLoadSuccess(string arg1, MaxSdkBase.AdInfo arg2)
    {
        loadingAOA = false;
        Debug.LogError("========== AOA load success");
    }

    private void OnAppOpenDisplay(/*string arg1, MaxSdkBase.AdInfo arg2*/)
    {
        EventManager.SUM_OPENADS_ALL_GAME(nameEventAOA);
        MusicManager.instance.MuteAllMusic();
        MusicManager.instance.MuteAllSound();
        //  Time.timeScale = 0;
        Debug.LogError("===== displayed aoa");
    }

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

        MusicManager.instance.ChangeSettingMusic();
        MusicManager.instance.ChangeSettingSound();
        DataParamManager.beginShowInter = System.DateTime.Now;
        DataParamManager.ResumeFromAds = false;
        //   InitAOAAds();
        RequestAOA();
    }

    public Color colorBanner;
    public void InitBanner()
    {
        MaxSdkCallbacks.OnBannerAdLoadedEvent += BannerAdLoadedEvent;
        MaxSdkCallbacks.OnBannerAdLoadFailedEvent += BannerAdLoadedEventFalse;
        MaxSdk.CreateBanner(bannerId, MaxSdkBase.BannerPosition.TopCenter);

        RequestBannerAds();
    }



    public void InitializeInterstitialAds()
    {
        //MaxSdkCallbacks.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
        //MaxSdkCallbacks.OnInterstitialLoadFailedEvent -= OnInterstitialFailedEvent;
        //MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent -= InterstitialFailedToDisplayEvent;
        //MaxSdkCallbacks.OnInterstitialHiddenEvent -= OnInterstitialDismissedEvent;
        //MaxSdkCallbacks.OnInterstitialDisplayedEvent -= OnInterstitialDisplayEvent;
        //// Attach callback
        MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;
        // Load the first interstitial
        RequestInterAds();
    }
    public void InitializeRewardedAds()
    {
        //MaxSdkCallbacks.OnRewardedAdLoadedEvent -= OnRewardedAdLoadedEvent;
        //MaxSdkCallbacks.OnRewardedAdLoadFailedEvent -= OnRewardedAdFailedEvent;
        //MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent -= OnRewardedAdFailedToDisplayEvent;
        //MaxSdkCallbacks.OnRewardedAdHiddenEvent -= OnRewardedAdDismissedEvent;
        //MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
        //MaxSdkCallbacks.OnRewardedAdDisplayedEvent -= OnRewardDisplayEvent;
        //// Attach callback
        MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        // Load the first rewarded ad
        RequestVideoAds();
    }
    private void OnRewardedAdLoadedEvent(string adUnitId/*IronSourceAdInfo adInfo*/)
    {
        loadingReward = false;
        Debug.LogError("========= video load sucess:");
    }

    private void OnRewardedAdFailedEvent(string adUnitId, int errorCode /*IronSourceError error*/)
    {
        loadingReward = false;
        Debug.LogError("========= video load false:" /*+ error.ToString()*/);
    }

    private void OnRewardedAdDismissedEvent(string adUnitId /*IronSourceAdInfo adInfo*/)
    {
        DataParamManager.ResumeFromAds = false;
        MusicManager.instance.ChangeSettingMusic();
        MusicManager.instance.ChangeSettingSound();
        DataParamManager.beginShowInter = System.DateTime.Now;
        //  StartCoroutine(DelayActionAfterWatchReward());

        if (doneWatchAds)
        {
            if (acreward != null)
            {
                acreward();
                acreward = null;
            }
            EventManager.SUM_VIDEO_SHOW_SUCCESS_NAME(nameEventVideo);
        }


        if (SceneManager.GetActiveScene().name == "Play")
        {
            if (nameEventVideo.Contains("AddTime_") || nameEventVideo.Contains("Hint_"))
            {
                GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
            }
        }

        RequestVideoAds();
        doneWatchAds = false;

    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward /*IronSourcePlacement placement, IronSourceAdInfo adInfo*/)
    {
        doneWatchAds = true;
    }
    private void OnInterstitialLoadedEvent(string adUnitId /*IronSourceAdInfo adInfo*/)
    {
        Debug.LogError("========= inter load sucess");
        loadingInter = false;
    }
    private void OnInterstitialFailedEvent(string adUnitId, int errorCode /*IronSourceError ironSourceError*/)
    {
        Debug.LogError("========= inter load false");
        loadingInter = false;
    }
    private void OnInterstitialDismissedEvent(string adUnitId /*IronSourceAdInfo adInfo*/)
    {
        MusicManager.instance.ChangeSettingMusic();
        MusicManager.instance.ChangeSettingSound();
        RequestInterAds();
        DataParamManager.beginShowInter = System.DateTime.Now;
        DataParamManager.ResumeFromAds = false;
        Debug.LogError("=== bam' close inter");
        EventManager.SUM_INTER_ALL_GAME_SUCCESS(nameEventInter);
    }
    public void RequestVideoAds()
    {
        if (!loadingReward)
        {
            MaxSdk.LoadRewardedAd(videoId);
            loadingReward = true;
        }
        // IronSource.Agent.loadRewardedVideo();
    }
    public void RequestInterAds()
    {
        if(!loadingInter){
            MaxSdk.LoadInterstitial(interId);
            loadingInter = true;
            // IronSource.Agent.loadInterstitial();
        }
    }
    void RequestBannerAds()
    {
        MaxSdk.LoadBanner(bannerId);
        // IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.TOP);
        // Debug.LogError("========== init banner IRS");

    }
    public bool bannerOK;
    private void BannerAdLoadedEvent(string adUnitId /*IronSourceAdInfo adInfo*/)
    {
        if (!bannerOK)
        {

            Debug.LogError("====load banner  success :" + bannerId);
            bannerOK = true;
            //if (SceneManager.GetActiveScene().name != "Loading")
            //{

            ShowBannerAds();

            //}
        }
    }
    private void BannerAdLoadedEventFalse(string s, int i /*IronSourceError ironSourceError*/)
    {
        Debug.LogError("====load banner  false ");
        bannerOK = false;
    }

    IEnumerator DelayActionAfterWatchReward()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (doneWatchAds)
        {
            acreward();
            EventManager.SUM_VIDEO_SHOW_SUCCESS_NAME(nameEventVideo);
        }

        RequestVideoAds();
        acreward = null;
        doneWatchAds = false;
    }

    public bool CheckVideoReady()
    {
        if (DataManager.instance == null)
            return false;
        if (DataManager.instance.AnAds)
            return true;
        else
            return MaxSdk.IsRewardedAdReady(videoId) /*IronSource.Agent.isRewardedVideoAvailable()*/;
    }
    public void ShowAOAAds(string _nameEventAOA)
    {

#if !UNITY_EDITOR
        if (DataManager.instance != null && (DataManager.instance.SaveData().removeAds || DataManager.instance.AnAds))
            return;
        //if (appOpenAd == null)
        //{
        //    return;
        //}
        //if (!appOpenAd.CanShowAd())
        //    return;
        if (MaxSdk.IsAppOpenAdReady(aoaId) /*appOpenAd.CanShowAd()*/)
        {
            nameEventAOA = _nameEventAOA;
           // appOpenAd.Show();
             MaxSdk.ShowAppOpenAd(aoaId);
            DataParamManager.ResumeFromAds = true;
        }
        else
        {
            RequestAOA();
        }
#endif
    }

    void RequestAOA()
    {
        if(!loadingAOA)
        {
            MaxSdk.LoadAppOpenAd(aoaId);
            loadingAOA = true;
        }
    }

    public void ShowVideoAds(Action _ac, string name)
    {
        if (DataManager.instance == null)
            return;

        if (DataManager.instance.AnAds)
        {
            _ac();
            return;
        }

#if UNITY_EDITOR
        _ac();
#else

        if (/*IronSource.Agent.isRewardedVideoAvailable()*/MaxSdk.IsRewardedAdReady(videoId))
        {
            acreward = _ac;
            doneWatchAds = false;
            nameEventVideo = name;
             MaxSdk.ShowRewardedAd(videoId);
            //IronSource.Agent.showRewardedVideo(nameEventVideo);
            MusicManager.instance.MuteAllMusic();
            MusicManager.instance.MuteAllSound();
            DataParamManager.ResumeFromAds = true;
            DataManager.instance.SaveData().countShowVideo++;
            EventManager.SUM_VIDEO_SHOW_NAME(nameEventVideo);
        }
        else
        {
            RequestVideoAds();
            DataManager.instance.ShowPopUpMess(Vector3.one,Vector2.zero, "Ads not available");
        }
#endif


    }

    bool loadingInter, loadingAOA;

    public void ShowInterAds(string _nameEventInter)
    {
        if (DataManager.instance == null)
            return;
        if (DataManager.instance.AnAds)
            return;
        if (/*IronSource.Agent.isInterstitialReady()*/MaxSdk.IsInterstitialReady(interId))
        {
            nameEventInter = _nameEventInter;
            // IronSource.Agent.showInterstitial(nameEventInter);
            MaxSdk.ShowInterstitial(interId);
            DataParamManager.ResumeFromAds = true;

            EventManager.SUM_INTER_ALL_GAME(nameEventInter);
            Debug.LogError("========= show inter:" + nameEventInter);
        }
        else
        {
            RequestInterAds();
        }
    }
    public void ShowBannerAds()
    {

        if (DataManager.instance.SaveData().removeAds || DataManager.instance.AnAds)
            return;


        if (!bannerOK)
        {
            if (_bannerView != null)
            {
                _bannerView.Show();
                bannerOK = true;
                EventManager.SUM_BANNER_ALL_GAME();
                Debug.LogError("================= show banner collapsed");
            }
        }
        //#if !UNITY_EDITOR
        //        if (bannerOK)
        //        {
        //            if (!bannerIsShowing)
        //            {
        //                // IronSource.Agent.displayBanner();
        //                MaxSdk.ShowBanner(bannerId);
        //                EventManager.SUM_BANNER_ALL_GAME();
        //                Debug.LogError("================= show banner");
        //                bannerIsShowing = true;
        //            }
        //        }
        //        else
        //            RequestBannerAds();
        //#endif

    }
    // bool bannerIsShowing;
    public void HideBannerAds()
    {

        if (bannerOK)
        {
            bannerOK = false;
        }
        if (_bannerView != null)
            _bannerView.Hide();
        // Debug.LogError("============= Hide Banner collapsed");


        // if (bannerOK)
        //MaxSdk.HideBanner(bannerId);
        //bannerIsShowing = false;
        //Debug.LogError("============= Hide Banner IRS");
        //IronSource.Agent.hideBanner();
        //isShowingBannerIRS = false;

    }
}
