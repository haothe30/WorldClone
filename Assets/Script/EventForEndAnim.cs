using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventForEndAnim : MonoBehaviour
{
    [SerializeField] UIProperties myUIProperties;
    [SerializeField] GameObject objectDisplay;
    [SerializeField] int indexSound = -1;
    public void SetMyUIProperties(UIProperties _myUIProperties)
    {
        myUIProperties = _myUIProperties;
    }
    public void EventEnableBtn()
    {
        if (myUIProperties != null)
        {
            myUIProperties.ActiveAllBtn(true);
            myUIProperties.ShowNativeAds();
        }
    }
    public void EventClosePopUp()
    {
        myUIProperties.CloseMe();
    }
    public void EventEndAnimDisplayObj()
    {
        if (objectDisplay != null)
            objectDisplay.SetActive(true);
        MusicManager.instance.PlaySoundOtherOneShot(true, indexSound);

        if (myUIProperties != null)
        {
            myUIProperties.ShowNativeAds();

            if (!DataManager.instance.SaveData().rated)
            {
                if (DataManager.instance.GetCurrentLevel().indexLevel >= 1)
                {
                    if (myUIProperties == DataManager.instance.GetEndPanel())
                    {
                        if (GamePlayManager.Instance.GetWin())
                        {
                            DataManager.instance.ShowRatePopUp();
                        }
                    }
                }
            }

        }

    }
    public void EventEndAnimDisableObj()
    {
        if (objectDisplay != null)
            objectDisplay.SetActive(false);
    }
}
