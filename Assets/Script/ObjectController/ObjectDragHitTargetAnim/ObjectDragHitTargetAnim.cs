using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragHitTargetAnim : ObjectDragParent
{
    [SerializeField] Animator anim;
    [SerializeField] string nameAnim, nameIdle;
    [SerializeField] bool loopAnim;
    [SerializeField] bool clickToHit;
    bool canChangeToDone;
    public override void OnAwake()
    {
        base.OnAwake();
        if (moveObjectBegin != MOVEOBJECTBEGIN.NONE)
        {
            gameObject.SetActive(false);
        }
    }
    public override void ActiveMe()
    {
        base.ActiveMe();
        canChangeToDone = false;
        GetIsDone = false;
        if (moveObjectBegin != MOVEOBJECTBEGIN.NONE)
        {
            Vector2 posMoveEndForDragObj = Vector2.zero;

            if (moveObjectBegin == MOVEOBJECTBEGIN.MOVEFROMRIGHT)
            {
                posMoveEndForDragObj = new Vector2(GamePlayManager.Instance.GetWidthCam() + 2, GetPosBegin.y);
            }
            else if (moveObjectBegin == MOVEOBJECTBEGIN.MOVEFROMLEFT)
            {
                posMoveEndForDragObj = new Vector2(-GamePlayManager.Instance.GetWidthCam() - 2, GetPosBegin.y);
            }

            ChangePosition(posMoveEndForDragObj);

            posMoveEndForDragObj.x = 0;
            transform.DOMove(posMoveEndForDragObj, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GetMyCollider2D().enabled = true;
            });
        }
        else
        {
            GetMyCollider2D().enabled = true;
        }
        gameObject.SetActive(true);
    }

    public override void DragFunc()
    {
        if (GetIsWrong)
            return;

        if (!canChangeToDone)
        {
            base.DragFunc();

            if (!clickToHit)
            {
                Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), GetLayerMask());

                if (hit != null)
                {
                    ObjectTargetHitAnim _objectTarget = hit.GetComponent<ObjectTargetHitAnim>();
                    if (_objectTarget != null)
                    {
                        _objectTarget.HitDragObject();
                        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDone());
                        GetLstObjectTarget().Remove(_objectTarget);
                        if (anim != null)
                            anim.Play(nameAnim, 0, 0f);

                        if (GetLstObjectTarget().Count == 0)
                        {
                            canChangeToDone = true;
                            if (GetDisplayHeartDone())
                            {
                                GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                            }
                        }
                    }
                }
            }
        }
    }
    public override void UpFunc()
    {

        if (!GetIsWrong)
        {
            if (canChangeToDone)
            {
                DoneMe();
                GetMyCollider2D().enabled = false;

                
                if (!GetAutoBackPosBeginAfterDone())
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    AutoBackPosBegin();
                }

                GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());
                GamePlayManager.Instance.GetLevelController().ActiveNextObect();

            }
            else
            {
                GetMyCollider2D().enabled = true;

                if (!clickToHit)
                {
                    if (GetAutoBackPosBeginAfterDone())
                        AutoBackPosBegin();
                }
            }

        }
        else
        {
            AutoBackPosBegin();
        }

        PlayAnim(nameIdle, true);
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());

    }
    public override void DownFunc()
    {
        base.DownFunc();
        PlayAnim(nameAnim, loopAnim);

        if(clickToHit)
        {
            Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), GetLayerMask());

            if (hit != null)
            {
             //   Debug.LogError("================ hit:" + hit.gameObject.name);
                ObjectTargetHitAnim _objectTarget = hit.GetComponent<ObjectTargetHitAnim>();
                if (_objectTarget != null)
                {
                    _objectTarget.HitDragObject();
                    MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDone());
                    GetLstObjectTarget().Remove(_objectTarget);
                    if (anim != null)
                        anim.Play(nameAnim, 0, 0f);

                    if (GetLstObjectTarget().Count == 0)
                    {
                        canChangeToDone = true;
                        if (GetDisplayHeartDone())
                        {
                            GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                        }
                    }
                }
            }
        }

        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDown());
    }
    public override void SkipFunc()
    {
        base.SkipFunc();
        DoneMe();
        gameObject.SetActive(false);
        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
            GetLstObjectTarget()[i].gameObject.SetActive(false);
        }
        //  GetLstObjectTarget()[0].myObjectDrag = this;
    }
}
