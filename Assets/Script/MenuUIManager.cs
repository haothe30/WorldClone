
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : UIParent
{
    public static MenuUIManager Instance;
    [SerializeField] Image[] bouderBtnImg, iconBtnImg;
    [SerializeField] Sprite[] bouderSp, iconSpOn, iconSpOff;

    int currentTab = 0;
    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    public override void Start()
    {
        base.Start();

        GetDataManager().ShowSelectLevelPanel();


       // DisplayTab();


        MusicManager.instance.PlaySoundBGHome(true, GetDataManager().SaveData().currentMusicHome);
        AdsManager.instance.ShowBannerAds();
        AdsManager.instance.AutoShowBannerAdaptive();

    }

    public void BtnChangeTab(int index)
    {
        if (GetDataManager().CanNotAction() || LoadingPanel.loading.GetMaskObject().activeSelf)
            return;

        if (currentTab == index)
            return;

        MusicManager.instance.SoundClickButton();

        currentTab = index;
        if (index == 0)
        {
            GetDataManager().ShowSelectLevelPanel();
        }
        else if (index == 1)
        {
            GetDataManager().ShowShopPanel("0");
        }
        else if (index == 2)
        {
            GetDataManager().ShowRemoveAdsPanel();
        }
        else if (index == 3)
        {
            GetDataManager().ShowSettingPanel();
        }
        //  DisplayTab();
        currentTab = -1;
        DataManager.instance.ShowInterAllGame("ChangeTab");
    }
    void DisplayTab()
    {
        for (int i = 0; i < bouderBtnImg.Length; i++)
        {
            bouderBtnImg[i].sprite = bouderSp[0];
            iconBtnImg[i].sprite = iconSpOff[i];
        }
        bouderBtnImg[currentTab].sprite = bouderSp[1];
        iconBtnImg[currentTab].sprite = iconSpOn[currentTab];
    }
}
