using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using EnhancedScrollerDemos.GridSimulation;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopPanel : UIProperties, IEnhancedScrollerDelegate
{
    [SerializeField] Image panelImg;
    [SerializeField] GameObject titleMenu, titleGP;
    SmallList<Data> _data;
    [SerializeField] EnhancedScroller scroller;
    [SerializeField] EnhancedScrollerCellView managerSlot;
    [SerializeField] float sizeSlots;
    [SerializeField] int numberOfCellsPerRow = 3;
    [SerializeField] Text ticketTextGP, ticketTextMenu;
    int afterClose;
    void DisplayTicketText()
    {
        ticketTextMenu.text = ticketTextGP.text = "" + DataManager.instance.SaveData().totalTicket;
    }
    public override void OpenMe(string value)
    {
        afterClose = 0;
        base.OpenMe(value);
        scroller.Delegate = this;
        LoadData();
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            panelImg.enabled = false;
            titleMenu.SetActive(true);
            titleGP.SetActive(false);
        }
        else
        {
            panelImg.enabled = true;
            titleMenu.SetActive(false);
            titleGP.SetActive(true);
            afterClose = int.Parse(value);
        }
        DisplayTicketText();
        DataParamManager.displayTicket += DisplayTicketText;
    }
    public override void CloseMe()
    {
        base.CloseMe();

        if (SceneManager.GetActiveScene().name == "Play")
        {
            if (afterClose == 1)
            {
                DataManager.instance.ShowSkipPopUp();
            }
            else if (afterClose == 2)
            {
                DataManager.instance.ShowTimeOutPanel();
            }
        }
        DataParamManager.displayTicket -= DisplayTicketText;
    }
    private void LoadData()
    {
        _data = new SmallList<Data>();
        for (var i = 0; i < DataManager.instance.GetDataShop().shopInfo.Length; i++)
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
    ManagerSlotShop cellView;
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        cellView = scroller.GetCellView(managerSlot) as ManagerSlotShop;
        cellView.name = "Cell " + (dataIndex * numberOfCellsPerRow).ToString() + " to " + ((dataIndex * numberOfCellsPerRow) + numberOfCellsPerRow - 1).ToString();
        cellView.SetData(ref _data, dataIndex * numberOfCellsPerRow);
        return cellView;
    }
}
