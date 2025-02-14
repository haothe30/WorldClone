using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedScrollerDemos.GridSimulation;
using UnityEngine.UI;
public class SlotLevel : MonoBehaviour
{
    [SerializeField] GameObject container, lockObj;
    [SerializeField] GameObject[] stars;
    [SerializeField] Image icon;
    [SerializeField] Text desText, totalStarUnlockText;

    public int index;
    public int indexTemp;
    public Sprite GetSpriteIcon()
    {
        return icon.sprite;
    }
    public void SetData(Data data)
    {
        container.gameObject.SetActive(data != null);
        if (data != null)
        {
            index = int.Parse(data.someText);
            if (DataManager.instance.GetDataLevel().levelInfo[index].indexLevel >= 0)
            {
                ActiveMe(DataManager.instance.GetDataLevel().levelInfo[index].indexPrefab);
            }
            else
            {
                // coming soon;
            }
        }
       
    }
    public void BtnClick()
    {
        if (lockObj.activeSelf)
        {
            DataManager.instance.ShowLevelUnlockPopUp(DataManager.instance.GetListLevelInfo()[index], this);
        }
        else
        {
            MusicManager.instance.SoundClickButton();
            DataManager.instance.SetCurrentLevel(index);
            DataManager.instance.ShowLoadingPanel("Play");
            DataManager.instance.ShowInterAllGame("SelectLevel");
        }
    }
    public void DisplayUnlock()
    {
        lockObj.SetActive(false);
    }
    public void AutoPlay()
    {
        DataManager.instance.SetCurrentLevel(index);
        DataManager.instance.ShowLoadingPanel("Play");
    }
    public void ActiveMe(int indexPrefab)
    {

        for (int i = 0; i < DataManager.instance.GetListLevelInfo().Count; i++)
        {
            if (DataManager.instance.GetListLevelInfo()[i].indexPrefab == indexPrefab)
            {
                indexTemp = DataManager.instance.GetListLevelInfo()[i].indexLevel;
            }
        }

        if (indexPrefab >= 0)
        {
            lockObj.SetActive(false);
            icon.sprite = DataManager.instance.GetDataSpriteLevel().lstDataSpriteLevel[index].selectLevel;
            desText.text = "" + (indexTemp + 1);
            if (DataManager.instance.SaveData().lstSaveLevel[indexTemp].stage == 0)
            {
                lockObj.SetActive(true);
            }
           
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
}
