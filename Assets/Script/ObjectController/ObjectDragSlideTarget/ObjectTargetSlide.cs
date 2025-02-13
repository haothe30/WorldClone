using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTargetSlide : ObjectTargetParent
{
    [SerializeField] List<GameObject> myBox;

    public bool CheckDone()
    {
        return myBox.FindAll(x => !x.activeSelf).Count == myBox.Count;
    }    
}
