using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

public class PurchaserManager : MonoBehaviour, IStoreListener
{
    public static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    DataManager dataController;
    public string[] removeAds;
    public string[] ticketPack;
    public static PurchaserManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < removeAds.Length; i++)
        {
            removeAds[i] = Application.identifier + ".removeads_" + i;
            builder.AddProduct(removeAds[i], ProductType.Consumable);
        }

        for(int i = 0; i < ticketPack.Length; i ++)
        {
            ticketPack[i] = Application.identifier + ".ticketpack_" + i;
            builder.AddProduct(ticketPack[i], ProductType.Consumable);
        }    

        UnityPurchasing.Initialize(this, builder);
    }



    void Start()
    {
        dataController = DataManager.instance;
        InitializePurchasing();
    }
    public static bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    public void BuyFunc()
    {
        BuyProductID(DataParamManager.packBuyIAP);
    }
    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            m_StoreController.InitiatePurchase(productId);
            DataParamManager.ResumeFromOtherAction = true;
        }
        else
        {
            Debug.LogError("=======BuyProductID FAIL. Not initialized.");
        }
    }
    //public void BtnRestorePurchaseIOS()
    //{
    //    if (!IsInitialized())
    //    {
    //        Debug.LogError("==========RestorePurchases FAIL. Not initialized.");
    //        return;
    //    }
    //    if (Application.platform == RuntimePlatform.IPhonePlayer ||
    //        Application.platform == RuntimePlatform.OSXPlayer)
    //    {
    //        Debug.LogError("===========RestorePurchases started ...");
    //        var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
    //        apple.RestoreTransactions((result) =>
    //        {
    //            Debug.LogError("===========RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
    //        });

    //    }
    //    else
    //    {
    //        Debug.LogError("==============RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
    //    }
    //}
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    Product product;
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("=================== init false IAP:" + error.ToString());
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("=================== init false IAP:" + error.ToString() + "_mess_" + message);
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.LogError(string.Format("==========: PASS. Product: '{0}'", args.purchasedProduct.definition.id) + ":" + DataParamManager.packBuyIAP);
        if (DataParamManager.packBuyIAP.Contains("removeads"))
        {
            if (dataController != null)
            {
                dataController.RemoveAdsFunc(args.purchasedProduct);
            }
        }

        else if(DataParamManager.packBuyIAP.Contains("ticketpack"))
        {
            if (dataController != null)
            {
                dataController.BuyTicketFunc(args.purchasedProduct);
            }
        }    

        return PurchaseProcessingResult.Complete;
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {

        Debug.LogError(string.Format("=============OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

}
