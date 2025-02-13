using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using EnhancedScrollerDemos.GridSimulation;
public class ManagerSlotLevel : EnhancedScrollerCellView
{
    public SlotLevel[] slotLevel;
    public void SetData(ref SmallList<Data> data, int startingIndex)
    {
        // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
        for (var i = 0; i < slotLevel.Length; i++)
        {
           // Debug.LogError("================= set data for slot level:" + i);
            slotLevel[i].SetData(startingIndex + i < data.Count ? data[startingIndex + i] : null);
        }
    }
}
