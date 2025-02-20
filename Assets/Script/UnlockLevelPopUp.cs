using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DataLevel;

public class UnlockLevelPopUp : UIProperties
{
    [SerializeField] GameObject specialBouder, normalBouder;
    [SerializeField] Text starNeedUnlockText, unlockPreviousLevelText, levelTextNormal, levelTextSpecial;
    LevelInfo levelInfo;
    [SerializeField] Image iconNormal, iconSpecial;
    SlotLevel slotLevel;
    public void SetInfoLevelNeedUnlock(LevelInfo _levelInfo, SlotLevel _slotLevel)
    {
        levelInfo = _levelInfo;
        slotLevel = _slotLevel;
        specialBouder.SetActive(false);
        normalBouder.SetActive(false);

        // special level
        
        //if (levelInfo.totalStarToUnlock > 0)
        //{
        //    starNeedUnlockText.text = "Star to unlock: " + levelInfo.totalStarToUnlock;
        //    levelTextSpecial.text = "Level " + (levelInfo.indexLevel + 1);
        //    iconSpecial.sprite = slotLevel.GetSpriteIcon();
        //    specialBouder.SetActive(true);
        //}
        //else
        {
            unlockPreviousLevelText.text = "Need finish Lv " + levelInfo.indexLevel;
            levelTextNormal.text = "Level " + (levelInfo.indexLevel + 1);
            iconNormal.sprite = slotLevel.GetSpriteIcon();
            normalBouder.SetActive(true);
        }
    }
    public void BtnUnlockLevel()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        AdsManager.instance.ShowVideoAds(Reward, "btnunlockLevel_" + (levelInfo.indexLevel + 1) + "_pf_" + (levelInfo.indexPrefab + 1));
    }
    void Reward()
    {
        DataManager.instance.SaveData().lstSaveLevel[levelInfo.indexLevel].stage = 1;
        slotLevel.DisplayUnlock();
        BeforeClose();

    }
    public override void CloseMe()
    {
        base.CloseMe();

        if (DataManager.instance.SaveData().lstSaveLevel[levelInfo.indexLevel].stage > 0)
            slotLevel.AutoPlay();
    }
}
