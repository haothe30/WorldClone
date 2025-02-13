using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIParent : MonoBehaviour
{
    DataManager dataController;



    [Header("Hack")]
    [SerializeField] GameObject btnHack;
    [SerializeField] Image[] imgAn;
    [SerializeField] GameObject[] objAn;
    [SerializeField] Text[] textAn;
    Color colorAn = Color.white;



    public DataManager GetDataManager()
    {
        return dataController;
    }
    public virtual void Awake()
    {
        dataController = DataManager.instance;
    }
    public virtual void Start()
    {

        if (btnHack != null && dataController != null)
            btnHack.SetActive(dataController.GetHack());
        AnUI();
    }
    public void BtnSetting()
    {
        if (GetDataManager().CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        GetDataManager().ShowPausePopUp();
    }

    public void BtnHack()
    {
        if (GetDataManager().CanNotAction())
            return;
        GetDataManager().ShowHackPopUp();
    }

    public void AnUI()
    {
        if (dataController == null)
            return;
        if (dataController.AnUI)
        {
            colorAn.a = 0;
            for (int i = 0; i < objAn.Length; i++)
            {
                objAn[i].SetActive(false);
            }
            for (int i = 0; i < imgAn.Length; i++)
            {
                imgAn[i].color = colorAn;
            }
            for (int i = 0; i < textAn.Length; i++)
            {
                textAn[i].color = colorAn;
            }
        }
        else
        {
            colorAn.a = 1;
            for (int i = 0; i < objAn.Length; i++)
            {
                objAn[i].SetActive(true);
            }
            for (int i = 0; i < imgAn.Length; i++)
            {
                imgAn[i].color = colorAn;
            }
            for (int i = 0; i < textAn.Length; i++)
            {
                textAn[i].color = colorAn;
            }
        }
    }
}
