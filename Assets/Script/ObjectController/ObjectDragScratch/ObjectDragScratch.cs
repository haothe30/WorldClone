using DG.Tweening;
using ScratchCardAsset;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragScratch : ObjectDragParent
{
    int currentTarget;
    [SerializeField] string nameAnimation, nameIdle;
    [SerializeField] bool loopAnim = true;
    ObjectTargetScratch objectTargetScratch;
    List<ObjectTargetScratch> multiObjectTargetScratches = new List<ObjectTargetScratch>();
    [SerializeField] ScratchCard.ScratchMode modeScratch;
    [SerializeField] Vector2 scaleBrush = Vector2.one;
    [SerializeField] float processCanDisplayEffect, processCanDone, processDisplayBtnNext;

    [SerializeField] SpriteRenderer spriteRenderNeedChangeAfterDoneMe;
    [SerializeField] Sprite[] spForObjectChange;

    [SerializeField] bool changeAlphaScratch;
    [SerializeField] bool canNotActiveBtnNext;
    [SerializeField] bool rotationFollowMouse, multiSractchs, dropDecorAfterDone;


    [SerializeField] SkeletonAnimation effectSpineActiveWhenAction;
    [SerializeField] GameObject effectParticleActiveWhenAction;
    [SerializeField] bool loopEffect;
    [SerializeField] string[] nameSkinForEffect;

    bool activeChangeAlpha;
    bool canChangeToDone;
    bool activeEffect = false;
    bool activeBtnNext = false;
    int currentEffect;

    public override void OnAwake()
    {
        base.OnAwake();
        //  Debug.LogError("===========" + gameObject.name);
        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            GetLstObjectTarget()[i].GetComponent<ObjectTargetScratch>().OnAwake();
        }
        if (moveObjectBegin != MOVEOBJECTBEGIN.NONE)
        {
            gameObject.SetActive(false);
        }

    }

    public override void OnStart()
    {
        base.OnStart();
        currentTarget = 0;
        // GetMyCollider2D().enabled = false;

    }
    public void MustActive(ObjectTargetScratch targetTemp)
    {
        base.ActiveMe();
        GetIsDone = false;
        canChangeToDone = false;

        objectTargetScratch = targetTemp;
        objectTargetScratch.SetModeForScratch(modeScratch, scaleBrush);
        objectTargetScratch.SetPoint(null);
        if (effectSpineActiveWhenAction != null)
        {
            activeEffect = true;
        }
        if (effectParticleActiveWhenAction != null)
        {
            activeEffect = true;
            effectParticleActiveWhenAction.gameObject.SetActive(false);
        }


        if (changeAlphaScratch)
        {
            activeChangeAlpha = true;
        }

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
                objectTargetScratch.ActiveTargetScratch();
                GetMyCollider2D().enabled = true;
            });
        }
        else
        {
            objectTargetScratch.ActiveTargetScratch();
            GetMyCollider2D().enabled = true;
            //  Debug.LogError("==================== active cho nay:" + gameObject.name);
        }
        gameObject.SetActive(true);
    }
    public override void ActiveMe()
    {
        base.ActiveMe();
        GetIsDone = false;
        canChangeToDone = false;
        activeBtnNext = false;
        GamePlayManager.Instance.GetObjectDragScratchCanNext = this;

        //  Debug.LogError("====================== current target:" + currentTarget);
        if (!multiSractchs)
        {
            objectTargetScratch = GetLstObjectTarget()[currentTarget].GetComponent<ObjectTargetScratch>();
            objectTargetScratch.SetModeForScratch(modeScratch, scaleBrush);
            objectTargetScratch.SetPoint(null);
        }
        else
        {
            if (multiObjectTargetScratches.Count == 0)
            {
                for (int i = 0; i < GetLstObjectTarget().Count; i++)
                {
                    multiObjectTargetScratches.Add(GetLstObjectTarget()[i].GetComponent<ObjectTargetScratch>());
                    multiObjectTargetScratches[i].SetModeForScratch(modeScratch, scaleBrush);
                    multiObjectTargetScratches[i].SetPoint(null);
                }
            }
        }
        if (rotationFollowMouse)
            objectTargetScratch.GetWaitCalculate = true;

        if (effectSpineActiveWhenAction != null)
        {
            activeEffect = true;
            // effectSpineActiveWhenAction().AnimationState.SetAnimation(0, "idle", false);
        }
        if (effectParticleActiveWhenAction != null)
        {
            activeEffect = true;
            effectParticleActiveWhenAction.gameObject.SetActive(false);
        }


        if (changeAlphaScratch)
        {
            activeChangeAlpha = true;
        }

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
                ActiveTargetScratchs();
            });

            gameObject.SetActive(true);
        }
        else
        {
            ActiveTargetScratchs();
            //  Debug.LogError("==================== active cho nay:" + gameObject.name + ":" + objectTargetScratch.gameObject.name);
        }



    }
    void ActiveTargetScratchs()
    {
        if (!multiSractchs)
        {
            objectTargetScratch.ActiveTargetScratch();
            GetMyCollider2D().enabled = true;


            if (rotationFollowMouse)
            {
                if (!GetIsWrong)
                    GetLstObjectTarget()[currentTarget].GetMyCollider2D().enabled = true;
                if (objectTargetScratch != null)
                    objectTargetScratch.SetPoint(GetPoint());
            }
        }
        else
        {
            GetMyCollider2D().enabled = true;

            for (int i = 0; i < multiObjectTargetScratches.Count; i++)
            {
                multiObjectTargetScratches[i].ActiveTargetScratch();
                if (rotationFollowMouse)
                {
                    if (!GetIsWrong)
                        GetLstObjectTarget()[i].GetMyCollider2D().enabled = true;
                    if (multiObjectTargetScratches[i] != null)
                        multiObjectTargetScratches[i].SetPoint(GetPoint());
                }
            }
        }
    }
    public override void DownFunc()
    {
        GetMyCollider2D().enabled = false;
        ChangeSortingLayer(GetOrderLayerDrag());
        if (!GetIsWrong)
            GetLstObjectTarget()[currentTarget].GetMyCollider2D().enabled = true;
        PlayAnim(nameAnimation, loopAnim);

        //if (effectSpineActiveWhenAction() != null)
        //{
        //    effectSpineActiveWhenAction().timeScale = 1;
        //}
        if (!multiSractchs)
        {
            if (objectTargetScratch != null)
                objectTargetScratch.SetPoint(GetPoint());
        }
        else
        {
            for (int i = 0; i < multiObjectTargetScratches.Count; i++)
            {
                if (multiObjectTargetScratches[i] != null)
                    multiObjectTargetScratches[i].SetPoint(GetPoint());
            }
        }
        MusicManager.instance.PlaySoundLevelLoop(true, GetIndexSoundDown());
    }
    public override void DragFunc()
    {
        if (GetIsWrong)
            return;
        base.DragFunc();
        ChangeAlphaScratch();
        ActiveEffectWhenActiveMe();
        ActiveBtnNext();
        CheckChangeToDone();
        RotateFollowMouse();
    }
    void RotateFollowMouse()
    {
        if (!rotationFollowMouse)
            return;
        GetPoint().transform.rotation = Quaternion.Slerp(GetPoint().transform.rotation, RotationDragObj(), Time.deltaTime * 10);
        //  Debug.LogError("============= roate 111111");
        if (!canChangeToDone)
        {
            if (objectTargetScratch == null)
                return;
            if (objectTargetScratch.GetWaitCalculate)
                return;

            if (objectTargetScratch.GetModeScratch() == ScratchCardAsset.ScratchCard.ScratchMode.Restore)
            {
                if (objectTargetScratch.GetProcessScratch() <= processCanDone)
                {
                    canChangeToDone = true;
                    if (GetDisplayHeartDone())
                    {
                        GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                    }
                    DoneMe();

                }
            }
            else
            {
                //   Debug.LogError("================:" + objectTargetScratch.GetProcessScratch());
                if (objectTargetScratch.GetProcessScratch() >= processCanDone)
                {
                    canChangeToDone = true;
                    if (GetDisplayHeartDone())
                    {
                        GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                    }
                    DoneMe();

                }
            }

        }
    }
    Quaternion RotationDragObj()
    {
        return Quaternion.Euler(0, 0, Mathf.Clamp((GetPoint().transform.position - Camera.main.transform.position).x * -5, -25f, 25f));
    }
    void ActiveBtnNext()
    {
        if (canNotActiveBtnNext)
            return;
        if (activeBtnNext)
            return;
        if (objectTargetScratch == null)
            return;
        //   Debug.LogError("==================:" + objectTargetScratch.GetProcessScratch() + ":" + processDisplayBtnNext);
        if (modeScratch == ScratchCard.ScratchMode.Erase)
        {
            if (objectTargetScratch.GetProcessScratch() >= processDisplayBtnNext)
            {
                activeBtnNext = true;
                GamePlayUIManager.Instance.GetBtnNextStep().SetActive(true);
            }
        }
        else
        {
            if (objectTargetScratch.GetProcessScratch() <= processDisplayBtnNext)
            {
                activeBtnNext = true;
                GamePlayUIManager.Instance.GetBtnNextStep().SetActive(true);
            }
        }

    }
    void ActiveEffectWhenActiveMe()
    {
        if (!activeEffect)
            return;
        if (objectTargetScratch == null)
            return;
        if (effectSpineActiveWhenAction != null)
        {
            if (modeScratch == ScratchCard.ScratchMode.Erase)
            {
                if (objectTargetScratch.GetProcessScratch() >= processCanDisplayEffect)
                {
                    effectSpineActiveWhenAction.gameObject.SetActive(true);
                    effectSpineActiveWhenAction.AnimationState.SetAnimation(0, "animation", loopEffect);

                    if (nameSkinForEffect.Length > 0)
                    {
                        effectSpineActiveWhenAction.Skeleton.SetSkin(nameSkinForEffect[currentEffect]);
                        effectSpineActiveWhenAction.Skeleton.SetSlotsToSetupPose();
                        effectSpineActiveWhenAction.Update(0);
                    }

                    activeEffect = false;
                    currentEffect++;
                }
            }
            else
            {
                if (objectTargetScratch.GetProcessScratch() <= processCanDisplayEffect)
                {
                    effectSpineActiveWhenAction.gameObject.SetActive(true);
                    effectSpineActiveWhenAction.AnimationState.SetAnimation(0, "animation", loopEffect);

                    if (nameSkinForEffect.Length > 0)
                    {
                        effectSpineActiveWhenAction.Skeleton.SetSkin(nameSkinForEffect[currentEffect]);
                        effectSpineActiveWhenAction.Skeleton.SetSlotsToSetupPose();
                        effectSpineActiveWhenAction.Update(0);
                    }
                    activeEffect = false;
                    currentEffect++;
                }
            }
        }
        if (effectParticleActiveWhenAction != null)
        {
            if (modeScratch == ScratchCard.ScratchMode.Erase)
            {
                if (objectTargetScratch.GetProcessScratch() >= processCanDisplayEffect)
                {
                    effectParticleActiveWhenAction.gameObject.SetActive(true);
                    activeEffect = false;
                }
            }
            else
            {
                if (objectTargetScratch.GetProcessScratch() <= processCanDisplayEffect)
                {
                    effectParticleActiveWhenAction.gameObject.SetActive(true);
                    activeEffect = false;
                }
            }
        }
        // Debug.LogError("======== display effect :" + gameObject.name + ":" + objectTargetScratch.GetProcessScratch());
    }
    void ChangeAlphaScratch()
    {
        if (!activeChangeAlpha)
            return;
        if (objectTargetScratch == null)
            return;
        objectTargetScratch.ChangeColorForScratch(1 - objectTargetScratch.GetProcessScratch());
    }

    void CheckChangeToDone()
    {
        if (rotationFollowMouse)
            return;
        if (!canChangeToDone)
        {
            if (!multiSractchs)
            {
                if (objectTargetScratch == null)
                    return;
                if (objectTargetScratch.GetModeScratch() == ScratchCardAsset.ScratchCard.ScratchMode.Restore)
                {
                    if (objectTargetScratch.GetProcessScratch() <= processCanDone)
                    {
                        canChangeToDone = true;
                        if (GetDisplayHeartDone())
                        {
                            GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                        }
                    }
                }
                else
                {
                    //   Debug.LogError("================:" + objectTargetScratch.GetProcessScratch());

                    if (objectTargetScratch.GetProcessScratch() >= processCanDone)
                    {
                        canChangeToDone = true;
                        if (GetDisplayHeartDone())
                        {
                            GamePlayManager.Instance.GetLevelController().ShowCorrectObj(null, GetIndexSoundForCorrect());
                        }
                    }
                }
            }
            else
            {
                if (multiObjectTargetScratches.Count == 0) return;
                bool multiScratchDone = true;
                for (int i = 0; i < multiObjectTargetScratches.Count; i++)
                {
                    if (multiObjectTargetScratches[i].GetModeScratch() == ScratchCardAsset.ScratchCard.ScratchMode.Restore)
                    {
                        if (multiObjectTargetScratches[i].GetProcessScratch() > processCanDone)
                        {
                            multiScratchDone = false;
                            break;

                        }
                    }
                    else
                    {
                        //   Debug.LogError("================:" + objectTargetScratch.GetProcessScratch());

                        if (multiObjectTargetScratches[i].GetProcessScratch() < processCanDone)
                        {
                            multiScratchDone = false;
                            break;
                        }
                    }
                }

                if (multiScratchDone)
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
    public override void UpFunc()
    {
        PlayAnim(nameIdle, true);

        if (!GetIsWrong)
        {
            MusicManager.instance.PlaySoundLevelLoop(false, -1);
            GetMyCollider2D().enabled = true;
            ChangeSortingLayer(GetOrderLayerUp);
            if (!multiSractchs)
            {
                GetLstObjectTarget()[currentTarget].GetMyCollider2D().enabled = false;
                if (objectTargetScratch != null)
                {
                    if (canChangeToDone)
                    {
                        //  if (!activeBtnNext)
                        DoneMe();
                    }
                    //else
                    //{
                    //    if (effectSpineActiveWhenAction() != null)
                    //    {
                    //        effectSpineActiveWhenAction().timeScale = 0;
                    //    }
                    //}
                }
            }
            else
            {

                if (canChangeToDone)
                {
                    DoneMe();
                   // Debug.LogError("=================== Done");
                }
                else
                {
                 //   Debug.LogError("================= Not Done");
                }
            }
        }
        else
        {
            AutoBackPosBegin();
        }
    }
    public override void AutoBackPosBegin()
    {
        base.AutoBackPosBegin();
        MusicManager.instance.PlaySoundLevelLoop(false, -1);
    }
    int currentSpriteForObjectChange;
    [SerializeField] bool canNotDisableMeAfterDone;
    public override void DoneMe()
    {
        base.DoneMe();
        if (!multiSractchs)
        {
            objectTargetScratch.DoneScratch();
            objectTargetScratch.ActiveLstObjectAfterDoneMe();
            objectTargetScratch = null;
        }
        else
        {
            for (int i = 0; i < multiObjectTargetScratches.Count; i++)
            {

                multiObjectTargetScratches[i].DoneScratch();
                multiObjectTargetScratches[i].ActiveLstObjectAfterDoneMe();
                multiObjectTargetScratches[i] = null;
            }
        }


        if (!canNotDisableMeAfterDone)
            GetMyCollider2D().enabled = false;
        if (!GetAutoBackPosBeginAfterDone())
        {
            if (!canNotDisableMeAfterDone)
                gameObject.SetActive(false);
        }
        else
        {
            AutoBackPosBegin();
        }


        if (currentTarget < GetLstObjectTarget().Count - 1)
        {
            if (rotationFollowMouse)
            {
                GetIsDone = false;
            }
            currentTarget++;
          //  Debug.LogError("=================== current target:" + currentTarget + ":" + GetIsDone);
        }
        else
        {
            if (GetIsDone && rotationFollowMouse)
                MusicManager.instance.PlaySoundLevelLoop(false, -1);
        }


        if (dropDecorAfterDone)
        {
            GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Remove(this);
            GamePlayManager.Instance.GetLevelController().DropRandomElement(GamePlayManager.Instance.GeHeightCam(), null);
        }
        GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());
        GamePlayManager.Instance.GetLevelController().ActiveNextObect();



        if (spriteRenderNeedChangeAfterDoneMe != null)
        {
            if (currentSpriteForObjectChange < spForObjectChange.Length - 1)
            {
                currentSpriteForObjectChange++;
                spriteRenderNeedChangeAfterDoneMe.sprite = spForObjectChange[currentSpriteForObjectChange];
            }
            else
            {
                spriteRenderNeedChangeAfterDoneMe.gameObject.SetActive(false);
            }
        }

        activeBtnNext = false;
    }
    public override void UpFuncBecauseEndTime()
    {
        ChangeSortingLayer(GetOrderLayerUp);
        GetMyCollider2D().enabled = true;
        GetLstObjectTarget()[currentTarget].GetMyCollider2D().enabled = false;

        PlayAnim(nameIdle, true);
        //if (effectSpineActiveWhenAction() != null)
        //{
        //    effectSpineActiveWhenAction().timeScale = 0;
        //}
        if (GetIsWrong)
        {
            AutoBackPosBegin();
        }

        MusicManager.instance.PlaySoundLevelLoop(false, -1);
    }
    public override void SkipFunc()
    {
        base.SkipFunc();
        base.DoneMe();
        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            ObjectTargetScratch _objTargetScratch = GetLstObjectTarget()[i].GetComponent<ObjectTargetScratch>();
            _objTargetScratch.SetModeForScratch(modeScratch, scaleBrush);
            _objTargetScratch.DoneScratch();
        }
        if (!canNotDisableMeAfterDone)
            gameObject.SetActive(false);
        GetMyCollider2D().enabled = false;
        if (spriteRenderNeedChangeAfterDoneMe != null)
            spriteRenderNeedChangeAfterDoneMe.gameObject.SetActive(false);
        if (effectParticleActiveWhenAction != null)
            effectParticleActiveWhenAction.SetActive(false);
        if (effectSpineActiveWhenAction != null)
            effectSpineActiveWhenAction.gameObject.SetActive(false);

        MusicManager.instance.PlaySoundLevelLoop(false, -1);
    }
}
