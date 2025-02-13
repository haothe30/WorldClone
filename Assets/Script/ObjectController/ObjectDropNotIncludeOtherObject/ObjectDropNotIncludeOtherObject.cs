using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDropNotIncludeOtherObject : MonoBehaviour
{
    [SerializeField] ObjectTarget[] objectTargets;

    public void CheckWinObjectNotIncludeOtherObject()
    {
        bool doneAll = true;
        for (int i = 0; i < objectTargets.Length; i++)
        {
            if (objectTargets[i].GetIndexObjectNotInCludeOtherObject != objectTargets[0].GetIndexObjectNotInCludeOtherObject)
            {
                doneAll = false;
                break;
            }
        }
        if (doneAll)
        {
            for (int i = 0; i < objectTargets.Length; i++)
            {
                objectTargets[i].GetMyObjectDrag.GetIsDone = true;
                objectTargets[i].GetMyObjectDrag.GetMyCollider2D().enabled = false;
                objectTargets[i].GetMyCollider2D().enabled = false;
            }
            GamePlayManager.Instance.CheckWin();
        }
    }
}
