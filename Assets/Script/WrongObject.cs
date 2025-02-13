using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongObject : MonoBehaviour
{
    [SerializeField] SkeletonAnimation sa;
    public void OnStart()
    {
        gameObject.SetActive(false);
        sa.AnimationState.Complete+=Complete;
    }

    private void Complete(TrackEntry trackEntry)
    {
        gameObject.SetActive(false);
    }

    public void OpenMe()
    {
        gameObject.SetActive(true);
        sa.AnimationState.SetAnimation(0, "animation", false);
    }
}
