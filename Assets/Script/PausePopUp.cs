using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePopUp : UIProperties
{
    //public Text currentLanguageText;
    //public string[] languageName;
    //public string[] lstLanguageCurrent;

    public Image musicImg, soundImg,vibrateImg;
    public Sprite[] onSp, offSp;
    DataManager dataController;

   // int currentLanguage;

    public void DisplaySetting()
    {
        musicImg.sprite = dataController.SaveData().offmusic ? offSp[0] : onSp[0];
        soundImg.sprite = dataController.SaveData().offsound ? offSp[1] : onSp[1];
        vibrateImg.sprite = dataController.SaveData().offvibra ? offSp[2] : onSp[2];
    }

    public override void OpenMe()
    {
        if (dataController == null)
            dataController = DataManager.instance;

        DisplaySetting();
        base.OpenMe();
    }

    public void BtnSound()
    {
        if (dataController.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        dataController.SaveData().offsound = !dataController.SaveData().offsound;
        DisplaySetting();
        MusicManager.instance.ChangeSettingSound();
    }
    public void BtnMusic()
    {
        if (dataController.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        dataController.SaveData().offmusic = !dataController.SaveData().offmusic;
        DisplaySetting();
        MusicManager.instance.ChangeSettingMusic();
    }
    public void BtnVibrate()
    {
        if (dataController.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        dataController.SaveData().offvibra = !dataController.SaveData().offvibra;
        DisplaySetting();
    }

    public override void CloseMe()
    {
        base.CloseMe();

        if(SceneManager.GetActiveScene().name == "Play")
        {
            GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
        }
    }
    public override void BtnClose()
    {
        DataManager.instance.ShowInterAllGame("ClosePause");
        base.BtnClose();
    }
    public void BtnHome()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        DataManager.instance.ShowLoadingPanel("Menu");
        EventManager.BACKHOME();
        DataManager.instance.ShowInterAllGame("HomePause");
    }

    public void BtnSkip()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        AdsManager.instance.ShowVideoAds(RewardSkip, "Skip_" + (DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1));
    }
    void RewardSkip()
    {
        GamePlayManager.Instance.SkinFunc();
        CloseMe();
    }
}
