using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTargetParent : MonoBehaviour
{
    [SerializeField] ObjectDragParent myObjectDrag;
    [SerializeField] Collider2D myCollider2D;
    [SerializeField] GameObject point;
    [SerializeField] GameObject[] lstGameObjectAtiveAfterDoneMe,lstObjDisableAfterDoneMe;


    public void DisableLstObjectAfterDoneMe()
    {
        for (int i = 0; i < lstObjDisableAfterDoneMe.Length; i++)
        {
            lstObjDisableAfterDoneMe[i].SetActive(false);
        }
    }
    public void ActiveLstObjectAfterDoneMe()
    {
        for (int i = 0; i < lstGameObjectAtiveAfterDoneMe.Length; i++)
        {
            lstGameObjectAtiveAfterDoneMe[i].SetActive(true);
        }
    }

    public virtual void OnAwake()
    {

    }
    public virtual void OnStart()
    {
        myCollider2D.enabled = false;
    }
    public GameObject GetPoint()
    {
        return point;
    }
    public ObjectDragParent GetMyObjectDrag
    {
        get { return myObjectDrag; }
        set { myObjectDrag = value; }
    }
    public Collider2D GetMyCollider2D()
    {
        return myCollider2D;
    }
}
