using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEndLevel : MonoBehaviour
{
    [SerializeField] Animator animEndLevel;
    [SerializeField] SkeletonAnimation sa;
    Action ac;
    [SerializeField] int indexSound = -1;
    [SerializeField] string nameAnim;
    [SerializeField] GameObject gameObjectDisplayAfterAnim, objDisableWhenPlayAnim;
    [SerializeField] bool delayChutRoiMoiHienThi;
    public bool GetDelayChutRoiMoiHienThi()
    {
        return delayChutRoiMoiHienThi;
    }    
    public void OnAwake()
    {
        if (animEndLevel != null)
            animEndLevel.enabled = false;
        if (sa != null)
            sa.AnimationState.Complete += Complete;
    }

    private void Complete(TrackEntry trackEntry)
    {
        EventEnd();
    }

    public void OpenMe(Action _ac)
    {
        if (animEndLevel != null)
            animEndLevel.enabled = true;
        if (sa != null)
            sa.AnimationState.SetAnimation(0, nameAnim, true);
        ac = _ac;
        if (objDisableWhenPlayAnim != null)
            objDisableWhenPlayAnim.SetActive(false);
        MusicManager.instance.PlaySoundLevelOneShot(true, indexSound);
        gameObject.SetActive(true);
    }
    public void EventEnd()
    {
        if (ac != null)
        {
            Debug.LogError("=================== ??????");
            ac();
            ac = null;
            if (gameObjectDisplayAfterAnim != null)
                gameObjectDisplayAfterAnim.SetActive(true);
        }
    }
}
