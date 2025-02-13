using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;
using DG.Tweening;

public class TextNeedTranslate : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] TextMeshProUGUI textMeshUI;
    [SerializeField] TextMeshPro textMeshNormal;
    [SerializeField] Localize localize;
    [SerializeField] RectTransform textContainer, mask;
    Vector2 /*textSize,*/scaleTemp;
    Color colorTemp;
    //float colorCalculate;
    [SerializeField] float paddingText;
    public Localize GetLocalize()
    {
        return localize;
    }
    public TextMeshProUGUI GetTextMeshUI()
    {
        return textMeshUI;
    }
    public TextMeshPro GetTextMesh()
    {
        return textMeshNormal;
    }
    public Text GetTextUI()
    {
        return text;
    }

    public Vector2 SizeDeltaMask
    {
        get { return mask.sizeDelta; }
        set
        {
            mask.sizeDelta = value;
        }
    }    
    public void SetColorForText(float color)
    {
        colorTemp = text.color;
        colorTemp.a = color;
        text.color = colorTemp;
    }
    public void CalculateContainerSize()
    {

        if (textContainer == null)
            return;
        scaleTemp.x = mask.sizeDelta.x;
        scaleTemp.y = text.preferredHeight;


        mask.sizeDelta = scaleTemp;

        Vector2 textScaleTemp = new Vector2(textContainer.sizeDelta.x, scaleTemp.y);

        textContainer.sizeDelta = textScaleTemp;


        textContainer.anchorMin = new Vector2(0.5f, 1f); // Neo ở giữa trên
        textContainer.anchorMax = new Vector2(0.5f, 1f); // Neo ở giữa trên
        textContainer.pivot = new Vector2(0.5f, 1f);     // Điểm xoay cũng ở giữa trên

        textContainer.anchoredPosition = new Vector2(paddingText, 0); // Đặt vị trí offset từ điểm neo


        //textContainer.offsetMin = Vector3.zero;
        //textContainer.offsetMax = Vector3.zero;
        //textContainer.anchorMax = Vector3.one;
        //textContainer.anchorMin = Vector3.zero;
        //textContainer.pivot = Vector3.one / 2;

        //  Debug.LogError("============ :" + textSize);
        SetColorForText(1);

        //  colorCalculate = colorTemp.a;

        //DOTween.To(() => colorCalculate, x => colorCalculate = x, 1, 0.5f).OnUpdate(() =>
        //{
        //    colorTemp.a = colorCalculate;
        //    textMeshUI.color = colorTemp;
        //}).OnComplete(() =>
        //{
        //    textMeshUI.color = colorTemp;
        //});
    }
}
