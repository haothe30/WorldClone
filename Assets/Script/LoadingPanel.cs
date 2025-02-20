using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    public static LoadingPanel loading;
    [SerializeField] GameObject Mask, BG;
    [SerializeField] Animator anim;
    string nameNextScene;
    AsyncOperation currentLoadingOperation = null;
    float width, height, worldScreenHeight, worldScreenWidth;
    Vector3 scale;
    private void Awake()
    {
        loading = this;
        DontDestroyOnLoad(this);
    }
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

        worldScreenHeight = Camera.main.orthographicSize * 2f;
        worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        scale = new Vector3((worldScreenWidth / width) * 1f, (worldScreenHeight / height) * 1f, 1);

        if (DataManager.instance.SaveData().session == 1)
        {
            nameNextScene = "Play";
            DataParamManager.isTuroring = true;
            Instantiate(Resources.Load<TutorialManager>("TutorialManager"), DataManager.instance.transform);
        }
        else
        {
            nameNextScene = "Menu";
        }
        currentLoadingOperation = SceneManager.LoadSceneAsync(nameNextScene);
        currentLoadingOperation.allowSceneActivation = false;
        StartCoroutine(DelayMaskOut());

    }
    SkeletonAnimation saAnimDog;

    public void ChangeUIAfterCheckXMas()
    {

        {
            InvokeRepeating(nameof(RandomBGLogoAnim), 0, 0.5f);
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
    }
    Vector3 posCam;
    Action actionAfterMaskOut = null;
    public void OpenMe(string _nameNextScene, Action _actionAfterMaskOut = null)
    {

        BG.SetActive(true);

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

       

        anim.Play("MaskIN");
        Debug.LogError("================ Call open me");
   
        if (!string.IsNullOrEmpty(_nameNextScene) /*&& SceneManager.GetActiveScene().name != "Menu"*/)
        {
        }
    }
    bool logoScene = false;
    public void EventEndMaskIn()
    {
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
            yield return DataParamManager.GETTIME5S();
            if (!string.IsNullOrEmpty(nameNextScene))
            {
                logoScene = true;
                currentLoadingOperation.allowSceneActivation = true;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(nameNextScene))
            {
                currentLoadingOperation.allowSceneActivation = true;
            }
        }

        transform.position = Vector3.zero;

        yield return DataParamManager.GETTIME2S();
        anim.Play("MaskOUT");
        if (showAOA)
        {
            AdsManager.instance.ShowAOAAds("loadingAOA");
            showAOA = false;
        }
    }
}
