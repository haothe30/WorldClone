using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;
using System;
using System.Globalization;
using UnityEngine.Networking;

public class GDPRScript : MonoBehaviour
{
    [System.Serializable]
    public class IpInfo
    {
        public string ip;
        public string country;
        // Các thông tin khác bạn có thể muốn lấy từ dịch vụ
    }
    ConsentForm _consentForm;
 //   [SerializeField] GameObject GDPRPanel;
 //   [SerializeField] bool testGDPR;
    // Start is called before the first frame update
    void Start()
    {

            var debugSettings = new ConsentDebugSettings
            {
                // Geography appears as in EEA for debug devices.
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = new List<string>
            {
                "9FB81BC5C62DE2B7D77EBAC33BDBECDE"
            }
            };


        //GDPRPanel.SetActive(true);
        //GDPRPanel.transform.SetAsLastSibling();
        //Time.timeScale = 0;
      //  Debug.LogError("============== start GDPR");


        // Here false means users are not under age.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = debugSettings,
            
        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);

    }


    //public void BtnContinue()
    //{
    //    Datacontroller.instance.SaveData().showGDPR = true;
    //    Time.timeScale = 1;
    //    GDPRPanel.SetActive(false);
    //    AdsController.instance.OnStart();
    //    Datacontroller.instance.CallCountTimePlaying();
    //}


    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            AdsManager.instance.InitAdsAfterGDPR();
            UnityEngine.Debug.LogError("============ error GDPR 11:" + error);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
            Debug.LogError("=========================== Request GDPR");
        }
        else
        {
            AdsManager.instance.InitAdsAfterGDPR();
             Debug.LogError("========= not available GDPR");
        }
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            AdsManager.instance.InitAdsAfterGDPR();
            UnityEngine.Debug.LogError("========= error GDPR 22:" + error);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {

            _consentForm.Show(OnShowForm);
            Time.timeScale = 0;
            Debug.LogError("=========================== hien thi popup GDPR");
        }
        else if (ConsentInformation.ConsentStatus == ConsentStatus.Obtained)
        {
            AdsManager.instance.InitAdsAfterGDPR();
            Debug.LogError("=========================== chon roi GDPR");
        }
    }

    void OnShowForm(FormError error)
    {
        Time.timeScale = 1;
        AdsManager.instance.InitAdsAfterGDPR();
        Debug.LogError("=========================== dong popup GDPR:" + ConsentInformation.ConsentStatus + ":" + ConsentInformation.CanRequestAds());
        if (error != null)
        {
            // Handle the error.
          UnityEngine.Debug.LogError("========= error GDPR 333:" + error);
            return;
        }
    }

}
