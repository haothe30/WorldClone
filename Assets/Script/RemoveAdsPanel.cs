using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RemoveAdsPanel : UIProperties
{
    //[SerializeField] Image panelImg;
    //[SerializeField] RectTransform table;
    [SerializeField] Text priceText;
    public override void OpenMe()
    {
        base.OpenMe();
        //if (SceneManager.GetActiveScene().name == "Menu")
        //{
        //    panelImg.enabled = false;
        //    table.transform.localScale = Vector3.one;
        //    table.transform.localPosition = new Vector2(0, 80);
        //   // AdsManager.instance.HideBannerAds();
        //}
        //else
        //{
        //    panelImg.enabled = true;
        //    table.transform.localScale = new Vector3(0.9f, 0.9f,1);
        //    table.transform.localPosition = new Vector2(0, -50);
        //}
      //  Debug.LogError("============== table transform:" + table.transform.position);

        Display();
        if (PurchaserManager.m_StoreController != null)
        {
            priceText.text = PurchaserManager.m_StoreController.products.WithID(PurchaserManager.instance.removeAds[0]).metadata.localizedPriceString;
            //priceVipText.text = PurchaserManager.m_StoreController.products.WithID(PurchaserManager.instance.removeAds[1]).metadata.localizedPriceString;
        }
        else
        {
            priceText.text = DataManager.instance.GetDataRemoveAds().shopInfo[0].price + "$";
            //priceVipText.text = DataManager.instance.GetDataRemoveAds().shopInfo[1].price + "$";
        }
        DataParamManager.displayRemoveAds += Display;

        AdsManager.instance.ActiveNativeAds(false, 0, null);
        AdsManager.instance.ActiveMREC(true);
    }
    public void Display()
    {
        //if (!DataManager.instance.SaveData().vip)
        //{
        //    if (DataManager.instance.SaveData().removeAds)
        //    {
        //        khung.SetActive(false);
        //    }
        //    else
        //    {
        //        khung.SetActive(true);
        //    }
        //    khungVIP.SetActive(true);
        //    nothavepack.SetActive(false);
        //}
        //else
        //{
        //    khung.SetActive(false);
        //    khungVIP.SetActive(false);
        //    nothavepack.SetActive(true);
        //}
    }    
    public void BtnRemoveAds(int index)
    {
        if (DataManager.instance.CanNotAction())
            return;
        MusicManager.instance.SoundClickButton();
        DataParamManager.indexPackIAP = index;
        DataParamManager.packBuyIAP = PurchaserManager.instance.removeAds[index];
        PurchaserManager.instance.BuyFunc();
    }
    public override void CloseMe()
    {
        base.CloseMe();
        DataParamManager.displayRemoveAds -= Display;
        if (DataManager.instance.GetEndPanel() != null && DataManager.instance.GetEndPanel().gameObject.activeSelf)
        {
            DataManager.instance.GetEndPanel().ShowNativeAds();
        }
        if (SceneManager.GetActiveScene().name == "Play")
        {
            GamePlayManager.Instance.ChangeStageDisplayPopUp(false);
        }
        else
        {
            if (DataManager.instance.GetSelectLevelPanel() != null)
                DataManager.instance.GetSelectLevelPanel().OpenMe();
        }
        // AdsManager.instance.ShowBannerAds();
    }

}
