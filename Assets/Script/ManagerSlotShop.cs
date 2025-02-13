using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using EnhancedScrollerDemos.GridSimulation;
public class ManagerSlotShop : EnhancedScrollerCellView
{
    public SlotShop[] slotShop;
    public void SetData(ref SmallList<Data> data, int startingIndex)
    {
        // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
        for (var i = 0; i < slotShop.Length; i++)
        {
            // Debug.LogError("================= set data for slot level:" + i);
            slotShop[i].SetData(startingIndex + i < data.Count ? data[startingIndex + i] : null);
        }
    }
}
