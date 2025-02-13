using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSlotUnit : MonoBehaviour
{
    public SlotLevel slotLevel;
    [SerializeField] SlotLevelUnit[] slotLevelUnit;
    public void ActiveMe(bool active)
    {
        if (active)
        {
            for(int i = 0; i < slotLevelUnit.Length; i ++)
            {
                slotLevelUnit[i].ActiveMe();
            }
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
