using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopUpMess : UnitParent
{
    [SerializeField] TextNeedTranslate desText; 
    [SerializeField] Animator anim;
    public override void OnStart(Vector3 scale, Vector2 pos, string des)
    {
        base.OnStart(scale, pos, des);
        // desText.GetLocalize().Term = des;
        desText.GetTextMesh().text = des;
    }

}
