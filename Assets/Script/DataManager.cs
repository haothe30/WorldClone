using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;
using System.Threading;
using System.Globalization;
using System;
using static SaveData;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;
using Unity.Advertisement.IosSupport;
using TMPro;
using I2.Loc;
using System.Linq;
using Spine.Unity;
using static DataLevel;
#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif


[System.Serializable]
public class SaveData
{
    public bool offmusic, offsound, offvibra, rated, removeAds, pushedNotice, vip, buyTicketShop;
    public int session, countShowInter, countShowVideo, showTut, currentStrNotice, totalTicket, totalPlay, currentMusicHome;
    public DateTime oldDay = System.DateTime.Now;
    public List<bool> pushEventTimePlaying = new List<bool>();
    public bool pushEventTimePlayingmorethantwentyfive = false;
    public List<SaveLevel> lstSaveLevel = new List<SaveLevel>();
    [System.Serializable]
    public class SaveLevel
    {
        public int maxStar;
        public int stage;
    }

}
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    [SerializeField] bool debug;
    [SerializeField] bool anUI;
    [SerializeField] bool anAds, hack;
    [SerializeField] SaveData saveData;
    [SerializeField] string appIDIos;
    [SerializeField] GameObject GDPR;
    [SerializeField] LoadingPanel loadingPanel;
    [SerializeField] DataLevel dataLevel;
    [SerializeField] DataSpriteLevel dataSpriteLevel;
    [SerializeField] DataShop dataShop, dataIAP;

    string[] noticeStr = { "Welcome back", "Join with us" };
    List<int> timePushNotification = new List<int> { 6, 18 };
    PopUpMess popupMess;
    PausePopUp pausePopUp;
    RatingManager ratePopUp;
    SkipPopUp skipPopUp;
    HintPopUp hintPopUp;
    EndPanel endPanel;
    TimeOutPanel timeOutPanel;
    UnlockLevelPopUp unlockLevelPopUp;
    SelectLevelPanel selectLevelPanel;
    SettingPanel settingPanel;
    //ShopPanel shopPanel;
    RemoveAdsPanel removeAdsPanel;
    WarningPopUp warningPopUp;
    WarningInternet warningInternet;
    HackPopUp hackPopUp;

    List<GameObject> lstPanelLoaded = new List<GameObject>();
    [SerializeField] List<LevelInfo> lstLevelInfo = new List<LevelInfo>();
    LevelInfo currentLevel;
    DataLevel dataLevelTemp;
    DataLevel dataLevelFromRemoteConfig;

    public SelectLevelPanel GetSelectLevelPanel()
    {
        return selectLevelPanel;
    }
    public bool CheckNotInterNet()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }
    public DataSpriteLevel GetDataSpriteLevel()
    {
        return dataSpriteLevel;
    }
    public DataShop GetDataRemoveAds()
    {
        return dataIAP;
    }
    public DataShop GetDataShop()
    {
        return dataShop;
    }
    public LevelInfo GetCurrentLevel()
    {
        return currentLevel;
    }
    public List<LevelInfo> GetListLevelInfo()
    {
        return lstLevelInfo;
    }
    public DataLevel GetDataLevel()
    {
        return dataLevelTemp;
    }
    public EndPanel GetEndPanel()
    {
        return endPanel;
    }
    public bool CanNotAction()
    {
        return loadingPanel.gameObject.activeSelf || DataParamManager.playingAnim || (warningInternet != null && warningInternet.gameObject.activeSelf);
    }

    public string GetAppIDIOS()
    {
        return appIDIos;
    }
    public bool GetHack()
    {
        return hack;
    }
    public bool AnAds
    {
        get
        {
            return anAds;
        }
        set
        {
            anAds = value;
        }
    }

    public bool AnUI
    {
        get { return anUI; }
        set { anUI = value; }
    }
    public SaveData SaveData()
    {
        return saveData;
    }

    public void RemoveAdsFunc(UnityEngine.Purchasing.Product product)
    {
        saveData.removeAds = true;
        AdsManager.instance.HideBannerAds();
        AdsManager.instance.ActiveNativeAds(false, 0, null);
        EventManager.REMOVEADS(product);


        if (DataParamManager.indexPackIAP == 0)
        {
            AddTicket(dataIAP.shopInfo[DataParamManager.indexPackIAP].reward);
        }
        else if (DataParamManager.indexPackIAP == 1)
        {
            AddTicket(dataIAP.shopInfo[DataParamManager.indexPackIAP].reward);
            saveData.vip = true;
            for (int i = 0; i < saveData.lstSaveLevel.Count; i++)
            {
                if (lstLevelInfo[i].totalStarToUnlock > 0)
                {
                    if (saveData.lstSaveLevel[i].stage == 0)
                    {
                        saveData.lstSaveLevel[i].stage = 1;
                    }
                }
            }
        }
        if (DataParamManager.displayRemoveAds != null)
            DataParamManager.displayRemoveAds();
        if (removeAdsPanel != null)
        {
            if (SceneManager.GetActiveScene().name == "Play")
            {
                if (saveData.vip)
                    removeAdsPanel.CloseMe();
            }
        }

        ShowWarningPopUp(dataIAP.shopInfo[DataParamManager.indexPackIAP].icon, null);
    }
    public void BuyTicketFunc(UnityEngine.Purchasing.Product product)
    {
        AddTicket(dataShop.shopInfo[DataParamManager.indexPackIAP + 1].reward);
        EventManager.BUYTICKET(product);

        ShowWarningPopUp(dataShop.shopInfo[DataParamManager.indexPackIAP + 1].icon, dataShop.shopInfo[DataParamManager.indexPackIAP + 1].reward.ToString());
    }

    public bool CheckInterNet()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }
    private void Awake()
    {
        if (instance == null)
        {
            Application.targetFrameRate = 300;
            Debug.unityLogger.logEnabled = debug;
            Input.multiTouchEnabled = false;
            CultureInfo ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            DestroyImmediate(gameObject);
        }


    }


    public void CallGDPR()
    {

#if UNITY_ANDROID
        GDPR.SetActive(true);
#else
        AdsManager.instance.InitAdsAfterGDPR();
#endif
        Debug.LogError("==================================== call GDPR");
        SetData();
    }
    public void AddTicket(int number)
    {
        saveData.totalTicket += number;
        if (DataParamManager.displayTicket != null)
            DataParamManager.displayTicket();
    }
    public void CallCountTimePlaying()
    {
        if (DontNeedCountTime())
        {
            //Debug.LogError("============ du het roi ko can dem nua");
            return;
        }
        InvokeRepeating(nameof(CountTimePlaying), 0, 60);
    }
    public void CallCheckInternet()
    {
        InvokeRepeating(nameof(CheckInternet), 0, 1);
    }
    void CheckInternet()
    {
        if (CheckNotInterNet())
        {
            ShowWarningInternet();
            CancelInvoke(nameof(CheckInternet));
        }
    }
    bool DontNeedCountTime()
    {
        return saveData.pushEventTimePlaying.FindAll(x => x == true).Count == saveData.pushEventTimePlaying.Count && saveData.pushEventTimePlayingmorethantwentyfive;
    }
    void CountTimePlaying()
    {
        double timePlaying = (DateTime.Now - DataParamManager.timePlaying).TotalMinutes;
        int getIntTimePlaying = (int)timePlaying;
        if (getIntTimePlaying > 0)
        {
            for (int i = 0; i < getIntTimePlaying; i++)
            {
                if (i < saveData.pushEventTimePlaying.Count && !saveData.pushEventTimePlaying[i])
                {
                    if (i == 2 || i == 4 || i == 6 || i == 9 || i == 14)
                    {
                        EventManager.EVENTTIMEPLAYING(i + 1);
                        saveData.pushEventTimePlaying[i] = true;
                    }
                }
            }
            if (getIntTimePlaying > saveData.pushEventTimePlaying.Count)
            {
                if (!saveData.pushEventTimePlayingmorethantwentyfive)
                {
                    EventManager.EVENTTIMEPLAYINGMORE25MINUS();
                    saveData.pushEventTimePlayingmorethantwentyfive = true;
                    CancelInvoke("CountTimePlaying");
                    // Debug.LogError("============== stop all count time");
                }
            }
        }
    }


    void CheckResetAllDaily()
    {
        if (saveData.oldDay != System.DateTime.Today)
        {
            saveData.oldDay = System.DateTime.Today;
            saveData.pushedNotice = false;
            saveData.buyTicketShop = false;
        }
    }

    void ReadPlayer(string value)
    {
        if (!string.IsNullOrEmpty(value) && value != "" && value != "[]")
        {
            saveData = JsonMapper.ToObject<SaveData>(value);
        }
    }
    void LoadData(string value)
    {
        saveData = new SaveData();
        ReadPlayer(value);
    }
    void LoadAllData()
    {
        LoadData(PlayerPrefs.GetString(DataParamManager.SAVEDATA));
        if (saveData.session == 0)
        {
            saveData.oldDay = System.DateTime.Today;
        }
        else
        {
            saveData.showTut = -1;
        }
        saveData.session++;

    }

    void SaveDataFunc()
    {
        PlayerPrefs.SetString(DataParamManager.SAVEDATA, JsonMapper.ToJson(saveData));
        PlayerPrefs.Save();
    }

    void CheckStrForNotice()
    {
        if (saveData.currentStrNotice < noticeStr.Length - 1)
        {
            saveData.currentStrNotice++;
        }
        else
        {
            saveData.currentStrNotice = 0;
        }
    }

#if UNITY_ANDROID
    void RequestAuthorizationAndroid()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }
    void RegisterNotificationChannelAndroid()
    {
        AndroidNotificationChannel notificationChannel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
        AndroidNotificationCenter.OnNotificationReceived += ReceivedNotificationHandler;
    }
    private void ReceivedNotificationHandler(AndroidNotificationIntentData data)
    {
        Debug.LogError("=============== recive notice:" + data.ToString());
        Debug.LogError("=============== recive notice:" + data.Channel + ":" + data.Id + ":" + data.NativeNotification + ":" + data.Notification);
    }

    public void PushNoticeForStory(int index, int month, int day, string title, string des)
    {
        DateTime nowNotification = DateTime.Now;
        DateTime nextNotificationTime = new DateTime(nowNotification.Year, month, day, 0, 0, 0);
        TimeSpan timeUntilNextNotification = nextNotificationTime - nowNotification;
        if (timeUntilNextNotification < TimeSpan.Zero)
        {
            nextNotificationTime = nextNotificationTime.AddDays(1);
            timeUntilNextNotification = nextNotificationTime - nowNotification;
        }
        AndroidNotification notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = des;
        notification.FireTime = nowNotification.Add(timeUntilNextNotification);
        notification.RepeatInterval = new TimeSpan(24, 0, 0); // 24 hours
        int notificationId = index * 100 + month * 10 + day;
        Debug.LogError("=========================== Status:" + AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) + " index:" + notificationId + "title:" + title + " des:" + des + ": " + timeUntilNextNotification);

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Unknown)
        {
            // Gửi thông báo nếu chưa có thông báo nào với index này
            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "default_channel", notificationId);
            Debug.LogError("================ Push notice first time");
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Scheduled)
        {
            // Cập nhật thông báo nếu đã được lên lịch
            AndroidNotificationCenter.UpdateScheduledNotification(notificationId, notification, "default_channel");
            Debug.LogError("================ Update scheduled notice");
        }
        //else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Delivered || AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Unavailable)
        //{
        //    // Hủy thông báo nếu đã được gửi hoặc không khả dụng và lên lịch lại
        //    AndroidNotificationCenter.CancelNotification(notificationId);
        //    AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "default_channel", notificationId);
        //    Debug.LogError("================ Cancel and resend notification");
        //}

    }
    void SendNotificationAndroid(int index)
    {

        DateTime nowNotification = DateTime.Now;
        DateTime nextNotificationTime = new DateTime(nowNotification.Year, nowNotification.Month, nowNotification.Day, timePushNotification[index], 0, 0);
        TimeSpan timeUntilNextNotification = nextNotificationTime - nowNotification;


        if (timeUntilNextNotification < TimeSpan.Zero)
        {
            nextNotificationTime = nextNotificationTime.AddDays(1);
            timeUntilNextNotification = nextNotificationTime - nowNotification;
        }


        AndroidNotification notification = new AndroidNotification();
        notification.Title = "Welcome";
        notification.Text = noticeStr[saveData.currentStrNotice];
        //notification.SmallIcon = "icon_1";
        //notification.LargeIcon = "icon_2";
        //notification.ShowTimestamp = true;
        notification.FireTime = nowNotification.Add(timeUntilNextNotification);
        notification.RepeatInterval = new TimeSpan(24, 0, 0); // 24 hours

        //   Debug.LogError("=========================== Status:" + AndroidNotificationCenter.CheckScheduledNotificationStatus(index) + " index:" + index + " hour:" + timePushNotification[index] + " des:" + noticeStr[saveData.currentStrNotice] + ": " + timeUntilNextNotification);

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(index) == NotificationStatus.Unknown)
        {
            // Gửi thông báo nếu chưa có thông báo nào với index này
            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "default_channel", index);
            //   Debug.LogError("================ Push notice first time");
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(index) == NotificationStatus.Scheduled)
        {
            // Cập nhật thông báo nếu đã được lên lịch
            AndroidNotificationCenter.UpdateScheduledNotification(index, notification, "default_channel");
            // Debug.LogError("================ Update scheduled notice");
        }
        //else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(index) == NotificationStatus.Delivered || AndroidNotificationCenter.CheckScheduledNotificationStatus(index) == NotificationStatus.Unavailable)
        //{
        //    // Hủy thông báo nếu đã được gửi hoặc không khả dụng và lên lịch lại
        //    AndroidNotificationCenter.CancelNotification(index);
        //    AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "default_channel", index);
        //    Debug.LogError("================ Cancel and resend notification");
        //}

        CheckStrForNotice();
        //  Debug.LogError("=========== Send Push Notificattion:" + index);

    }

#elif UNITY_IOS
    IEnumerator RequestAuthorizationIOS()
    {
        using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            yield return null;
        }
    }
    void SendNotificationIOS(int index)
    {
            // Kiểm tra nếu thông báo đã được lên lịch với Identifier này
        var scheduledNotifications = iOSNotificationCenter.GetScheduledNotifications();
        foreach (var notificationScheduleCheck in scheduledNotifications)
        {
            if (notificationScheduleCheck.Identifier == "default_channel_" + index)
            {
                iOSNotificationCenter.RemoveScheduledNotification("default_channel_" + index);
                break; // Hủy thông báo cũ trước khi lên lịch lại
            }
        }


          DateTime  nowNotification = DateTime.Now;
         DateTime   nextNotificationTime = new DateTime(nowNotification.Year, nowNotification.Month, nowNotification.Day, timePushNotification[index], 0, 0);
      if (nextNotificationTime < nowNotification)
    {
        nextNotificationTime = nextNotificationTime.AddDays(1);
    }

      var timeTrigger = new iOSNotificationCalendarTrigger()
    {
        Hour = nextNotificationTime.Hour,
        Minute = nextNotificationTime.Minute,
        Second = 0,
        Repeats = true // Đặt Repeats thành true để lặp lại hàng ngày
    };
        var notification = new iOSNotification()
        {
            Identifier = "default_channel_" + index,
            Title = "Welcome",
            Body =  noticeStr[saveData.currentStrNotice],
            Subtitle = "Come back to Game!",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Badge),
            CategoryIdentifier = "default_category",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger
        };

        Debug.LogError("=========================== Scheduled:" + index + " hour:" + timePushNotification[index] + " des:" + noticeStr[saveData.currentStrNotice]);
        iOSNotificationCenter.ScheduleNotification(notification);
                CheckStrForNotice();
            Debug.LogError("=========== Send Push Notificattion");
        }
    
#endif
    void PushNotice()
    {
#if UNITY_ANDROID
        // AndroidNotificationCenter.CancelAllDisplayedNotifications();
        if (!saveData.pushedNotice)
        {
            for (int i = 0; i < timePushNotification.Count; i++)
            {
                SendNotificationAndroid(i);
            }
            saveData.pushedNotice = true;
        }

#elif UNITY_IOS
        //  iOSNotificationCenter.RemoveAllDeliveredNotifications();
               if (!saveData.pushedNotice)
                {
                    for (int i = 0; i < timePushNotification.Count; i++)
                    {
                        SendNotificationIOS(i);
                    }
                  saveData.pushedNotice = true;
                }
#endif
    }
    void BackToGame()
    {
        if (!DataParamManager.ResumeFromAds)
        {
#if !UNITY_EDITOR
            if (SceneManager.GetActiveScene().name != "Loading" && !DataParamManager.ResumeFromOtherAction)
            {
                AdsManager.instance.ShowAOAAds("BackGame");
            }
#endif
        }

        if (DataParamManager.ResumeFromOtherAction)
        {
            DataParamManager.ResumeFromOtherAction = false;
        }

    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveDataFunc();
            PushNotice();
        }
        else
        {
            BackToGame();
        }
    }
    //private void OnApplicationPause(bool pauseStatus)
    //{
    //    IronSource.Agent.onApplicationPause(pauseStatus);
    //}
    public LoadingPanel GetLoadingPanel()
    {
        return loadingPanel;
    }
    public void ShowInterAllGame(string nameEventInter)
    {
        if (saveData.removeAds || saveData.totalPlay < 1 || anAds)
            return;
        if ((System.DateTime.Now - DataParamManager.beginShowInter).TotalSeconds > DataParamManager.timeDelayShowAds)
        {
            if (AdsManager.instance != null)
                AdsManager.instance.ShowInterAds(nameEventInter);
        }
    }


    public void ShowLoadingPanel(string value)
    {
        loadingPanel.OpenMe(value);
    }
    public void ShowShopPanel(string value)
    {
        //if (shopPanel == null)
        //{
        //    shopPanel = Instantiate(Resources.Load<ShopPanel>("Panel/ShopPanel"));
        //    AddPanelToList(shopPanel.gameObject);
        //}
        //shopPanel.OpenMe(value);

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            if (settingPanel != null)
                settingPanel.CloseMe();
            if (selectLevelPanel != null)
                selectLevelPanel.CloseMe();
            if (removeAdsPanel != null)
                removeAdsPanel.CloseMe();
        }
    }
    public void ShowLevelUnlockPopUp(LevelInfo _levelInfo, SlotLevel _slotLevelUnit)
    {
        if (unlockLevelPopUp == null)
        {
            unlockLevelPopUp = Instantiate(Resources.Load<UnlockLevelPopUp>("Panel/UnlockLevelPopUp"));
            AddPanelToList(unlockLevelPopUp.gameObject);
        }
        unlockLevelPopUp.SetInfoLevelNeedUnlock(_levelInfo, _slotLevelUnit);
        unlockLevelPopUp.OpenMe();
    }
    public void ShowRemoveAdsPanel()
    {
        if (removeAdsPanel == null)
        {
            removeAdsPanel = Instantiate(Resources.Load<RemoveAdsPanel>("Panel/RemoveAdsPanel"));
            AddPanelToList(removeAdsPanel.gameObject);
        }
        removeAdsPanel.OpenMe();

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            //if (settingPanel != null)
            //    settingPanel.CloseMe();
            if (selectLevelPanel != null)
                selectLevelPanel.CloseMe();
            //if (shopPanel != null)
            //    shopPanel.CloseMe();
        }
    }
    public void ShowSelectLevelPanel()
    {
        if (selectLevelPanel == null)
        {
            selectLevelPanel = Instantiate(Resources.Load<SelectLevelPanel>("Panel/SelectLevelPanel"));
            AddPanelToList(selectLevelPanel.gameObject);
        }
        selectLevelPanel.OpenMe();

        if (settingPanel != null)
            settingPanel.CloseMe();
        //if (shopPanel != null)
        //    shopPanel.CloseMe();
        if (removeAdsPanel != null)
            removeAdsPanel.CloseMe();
    }
    public void ShowSettingPanel()
    {
        if (settingPanel == null)
        {
            settingPanel = Instantiate(Resources.Load<SettingPanel>("Panel/SettingPanel"));
            AddPanelToList(settingPanel.gameObject);
        }
        settingPanel.OpenMe();

        if (selectLevelPanel != null)
        {
            selectLevelPanel.CloseMe();
            Debug.LogError("====================== nani");
        }
        //if (shopPanel != null)
        //    shopPanel.CloseMe();
        //if (removeAdsPanel != null)
        //    removeAdsPanel.CloseMe();
    }

    public void ShowPausePopUp()
    {
        if (pausePopUp == null)
        {
            pausePopUp = Instantiate(Resources.Load<PausePopUp>("Panel/PausePopUp"));
            AddPanelToList(pausePopUp.gameObject);
        }
        pausePopUp.OpenMe();
    }
    public bool CheckWarningInternetPanelActive()
    {
        return warningInternet != null && warningInternet.gameObject.activeSelf;
    }
    public bool CheckPanelActiveInGP()
    {
        return (pausePopUp != null && pausePopUp.gameObject.activeSelf) || (timeOutPanel != null && timeOutPanel.gameObject.activeSelf) || (skipPopUp != null && skipPopUp.gameObject.activeSelf) || (hintPopUp != null && hintPopUp.gameObject.activeSelf) /*|| (shopPanel != null&& shopPanel.gameObject.activeSelf )*/;
    }
    public void ShowWarningInternet()
    {
        if (warningInternet == null)
        {
            warningInternet = Instantiate(Resources.Load<WarningInternet>("Panel/WarningInternet"));
            DontDestroyOnLoad(warningInternet.gameObject);
            AddPanelToList(warningInternet.gameObject);
        }
        warningInternet.OpenMe();
    }
    public void ShowWarningPopUp(Sprite icon, string value)
    {
        if (warningPopUp == null)
        {
            warningPopUp = Instantiate(Resources.Load<WarningPopUp>("Panel/WarningPopUp"));
            AddPanelToList(warningPopUp.gameObject);
        }
        warningPopUp.SetUp(icon, value);
        warningPopUp.OpenMe();
    }
    public void ShowSkipPopUp()
    {
        if (skipPopUp == null)
        {
            skipPopUp = Instantiate(Resources.Load<SkipPopUp>("Panel/SkipPopUp"));
            AddPanelToList(skipPopUp.gameObject);
        }
        skipPopUp.OpenMe();
    }
    public void ShowHintPopUp()
    {
        if (hintPopUp == null)
        {
            hintPopUp = Instantiate(Resources.Load<HintPopUp>("Panel/HintPopUp"));
            AddPanelToList(hintPopUp.gameObject);
        }
        hintPopUp.OpenMe();
    }
    public void ShowRatePopUp()
    {
        if (ratePopUp == null)
        {
            ratePopUp = Instantiate(Resources.Load<RatingManager>("Panel/RatePopUp"));
            AddPanelToList(ratePopUp.gameObject);
        }
        ratePopUp.OpenMe();
        saveData.rated = true;
    }
    public void ShowEndPanel()
    {
        if (endPanel == null)
        {
            endPanel = Instantiate(Resources.Load<EndPanel>("Panel/EndPanel"));
            AddPanelToList(endPanel.gameObject);
        }
        endPanel.OpenMe();
    }
    public void ShowTimeOutPanel()
    {
        if (timeOutPanel == null)
        {
            timeOutPanel = Instantiate(Resources.Load<TimeOutPanel>("Panel/TimeOutPanel"));
            AddPanelToList(timeOutPanel.gameObject);
        }
        timeOutPanel.OpenMe();
    }

    public void ShowHackPopUp()
    {
        if (hackPopUp == null)
        {
            hackPopUp = Instantiate(Resources.Load<HackPopUp>("Panel/HackPopUp"));
            AddPanelToList(hackPopUp.gameObject);
        }
        hackPopUp.OpenMe();
    }
    public void ChangeSceneFunc()
    {
        for (int i = 0; i < lstPanelLoaded.Count; i++)
        {
            lstPanelLoaded[i].transform.parent = null;
            lstPanelLoaded[i].SetActive(false);
            DontDestroyOnLoad(lstPanelLoaded[i]);
        }
        DataParamManager.displayRemoveAds = null;
        DataParamManager.displayTicket = null;
        ObjectPoolManagerHaveScript.Instance.ClearAllPool();
        AdsManager.instance.ActiveNativeAds(false, -1, null);
    }
    public void ShowPopUpMess(Vector3 scale, Vector2 pos, string des)
    {
        UnitParent unitParent = ObjectPoolManagerHaveScript.Instance.listPooler[0].GetUnitPooledObject();
        unitParent.OnStart(scale, pos, des);
    }

    private void Start()
    {
        DataParamManager.beginShowInter = DataParamManager.timePlaying = DateTime.Now;
        CheckResetAllDaily();
        CallCountTimePlaying();
        CallCheckInternet();

#if UNITY_ANDROID
        RequestAuthorizationAndroid();
        RegisterNotificationChannelAndroid();
#elif UNITY_IOS
        StartCoroutine(RequestAuthorizationIOS());
#endif

        // loadingPanel.CreateBegin();

        //SetData();
        Debug.LogError("============== json:" + JsonMapper.ToJson(dataLevel));
    }
    public void SetCurrentLevel(int indexLevel)
    {
        currentLevel = lstLevelInfo[indexLevel];
    }

    public int TotalStar()
    {
        int totarStar = 0;
        for (int i = 0; i < saveData.lstSaveLevel.Count; i++)
        {
            if (saveData.lstSaveLevel[i].stage > 0)
                totarStar += saveData.lstSaveLevel[i].maxStar;
        }
        return totarStar;
    }
    public void GetData(string dataStr)
    {
        dataLevelFromRemoteConfig = JsonMapper.ToObject<DataLevel>(dataStr);
        //Debug.LogError("============================= GetData");
        //for(int i = 0; i < dataLevelFromRemoteConfig.lstDataLevel.Length; i ++)
        //{
        //    for(int j = 0; j < dataLevelFromRemoteConfig.lstDataLevel[i].levelInfo.Length; j ++)
        //    {
        //        Debug.LogError("================= :" + dataLevelFromRemoteConfig.lstDataLevel[i].levelInfo[j].indexLevel);
        //    }    

        //}    
    }
    public void SetData()
    {
        Debug.LogError("================================= setdata");
        CalculateData();
        SetCurrentLevel(0);
    }

    void CalculateData()
    {
        if (dataLevelFromRemoteConfig == null || dataLevelFromRemoteConfig.levelInfo.Length == 0)
        {
            dataLevelTemp = dataLevel;
            Debug.LogError("============================== Data bị null nên lấy data mặc định");
        }
        else
        {
            dataLevelTemp = dataLevelFromRemoteConfig;
            Debug.LogError("============================== Data lấy từ remote config");
        }

        int index = 0;
        for (int i = 0; i < dataLevelTemp.levelInfo.Length; i++)
        {
            if (dataLevelTemp.levelInfo[i].indexPrefab >= 0)
            {
                LevelInfo _levelInfo = dataLevelTemp.levelInfo[i];
                _levelInfo.indexLevel = index;
                index++;
                lstLevelInfo.Add(_levelInfo);
            }
        }

        int calculate = lstLevelInfo.Count - saveData.lstSaveLevel.Count;
        if (calculate > 0)
        {
            for (int i = saveData.lstSaveLevel.Count; i < lstLevelInfo.Count; i++)
            {
                SaveLevel saveLevel = new SaveLevel();
                saveData.lstSaveLevel.Add(saveLevel);
            }
        }
        else if (calculate < 0)
        {
            calculate = Mathf.Abs(calculate);
            saveData.lstSaveLevel.RemoveRange(saveData.lstSaveLevel.Count - calculate, calculate);
        }

        for (int i = 0; i < saveData.lstSaveLevel.Count; i++)
        {
            if (i == 0)
            {
                if (saveData.lstSaveLevel[i].stage == 0)
                    saveData.lstSaveLevel[i].stage = 1;
            }
            else
            {
                if (saveData.lstSaveLevel[i - 1].stage == 2)
                {
                    if (saveData.lstSaveLevel[i].stage == 0)
                    {
                        if (lstLevelInfo[i].totalStarToUnlock > 0)
                        {
                            if (TotalStar() >= lstLevelInfo[i].totalStarToUnlock)
                            {
                                saveData.lstSaveLevel[i].stage = 1;
                            }
                        }
                        else
                        {
                            saveData.lstSaveLevel[i].stage = 1;
                        }
                    }
                }
            }
        }

        for (int i = saveData.pushEventTimePlaying.Count; i < 25; i++)
        {
            saveData.pushEventTimePlaying.Add(false);
        }
    }
    public void CheckDoneLevel(int starCanTake)
    {
        EventManager.DONELEVEL();
        if (saveData.lstSaveLevel[currentLevel.indexLevel].stage == 1)
        {
            saveData.lstSaveLevel[currentLevel.indexLevel].stage = 2;
            if (lstLevelInfo[currentLevel.indexLevel].totalStarToUnlock > 0)
            {
                AddTicket(10);
            }
            else
            {
                AddTicket(5);
            }
        }
        if (saveData.lstSaveLevel[currentLevel.indexLevel].maxStar < starCanTake)
            saveData.lstSaveLevel[currentLevel.indexLevel].maxStar = starCanTake;

        if (currentLevel.indexLevel < lstLevelInfo.Count - 1)
        {
            if (lstLevelInfo[currentLevel.indexLevel + 1].totalStarToUnlock == 0)
            {
                if (saveData.lstSaveLevel[currentLevel.indexLevel + 1].stage == 0)
                {
                    saveData.lstSaveLevel[currentLevel.indexLevel + 1].stage = 1;
                }
            }
        }

        for (int i = 0; i < lstLevelInfo.Count; i++)
        {
            if (lstLevelInfo[i].totalStarToUnlock > 0)
            {
                if (saveData.lstSaveLevel[i].stage == 0)
                {
                    if (TotalStar() >= lstLevelInfo[i].totalStarToUnlock)
                    {
                        saveData.lstSaveLevel[i].stage = 1;
                    }
                }
            }
        }
    }
    int NextLevel()
    {
        int nextLevel = -1;
        if (currentLevel.indexLevel < lstLevelInfo.Count - 1)
        {
            if (saveData.lstSaveLevel[currentLevel.indexLevel + 1].stage > 0)
            {
                nextLevel = currentLevel.indexLevel + 1;
                currentLevel = lstLevelInfo[nextLevel];
            }
        }
        return nextLevel;
    }
    public void ChangeSceneAfterCheckNextLevel()
    {
        if (NextLevel() == -1)
        {
            ShowLoadingPanel("Menu");
        }
        else
        {
            ShowLoadingPanel("Play");
        }
    }
    void AddPanelToList(GameObject g)
    {
        // Debug.LogError(g.name);
        string name = g.name.Replace("(Clone)", "");
        g.name = name;
        lstPanelLoaded.Add(g);
    }
}

