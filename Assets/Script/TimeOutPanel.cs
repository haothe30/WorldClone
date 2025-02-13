using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeOutPanel : UIProperties
{
    [SerializeField] Text ticketText;
    [SerializeField] GameObject bouderTop;
    public override void OpenMe()
    {
        base.OpenMe();
        ticketText.text = "" + DataManager.instance.SaveData().totalTicket;
        bouderTop.SetActive(false);
        MusicManager.instance.PlaySoundOtherOneShot(true, 1);
    }
    public override void ActiveAllBtn(bool active)
    {
        base.ActiveAllBtn(active);
        bouderTop.SetActive(true);
    }
    public void BtnSaveMe()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        if(DataManager.instance.SaveData().totalTicket >= 150)
        {
            CallAddTime();
            DataManager.instance.AddTicket(-150);
            EventManager.PAYADDTIME();
        }    
        else
        {
            DataManager.instance.ShowShopPanel("2");
            CloseMe();
        }    
    }   
    public void BtnGetUnlimited()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        DataManager.instance.ShowShopPanel("2");
        CloseMe();
    }    
    public void BtnAddTime()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        AdsManager.instance.ShowVideoAds(CallAddTime, "AddTime_" +(DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1));
    }   
    public void BtnGiveUp()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();

        GamePlayManager.Instance.GiveUpFunc();
        CloseMe();
    }
    void CallAddTime()
    {
        GamePlayManager.Instance.AddMaxTime(60);
        GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
        CloseMe();
    }
}
