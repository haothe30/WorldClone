using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerHaveScript : MonoBehaviour
{
    public Transform Parent;
    public UnitParent unitPooledObject;
    private List<UnitParent> PooledUnit;

    public int PoolLength;

    void Awake()
    {
        PoolLength = 10;
    }

    #region text number damage
    public void InitializeUnit(int length)
    {
        PooledUnit = new List<UnitParent>();
        for (int i = 0; i < length; i++)
        {
            CreateUnitObjectInPool();
        }
    }

    public UnitParent GetUnitPooledObject()
    {
        for (int i = 0; i < PooledUnit.Count; i++)
        {
            if (!PooledUnit[i].gameObject.activeInHierarchy)
            {
                return PooledUnit[i];
            }
        }
        int indexToReturn = PooledUnit.Count;
        //create more
        CreateUnitObjectInPool();
        //will return the first one that we created
        return PooledUnit[indexToReturn];
    }


    private void CreateUnitObjectInPool()
    {
        UnitParent go;
        if (unitPooledObject == null)
            go = new UnitParent();
        else
        {
            go = Instantiate(unitPooledObject) as UnitParent;
        }

        go.gameObject.SetActive(false);
        PooledUnit.Add(go);
        if (Parent != null)
            go.transform.parent = this.Parent;
        else
            go.transform.parent = transform;
    }
    #endregion

}
