using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPanel : UIProperties
{
    [SerializeField] GameObject winPanel, losePanel/*, btnRemoveAdsWin, btnRemoveAdsLose*/;
    [SerializeField] GameObject[] lstStar;
    //[SerializeField] Text ticketText, desWinText, desLoseText;
    [SerializeField] Image loseImg, winImg;
    [SerializeField] UIProperties thisUI;
    //string[] desWin3Star = { "You did it!", "Excellent!", "Impressive!", "Fantastic!", "You Nailed it!" };
    //string[] desWin2Star = { "You did it!", "Impressive!", "Nearly perfect!" };
    //string[] desWin1Star = { "Don't give up!", "Still Messy", "Not so clean" };
    //string[] desLose = { "Still Messy", "Not so Clean" };
    //void CheckDisplayBtnRemoveAds()
    //{
    //    btnRemoveAdsWin.SetActive(!DataManager.instance.SaveData().vip);
    //    btnRemoveAdsLose.SetActive(!DataManager.instance.SaveData().vip);
    //}
    //void DisplayTicket()
    //{
    //    ticketText.text = "" + DataManager.instance.SaveData().totalTicket;
    //}

    public override void OpenMe()
    {
        base.OpenMe();
        if(thisUI != null)
        {
            thisUI.ShowNativeAds();
        }
        winPanel.SetActive(false);
        //effectFireWork.SetActive(false);
        //for (int i = 0; i < effectFireWork.transform.childCount; i++)
        //{
        //    effectFireWork.transform.GetChild(i).gameObject.SetActive(true);
        //}
        losePanel.SetActive(false);
        if (GamePlayManager.Instance.GetWin())
        {
            //int maxStarCanTake = GamePlayManager.Instance.GetMaxStarCanTake();
            winPanel.SetActive(true);
            winImg.sprite = GamePlayManager.Instance.GetSpriteEnd();
            //DisplayTicket();
            //DataParamManager.displayTicket += DisplayTicket;

            //for (int i = 0; i < lstStar.Length; i++)
            //{
            //    if (i < maxStarCanTake)
            //    {
            //        lstStar[i].SetActive(true);
            //    }
            //    else
            //    {
            //        lstStar[i].SetActive(false);
            //    }
            //}
            //if (maxStarCanTake == 1)
            //{
            //    desWinText.text = desWin1Star[Random.Range(0, desWin1Star.Length)];
            //}
            //else if (maxStarCanTake == 2)
            //{
            //    desWinText.text = desWin2Star[Random.Range(0, desWin2Star.Length)];
            //}
            //else if (maxStarCanTake == 3)
            //{
            //    desWinText.text = desWin3Star[Random.Range(0, desWin3Star.Length)];
            //}
            MusicManager.instance.PlaySoundOtherOneShot(true, 3);
        }
        else
        {
            losePanel.SetActive(true);
            loseImg.sprite = GamePlayManager.Instance.GetSpriteEnd();
            //  desLoseText.text = desLose[Random.Range(0, desLose.Length)];

            MusicManager.instance.PlaySoundOtherOneShot(true, 2);
            EventManager.LOSELEVEL();
        }
        //CheckDisplayBtnRemoveAds();
        //DataParamManager.displayRemoveAds += CheckDisplayBtnRemoveAds;

        if (DataParamManager.activeCountPlay)
        {
            DataManager.instance.SaveData().totalPlay++;
            DataParamManager.activeCountPlay = false;
            // Debug.LogError("==================== total play:" + DataManager.instance.SaveData().totalPlay);
        }

        AdsManager.instance.AutoShowBannerAdaptive();
    }

    public void BtnRemoveAds()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        DataManager.instance.ShowRemoveAdsPanel();

    }
    public void BtnNext()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();

        DataManager.instance.ShowInterAllGame("Next");
        DataManager.instance.ChangeSceneAfterCheckNextLevel();
    }
    public void BtnReplay()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        DataManager.instance.ShowLoadingPanel("Play");

        EventManager.REPLAYLEVEL();

        DataManager.instance.ShowInterAllGame("Replay");


    }
    public void BtnHome()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        DataManager.instance.ShowLoadingPanel("Menu");
        DataManager.instance.ShowInterAllGame("HomeEnd");

    }
    public void BtnSkip()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        AdsManager.instance.ShowVideoAds(RewardSkip, "btnlosenextlevel_" + (DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1));
    }
    void RewardSkip()
    {
        GamePlayManager.Instance.SkinFunc();
    }
}
