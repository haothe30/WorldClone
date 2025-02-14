using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;
    [SerializeField] LayerMask lm;
    [SerializeField] float speedDragObj = 5f;
    [SerializeField] GameObject objectGroupPos;
    DataManager dataController;
    bool win;
    Sprite endSprite;
    float maxTime;
    Camera cam;
    Vector2 deltaPos, claimPos;
    ObjectDragParent objectDrag;
    int maxStarCanTake = 3;
    float widthCam, heightCam;



    public float GetWidthCam()
    {
        return widthCam;
    }
    public float GeHeightCam()
    {
        return heightCam;
    }
    public Sprite GetSpriteEnd()
    {
        return endSprite;
    }
    public bool GetWin()
    {
        return win;
    }
    private void Awake()
    {
        dataController = DataManager.instance;
        Instance = this;
        cam = Camera.main;
        maxStarCanTake = 3;
        LoadLevel();
        DataParamManager.activeCountPlay = true;
    }
    void ActiveCountTime(bool active)
    {
        if (vip)
            return;
        if (active)
        {
            if (!IsInvoking(nameof(CalculateTime)))
            {
                InvokeRepeating(nameof(CalculateTime), 0, 1);
            }
        }
        else
        {
            if (IsInvoking(nameof(CalculateTime)))
            {
                CancelInvoke(nameof(CalculateTime));
            }
        }
    }

    void CalculateTime()
    {
        if (vip)
            return;
        maxTime--;
        if (maxTime == 10)
        {
            GamePlayUIManager.Instance.SetAnimForBtnAddTime(true);
        }
        if (maxTime <= 10)
        {
            MusicManager.instance.PlaySoundOtherOneShot(true, 0);
        }
        GamePlayUIManager.Instance.GetTimeText().text = DataParamManager.ConvertTime((double)maxTime, 0);
        if (maxTime <= 0)
        {
            CheckLose();
        }
    }
    bool vip;

    public void OnStart()
    {

        vip = dataController.SaveData().vip;
        GamePlayUIManager.Instance.GetBtnNextStep().gameObject.SetActive(false);
        if (!vip)
        {
            GamePlayUIManager.Instance.GetBtnAddTime().SetActive(true);
            GamePlayUIManager.Instance.SetAnimForBtnAddTime(false);
        }
        else
        {
            GamePlayUIManager.Instance.GetTimeText().text = "Unlimited";
            GamePlayUIManager.Instance.GetBtnAddTime().SetActive(false);
        }

        cam = Camera.main;
        widthCam = cam.orthographicSize * cam.aspect;
        heightCam = cam.orthographicSize;
        levelController.OnStart();

        if (levelController.GetCameraDrag() != null)
        {
            widthCam = levelController.GetCameraDrag().XMax();
        }

        //if (!levelController.GetStartDropElement())
        //{
        ChangeStageToPlay();
        //}
        //else
        //{
        //    levelController.DropRandomDecor(heightCam, ChangeStageToPlay);
        //}



        AdsManager.instance.ShowBannerAds();
        MusicManager.instance.PlaySoundBGGP(true, MusicManager.instance.RandomBGGP());
    }
    void ChangeStageToPlay()
    {
        Debug.LogError("======================== play nè");
        DataParamManager.state = DataParamManager.STATEGAMEPLAY.PLAY;
        if (!vip)
        {
            ActiveCountTime(true);
        }
    }
    bool CanNotAction()
    {
        return DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY || EventSystem.current.currentSelectedGameObject != null;
    }
    private void Update()
    {

        if (CanNotAction())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.LogError("======================== Chọn nè");

            DownFunc();

        }
        else if (Input.GetMouseButton(0))
        {
            DragFunc();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            UpFunc();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            maxTime = 5;
        }
#endif
    }
    public ObjectDragParent GetObjectDrag()
    {
        return objectDrag;
    }
    void DownFunc()
    {
        Vector2 mousePos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit2D hitOfDrag = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, lm);
        if (hitOfDrag.collider != null)
        {
            ObjectDragParent objectDragTemp = hitOfDrag.collider.GetComponent<ObjectDragParent>();

            if (objectDragTemp != null)
            {
                objectDrag = objectDragTemp;

                mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                deltaPos = (Vector2)objectDrag.transform.position - mousePos;
                claimPos = objectDrag.transform.position = mousePos + deltaPos;
                DataParamManager.dragging = true;

                objectDrag.DownFunc();

                //if(objectDrag.GetActiveDropUnDone)
                //{
                //    objectDrag.ChangeRotationFollowMouse();
                //}
            }
            else
            {
                objectDrag = null;
                DataParamManager.dragging = false;
                Debug.LogError("============== bi null script");
            }
        }
        else
        {
            objectDrag = null;
            DataParamManager.dragging = false;
            //  Debug.LogError("============== ko phat hien duoc gi");
        }
    }
    void DragFunc()
    {
        if (objectDrag == null)
            return;

        objectDrag.ChangePosition(ClaimPositionCurrentObj());
        objectDrag.DragFunc();
        CheckWrong();
    }

    //private void OnDrawGizmos()
    //{
    //    if (objectDrag == null)
    //        return;
    //    if (!objectDrag.GetIsWrong)
    //        return;
    //    Gizmos.DrawWireSphere(objectDrag.transform.position, objectDrag.GetRadius());
    //}
    public virtual void CheckWrong()
    {
        if (!objectDrag.GetIsWrong)
            return;
        Collider2D hit = Physics2D.OverlapCircle(objectDrag.transform.position, objectDrag.GetRadius(), objectDrag.GetLayerMaskWrong());

        // Debug.LogError("================ check wrong:" + objectDrag.name);
        if (hit != null)
        {

            objectDrag.AutoBackPosBegin();
            levelController.ShowWrongObj(objectDrag.GetIndexSoundForWrong());
            UpFunc();


        }
    }
    void UpFunc()
    {
        if (objectDrag == null)
            return;
        objectDrag.UpFunc();
        objectDrag = null;
        DataParamManager.dragging = false;
    }

    Vector2 ClaimPositionCurrentObj()
    {
        var deltaTime = Time.deltaTime;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        claimPos = Vector2.MoveTowards(claimPos, mousePos + deltaPos, deltaTime * speedDragObj);
        //if (claimPos.x <= -widthCam)
        //{
        //    claimPos.x = -widthCam;
        //}
        //else if (claimPos.x >= widthCam)
        //{
        //    claimPos.x = widthCam;
        //}
        return claimPos;
    }


    public void ChangeStageDisplayPopUp(bool pause)
    {
        if (pause)
        {
            if (DataParamManager.state == DataParamManager.STATEGAMEPLAY.PLAY)
            {
                ActiveCountTime(false);
                DataParamManager.state = DataParamManager.STATEGAMEPLAY.PAUSE;
            }
        }
        else
        {
            if (DataParamManager.state == DataParamManager.STATEGAMEPLAY.PAUSE || DataParamManager.state == DataParamManager.STATEGAMEPLAY.TIMEOUT)
            {
                ActiveCountTime(true);
                DataParamManager.state = DataParamManager.STATEGAMEPLAY.PLAY;
            }
        }
    }
    LevelController levelController;
    void LoadLevel()
    {
        DataParamManager.state = DataParamManager.STATEGAMEPLAY.BEGIN;
        levelController = Instantiate(Resources.Load<LevelController>("Level/Level " + (dataController.GetCurrentLevel().indexPrefab)));
        levelController.OnAwake();
        maxTime = (float)dataController.GetCurrentLevel().maxTime;
        GamePlayUIManager.Instance.GetTimeText().text = DataParamManager.ConvertTime((double)maxTime, 0);
        EventManager.STARTLEVEL();
    }
    public void AddMaxTime(float number)
    {
        maxStarCanTake = 2;
        maxTime += number;
        GamePlayUIManager.Instance.SetAnimForBtnAddTime(false);
        GamePlayUIManager.Instance.GetTimeText().text = DataParamManager.ConvertTime((double)maxTime, 0);
        DataManager.instance.ShowPopUpMess(Vector3.one, GamePlayUIManager.Instance.GetBtnAddTime().transform.position, "+60s");
        EventManager.BOOSTERADDTIME();
    }
    public void CheckWin()
    {
        bool doneAll = true;
        for (int i = 0; i < levelController.GetLstObjectDrag().Count; i++)
        {
            if (!levelController.GetLstObjectDrag()[i].GetIsDone)
            {
                doneAll = false;
                break;
            }
        }

        if (doneAll)
        {
            DataManager.instance.CheckDoneLevel(maxStarCanTake);
            ActiveCountTime(false);
            DataParamManager.state = DataParamManager.STATEGAMEPLAY.RESULT;
            GamePlayUIManager.Instance.DisableAllUI();
            win = true;

            levelController.DoneAll(ShowEndPanel);

        }
    }

    public void CheckLose()
    {
        ActiveCountTime(false);
        DataManager.instance.ShowTimeOutPanel();
        DataParamManager.state = DataParamManager.STATEGAMEPLAY.TIMEOUT;
        if (objectDrag != null)
        {
            objectDrag.UpFuncBecauseEndTime();
            objectDrag = null;
        }

    }
    public void SkinFunc()
    {
        win = true;
        maxStarCanTake = 1;
        DataManager.instance.CheckDoneLevel(maxStarCanTake);
        DataParamManager.state = DataParamManager.STATEGAMEPLAY.RESULT;
        if (DataManager.instance.GetEndPanel() != null && DataManager.instance.GetEndPanel().gameObject.activeSelf)
        {
            DataManager.instance.GetEndPanel().CloseMe();
        }
        GamePlayUIManager.Instance.DisableAllUI();

        levelController.SkipFunc(ShowEndPanel);

        EventManager.BOOSTERSKIP();
    }
    public void GiveUpFunc()
    {
        GamePlayUIManager.Instance.DisableAllUI();
        DataParamManager.state = DataParamManager.STATEGAMEPLAY.RESULT;
        ShowEndPanel();
    }
    void ShowEndPanel()
    {
        if (win)
        {
            if (levelController.GetAnimEndLevel() == null)
            {
                DisplayCorrecntObjEndLevel();
            }
            else
            {

                if (!levelController.GetAnimEndLevel().GetDelayChutRoiMoiHienThi())
                {
                    levelController.GetAnimEndLevel().OpenMe(DisplayCorrecntObjEndLevel);
                }
                else
                {
                    StartCoroutine(CallDelayDisplayAnimEndLevel());
                }
            }

        }
        else
        {
            CallStartCapture();
        }
    }
    IEnumerator CallDelayDisplayAnimEndLevel()
    {
        yield return DataParamManager.GETTIME1S();
        levelController.GetAnimEndLevel().OpenMe(DisplayCorrecntObjEndLevel);
    }
    void DisplayCorrecntObjEndLevel()
    {
        if (levelController.GetCorrectObj() != null)
        {
            levelController.ShowCorrectObj(CallStartCapture, -1);
        }
        else
        {
            CallStartCapture();
        }
    }
    void CallStartCapture()
    {
        StartCoroutine(CaptureScreenShotAfterFrame());
    }
    public void HintFunc()
    {
        maxStarCanTake = 2;
        ChangeStageDisplayPopUp(true);
        DataManager.instance.ShowHintPopUp();
        EventManager.BOOSTERHINT();
    }
    public int GetMaxStarCanTake()
    {
        return maxStarCanTake;
    }
    public LevelController GetLevelController()
    {
        return levelController;
    }

    IEnumerator CaptureScreenShotAfterFrame()
    {
        yield return new WaitForEndOfFrame();

        int captureWidth = Mathf.RoundToInt(DataParamManager.percentTakePicture) + 200;
        if (captureWidth >= Screen.width)
        {
            captureWidth = Screen.width;
        }
        int captureHeight = captureWidth /*+ captureWidth / 2*/;

        if (captureHeight >= Screen.height)
        {
            captureHeight = Screen.height;
        }

        float captureX = (Screen.width - captureWidth) / 2;
        float captureY = (Screen.height - captureHeight) / DataParamManager.percentDown;
        Texture2D capturedImage = new Texture2D(captureWidth, captureHeight);
        capturedImage.ReadPixels(new Rect(captureX, captureY, captureWidth, captureHeight), 0, 0);
        capturedImage.Apply();
        //byte[] bytes = capturedImage.EncodeToPNG();
        //string filePath = Path.Combine(Application.persistentDataPath,/* (DataParam.currentLevel + 1) + */namePng);
        //File.WriteAllBytes(filePath, bytes);
        endSprite = Sprite.Create(capturedImage, new Rect(0, 0, capturedImage.width, capturedImage.height), new Vector2(0.5f, 0.5f));


        yield return DataParamManager.GETTIME1S();
        DataManager.instance.ShowEndPanel();
    }


    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            if (objectDrag != null)
            {
                objectDrag.UpFuncBecauseEndTime();
                objectDrag = null;
                DataParamManager.dragging = false;
            }
        }
    }

}
