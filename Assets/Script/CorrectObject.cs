using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectObject : MonoBehaviour
{
    [SerializeField] SkeletonAnimation sa;
    Action ac;
    public void OnStart()
    {
        gameObject.SetActive(false);
        sa.AnimationState.Complete += Complete;
    }

    private void Complete(TrackEntry trackEntry)
    {
        gameObject.SetActive(false);
        if (ac != null)
        {
            ac();
            ac = null;
        }
    }

    public void OpenMe(Action _ac)
    {
        gameObject.SetActive(true);
        ac = _ac;
        sa.AnimationState.SetAnimation(0, "animation", false);
    }
}
