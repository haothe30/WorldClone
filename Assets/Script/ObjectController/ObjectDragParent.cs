using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragParent : MonoBehaviour
{
    [SerializeField] int originalIndex, indexSoundDown = -1, indexSoundUp = -1, indexSoundDone = -1, indexSoundForWrong = -1, indexSoundForCorrect = -1;
    [SerializeField] bool isDone, wrongItem, autoBackPosBeginAfterDone, displayHeartDone;
    [SerializeField] List<ObjectTargetParent> lstObjectTarget;
    [SerializeField] LayerMask lm, wrongLm;
    [SerializeField] GameObject point;
    [SerializeField] float radius = 0.2f;
    [SerializeField] Collider2D myCollider2D;
    [SerializeField] List<ObjectDragParent> lstNextObjectActiveAfterDone = new List<ObjectDragParent>();
    [SerializeField] GameObject[] lstObjectDisableAfterDone, lstObjectActiveAfterDone, lstObjectDisableWhenActiveMe, lstObjectActiveWhenActiveMe;
    [SerializeField] SpriteRenderer sp;
    [SerializeField] SkeletonAnimation sa;
    [SerializeField] bool manyTime;
    [SerializeField] int intValue;

    [SerializeField] int orderLayerUp = 2, orderLayerDrag = 4;
    BoxCollider2D myBoxCollider2D;
    Vector3 posBegin;

    public int GetIndexSoundForWrong()
    {
        return indexSoundForWrong;
    }
    public int GetIndexSoundForCorrect()
    {
        return indexSoundForCorrect;
    }
    public int GetIndexSoundDown()
    {
        return indexSoundDown;
    }
    public int GetIndexSoundUp()
    {
        return indexSoundUp;
    }
    public int GetIndexSoundDone()
    {
        return indexSoundDone;
    }

    public bool GetDisplayHeartDone()
    {
        return displayHeartDone;
    }

    public bool GetAutoBackPosBeginAfterDone()
    {
        return autoBackPosBeginAfterDone;
    }
    public LayerMask GetLayerMaskWrong()
    {
        return wrongLm;
    }
    public virtual void AutoBackPosBegin()
    {
        ChangeSortingLayer(orderLayerUp);
        transform.DOMove(posBegin, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            myCollider2D.enabled = true;
            if (isDone)
                GetIsWrong = true;
        });
    }
    public GameObject[] GetLstObjectDisableAfterDone()
    {
        return lstObjectDisableAfterDone;
    }
    public GameObject[] GetLstObjectActiveAfterDone()
    {
        return lstObjectActiveAfterDone;
    }
    public int GetOrderLayerUp
    {
        get { return orderLayerUp; }
        set { orderLayerUp = value; }

    }
    public int GetOrderLayerDrag()
    {
        return orderLayerDrag;
    }

    public Vector2 GetSizeBoxCollider2D()
    {
        return myBoxCollider2D.size;
    }

    public int GetOriginalIndex()
    {
        return originalIndex;
    }
    public SpriteRenderer GetSpRender()
    {
        return sp;
    }

    public bool GetManyTime()
    {
        return manyTime;
    }
    public int GetIntValue
    {
        get { return intValue; }
        set { intValue = value; }
    }
    public bool GetIsDone
    {
        get { return isDone; }
        set { isDone = value; }
    }
    public bool GetIsWrong
    {
        get { return wrongItem; }
        set { wrongItem = value; }
    }
    public List<ObjectTargetParent> GetLstObjectTarget()
    {
        return lstObjectTarget;
    }
    public LayerMask GetLayerMask()
    {
        return lm;
    }
    public GameObject GetPoint()
    {
        return point;
    }
    public float GetRadius()
    {
        return radius;
    }
    public Collider2D GetMyCollider2D()
    {
        return myCollider2D;
    }
    public List<ObjectDragParent> GetLstNextObjectActiveAfterDone
    {
        get { return lstNextObjectActiveAfterDone; }
        set { lstNextObjectActiveAfterDone = value; }
    }
    public Vector3 GetPosBegin
    {
        get { return posBegin; }
        set { posBegin = value; }
    }

    public enum MOVEOBJECTBEGIN
    {
        NONE, MOVEFROMRIGHT, MOVEFROMLEFT
    }
    public MOVEOBJECTBEGIN moveObjectBegin;

    public virtual void OnAwake()
    {
        if (myCollider2D != null)
            myBoxCollider2D = myCollider2D.GetComponent<BoxCollider2D>();
    }
    public virtual void OnStart()
    {
        posBegin.x = transform.position.x;
        posBegin.y = transform.position.y;
        posBegin.z = posBegin.y * -0.001f;
        transform.position = posBegin;
        for (int i = 0; i < lstObjectTarget.Count; i++)
        {
            lstObjectTarget[i].OnStart();
        }
    }
    public virtual void SkipFunc()
    {
       
    }
    public virtual void ActiveMe()
    {
        for (int i = 0; i < lstObjectDisableWhenActiveMe.Length; i++)
        {
            lstObjectDisableWhenActiveMe[i].SetActive(false);
        }
        for (int i = 0; i < lstObjectActiveWhenActiveMe.Length; i++)
        {
            lstObjectActiveWhenActiveMe[i].SetActive(true);
        }
        if(GetMyCollider2D() != null)
        GetMyCollider2D().enabled = true;
        wrongItem = false;
    }
    public virtual void DoneMe()
    {
        isDone = true;
        for (int i = 0; i < lstObjectDisableAfterDone.Length; i++)
        {
            lstObjectDisableAfterDone[i].SetActive(false);
        }
        for (int i = 0; i < lstObjectActiveAfterDone.Length; i++)
        {
            lstObjectActiveAfterDone[i].SetActive(true);
        }

    }

    public virtual void ChangePosition(Vector2 pos)
    {
        Vector3 myPos = new Vector3(pos.x, pos.y, pos.y * -0.001f);
        transform.position = myPos;
    }
    public virtual void DownFunc()
    {

        myCollider2D.enabled = false;
        ChangeSortingLayer(orderLayerDrag);

        if (!wrongItem)
        {
            for (int i = 0; i < lstObjectTarget.Count; i++)
            {
                lstObjectTarget[i].GetMyCollider2D().enabled = true;
            }
        }
    }
    public virtual void UpFuncBecauseEndTime()
    {
        ChangeSortingLayer(orderLayerUp);
        isDone = false;
        myCollider2D.enabled = true;
        for (int i = 0; i < lstObjectTarget.Count; i++)
        {
            lstObjectTarget[i].GetMyCollider2D().enabled = false;
        }

        if (GetIsWrong)
        {
            AutoBackPosBegin();
        }
    }
    public void ChangeSortingLayer(int layer)
    {
        if (sp != null)
        {
            sp.sortingOrder = layer;
        }
        else
        {
            if (sa != null)
            {
                sa.GetComponent<Renderer>().sortingOrder = layer;
            }
        }
    }
    public virtual void PlayAnimStartGame() { }
    public virtual void DisplayAfterPlayAnimBeginLevel() { }
    public virtual void UpFunc() { }
    public virtual void DragFunc() { }
    public virtual void GetIndexOf() { }
    public void PlayAnim(string nameAnim, bool loop)
    {
        if (sa != null)
        {
            sa.AnimationState.SetAnimation(0, nameAnim, loop);
        }
    }
    public SkeletonAnimation GetSa()
    {
        return sa;
    }

    #region Load Editor
    public void Load()
    {
        for (int i = 0; i < lstObjectTarget.Count; i++)
        {
            lstObjectTarget[0].transform.position = transform.position;
            lstObjectTarget[i].name = "TargetObject_" + gameObject.name;
        }
        if (sp != null)
            gameObject.name = sp.sprite.name;
    }
    public void Rotate()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-90, 90));
        Vector2 xRange = new Vector2(-3f, 3f);
        Vector2 yRange = new Vector2(-3f, 3f);
        float randomX = Random.Range(xRange.x, xRange.y);
        float randomY = Random.Range(yRange.x, yRange.y);
        transform.position = new Vector2(randomX, randomY);
    }
    #endregion
}
