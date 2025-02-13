using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClickPlayAnim : ObjectDragParent
{
    [SerializeField] string nameAnim, nameIdleAfterPlayAnim;
    public override void DownFunc()
    {
        base.DownFunc();
        PlayAnim(nameAnim,false);
    }
    public override void DragFunc()
    {

    }
    public override void ChangePosition(Vector2 pos)
    {

    }
    public override void OnStart()
    {
        base.OnStart();
        GetSa().AnimationState.Complete += CompleteFunc;
    }

    private void CompleteFunc(TrackEntry trackEntry)
    {
        if(trackEntry.Animation.Name == nameAnim)
        {
            PlayAnim(nameIdleAfterPlayAnim, false);
            DoneMe();
            GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());
        }    
    }
}
