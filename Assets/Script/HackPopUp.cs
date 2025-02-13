using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HackPopUp : UIProperties
{
    public void BtnUnlockAll()
    {
        for(int i = 0; i < DataManager.instance.SaveData().lstSaveLevel.Count; i ++)
        {
            DataManager.instance.SaveData().lstSaveLevel[i].stage = 3;
        }    
    }   
    public void BtnAnUI()
    {
        if(SceneManager.GetActiveScene().name == "Play")
        {
            DataManager.instance.AnUI = !DataManager.instance.AnUI;
            GamePlayUIManager.Instance.AnUI();
        }    
    }    
    public void BtnAnAds()
    {
        DataManager.instance.AnAds = true;
        AdsManager.instance.HideBannerAds();
        AdsManager.instance.ActiveNativeAds(false, 0, null);
    }    

}
