using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkipPopUp : UIProperties
{
    [SerializeField] Text ticketText;
    [SerializeField] GameObject bouderTop;

    public override void OpenMe()
    {
        base.OpenMe();
        ticketText.text = "" + DataManager.instance.SaveData().totalTicket;
        bouderTop.SetActive(false);
    }
    public override void ActiveAllBtn(bool active)
    {
        base.ActiveAllBtn(active);
        bouderTop.SetActive(true);
    }
    public void BtnSkip(bool free)
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        if (free)
        {
            AdsManager.instance.ShowVideoAds(Reward, "Skip_" +(DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1));
        }
        else
        {
            if (DataManager.instance.SaveData().totalTicket >= 200)
            {
                DataManager.instance.AddTicket(-200);
                Reward();
                EventManager.PAYSKIP();
            }
            else
            {
                base.CloseMe();
                DataManager.instance.ShowShopPanel("1");
            }
        }
    }
    public override void CloseMe()
    {
        base.CloseMe();
        GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
    }
    void Reward()
    {
        GamePlayManager.Instance.SkinFunc();
        CloseMe();
    }
}
