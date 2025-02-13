using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotLevelUnit : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] Text desText, totalStarUnlockText;
    [SerializeField] ManagerSlotUnit managerSLotUnit;
    [SerializeField] GameObject lockObj/*, doneObj*/, bouderStarUnlock;
    [SerializeField] GameObject[] stars;
    [SerializeField] Image icon;
    int indexLevel;
    int indexTemp;
    public Sprite GetSpriteIcon()
    {
        return icon.sprite;
    }
    public void ActiveMe()
    {
        indexLevel = DataManager.instance.GetDataLevel().lstDataLevel[managerSLotUnit.slotLevel.index].levelInfo[index].indexPrefab;

        for (int i = 0; i < DataManager.instance.GetListLevelInfo().Count; i++)
        {
            if (DataManager.instance.GetListLevelInfo()[i].indexPrefab == indexLevel)
            {
                indexTemp = DataManager.instance.GetListLevelInfo()[i].indexLevel;
            }
        }

        if (indexLevel >= 0)
        {
            lockObj.SetActive(false);
            icon.sprite = DataManager.instance.GetDataSpriteLevel().lstDataSpriteLevel[indexLevel].selectLevel;
            //   doneObj.SetActive(false);

            desText.text = "" + (indexTemp + 1);
            if (DataManager.instance.SaveData().lstSaveLevel[indexTemp].stage == 0)
            {
                lockObj.SetActive(true);
                if (DataManager.instance.GetListLevelInfo()[indexTemp].totalStarToUnlock > 0)
                {
                    totalStarUnlockText.text = "Star To Unlock: " + DataManager.instance.GetListLevelInfo()[indexTemp].totalStarToUnlock;
                    bouderStarUnlock.SetActive(true);
                }
                else
                {
                    bouderStarUnlock.SetActive(false);
                }
            }
            //else
            //{
            //    if (DataManager.instance.SaveData().lstSaveLevel[indexLevel].stage == 2)
            //    {
            //        doneObj.SetActive(true);
            //    }
            //}
            for (int i = 0; i < stars.Length; i++)
            {
                if (i < DataManager.instance.SaveData().lstSaveLevel[indexTemp].maxStar)
                {
                    stars[i].SetActive(true);
                }
                else
                {
                    stars[i].SetActive(false);
                }
            }
            gameObject.SetActive(true);

        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void DisplayUnlock()
    {
        lockObj.SetActive(false);
    }
    public void BtnClick()
    {
        if (lockObj.activeSelf)
        {
            DataManager.instance.ShowLevelUnlockPopUp(DataManager.instance.GetListLevelInfo()[indexTemp], this);
        }
        else
        {
            MusicManager.instance.SoundClickButton();
            DataManager.instance.SetCurrentLevel(indexTemp);
            DataManager.instance.ShowLoadingPanel("Play");
            DataManager.instance.ShowInterAllGame("SelectLevel");
        }
    }
    public void AutoPlay()
    {
        DataManager.instance.SetCurrentLevel(indexTemp);
        DataManager.instance.ShowLoadingPanel("Play");
    }

}
