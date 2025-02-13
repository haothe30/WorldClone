using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningInternet : UIProperties
{
    public override void OpenMe()
    {
        ActiveAllBtn(false);

        gameObject.SetActive(true);



        if (GetAnimOfPopUp() != null)
            GetAnimOfPopUp().Play(GetNameForAnimPopUpOpen());

        if (SceneManager.GetActiveScene().name == "Play")
        {
            GamePlayManager.Instance.ChangeStageDisplayPopUp(true);
        }
    }
    public override void BtnClose()
    {

        if (SceneManager.GetActiveScene().name == "Loading")
        {
            Application.Quit();
        }
        else
        {
            if (DataManager.instance.CheckNotInterNet())
            {
                //DataManager.instance.ShowPopUpMess(Vector3.one, Vector2.zero, "Check your internet");
            }
            else
            {
                MusicManager.instance.SoundClickButton();
                BeforeClose();
            }
        }
    }
    public override void CloseMe()
    {
        base.CloseMe();
        DataManager.instance.CallCheckInternet();

        if (SceneManager.GetActiveScene().name == "Play")
        {
            if (!DataManager.instance.CheckPanelActiveInGP())
                GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
        }
    }
}
