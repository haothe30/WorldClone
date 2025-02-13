using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spBg;
    DataManager dataController;
    void Start()
    {
        if (dataController == null)
            dataController = DataManager.instance;
        float width = 7.2f;
        float height = 12.8f;
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        float targetAspect = width / height;
        float screenAspect = worldScreenWidth / worldScreenHeight;
        float padding = 1.01f;
        Vector3 scaleTemp = Vector3.one;
        if (screenAspect > targetAspect)
        {
            scaleTemp.x = (worldScreenWidth / width) * padding;
            scaleTemp.y = scaleTemp.x;
        }
        else
        {
            scaleTemp.y = (worldScreenHeight / height) * padding;
            scaleTemp.x = scaleTemp.y;
        }
        scaleTemp.z = 1;
        spBg.transform.localScale = scaleTemp;
    }
}
