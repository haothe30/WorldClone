using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedScrollerDemos.GridSimulation;
using UnityEngine.UI;
public class SlotLevel : MonoBehaviour
{
    [SerializeField] GameObject container,commingSoon;
    [SerializeField] ManagerSlotUnit managerSlot1Khung, managerSlot2Khung;
    DataManager dataController;
    public int index;
    public void SetData(Data data)
    {
        container.gameObject.SetActive(data != null);
        if (data != null)
        {
            index = int.Parse(data.someText);
            if(DataManager.instance.GetDataLevel().lstDataLevel[index].levelInfo.Length == 1)
            {
                managerSlot1Khung.ActiveMe(true);
                managerSlot2Khung.ActiveMe(false);
                commingSoon.SetActive(false);
            }
            else if(DataManager.instance.GetDataLevel().lstDataLevel[index].levelInfo.Length == 2)
            {
                managerSlot1Khung.ActiveMe(false);
                managerSlot2Khung.ActiveMe(true);
                commingSoon.SetActive(false);
            }
            else if(DataManager.instance.GetDataLevel().lstDataLevel[index].levelInfo.Length == 0)
            {
                managerSlot1Khung.ActiveMe(false);
                managerSlot2Khung.ActiveMe(false);
                commingSoon.SetActive(true);
            }
        }
    }
}
