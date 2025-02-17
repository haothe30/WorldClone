using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    public static LoadingPanel loading;
    //[SerializeField] SpriteRenderer BGSP, BGLogoSP, chanchoSP, Logo;
    [SerializeField] GameObject Mask, BG, BGLogo;
    [SerializeField] Animator anim;
    //[SerializeField] GameObject[] animDog;

    string nameNextScene;
    AsyncOperation currentLoadingOperation = null;
    float width, height, worldScreenHeight, worldScreenWidth;
    Vector3 scale;
    //[SerializeField] SkeletonAnimation animBGLogo;
    private void Awake()
    {
        loading = this;
        DontDestroyOnLoad(this);
    }
    //[SerializeField] Sprite[] bgLogospLst, chanchospLst, logoLst;
    public GameObject GetMaskObject()
    {
        return Mask;
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Loading")
            CreateMe();
    }
    static bool showAOA;
    public void CreateMe()
    {
        randomLogo = 1;

        //BGSP.transform.localScale = Vector3.one;
        //width = BGSP.sprite.bounds.size.x;
        //height = BGSP.sprite.bounds.size.y;
        worldScreenHeight = Camera.main.orthographicSize * 2f;
        worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        scale = new Vector3((worldScreenWidth / width) * 1f, (worldScreenHeight / height) * 1f, 1);
        //BGLogoSP.transform.localScale = BGSP.transform.localScale = scale;

        if (DataManager.instance.SaveData().session == 1)
        {
            nameNextScene = "Play";
            DataParamManager.isTuroring = true;
        }
        else
        {
            nameNextScene = "Menu";
        }
        //  Debug.LogError("=============== create :" + nameNextScene);
        currentLoadingOperation = SceneManager.LoadSceneAsync(nameNextScene);
        currentLoadingOperation.allowSceneActivation = false;
        // Mask.SetActive(true);
        StartCoroutine(DelayMaskOut());

        //if (DataController.ins.saveData.session >= 2)
        //{
        //    showAOA = true;
        //}
        //  anim.Play("MaskIn", -1, 0);

    }
    SkeletonAnimation saAnimDog;

    public void ChangeUIAfterCheckXMas()
    {
    //    if (DataParam.RemoteConfigData.xmas)
    //    {
            //MusicController.musicIns.PlayMusic(true, 6);
            //chanchoSP.sprite = chanchospLst[1];
            //BGLogoSP.sprite = bgLogospLst[1];
            //Logo.sprite = logoLst[1];

            //for (int i = 0; i < animDog.Length; i++)
            //{
            //    saAnimDog = animDog[i].GetComponent<SkeletonAnimation>();
            //    saAnimDog.Skeleton.SetSkin("2");
            //    saAnimDog.Skeleton.SetSlotsToSetupPose();
            //    saAnimDog.Update(0);
            //}


            //animBGLogo.Skeleton.SetSkin("5");
            //animBGLogo.Skeleton.SetSlotsToSetupPose();
            //animBGLogo.Update(0);


        //}
        //else
        {
            //MusicController.musicIns.PlayMusic(true, 1);

            //chanchoSP.sprite = chanchospLst[0];
            //BGLogoSP.sprite = bgLogospLst[0];
            //Logo.sprite = logoLst[0];

            InvokeRepeating(nameof(RandomBGLogoAnim), 0, 0.5f);

            //for (int i = 0; i < animDog.Length; i++)
            //{
            //    saAnimDog = animDog[i].GetComponent<SkeletonAnimation>();
            //    saAnimDog.Skeleton.SetSkin("1");
            //    saAnimDog.Skeleton.SetSlotsToSetupPose();
            //    saAnimDog.Update(0);
            //}
        }
    }
    int randomLogo = 1;
    void RandomBGLogoAnim()
    {

        if (randomLogo < 4)
        {
            randomLogo++;
        }
        else
        {
            randomLogo = 1;
        }


        //animBGLogo.Skeleton.SetSkin(randomLogo.ToString());
        //animBGLogo.Skeleton.SetSlotsToSetupPose();
        //animBGLogo.Update(0);

    }
    Vector3 posCam;
    Action actionAfterMaskOut = null;
    public void OpenMe(string _nameNextScene, Action _actionAfterMaskOut = null)
    {

        BG.SetActive(true);
        BGLogo.SetActive(false);

        actionAfterMaskOut = _actionAfterMaskOut;
        transform.position = Vector3.zero;
        nameNextScene = _nameNextScene;
        Mask.SetActive(true);


        if (!string.IsNullOrEmpty(nameNextScene))
        {
            currentLoadingOperation = SceneManager.LoadSceneAsync(nameNextScene);
            currentLoadingOperation.allowSceneActivation = false;
        }

        gameObject.SetActive(true);

        //for (int i = 0; i < animDog.Length; i++)
        //{
        //    animDog[i].gameObject.SetActive(false);
        //}
        //animDog[UnityEngine.Random.Range(0, animDog.Length)].gameObject.SetActive(true);

        anim.Play("MaskIN");
        Debug.LogError("================ Call open me");
   
        if (!string.IsNullOrEmpty(_nameNextScene) /*&& SceneManager.GetActiveScene().name != "Menu"*/)
        {
        }
    }
    bool logoScene = false;
    public void EventEndMaskIn()
    {
        //DeliveryController.instance.ChangeScene();
        StartCoroutine(DelayMaskOut());
    }
    public void EventEndMaskOut()
    {
        gameObject.SetActive(false);

        AdsManager.instance.ShowBannerAds();
        AdsManager.instance.AutoShowBannerAdaptive();

        if (actionAfterMaskOut != null)
            actionAfterMaskOut();

        Mask.SetActive(false);

        if (IsInvoking(nameof(RandomBGLogoAnim)))
        {
            CancelInvoke(nameof(RandomBGLogoAnim));
        }
        if (SceneManager.GetActiveScene().name == "Play")
        {
            GamePlayManager.Instance.OnStart();
        }
            Debug.LogError("========================== ket thua mask out");
    }
    IEnumerator DelayMaskOut()
    {
        if (SceneManager.GetActiveScene().name == "Loading")
        {
            //yield return DataParam.GETTIME1();

            //if (!IronSource.Agent.isInterstitialReady())
            //    AdsController.instanceAds.RequestInter();

            //Debug.LogError("================= inter logo");
            //AdsController.instanceAds.ShowInter("InterMenu");


            yield return DataParamManager.GETTIME1S();
            if (!string.IsNullOrEmpty(nameNextScene))
            {
                logoScene = true;
                currentLoadingOperation.allowSceneActivation = true;
                  Debug.LogError("============== chuyen sang scene");

            }
        }
        else
        {
            if (!string.IsNullOrEmpty(nameNextScene))
            {
                currentLoadingOperation.allowSceneActivation = true;
                  Debug.LogError("============== chuyen sang scene");
            }
        }


        transform.position = Vector3.zero;

        yield return DataParamManager.GETTIME1S();
        anim.Play("MaskOUT");
        if (showAOA)
        {
            AdsManager.instance.ShowAOAAds("loadingAOA");
            showAOA = false;
        }
    }
}
