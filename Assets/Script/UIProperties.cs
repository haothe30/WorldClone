using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIProperties : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] List<Button> lstBtn = new List<Button>();
    [SerializeField] EventForEndAnim eventForEndAnim;
    [SerializeField] Animator animOfPopUp;
    [SerializeField] string nameForAnimPopUpOpen = "PopUpOpen", nameForAnimPopUpClose = "PopUpClose";
    [SerializeField] GameObject pointNativeAds;

    public void ShowNativeAds()
    {
        if (DataManager.instance.SaveData().removeAds || DataManager.instance.AnAds)
            return;
        if (pointNativeAds != null)
            AdsManager.instance.ActiveNativeAds(true, 0, pointNativeAds.transform);
    }
    public string GetNameForAnimPopUpOpen()
    {
        return nameForAnimPopUpOpen;
    }
    public string GetNameForAnimPopUpClose()
    {
        return nameForAnimPopUpClose;
    }
    public RectTransform GetRect()
    {
        return rect;
    }
    public Animator GetAnimOfPopUp()
    {
        return animOfPopUp;
    }
    public virtual void ActiveAllBtn(bool active)
    {
        if (lstBtn.Count == 0)
            return;
        for (int i = 0; i < lstBtn.Count; i++)
        {
            lstBtn[i].interactable = active;
        }
    }
    public void SetRect()
    {

        transform.localScale = Vector2.one;
        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.zero;
        rect.anchorMax = Vector3.one;
        rect.anchorMin = Vector3.zero;
        rect.pivot = Vector3.one / 2;
        rect.localPosition = Vector3.zero;
    }
    public virtual void OpenMe()
    {
        ActiveAllBtn(false);

        if (rect != null)
        {
            if (SceneManager.GetActiveScene().name == "Menu")
                transform.parent = MenuUIManager.Instance.transform;
            else if (SceneManager.GetActiveScene().name == "Play")
                transform.parent = GamePlayUIManager.Instance.transform;
            SetRect();
            transform.SetAsLastSibling();
        }

        gameObject.SetActive(true);


        if (animOfPopUp != null)
            animOfPopUp.Play(nameForAnimPopUpOpen);
    }
    public void PlayAnimPopUp()
    {
        if (animOfPopUp != null)
            animOfPopUp.Play(nameForAnimPopUpOpen);
    }
    public virtual void OpenMe(string value)
    {
        OpenMe();
    }
    public virtual void CloseMe()
    {
        gameObject.SetActive(false);
    }
    public void BeforeClose()
    {
        if (animOfPopUp != null)
        {
            animOfPopUp.Play(nameForAnimPopUpClose);
        }
        else
        {
            CloseMe();
        }
        ActiveAllBtn(false);
    }
    public virtual void BtnClose()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        BeforeClose();
    }
    public virtual void Load()
    {
        lstBtn.Clear();
        lstBtn.AddRange(transform.GetComponentsInChildren<Button>());
        eventForEndAnim = transform.GetComponentInChildren<EventForEndAnim>();
        if (eventForEndAnim != null)
            eventForEndAnim.SetMyUIProperties(this);
        animOfPopUp = transform.GetComponentInChildren<Animator>();

    }

}

