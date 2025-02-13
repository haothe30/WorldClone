using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HintPopUp : UIProperties
{
    [SerializeField] Image icon;
    public override void OpenMe()
    {
        base.OpenMe();
        icon.sprite = DataManager.instance.GetDataSpriteLevel().lstDataSpriteLevel[DataManager.instance.GetCurrentLevel().indexPrefab].hint;
    }
    public override void CloseMe()
    {
        base.CloseMe();
        if (SceneManager.GetActiveScene().name == "Play")
        {
            GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
        }
    }
}
