using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedScrollerDemos.GridSimulation;
using UnityEngine.UI;
public class SlotShop : MonoBehaviour
{
    [SerializeField] GameObject container, btnTicketAds, btnTicketIAP, soldout,iconVideoAds;
    [SerializeField] int index;
    [SerializeField] Text rewardText, priceText;
    [SerializeField] Image icon;
    public void SetData(Data data)
    {
        container.gameObject.SetActive(data != null);
        if (data != null)
        {
            index = int.Parse(data.someText);
            btnTicketIAP.SetActive(false);
            btnTicketAds.SetActive(false);
            soldout.SetActive(false);
            iconVideoAds.SetActive(false);
            rewardText.text = "x" + DataManager.instance.GetDataShop().shopInfo[index].reward;
            icon.sprite = DataManager.instance.GetDataShop().shopInfo[index].icon;
            if (index > 0)
            {
                if (PurchaserManager.m_StoreController != null)
                {
                    priceText.text = PurchaserManager.m_StoreController.products.WithID(PurchaserManager.instance.ticketPack[index - 1]).metadata.localizedPriceString;
                }
                else
                {
                    priceText.text = DataManager.instance.GetDataShop().shopInfo[index - 1].price + "$";
                }
                btnTicketIAP.SetActive(true);
            }
            else
            {
                btnTicketAds.SetActive(true);
                if (DataManager.instance.SaveData().buyTicketShop)
                {
                    soldout.SetActive(true);
                }
                else
                {
                    iconVideoAds.SetActive(true);
                }    
            }
        }
    }
    public void BtnTicketAds()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();

        if (soldout.activeSelf)
        {
            DataManager.instance.ShowPopUpMess(Vector3.one, Vector2.zero, "This item will be available on the next day.");
        }
        else
        {
            AdsManager.instance.ShowVideoAds(Reward, "BuyTicket");
        }

    }
    void Reward()
    {
        DataManager.instance.AddTicket(DataManager.instance.GetDataShop().shopInfo[index].reward);
        DataManager.instance.SaveData().buyTicketShop = true;
        soldout.SetActive(true);
        iconVideoAds.SetActive(false);
        DataManager.instance.ShowWarningPopUp(icon.sprite, DataManager.instance.GetDataShop().shopInfo[index].reward.ToString());

    }
    public void BtnTicketIAP()
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        DataParamManager.packBuyIAP = PurchaserManager.instance.ticketPack[index - 1];
        DataParamManager.indexPackIAP = index - 1;
        PurchaserManager.instance.BuyFunc();
    }
}
