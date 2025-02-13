using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    string nameNextScene;
    AsyncOperation currentLoadingOperation = null;
    [SerializeField] Image loadingFill;
    [SerializeField] Text progressText;
    DataManager dataController;
    bool showAoa;
    public void CreateBegin()
    {
        loadingFill.fillAmount = 0;
        progressText.text = "0%";
        percent = 0;
        progress = 0;
        dataController = DataManager.instance;

        if (dataController.SaveData().session == 1)
        {
            nameNextScene = "Play";
        }
        else
        {
            nameNextScene = "Menu";
        }
        currentLoadingOperation = SceneManager.LoadSceneAsync(nameNextScene);
        currentLoadingOperation.allowSceneActivation = false;
        showAoa = false;
    }



    float progress;
    int percent;
    private void Update()
    {
        if (dataController.CheckWarningInternetPanelActive())
            return;

        var deltaTime = Time.deltaTime;
        if (!showAoa)
        {
            loadingFill.fillAmount += deltaTime / 6;
        }
        else
        {
            loadingFill.fillAmount += deltaTime;
        }    
       
        progress = loadingFill.fillAmount / 1;
        percent = Mathf.RoundToInt(progress * 100);
        progressText.text = $"{percent}%";

        if (loadingFill.fillAmount >= 0.7f && loadingFill.fillAmount < 1)
        {
            if (!string.IsNullOrEmpty(nameNextScene))
            {
                if (!currentLoadingOperation.allowSceneActivation)
                {
                    currentLoadingOperation.allowSceneActivation = true;
                }
            }
            if(!showAoa)
            {
                dataController.SetData();
                if(dataController.SaveData().session >= 2)
                {
                    AdsManager.instance.ShowAOAAds("Loading");
                }    
                showAoa = true;
            }    
        }
        else if (loadingFill.fillAmount >= 1)
        {
            gameObject.SetActive(false);
            if(nameNextScene == "Play")
            {
                GamePlayManager.Instance.OnStart();
            }  
        }
    }

    public void OpenMe(string _nameNextScene)
    {
        nameNextScene = _nameNextScene;
        if (!string.IsNullOrEmpty(nameNextScene))
        {
            currentLoadingOperation = SceneManager.LoadSceneAsync(nameNextScene);
            currentLoadingOperation.allowSceneActivation = false;
        }
        loadingFill.fillAmount = 0;
        progressText.text = "0%";
        percent = 0;
        progress = 0;
        gameObject.SetActive(true);
        dataController.ChangeSceneFunc();
    }
}
