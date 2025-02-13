using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RatingManager : UIProperties
{
    DataManager dataController;
    [SerializeField] GameObject[] star;
    int maxStar;
    public override void CloseMe()
    {
        base.CloseMe();
        if(DataManager.instance.GetEndPanel() != null && DataManager.instance.GetEndPanel().gameObject.activeSelf)
        {
            DataManager.instance.GetEndPanel().ShowNativeAds();
        }    
    }
    public override void OpenMe()
    {
        base.OpenMe();
        if (dataController == null)
            dataController = DataManager.instance;
        maxStar = 5;
        DisplayStar();
    }
    void DisplayStar()
    {
        for(int i = 0; i < star.Length; i ++)
        {
            if(i < maxStar)
            {
                star[i].SetActive(true);
            }
            else
            {
                star[i].SetActive(false);
            }
        }
    }

    public void BtnRate()
    {
        if (dataController.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        if (maxStar == 5)
        {
            EventManager.instance.RateAndReview();
        }
        BeforeClose();
    }
    public void BtnSelectStar(int _maxStar)
    {
        if (dataController.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        maxStar = _maxStar;
        DisplayStar();
    }

 
}
#if UNITY_EDITOR
[CustomEditor(typeof(RatingManager))]
public class RatingManagerEditor : Editor
{
    public bool isCheck;
    private RatingManager myScript;

    private void OnSceneGUI()
    {
        myScript = (RatingManager)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (myScript == null)
            myScript = (RatingManager)target;
        GUIStyle SectionNameStyle = new GUIStyle();
        SectionNameStyle.fontSize = 16;
        SectionNameStyle.normal.textColor = Color.blue;

        if (myScript == null) return;

        EditorGUILayout.LabelField("----------\t----------\t----------\t----------\t----------", SectionNameStyle);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            if (GUILayout.Button("Load", GUILayout.Height(50)))
            {
                isCheck = true;
                myScript.Load();
            }
        }
        EditorGUILayout.EndVertical();
        if (isCheck)
        {
            EditorUtility.SetDirty(myScript);
            isCheck = false;
        }
        //
    }
}
#endif