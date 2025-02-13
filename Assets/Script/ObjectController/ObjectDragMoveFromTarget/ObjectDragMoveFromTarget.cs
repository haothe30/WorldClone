using DG.Tweening;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragMoveFromTarget : ObjectDragParent
{
    [System.Serializable]
    public class InfoLstNextObjectAtiveAfterDone
    {
        public ObjectDragParent[] arrayNextObjectActiveAfterDone;
        public GameObject[] lstObjectActiveAfterDone, lstObjectDisableWhenPlayAnim;

    }

    ObjectTarget _objectTarget;
    [SerializeField] SkeletonAnimation saNeedChange;
    [SerializeField] int[] indexSoundForSaNeedChange;
    [SerializeField] string[] nameAnim, nameSkin;

    [SerializeField] InfoLstNextObjectAtiveAfterDone[] infoLstNextObjectActiveAfterDone;

    int currentStep;
    public override void DownFunc()
    {
        base.DownFunc();
        if (GetIntValue == 0)
        {
            GetLstObjectTarget()[0].GetMyCollider2D().enabled = false;
        }
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDown());
    }
    public override void ActiveMe()
    {
        base.ActiveMe();
        GetIsDone = false;
        GetLstNextObjectActiveAfterDone.Clear();
        GetLstNextObjectActiveAfterDone.AddRange(infoLstNextObjectActiveAfterDone[currentStep].arrayNextObjectActiveAfterDone);
    }

    public override void DoneMe()
    {

        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
        }

        if (GetIntValue == 1)
        {
            base.DoneMe();
            currentStep++;
            Vector3 _objectTargetPos = _objectTarget.GetPoint().transform.position;
            transform.DOMove(_objectTargetPos, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (saNeedChange != null)
                {
                    MusicManager.instance.PlaySoundLevelOneShot(true, indexSoundForSaNeedChange[currentStep - 1]);

                    DataParamManager.playingAnim = true;
                    saNeedChange.Skeleton.SetSkin(nameSkin[1]);
                    saNeedChange.Skeleton.SetSlotsToSetupPose();
                    saNeedChange.Update(0);


                    for (int i = 0; i < infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectDisableWhenPlayAnim.Length; i++)
                    {
                        infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectDisableWhenPlayAnim[i].SetActive(false);
                    }

                    saNeedChange.AnimationState.SetAnimation(0, nameAnim[1], false).Complete += (TrackEntry e) =>
                    {
                        GetIntValue = 0;
                        GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());
                        GamePlayManager.Instance.GetLevelController().ActiveNextObect();
                        saNeedChange.AnimationState.SetAnimation(0, "Idle", false);
                        saNeedChange.Skeleton.SetSkin("default");
                        saNeedChange.Skeleton.SetSlotsToSetupPose();
                        saNeedChange.Update(0);

                        for (int i = 0; i < infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectActiveAfterDone.Length; i++)
                        {
                            infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectActiveAfterDone[i].SetActive(true);
                        }
                        DataParamManager.playingAnim = false;
                        if (GetDisplayHeartDone())
                        {
                            GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                        }
                    };
                }
                else
                {
                    if (GetDisplayHeartDone())
                    {
                        GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                    }
                    GetIntValue = 0;
                    GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());
                    GamePlayManager.Instance.GetLevelController().ActiveNextObect();
                }
            });
            GetMyCollider2D().enabled = false;
        }
        else
        {
            if (Vector2.Distance(transform.position, GetLstObjectTarget()[0].transform.position) >= 0.5f)
            {
                base.DoneMe();
                currentStep++;
                if (saNeedChange != null)
                {
                    MusicManager.instance.PlaySoundLevelOneShot(true, indexSoundForSaNeedChange[currentStep - 1]);

                    DataParamManager.playingAnim = true;
                    saNeedChange.Skeleton.SetSkin(nameSkin[0]);
                    saNeedChange.Skeleton.SetSlotsToSetupPose();
                    saNeedChange.Update(0);

                    for (int i = 0; i < infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectDisableWhenPlayAnim.Length; i++)
                    {
                        infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectDisableWhenPlayAnim[i].SetActive(false);
                    }

                    saNeedChange.AnimationState.SetAnimation(0, nameAnim[0], false).Complete += (TrackEntry e) =>
                    {
                        GetIntValue = 1;
                        GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());
                        GamePlayManager.Instance.GetLevelController().ActiveNextObect();
                        saNeedChange.AnimationState.SetAnimation(0, "Idle", false);
                        saNeedChange.Skeleton.SetSkin("default");
                        saNeedChange.Skeleton.SetSlotsToSetupPose();
                        saNeedChange.Update(0);

                        for (int i = 0; i < infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectActiveAfterDone.Length; i++)
                        {
                            infoLstNextObjectActiveAfterDone[currentStep - 1].lstObjectActiveAfterDone[i].SetActive(true);
                        }
                        DataParamManager.playingAnim = false;
                        if (GetDisplayHeartDone())
                        {
                            GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                        }
                    };
                }
                else
                {
                    if (GetDisplayHeartDone())
                    {
                        GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                    }
                    GetIntValue = 1;
                    GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());
                    GamePlayManager.Instance.GetLevelController().ActiveNextObect();
                }
                GetMyCollider2D().enabled = false;
            }
        }
    }
    public override void UpFunc()
    {
        base.UpFunc();
        Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), GetLayerMask());
        ChangeSortingLayer(GetOrderLayerUp);
        if (hit != null)
        {
            if (GetIntValue == 1)
            {
                _objectTarget = hit.GetComponent<ObjectTarget>();
                if (GetLstObjectTarget().Contains(_objectTarget))
                {
                    DoneMe();
                }
            }
        }
        else
        {
            GetMyCollider2D().enabled = true;
            if (GetIntValue == 0)
            {
                DoneMe();
            }
            else if (GetIntValue == 1)
            {
                if (GetAutoBackPosBeginAfterDone())
                {
                    AutoBackPosBegin();
                }
            }
            // Debug.LogError("======================= ????????");
        }

        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());
    }
    public override void SkipFunc()
    {
        base.SkipFunc();
        base.DoneMe();
        ChangeSortingLayer(GetOrderLayerUp);
        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
        }
        for (int i = 0; i < infoLstNextObjectActiveAfterDone.Length; i++)
        {
            for (int j = 0; j < infoLstNextObjectActiveAfterDone[i].lstObjectActiveAfterDone.Length; j++)
            {
                infoLstNextObjectActiveAfterDone[i].lstObjectActiveAfterDone[j].SetActive(true);
            }

        }
        transform.position = GetLstObjectTarget()[0].GetPoint().transform.position;
        transform.rotation = Quaternion.identity;
        //  GetLstObjectTarget()[0].myObjectDrag = this;
    }
}
