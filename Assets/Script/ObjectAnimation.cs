using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] string nameAnim, nameSkipFunc;
    Action ac;
    [SerializeField] int indexSound = -1;
    public void Start()
    {
        anim.Play(nameAnim, 0, 0f);
    }
    public void SkipFunc()
    {
        anim.Play(nameSkipFunc, 0, 0f);
    }
    public void OpenMe(string _nameAnim, Action _ac)
    {
        ac = _ac;
        if (ac != null)
        {
            DataParamManager.playingAnim = true;
        }
        MusicManager.instance.PlaySoundLevelOneShot(true, indexSound);
        anim.Play(_nameAnim, 0, 0f);
    }
    public void EventFuncClose()
    {
        if (ac != null)
        {
            ac();
            ac = null;
            DataParamManager.playingAnim = false;
        }
    }
}

