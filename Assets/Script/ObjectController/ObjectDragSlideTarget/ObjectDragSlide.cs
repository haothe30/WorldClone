using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragSlide : ObjectDragParent
{
    ObjectTargetSlide objectTargetSlide = null;
    bool canChangeToDone, playAnim;
    [SerializeField] string nameAnim, nameIdle;
    [SerializeField] string nameSkinStatic, nameSkinDrag;
    [SerializeField] LayerMask lmForBoxSlide;
    [SerializeField] int indexSoundSlide = -1;
    void CheckClearBox()
    {
        if (!canChangeToDone)
        {
            Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), lmForBoxSlide);
            if (hit != null)
            {
                hit.gameObject.SetActive(false);
                if (objectTargetSlide == null)
                {
                    objectTargetSlide = hit.transform.parent.GetComponent<ObjectTargetSlide>();
                }

                if (objectTargetSlide != null)
                {
                    if (objectTargetSlide.CheckDone())
                    {
                        canChangeToDone = true;
                        //MusicManager.instance.PlaySoundLevelLoop(false, -1);
                        //PlayAnim(nameIdle, true);
                        Debug.LogError("======================== Can Done Me");
                    }
                }
            }
        }
    }
    public override void DownFunc()
    {
        base.DownFunc();

        GetSa().Skeleton.SetSkin(nameSkinDrag);
        GetSa().Skeleton.SetSlotsToSetupPose();
        GetSa().Update(0);

    }
    public override void DragFunc()
    {

        Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), GetLayerMask());
        if (hit != null)
        {
            if (!playAnim)
            {
                MusicManager.instance.PlaySoundLevelLoop(true, indexSoundSlide);
                PlayAnim(nameAnim, true);
                playAnim = true;
            }
        }
        else
        {
            if (playAnim)
            {
                MusicManager.instance.PlaySoundLevelLoop(false, -1);
                PlayAnim(nameIdle, true);
                playAnim = false;
            }
        }

        CheckClearBox();
    }
    public override void UpFunc()
    {
        base.UpFunc();

        AutoBackPosBegin();

        GetSa().Skeleton.SetSkin(nameSkinStatic);
        GetSa().Skeleton.SetSlotsToSetupPose();
        GetSa().Update(0);

        PlayAnim(nameIdle, true);
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());
        MusicManager.instance.PlaySoundLevelLoop(false, -1);
        if (canChangeToDone)
        {
            Debug.LogError("======================== Done Me");
            DoneMe();
        }

        objectTargetSlide = null;

        playAnim = false;
        canChangeToDone = false;

    }
    public override void UpFuncBecauseEndTime()
    {
        base.UpFuncBecauseEndTime();
        UpFunc();
    }
    public override void DoneMe()
    {
        Debug.LogError("======================== Done Me");
        objectTargetSlide.ActiveLstObjectAfterDoneMe();

        for (int i = 0; i < GetLstObjectDisableAfterDone().Length; i++)
        {
            GetLstObjectDisableAfterDone()[i].SetActive(false);
        }
        for (int i = 0; i < GetLstObjectActiveAfterDone().Length; i++)
        {
            GetLstObjectActiveAfterDone()[i].SetActive(true);
        }
        GetIntValue--;
        if (GetIntValue == 0)
        {
            GetIsDone = true;
            GamePlayManager.Instance.CheckWin();
        }
    }
}
