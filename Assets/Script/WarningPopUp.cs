using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopUp : UIProperties
{
    [SerializeField] Image icon;
    [SerializeField] RectTransform iconRect;
    [SerializeField] Text rewardText;
    public void SetUp(Sprite _icon, string _reward)
    {
        icon.sprite = _icon;

        if (string.IsNullOrEmpty(_reward))
        {
            rewardText.gameObject.SetActive(false);
            iconRect.transform.localPosition = new Vector2(0, -30);
            iconRect.sizeDelta = new Vector2(300, 300);
        }
        else
        {
            rewardText.text = "x" + _reward;
            rewardText.gameObject.SetActive(true);
            iconRect.transform.localPosition = new Vector2(0, 0);
            iconRect.sizeDelta = new Vector2(250, 250);
        }

        MusicManager.instance.PlaySoundOtherOneShot(true, 4);
    }
}
