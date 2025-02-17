using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : ObjectDragParent
{
    [System.Serializable]
    public class InfoObjectAnimation
    {
        [SerializeField] ObjectAnimation objectAnimation;
        [SerializeField] string nameAnimForObjectAnimation;

        public void Open(Action ac)
        {
            objectAnimation.OpenMe(nameAnimForObjectAnimation, ac);
        }
        public void Skip()
        {
            objectAnimation.SkipFunc();
        }
    }


    [SerializeField] ObjectDragParent[] lstObjectDragLinked;
    [SerializeField] bool notRotate, animChangeSpAfterDone, needCorrectPos, RotateAfterDone, objectDropNotIncludeOtherObject;
    [SerializeField] Sprite spAfterDone;
    [SerializeField] int scaleSp;
    [SerializeField] Rigidbody2D rid2D;
    [SerializeField] float claimY = -1000, speedMoveToTarget = 0.1f, speedScaleToTarget = 0.1f;

    [SerializeField] public InfoObjectAnimation[] infoObjectAnimationDoneMe;
    [SerializeField] GameObject parentAfterDone, objectScaleClick;

    ObjectDropNotIncludeOtherObject objectDropNotIncludeOtherObjectGroup;

    Vector2 scaleOriginal;
    [SerializeField] ObjectTarget myObjectTarget;
    bool activeDropUnDone = false;

    [SerializeField] LayerMask objectNotIncludeLM;

    [SerializeField] Animator objectAnimator;

    public override void OnAwake()
    {
        base.OnAwake();
        OriginPosition = transform.position;
        transform.position = GamePlayManager.Instance.GetObjectGroupTras().position;
        transform.localScale = Vector3.zero;
    }
    Vector3 OriginPosition;
    public override void PlayAnimStartGame()
    {
        transform.DOScale(0.9f, 1);
        transform.DOMove(OriginPosition, 1f).OnComplete(() => 
        {
            objectAnimator.Play("ObjectFloat");

        });
    }
    public override void ActiveMe()
    {

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
                base.ActiveMe();
            });
        }
        else
        {
            base.ActiveMe();
            //  Debug.LogError("==================== active cho nay:" + gameObject.name);
        }
        //gameObject.SetActive(true);
    }

    public override void OnStart()
    {
        base.OnStart();

        if (rid2D != null)
        {
            rid2D.mass = 1; // Giữ khối lượng bình thường
            rid2D.drag = 1; // Thêm ma sát để giảm tốc độ
        }
        if (scaleSp == 1 || scaleSp == 3)
        {
            GetSpRender().transform.localScale = Vector2.zero;
        }
        else if (scaleSp == 2)
        {
            GetSpRender().transform.localScale = Vector2.one;
        }


        originalLayerUp = GetOrderLayerUp;
        scaleOriginal = transform.localScale;
    }
    int originalLayerUp = -1;
    bool scale = true;

    public override void DownFunc()
    {
        if (!activeDropUnDone)
        {
            base.DownFunc();
            if (!notRotate)
                transform.DORotate(Vector3.zero, 0.1f, RotateMode.Fast).SetEase(Ease.Linear);

            if (rid2D != null)
            {
                rid2D.bodyType = RigidbodyType2D.Kinematic; // Vô hiệu lực vật lý tạm thời
                rid2D.freezeRotation = true;
            }

            for (int i = 0; i < GetLstObjectTarget().Count; i++)
            {

                if (GetLstObjectTarget()[i].GetMyObjectDrag != null)
                {
                    GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
                }

            }

            objectAnimator.Play("ObjectIdle");

            if (scaleSp == 1 || scaleSp == 3)
            {
                GetSpRender().transform.DOScale(Vector2.one, speedScaleToTarget).SetEase(Ease.Linear);

            }
            if (scale)
            {
                if (objectScaleClick != null)
                {
                    objectScaleClick.transform.DOScale(1.1f, 0.1f).OnComplete(() =>
                    {
                        objectScaleClick.transform.DOScale(1, 0.1f);
                        scale = false;
                    });
                }
            }
            if (needCorrectPos)
            {
                GetIsDone = false;
                if (myObjectTarget != null)
                {
                    if (myObjectTarget.GetMyObjectDrag != null)
                    {
                        myObjectTarget.GetMyObjectDrag = null;
                    }
                    myObjectTarget = null;
                }

            }

            MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDown());
        }
        else
        {
            GetIsDone = false;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos -= objectPos;

            initialAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - transform.eulerAngles.z;
            isRotating = true;
        }

        GamePlayManager.Instance.GetLevelController().CheckPlayAnimEndOfObjectBeginLevel();
    }
    public override void ChangePosition(Vector2 pos)
    {
        if (!activeDropUnDone)
        {
            if (claimY != -1000)
            {
                if (pos.y <= claimY)
                    pos.y = claimY;
            }
            base.ChangePosition(pos);
        }
        else
        {
            // Ham xoayh
            ChangeRotationFollowMouse();
        }
    }
    public override void UpFuncBecauseEndTime()
    {
        if (!activeDropUnDone)
        {
            base.UpFuncBecauseEndTime();

            if (!notRotate && rid2D == null)
            {
                int random = UnityEngine.Random.Range(0, 100);
                int rotate = 0;
                if (random < 50)
                {
                    rotate = 5;
                }
                else
                {
                    rotate = -5;
                }
                transform.DORotate(new Vector3(0, 0, rotate), 0.1f, RotateMode.Fast).SetEase(Ease.Linear);
            }

            if (rid2D != null)
            {
                rid2D.bodyType = RigidbodyType2D.Dynamic;

                if (!RotateAfterDone)
                    rid2D.freezeRotation = false;
            }

            if (scaleSp == 1)
            {
                GetSpRender().transform.DOScale(Vector2.zero, speedScaleToTarget).SetEase(Ease.Linear);
            }
        }
        else
        {
            isRotating = false;
            GetMyCollider2D().enabled = true;
        }
    }
    ObjectTarget _objectTarget;
    void UpButNotHit()
    {
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());

        objectAnimator.Play("ObjectFloat");

        GetOrderLayerUp = originalLayerUp;

        ChangeSortingLayer(GetOrderLayerUp);
        GetIsDone = false;
        GetMyCollider2D().enabled = true;

        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
        }

        if (!notRotate && rid2D == null)
        {
            int random = UnityEngine.Random.Range(0, 100);
            int rotate = 0;
            if (random < 50)
            {
                rotate = 5;
            }
            else
            {
                rotate = -5;
            }
            transform.DORotate(new Vector3(0, 0, rotate), 0.1f, RotateMode.Fast).SetEase(Ease.Linear);
        }

        if (rid2D != null)
        {
            rid2D.bodyType = RigidbodyType2D.Dynamic;
            if (!RotateAfterDone)
                rid2D.freezeRotation = false;
        }

        if (GetAutoBackPosBeginAfterDone())
        {
            AutoBackPosBegin();
            if (scaleSp == 1)
            {
                GetSpRender().transform.DOScale(Vector2.zero, speedScaleToTarget).SetEase(Ease.Linear);
            }
        }
    }
    public override void UpFunc()
    {
        if (!activeDropUnDone)
        {
            if (!GetIsWrong)
            {
                Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), GetLayerMask());

                if (hit != null)
                {

                    _objectTarget = hit.GetComponent<ObjectTarget>();

                    if (GetLstObjectTarget().Contains(_objectTarget))
                    {
                        if (_objectTarget != null)
                        {
                            if (GetIndexSoundDone() == -1)
                            {
                                MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());
                            }
                            else
                            {
                                MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDone());
                            }

                            if (RotateAfterDone)
                                DropUnDone();
                            else if (objectDropNotIncludeOtherObject)
                            {
                                if (_objectTarget.GetMyObjectDrag != null)
                                    AutoBackPosBegin();
                                else
                                {
                                    Collider2D anotherHit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), objectNotIncludeLM);
                                    objectDropNotIncludeOtherObjectGroup = anotherHit.GetComponent<ObjectDropNotIncludeOtherObject>();
                                    //Debug.LogError("================ here" + objectDropNotIncludeOtherObjectGroup.gameObject.name);
                                    DragObjectNotIncludeOtherObject();

                                }
                            }
                            else
                                DoneMe();
                        }
                    }
                    else
                    {
                        UpButNotHit();
                    }
                }
                else
                {
                    UpButNotHit();
                }
            }

            else
            {
                AutoBackPosBegin();
                if (scaleSp == 1)
                {
                    GetSpRender().transform.DOScale(Vector2.zero, speedScaleToTarget).SetEase(Ease.Linear);
                }
            }
        }
        else
        {
            isRotating = false;

            float currentAngle = transform.eulerAngles.z;

            if (currentAngle > 180f)
                currentAngle -= 360f;
            if (currentAngle >= -5f && currentAngle <= 5f)
            {
                transform.rotation = Quaternion.identity;
                GetMyCollider2D().enabled = false;
                GetIsDone = true;
                GamePlayManager.Instance.CheckWin();
            }
        }
    }

    public void ActiveAllAfterDone(bool skip, Action ac)
    {
        for (int i = 0; i < GetLstObjectActiveAfterDone().Length; i++)
        {
            GetLstObjectActiveAfterDone()[i].SetActive(true);
        }

        if (infoObjectAnimationDoneMe.Length > 0)
        {
            for (int i = 0; i < infoObjectAnimationDoneMe.Length; i++)
            {
                if (!skip)
                    infoObjectAnimationDoneMe[i].Open(ac);
                else
                    infoObjectAnimationDoneMe[i].Skip();
            }
        }
        else
        {
            if (ac != null)
            {
                ac();
                ac = null;
            }
        }
    }
    void DragObjectNotIncludeOtherObject()
    {
        myObjectTarget.GetMyObjectDrag = null;
        myObjectTarget = null;

        myObjectTarget = _objectTarget;
        myObjectTarget.GetMyObjectDrag = this;
        myObjectTarget.GetIndexObjectNotInCludeOtherObject = GetOriginalIndex();

        GetOrderLayerUp = _objectTarget.GetOrderLayerAfterDone();
        ChangeSortingLayer(GetOrderLayerUp);
        Vector3 _objectTargetPos = _objectTarget.GetPoint().transform.position;
        transform.DOMove(_objectTargetPos, speedMoveToTarget).SetEase(Ease.Linear).OnComplete(() =>
        {
            GetPosBegin = transform.position;
            GetMyCollider2D().enabled = true;
            objectDropNotIncludeOtherObjectGroup.CheckWinObjectNotIncludeOtherObject();
            objectDropNotIncludeOtherObjectGroup = null;
        });
    }
    void DropUnDone()
    {

        if (rid2D != null)
        {
            rid2D.bodyType = RigidbodyType2D.Kinematic;
            rid2D.freezeRotation = false;
            rid2D.constraints = RigidbodyConstraints2D.FreezePosition;

        }

        GetOrderLayerUp = _objectTarget.GetOrderLayerAfterDone();
        ChangeSortingLayer(GetOrderLayerUp);

        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
        }
        Vector3 _objectTargetPos = _objectTarget.GetPoint().transform.position;
        _objectTarget.GetMyObjectDrag = this;
        myObjectTarget = _objectTarget;
        transform.DOMove(_objectTargetPos, speedMoveToTarget).SetEase(Ease.Linear).OnComplete(() =>
         {
             for (int i = 0; i < GetLstObjectDisableAfterDone().Length; i++)
             {
                 GetLstObjectDisableAfterDone()[i].SetActive(false);
             }
             float randomTiltAngle;

             if (UnityEngine.Random.value > 0.5f)
             {
                 randomTiltAngle = UnityEngine.Random.Range(-15f, -10f);
             }
             else
             {
                 randomTiltAngle = UnityEngine.Random.Range(10f, 15f);
             }

             transform.rotation = Quaternion.Euler(0, 0, randomTiltAngle);
             GetMyCollider2D().enabled = true;
         });
        activeDropUnDone = true;
    }
    [SerializeField] bool notDisableBoxOfTarget;
    public override void DoneMe()
    {
        objectAnimator.Play("ObjectIdle");

        if (needCorrectPos)
        {
            GetMyCollider2D().enabled = true;
            if (_objectTarget.gameObject == GetLstObjectTarget()[0].gameObject)
            {
                GetIsDone = true;
            }
        }
        else
        {
            GetIsDone = true;


        }


        if (rid2D != null)
        {
            rid2D.bodyType = RigidbodyType2D.Kinematic;
            rid2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }


        GetOrderLayerUp = _objectTarget.GetOrderLayerAfterDone();
        ChangeSortingLayer(GetOrderLayerUp);



        if (!notDisableBoxOfTarget)
        {
            _objectTarget.GetMyObjectDrag = this;
            myObjectTarget = _objectTarget;
            for (int i = 0; i < GetLstObjectTarget().Count; i++)
            {
                GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
            }
        }
        Vector3 _objectTargetPos = _objectTarget.GetPoint().transform.position;

        if (scaleSp == 2 || scaleSp == 3)
        {
            GetSpRender().transform.DOScale(Vector2.zero, speedScaleToTarget).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        transform.DOMoveZ(_objectTargetPos.z, 0.01f);
        transform.DOMove(_objectTargetPos, speedMoveToTarget).SetEase(Ease.Linear).OnComplete(() =>
        {
            _objectTarget.ActiveLstObjectAfterDoneMe();
            for (int i = 0; i < GetLstObjectDisableAfterDone().Length; i++)
            {
                GetLstObjectDisableAfterDone()[i].SetActive(false);
            }


            if (lstObjectDragLinked.Length == 0)
            {
                GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());



                if (GetLstNextObjectActiveAfterDone.Count == 0)
                {
                    ActiveAllAfterDone(false, GamePlayManager.Instance.CheckWin);
                }
                else
                {
                    ActiveAllAfterDone(false, null);
                    GamePlayManager.Instance.GetLevelController().ActiveNextObect();
                }

            }
            else
            {
                bool doneAllObjectLinked = true;

                for (int i = 0; i < lstObjectDragLinked.Length; i++)
                {
                    if (!lstObjectDragLinked[i].GetIsDone)
                    {
                        doneAllObjectLinked = false;
                        break;
                    }
                }

                if (doneAllObjectLinked)
                {
                    GamePlayManager.Instance.GetLevelController().PushEventCheckDoneStep(GetOriginalIndex().ToString());


                    if (GetLstNextObjectActiveAfterDone.Count == 0)
                    {
                        ActiveAllAfterDone(false, GamePlayManager.Instance.CheckWin);
                    }
                    else
                    {
                        ActiveAllAfterDone(false, null);
                        GamePlayManager.Instance.GetLevelController().GetCurrentObjectDrag = this;
                        GamePlayManager.Instance.GetLevelController().ActiveNextObect();
                        //  Debug.LogError("================================= ???????????????????");
                    }
                }
            }

            if (spAfterDone != null)
            {
                if (!animChangeSpAfterDone)
                    GetSpRender().sprite = spAfterDone;
            }

            if (parentAfterDone != null)
                transform.parent = parentAfterDone.transform;
        });

        if (scaleSp == 1)
        {
            GetSpRender().transform.localScale = Vector2.one;
            GetSpRender().transform.DOKill();
        }
    }
    public override void SkipFunc()
    {
        base.SkipFunc();
        base.DoneMe();
        ActiveAllAfterDone(true, null);
        GetOrderLayerUp = GetLstObjectTarget()[0].GetComponent<ObjectTarget>().GetOrderLayerAfterDone();
        ChangeSortingLayer(GetOrderLayerUp);
        for (int i = 0; i < GetLstObjectTarget().Count; i++)
        {
            GetLstObjectTarget()[i].GetMyCollider2D().enabled = false;
        }
        transform.position = GetLstObjectTarget()[0].GetPoint().transform.position;
        transform.rotation = Quaternion.identity;

        Debug.LogError("==================== skip func:" + gameObject.name);

        if (spAfterDone != null)
        {
            if (!animChangeSpAfterDone)
                GetSpRender().sprite = spAfterDone;
        }

        if (rid2D != null)
        {
            rid2D.bodyType = RigidbodyType2D.Kinematic;
            rid2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        gameObject.SetActive(true);
        transform.DOKill();
        transform.localScale = scaleOriginal;
        if (parentAfterDone != null)
            transform.parent = parentAfterDone.transform;
        if (scaleSp == 1)
        {
            GetSpRender().transform.localScale = Vector2.one;
            GetSpRender().transform.DOKill();
        }
        if (scaleSp == 2)
        {
            GetSpRender().transform.localScale = Vector2.zero;
            GetSpRender().transform.DOKill();
        }
    }
    public override void DisplayAfterPlayAnimBeginLevel()
    {
        base.DisplayAfterPlayAnimBeginLevel();
        GetMyCollider2D().enabled = false;
        transform.localScale = new Vector2(0.3f, 0.3f);
        transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            GetMyCollider2D().enabled = true;
        });
    }
    Sprite originalSprite;
    public void PlayChangeSpriteAnimation(int _timeDelay, Action ac)
    {
        originalSprite = GetSpRender().sprite;
        StartCoroutine(DelayChangeSpriteAnimation(_timeDelay, ac));
    }
    IEnumerator DelayChangeSpriteAnimation(int _timeDelay, Action ac)
    {
        yield return new WaitForSeconds(_timeDelay * 0.3f);
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDone());
        GetSpRender().sprite = spAfterDone;
        yield return new WaitForSeconds(0.2f);
        GetSpRender().sprite = originalSprite;
        if (ac != null)
        {
            ac();
            ac = null;
        }
    }


    float initialAngle;
    bool isRotating = false;
    void ChangeRotationFollowMouse()
    {
        if (isRotating)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos -= objectPos;

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - initialAngle + 360);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }
}
