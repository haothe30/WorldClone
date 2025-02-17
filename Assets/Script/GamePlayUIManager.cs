
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayUIManager : UIParent
{
    public static GamePlayUIManager Instance;
    [SerializeField] Text timeText, levelText;
    [SerializeField] GameObject allUI;
    [SerializeField] GameObject  iconVideoHint, boxButton, doneDecorIcon;
    [SerializeField] Animator btnAddTime;
    [SerializeField] Image fillIMG;
    public GameObject GetBtnAddTime()
    {
        return btnAddTime.gameObject;
    }
    public void SetAnimForBtnAddTime(bool play)
    {
        if (play)
        {
            btnAddTime.Play("BtnAddTime");
        }
        else
        {
            btnAddTime.Play("BtnAddTimeIdle");
        }
    }
    public Text GetTimeText()
    {
        return timeText;
    }
    private void Awake()
    {
        Instance = this;
    }
    public override void Start()
    {
        base.Start();
        levelText.text = "Level " + DataManager.instance.GetCurrentLevel().indexPrefab;
    }
    public void DisableAllUI()
    {
        allUI.SetActive(false);
    }
    public void BtnNextStep()
    {
        if (DataManager.instance.CanNotAction() || DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY)
            return;
        MusicManager.instance.SoundClickButton();

        DataManager.instance.ShowInterAllGame("BtnNextStep");
    }    
    public void BtnSkip()
    {
        if (DataManager.instance.CanNotAction() || DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY)
            return;
        MusicManager.instance.SoundClickButton();
        GamePlayManager.Instance.ChangeStageDisplayPopUp(true);
        DataManager.instance.ShowSkipPopUp();

        DataManager.instance.ShowInterAllGame("BtnSkip");
    }
    public void BtnHint()
    {
        if (DataManager.instance.CanNotAction() || DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY)
            return;
        MusicManager.instance.SoundClickButton();
        if (iconVideoHint.activeSelf)
        {
            if (AdsManager.instance.CheckVideoReady())
            {
                GamePlayManager.Instance.ChangeStageDisplayPopUp(true);
                AdsManager.instance.ShowVideoAds(RewardHint, "Hint_" + (DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1));
            }
            else
            {
                DataManager.instance.ShowPopUpMess(Vector3.one, Vector2.zero, "Ads not available");
            }
        }
        else
        {
            RewardHint();
        }
    }
    float countTemp;
    int currentDecor;
    public void CaculateProcessnig()
    {
        currentDecor++;
        DOTween.To(() => countTemp, x => countTemp = x, currentDecor, 0.5f).OnUpdate(() =>
        {
            fillIMG.fillAmount = countTemp / GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Count;
            if (fillIMG.fillAmount >= 1)
            {
                if(!doneDecorIcon.activeSelf)
                doneDecorIcon.SetActive(true);
            }
        });
    }
    void RewardHint()
    {
        GamePlayManager.Instance.HintFunc();
        iconVideoHint.SetActive(false);
    }
    public void BtnPause()
    {
        if (DataManager.instance.CanNotAction() || DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY)
            return;
        MusicManager.instance.SoundClickButton();
        GamePlayManager.Instance.ChangeStageDisplayPopUp(true);
        DataManager.instance.ShowPausePopUp();

    }
    public void BtnAddTime()
    {
        if (DataManager.instance.CanNotAction() || DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY)
            return;
        MusicManager.instance.SoundClickButton();

        if (AdsManager.instance.CheckVideoReady())
        {
            GamePlayManager.Instance.ChangeStageDisplayPopUp(true);
            AdsManager.instance.ShowVideoAds(CallAddTime, "AddTime_" + (DataManager.instance.GetCurrentLevel().indexLevel + 1) + "_pf_" + (DataManager.instance.GetCurrentLevel().indexPrefab + 1));
        }
        else
        {
            DataManager.instance.ShowPopUpMess(Vector3.one, Vector2.zero, "Ads not available");
        }
    }
    void CallAddTime()
    {
        GamePlayManager.Instance.AddMaxTime(60);
#if UNITY_EDITOR
        GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
#endif
    }
}
