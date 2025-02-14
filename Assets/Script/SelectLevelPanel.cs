using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using EnhancedScrollerDemos.GridSimulation;
using UnityEngine.UI;

public class SelectLevelPanel : UIProperties, IEnhancedScrollerDelegate
{
    SmallList<Data> _data;
    [SerializeField] EnhancedScroller scroller;
    [SerializeField] EnhancedScrollerCellView managerSlot2;
    [SerializeField] float sizeSlots;
    [SerializeField] int numberOfCellsPerRow = 1;
    //[SerializeField] Text starText,ticketText;
    public override void OpenMe()
    {
        base.OpenMe();
        scroller.Delegate = this;
        LoadData();
        //starText.text = "" + DataManager.instance.TotalStar();
        //ticketText.text = "" + DataManager.instance.SaveData().totalTicket;
    }
    private void LoadData()
    {
        _data = new SmallList<Data>();
        for (var i = 0; i < DataManager.instance.GetDataLevel().levelInfo.Length; i++)
        {
            _data.Add(new Data() { someText = i.ToString() });
        }

        scroller.ReloadData();
    }
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)_data.Count / (float)numberOfCellsPerRow);
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return sizeSlots;
    }
    ManagerSlotLevel cellView;
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        cellView = scroller.GetCellView(managerSlot2) as ManagerSlotLevel;
        cellView.name = "Cell " + (dataIndex * numberOfCellsPerRow).ToString() + " to " + ((dataIndex * numberOfCellsPerRow) + numberOfCellsPerRow - 1).ToString();
        cellView.SetData(ref _data, dataIndex * numberOfCellsPerRow);
        return cellView;
    }
}
