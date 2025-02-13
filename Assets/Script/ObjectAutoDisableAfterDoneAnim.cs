using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAutoDisableAfterDoneAnim : MonoBehaviour
{
    [SerializeField] SkeletonAnimation sa;
    [SerializeField] SkeletonGraphic sg;
    [SerializeField] string nameAnim;
    private void Start()
    {
        if (sa != null)
            sa.AnimationState.Complete += CompleteFunc;
        if (sg != null)
            sg.AnimationState.Complete += CompleteFunc;
    }

    private void CompleteFunc(TrackEntry trackEntry)
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (sa != null)
            sa.AnimationState.SetAnimation(0, nameAnim, false);
        if (sg != null)
            sg.AnimationState.SetAnimation(0, nameAnim, false);
    }
}
