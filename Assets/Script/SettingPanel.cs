using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIProperties
{
    public Text currentLanguageText, currentMusicHomeText;
    public string[] languageName;
    public string[] lstLanguageCurrent;
    public Image musicImg, soundImg, vibrateImg;
    public Sprite[] onSp, offSp;
    DataManager dataController;
    bool skip;
    int currentLanguage;

    public void DisplaySetting()
    {
        musicImg.sprite = dataController.SaveData().offmusic ? offSp[0] : onSp[0];
        soundImg.sprite = dataController.SaveData().offsound ? offSp[1] : onSp[1];
        vibrateImg.sprite = dataController.SaveData().offvibra ? offSp[2] : onSp[2];
        currentMusicHomeText.text = "BGM" + (dataController.SaveData().currentMusicHome + 1);
    }
    public void BtnChangeMusicHome(bool next)
    {

        if (next)
        {
            if (dataController.SaveData().currentMusicHome < MusicManager.instance.GetLengthBGHomeCl() - 1)
            {
                dataController.SaveData().currentMusicHome++;
            }
            else
            {
                dataController.SaveData().currentMusicHome = 0;
            }
        }
        else
        {
            if (dataController.SaveData().currentMusicHome > 0)
            {
                dataController.SaveData().currentMusicHome--;
            }
            else
            {
                dataController.SaveData().currentMusicHome = MusicManager.instance.GetLengthBGHomeCl() - 1;
            }
        }
        MusicManager.instance.PlaySoundBGHome(true, dataController.SaveData().currentMusicHome);
        DisplaySetting();
    }
    public override void OpenMe()
    {
        if (dataController == null)
            dataController = DataManager.instance;
        skip = false;
        for (int i = 0; i < languageName.Length; i++)
        {
            if (languageName[i] == LocalizationManager.CurrentLanguage)
            {
                currentLanguage = i;
            }
        }
        currentLanguageText.text = lstLanguageCurrent[currentLanguage];
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

    public void SelectLanguageBtn(bool next)
    {
        MusicManager.instance.SoundClickButton();

        if (next)
        {
            if (currentLanguage < languageName.Length - 1)
            {
                currentLanguage++;
            }
            else
            {
                currentLanguage = 0;
            }
        }
        else
        {
            if (currentLanguage > 0)
            {
                currentLanguage--;
            }
            else
            {
                currentLanguage = languageName.Length - 1;
            }

        }

        if (LocalizationManager.HasLanguage(languageName[currentLanguage]))
        {
            LocalizationManager.CurrentLanguage = languageName[currentLanguage];
            currentLanguageText.text = lstLanguageCurrent[currentLanguage];

        }
    }


    public void BtnClearAllRecord()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
    }

}
