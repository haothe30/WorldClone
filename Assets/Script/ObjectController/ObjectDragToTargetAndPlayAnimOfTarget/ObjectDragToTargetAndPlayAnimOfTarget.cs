using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragToTargetAndPlayAnimOfTarget : ObjectDragParent
{
    ObjectTargetOfObjectDragToTargetAndPlayAnimOfTarget _objectTarget;
    [SerializeField] int indexSoundPlayAnim = -1;
    [SerializeField] GameObject[] lstObjectDisplayWhenPlayAnim;
    [SerializeField] bool activeBoxTarget;
    [SerializeField] bool checkNearPlayAnim;
    [SerializeField] LayerMask layerMaskTemp;
    [SerializeField] string nameAnimNear;
    [SerializeField] ObjectTargetOfObjectDragToTargetAndPlayAnimOfTarget targetObjectPlayAnimNear;
    bool playAnim;
    public override void DownFunc()
    {
        base.DownFunc();

        if (GetIsWrong && activeBoxTarget)
        {
            for (int i = 0; i < GetLstObjectTarget().Count; i++)
            {
                GetLstObjectTarget()[i].GetMyCollider2D().enabled = true;
            }
        }
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDown());
    }
    //public override void OnStart()
    //{
    //    base.OnStart();

    //    if (activeBoxTarget)
    //    {
    //        for (int i = 0; i < GetLstObjectTarget().Count; i++)
    //        {
    //            GetLstObjectTarget()[i].GetMyCollider2D().enabled = true;
    //        }
    //    }
    //}
    public override void ActiveMe()
    {
        base.ActiveMe();
        GetIsWrong = false;
    }
    public override void DragFunc()
    {
        base.DragFunc();
        if (checkNearPlayAnim)
        {
            Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), layerMaskTemp);
            if (hit != null)
            {
                if (!playAnim)
                {
                    targetObjectPlayAnimNear.PlayAnimNear(true, nameAnimNear);
                    playAnim = true;
                }
            }
            else
            {
                if (playAnim)
                {
                    targetObjectPlayAnimNear.PlayAnimNear(false, "");
                    playAnim = false;
                }
            }
        }
    }
    //public override void OnStart()
    //{
    //    base.OnStart();
    //    for(int i = 0;i < GetLstObjectTarget().Count; i++)
    //    {
    //        GetLstObjectTarget()[i].GetMyCollider2D().enabled = true;
    //    }    
    //}
    public override void UpFunc()
    {
        base.UpFunc();
        Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), GetLayerMask());
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());
        if (hit != null)
        {
            _objectTarget = hit.GetComponent<ObjectTargetOfObjectDragToTargetAndPlayAnimOfTarget>();

            if (_objectTarget != null && GetLstObjectTarget().Contains(_objectTarget))
            {
                Vector3 _objectTargetPos = _objectTarget.GetPoint().transform.position;

                transform.DOMove(_objectTargetPos, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    MusicManager.instance.PlaySoundLevelOneShot(true, indexSoundPlayAnim);
                    _objectTarget.DisableLstObjectWhenPlayAnim();
                    _objectTarget.PlayAnim(DoneMe);
                    gameObject.SetActive(false);

                    for (int i = 0; i < lstObjectDisplayWhenPlayAnim.Length; i++)
                    {
                        lstObjectDisplayWhenPlayAnim[i].SetActive(true);
                    }
                });

            }
            else
            {
                GetMyCollider2D().enabled = true;
                if (GetAutoBackPosBeginAfterDone())
                {
                    AutoBackPosBegin();
                }
            }
        }
        else
        {
            GetMyCollider2D().enabled = true;

            if (GetAutoBackPosBeginAfterDone())
            {
                AutoBackPosBegin();
            }
            else
            {
                if(GetIsDone)
                {
                    AutoBackPosBegin();
                }    
            }    
        }

        if (checkNearPlayAnim)
        {
            if (playAnim)
            {
                targetObjectPlayAnimNear.PlayAnimNear(false, "");
                playAnim = false;
            }
        }

    }
    public override void DoneMe()
    {
        base.DoneMe();
        _objectTarget.ActiveLstObjectAfterDoneMe();
        _objectTarget.DisableLstObjectAfterDoneMe();
        //if(GetAutoBackPosBeginAfterDone())
        //{
        //    transform.position = GetPosBegin;
        //}    
        GetIntValue--;
        if (GetIntValue > 0)
        {
            GetIsDone = false;
            GetMyCollider2D().enabled = true;
        }
        if (GetIsDone)
        {
            AutoBackPosBegin();

            GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());

            if (GetLstNextObjectActiveAfterDone.Count > 0)
                GamePlayManager.Instance.GetLevelController().ActiveNextObect();
            else
                GamePlayManager.Instance.CheckWin();
        }
    }
}
