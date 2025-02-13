using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragToTargetAndChangeSkinWithAnim : ObjectDragParent
{
    ObjectTargetOfObjectDragToTargetAndPlayAnimOfTarget _objectTarget;
    ObjectDragParent objectSwitch;
    [SerializeField] string nameSkin, nameAfter;
    [SerializeField] float speedMoveToTarget;
    [SerializeField] Transform pointAfterShaft;

    [SerializeField] ObjectTarget presentObjectTarget;

    ObjectTarget previousObjectTarget;
    [SerializeField] int indexSoundPlayAnim = -1;
    public ObjectTarget GetObjectTarget
    {
        get { return presentObjectTarget; }
        set { presentObjectTarget = value; }
    }
    public bool GetShaft()
    {
        return sharped;
    }
    public override void DownFunc()
    {
        base.DownFunc();
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDown());
        previousObjectPos = transform.position;
        GetIsDone = false;
    }
    public override void OnStart()
    {
        base.OnStart();
        originalLayerUp = GetOrderLayerUp;
        
    }
    Vector3 previousObjectPos;
    int originalLayerUp = -1;

    public override void UpFunc()
    {
        base.UpFunc();
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());
        ChangeSortingLayer(GetOrderLayerUp);
        Collider2D hit = Physics2D.OverlapCircle(GetPoint().transform.position, GetRadius(), GetLayerMask());
        if (hit != null)
        {
            if (hit.tag == "PencilShaft")
            {
                if (!sharped)
                {
                    sharped = true;

                    _objectTarget = hit.GetComponent<ObjectTargetOfObjectDragToTargetAndPlayAnimOfTarget>();

                    if (_objectTarget != null)
                    {
                        MusicManager.instance.PlaySoundLevelOneShot(true, indexSoundPlayAnim);
                        _objectTarget.PlayAnim(AfterShaft, nameSkin);
                        PlayAnim("Idle", false);
                       // gameObject.SetActive(false);
                    }
                }
                else
                    GetMyCollider2D().enabled = true;

                if (presentObjectTarget != null)
                {
                    if (presentObjectTarget.GetMyObjectDrag != null)
                        presentObjectTarget.GetMyObjectDrag = null;
                    presentObjectTarget = null;
                }
            }
            else if (hit.tag == "PencilBox")
            {
                GetOrderLayerUp = originalLayerUp;
                ChangeSortingLayer(GetOrderLayerUp);

                ObjectTarget objectTargetBox = hit.GetComponent<ObjectTarget>();
                objectSwitch = objectTargetBox.GetMyObjectDrag;
                if (presentObjectTarget != null)
                {
                    previousObjectTarget = presentObjectTarget;
                    previousObjectTarget.GetMyCollider2D().enabled = false;

                    if (objectTargetBox.GetMyObjectDrag != null)
                    {
                        previousObjectTarget.GetMyObjectDrag = objectTargetBox.GetMyObjectDrag;
                        previousObjectTarget.GetMyObjectDrag.GetMyCollider2D().enabled = false;
                        objectSwitch.GetComponent<ObjectDragToTargetAndChangeSkinWithAnim>().GetObjectTarget = previousObjectTarget;
                    }
                    else
                    {
                        presentObjectTarget.GetMyObjectDrag = null;
                    }
                }
                else
                    previousObjectTarget = null;
                presentObjectTarget = objectTargetBox;
                presentObjectTarget.GetMyCollider2D().enabled = false;

                presentObjectTarget.GetMyObjectDrag = this;
                presentObjectTarget.GetMyObjectDrag.GetMyCollider2D().enabled = false;

                Vector3 _objectTargetPos = presentObjectTarget.GetPoint().transform.position;

                transform.DOMove(_objectTargetPos, speedMoveToTarget).SetEase(Ease.Linear).OnComplete(()=> 
                {
                    if (objectSwitch == null)
                    {
                        presentObjectTarget.GetMyCollider2D().enabled = true;
                        GetMyCollider2D().enabled = true;
                    }
                    CheckDoneMe();
                });

                if (objectSwitch != null)
                {
                    objectSwitch.GetIsDone = false;
                    objectSwitch.transform.DOMove(previousObjectPos, speedMoveToTarget).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        GetMyCollider2D().enabled = true;
                        presentObjectTarget.GetMyCollider2D().enabled = true;
                        if (previousObjectTarget != null)
                        {
                            previousObjectTarget.GetMyCollider2D().enabled = true;
                            previousObjectTarget.GetMyObjectDrag.GetMyCollider2D().enabled = true;
                        }
                        else
                        {
                            objectSwitch.GetMyCollider2D().enabled = true;
                            objectSwitch.GetComponent<ObjectDragToTargetAndChangeSkinWithAnim>().GetObjectTarget = null;
                        }

                        objectSwitch.GetComponent<ObjectDragToTargetAndChangeSkinWithAnim>().CheckDoneMe();
                    });
                }
                //else
                //{
                //    GetMyCollider2D().enabled = true;
                //    presentObjectTarget.GetMyCollider2D().enabled = true;
                //}

                
            }
        }
        else
        {
            if (presentObjectTarget != null)
            {
                if (presentObjectTarget.GetMyObjectDrag != null)
                    presentObjectTarget.GetMyObjectDrag = null;
                presentObjectTarget = null;
            }
            GetMyCollider2D().enabled = true;
        }
    }
    public void CheckDoneMe()
    {
        if (presentObjectTarget.gameObject == GetLstObjectTarget()[0].gameObject && sharped)
        {
            DoneMe();
            GamePlayManager.Instance.CheckWin();
        }
    }    
    public override void SkipFunc()
    {
        transform.position = GetLstObjectTarget()[0].transform.position;
        PlayAnim(nameAfter, false);
        DoneMe();
        GetMyCollider2D().enabled = false;
    }
    bool sharped = false;
    void AfterShaft()
    {
        GetMyCollider2D().enabled = true;
        transform.position = pointAfterShaft.position;
        PlayAnim(nameAfter, false);

        // gameObject.SetActive(true);
    }
}
