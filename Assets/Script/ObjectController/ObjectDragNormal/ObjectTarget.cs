using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTarget : ObjectTargetParent
{
    [SerializeField] int orderLayerAfterDone = 2;
    [SerializeField] int indexObjectNotInCludeOtherObject;
    public int GetIndexObjectNotInCludeOtherObject
    {
        get { return indexObjectNotInCludeOtherObject; }
        set { indexObjectNotInCludeOtherObject = value; }
    }
    public int GetOrderLayerAfterDone()
    {
        return orderLayerAfterDone;
    }
    public override void OnStart()
    {
        base.OnStart();
        if (GetMyObjectDrag != null)
            if (GetMyObjectDrag.GetOriginalIndex() > 0)
                indexObjectNotInCludeOtherObject = GetMyObjectDrag.GetOriginalIndex();
    }
}
