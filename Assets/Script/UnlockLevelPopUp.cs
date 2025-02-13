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
    SlotLevelUnit slotLevelUnit;
    [SerializeField] Image iconNormal, iconSpecial;
    public void SetInfoLevelNeedUnlock(LevelInfo _levelInfo, SlotLevelUnit _slotLevelUnit)
    {
        levelInfo = _levelInfo;
        slotLevelUnit = _slotLevelUnit;
        specialBouder.SetActive(false);
        normalBouder.SetActive(false);
        if (levelInfo.totalStarToUnlock > 0)
        {
            starNeedUnlockText.text = "Star to unlock: " + levelInfo.totalStarToUnlock;
            levelTextSpecial.text = "Level " + (levelInfo.indexLevel + 1);
            iconSpecial.sprite = slotLevelUnit.GetSpriteIcon();
            specialBouder.SetActive(true);
        }
        else
        {
            unlockPreviousLevelText.text = "Need finish Lv " + levelInfo.indexLevel;
            levelTextNormal.text = "Level " + (levelInfo.indexLevel + 1);
            iconNormal.sprite = slotLevelUnit.GetSpriteIcon();
            normalBouder.SetActive(true);
        }
    }
    public void BtnUnlockLevel()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        AdsManager.instance.ShowVideoAds(Reward, "UnlockLevel_" + (levelInfo.indexLevel + 1) + "_pf_" + (levelInfo.indexPrefab + 1));
    }
    void Reward()
    {
        DataManager.instance.SaveData().lstSaveLevel[levelInfo.indexLevel].stage = 1;
        slotLevelUnit.DisplayUnlock();
        BeforeClose();
    }
    public override void CloseMe()
    {
        base.CloseMe();

        if (DataManager.instance.SaveData().lstSaveLevel[levelInfo.indexLevel].stage > 0)
            slotLevelUnit.AutoPlay();
    }
}
