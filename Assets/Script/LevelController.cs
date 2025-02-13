
using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("0 scrath/1 sx vi tri/2 humbugur")]
    [SerializeField] int typeLevel = 0;
    [SerializeField] bool pushEventCheckDoneStep;
    [SerializeField] WrongObject wrongObj;
    [SerializeField] CorrectObject correctObj;
    [SerializeField] List<ObjectDragParent> lstObjectDrag = new List<ObjectDragParent>();
    [SerializeField] ObjectBeginLevel objectBeginLevel;
    [SerializeField] GameObject[] lstObjectDisplayAfterDone, lstObjectDisableAfterDone, lstObjectDisplayOnStart;
    [SerializeField] GameObject colliderClaimXLeft, colliderClaimXRight;
    [SerializeField] List<ObjectDrag> lstObjectDragHaveAnimationDone = new List<ObjectDrag>();
    [SerializeField] CameraDrag cameraDrag;
    [SerializeField] bool changeOrthosizeCamWhenDone;
    [SerializeField] bool displayHeartDoneAll;
    [SerializeField] GameObject all;
    [SerializeField] AnimEndLevel animEndLevel;
    [SerializeField] bool startDropElement, startDropDecore;
    [SerializeField] float[] posAll;
    [SerializeField] RotateObjectFunction RotateObjectFunction;
    // [SerializeField] float tempOrthor = 2;
    float originalOrthosizeCam;
    ObjectDragParent currentObjectDrag;


    public void CheckPlayAnimEndOfObjectBeginLevel()
    {
        if (objectBeginLevel == null)
            return;
        if (!string.IsNullOrEmpty(objectBeginLevel.GetNameAnimSkip()))
        {
            if (objectBeginLevel.GetSa().AnimationName == objectBeginLevel.GetNameAnimSkip())
                return;
            bool displayAnimEndLevel = true;

            for(int i = 0; i < lstObjectDrag.Count; i ++)
            {
                if (!lstObjectDrag[i].GetIsDone && lstObjectDrag[i].GetSpRender().sortingOrder < objectBeginLevel.GetLayerOfSA())
                {
                    displayAnimEndLevel = false;
                    break;
                }
      
            }
            if (displayAnimEndLevel)
            {
                objectBeginLevel.PlayAnimEnd();
            }
        }
    }

    public bool GetStartDropElement()
    {
        return startDropElement;
    }
    public bool GetStartDropDecore()
    {
        return startDropDecore;
    }
    #region for hamburger
    public void CheckDoneSxHumbuger()
    {
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            lstObjectDrag[i].GetIndexOf();
        }
        GamePlayManager.Instance.CheckWin();
    }

    void UpdatePositions()
    {
        foreach (var layer in lstObjectDrag)
        {
            if (layer != GamePlayManager.Instance.GetObjectDrag())
            {
                layer.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            }
        }
        List<Vector3> updatedPositions = new List<Vector3>();
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            Vector3 currentPos = lstObjectDrag[i].transform.position;
            currentPos.y = -i * GetLayerHeight(lstObjectDrag[i]);

            if (currentPos.y <= -4.5f)
                currentPos.y = -4.5f;

            currentPos.z = currentPos.y * -0.001f;

            updatedPositions.Add(currentPos);
        }
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            Vector3 targetPos = updatedPositions[i];
            lstObjectDrag[i].transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutBounce);
        }
        foreach (var layer in lstObjectDrag)
        {
            layer.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
    public void InsertLayerAt(ObjectDragParent layer, int index)
    {
        Rigidbody2D rb = layer.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        lstObjectDrag.Remove(layer);
        lstObjectDrag.Insert(index, layer);
        UpdatePositions();

        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    public void CheckAndInsertLayer(ObjectDragParent draggedLayer)
    {
        float yPos = draggedLayer.transform.position.y;
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            if (yPos > (i * GetLayerHeight(lstObjectDrag[i])) && yPos < ((i + 1) * GetLayerHeight(lstObjectDrag[i])))
            {
                InsertLayerAt(draggedLayer, i);
                break;
            }
        }
        if (yPos >= (lstObjectDrag.Count * GetLayerHeight(draggedLayer)))
        {
            lstObjectDrag.Insert(0, draggedLayer);
            UpdatePositions();
        }
    }
    private float GetLayerHeight(ObjectDragParent layer)
    {
        return layer.GetSizeBoxCollider2D().y;
    }

    #endregion
    public CorrectObject GetCorrectObj()
    {
        return correctObj;
    }
    public void ShowWrongObj(int indexSound)
    {
        if (wrongObj != null)
        {
            wrongObj.OpenMe();
            MusicManager.instance.PlaySoundLevelOneShot(true, indexSound);
        }
    }

    public void ShowCorrectObj(Action ac, int indexSound)
    {
        if (correctObj != null)
        {
            correctObj.OpenMe(ac);
            MusicManager.instance.PlaySoundLevelOneShot(true, indexSound);
        }
    }
    public AnimEndLevel GetAnimEndLevel()
    {
        return animEndLevel;
    }
    public List<ObjectDragParent> GetLstObjectDrag()
    {
        return lstObjectDrag;
    }
    public void OnAwake()
    {
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            lstObjectDrag[i].OnAwake();
        }
        if (objectBeginLevel != null)
        {
            objectBeginLevel.gameObject.SetActive(false);
        }

        if (animEndLevel != null)
        {
            animEndLevel.OnAwake();
        }

    }
    [SerializeField] float speedDropDecore = 0.5f, delayDropDecore = 0.1f;
    [SerializeField] float speedDropElement = 1f, delayDropElement = 0.01f;
    public void DropRandomDecor(float _heightCam, Action ac)
    {
        int count = 0;
        Vector3 HighPosition = Vector3.zero;
        List<Vector3> oriDecorPos = new List<Vector3>();
        if (startDropDecore)
        {
            for (int i = 0; i < lstObjectDisplayOnStart.Length; i++)
            {
                oriDecorPos.Add(lstObjectDisplayOnStart[i].transform.position);
                HighPosition = new Vector3(lstObjectDisplayOnStart[i].transform.position.x, _heightCam + 2, lstObjectDisplayOnStart[i].transform.position.z);
                lstObjectDisplayOnStart[i].transform.position = HighPosition;
                lstObjectDisplayOnStart[i].transform.DOMove(oriDecorPos[i], speedDropDecore).SetDelay(delayDropDecore * i).OnComplete(() =>
                {
                    count++;
                    if (count == lstObjectDisplayOnStart.Length)
                    {
                        DropRandomElement(_heightCam, ac);
                        Debug.LogError("===================== done drop Decor");
                    }
                });
            }
        }
        else
        {
            DropRandomElement(_heightCam, ac);
        }
    }
    public void DropRandomElement(float _heightCam, Action ac)
    {
        int count = 0;
        Vector2 HighPosition = Vector2.zero;
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            HighPosition = new Vector2(lstObjectDrag[i].transform.position.x, _heightCam + 2);
            lstObjectDrag[i].transform.position = HighPosition;
            lstObjectDrag[i].transform.gameObject.SetActive(true);
            lstObjectDrag[i].transform.DOMove(lstObjectDrag[i].GetPosBegin, speedDropElement).SetDelay(i * delayDropElement).SetEase(Ease.InQuad).OnComplete(() =>
            {
                count++;
                if (count == lstObjectDrag.Count)
                {
                    if (ac != null)
                    {
                        ac();
                        ac = null;
                    }
                    Debug.LogError("===================== done drop Element");
                }
            });
        }
    }
    public void OnStart()
    {
        originalOrthosizeCam = Camera.main.orthographicSize;
        if (all != null)
        {
            if (originalOrthosizeCam == 7.2f)
            {
                all.transform.position = new Vector2(0, posAll[1]);
            }
            else if (originalOrthosizeCam == 8f)
            {
                all.transform.position = new Vector2(0, posAll[2]);
            }
            else if (originalOrthosizeCam == 8.2f)
            {
                all.transform.position = new Vector2(0, posAll[3]);
            }
            else
            {
                all.transform.position = new Vector2(0, posAll[0]);
            }
        }


        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            lstObjectDrag[i].OnStart();
        }
        currentObjectDrag = lstObjectDrag[0];
        currentObjectDrag.ActiveMe();

        if (objectBeginLevel != null)
        {
            for (int i = 0; i < lstObjectDrag.Count; i++)
            {
                lstObjectDrag[i].gameObject.SetActive(false);
            }
            objectBeginLevel.OnStart();
        }

        if (lstObjectDisplayOnStart.Length > 0)
        {
            for (int i = 0; i < lstObjectDisplayOnStart.Length; i++)
            {
                lstObjectDisplayOnStart[i].SetActive(true);
            }
        }

        if (colliderClaimXLeft != null)
            colliderClaimXLeft.transform.position = new Vector2(-GamePlayManager.Instance.GetWidthCam() - 1, colliderClaimXLeft.transform.position.y);
        if (colliderClaimXRight != null)
            colliderClaimXRight.transform.position = new Vector2(GamePlayManager.Instance.GetWidthCam() + 1, colliderClaimXRight.transform.position.y);

        ActiveCameraDrag(true);


        if (wrongObj != null)
            wrongObj.OnStart();
        if (correctObj != null)
            correctObj.OnStart();


    }

    public ObjectBeginLevel GetObjectLevelBegin()
    {
        return objectBeginLevel;
    }
    void ActiveCameraDrag(bool active)
    {
        if (cameraDrag != null)
            cameraDrag.gameObject.SetActive(active);
    }
    public void ActionAfterPlayAnimBegin()
    {
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            lstObjectDrag[i].DisplayAfterPlayAnimBeginLevel();
            lstObjectDrag[i].gameObject.SetActive(true);
        }
    }
    public CameraDrag GetCameraDrag()
    {
        return cameraDrag;
    }
    public ObjectDragParent GetCurrentObjectDrag
    {
        get { return currentObjectDrag; }
        set { currentObjectDrag = value; }
    }
    public void SkipFunc(Action ac)
    {
        if (typeLevel == 2)
        {
            lstObjectDrag = lstObjectDrag.OrderBy(e => e.GetOriginalIndex()).ToList();
        }

        if (RotateObjectFunction == null)
        {
            for (int i = 0; i < lstObjectDrag.Count; i++)
            {
                if (typeLevel == 2)
                {
                    Vector3 currentPos = lstObjectDrag[i].transform.position;
                    currentPos.x = 0;
                    // Tính toán vị trí Y cho phần tử hiện tại
                    currentPos.y = -4.5f + (i * lstObjectDrag[i].GetSizeBoxCollider2D().y); // Tính giá trị Y theo thứ tự, mỗi lớp cách nhau 0.5f
                    currentPos.z = currentPos.y * -0.001f;
                    // Cập nhật lại vị trí
                    lstObjectDrag[i].transform.position = currentPos;
                }
                lstObjectDrag[i].SkipFunc();
            }
        }
        else
        {
            RotateObjectFunction.SkipFunc();
        }
        if (objectBeginLevel != null)
        {
            objectBeginLevel.SkipFunc();
        }

        DoneAll(ac);
    }
    public void ActiveNextObect()
    {
        GamePlayUIManager.Instance.GetBtnNextStep().SetActive(false);
        //  Debug.LogError("================ 1:" + currentObjectDrag.gameObject.name);
        if (currentObjectDrag.GetLstNextObjectActiveAfterDone.Count > 0)
        {
            if (currentObjectDrag.GetLstNextObjectActiveAfterDone.Count == 1)
            {
                if (!currentObjectDrag.GetManyTime())
                {
                    currentObjectDrag = currentObjectDrag.GetLstNextObjectActiveAfterDone[0];
                    currentObjectDrag.ActiveMe();
                    // Debug.LogError("================ 2:" + currentObjectDrag.gameObject.name);
                }
                else
                {
                    //  Debug.LogError("================ 3:" + currentObjectDrag.gameObject.name);
                    currentObjectDrag.GetIntValue--;
                    if (currentObjectDrag.GetIntValue > 0)
                    {
                        currentObjectDrag = currentObjectDrag.GetLstNextObjectActiveAfterDone[0];
                        currentObjectDrag.ActiveMe();
                    }
                    else
                    {
                        GamePlayManager.Instance.CheckWin();
                    }
                }
            }
            else
            {
                for (int i = 0; i < currentObjectDrag.GetLstNextObjectActiveAfterDone.Count; i++)
                {
                    currentObjectDrag.GetLstNextObjectActiveAfterDone[i].ActiveMe();
                }

            }
            //   Debug.LogError("================ 4:" + currentObjectDrag.gameObject.name);
        }
        else
        {
            //   Debug.LogError("================ 5:" + currentObjectDrag.gameObject.name);
            GamePlayManager.Instance.CheckWin();
        }
    }
    Action actionAfterDone;
    public void DoneAll(Action ac)
    {
        actionAfterDone = ac;
        if (lstObjectDisplayAfterDone.Length > 0)
        {
            for (int i = 0; i < lstObjectDisplayAfterDone.Length; i++)
            {
                lstObjectDisplayAfterDone[i].SetActive(true);
            }
        }
        if (lstObjectDisableAfterDone.Length > 0)
        {
            for (int i = 0; i < lstObjectDisableAfterDone.Length; i++)
            {
                lstObjectDisableAfterDone[i].SetActive(false);
            }
        }
        ActiveCameraDrag(false);


        maxCountComplete = 0;
        mainCam = Camera.main;

        if (mainCam.transform.position.x != 0 || mainCam.transform.position.y != 0)
        {
            moveCam = true;
            maxCountComplete++;
        }
        if (changeOrthosizeCamWhenDone)
        {
            zoomCam = true;
            if (RotateObjectFunction == null)
                targetOrtho = originalOrthosizeCam + /*tempOrthor*/originalOrthosizeCam / 2.5f;
            else
                targetOrtho = RotateObjectFunction.GetMaxCamZoom();
            maxCountComplete++;
        }

        if (moveCam || zoomCam)
        {
            MoveCamera(EndMoveCam);
        }
        else
        {
            EndMoveCam();
        }
    }
    public void PushEventCheckDoneStep(string value)
    {
        if (pushEventCheckDoneStep)
            EventManager.CHECKDONESTEPLEVEL(value);
    }
    Camera mainCam;
    bool moveCam = false;
    bool zoomCam = false;
    float targetOrtho;
    int maxCountComplete;
    void EndMoveCam()
    {
        if (lstObjectDragHaveAnimationDone.Count == 0)
        {
            AfterShowEffectEndLevel();
        }
        else
        {
            PlayAnimLstObjectDragHaveAnimationDone();
        }
    }
    void MoveCamera(Action ac)
    {
        int countComplete = 0;
        if (moveCam)
        {
            Vector3 posCamNeedMove = mainCam.transform.position;
            posCamNeedMove.x = 0;
            posCamNeedMove.y = 0;
            mainCam.transform.DOMove(posCamNeedMove, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                countComplete++;
                if (countComplete == maxCountComplete)
                {
                    if (ac != null)
                    {
                        ac();
                        ac = null;
                    }
                }
            });
        }
        if (zoomCam)
        {
            mainCam.DOOrthoSize(targetOrtho, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                countComplete++;
                if (countComplete == maxCountComplete)
                {
                    if (ac != null)
                    {
                        ac();
                        ac = null;
                    }
                }
            });
        }

    }
    void PlayAnimLstObjectDragHaveAnimationDone()
    {
        for (int i = 0; i < lstObjectDragHaveAnimationDone.Count; i++)
        {
            Debug.LogError("=========================== :" + i + ":" + lstObjectDragHaveAnimationDone.Count);
            if (i == lstObjectDragHaveAnimationDone.Count - 1)
            {
                lstObjectDragHaveAnimationDone[i].PlayChangeSpriteAnimation(i, AfterShowEffectEndLevel);
                //   Debug.LogError("================================ cuoi danh sach");
            }
            else
            {
                lstObjectDragHaveAnimationDone[i].PlayChangeSpriteAnimation(i, null);
            }
        }
    }
    void AfterShowEffectEndLevel()
    {
        if (actionAfterDone != null)
        {
            actionAfterDone();
            actionAfterDone = null;
        }
    }

    #region Load Editor

    public void Load()
    {
        #region for humbergur

        //// Kiểm tra danh sách có đủ phần tử không

        //// Lấy phần tử từ index 1 đến index 8 (các phần tử cần đảo vị trí)
        //List<ObjectDragParent> innerList = lstObjectDrag.GetRange(1, lstObjectDrag.Count - 2);

        //// Sử dụng phương pháp Fisher-Yates để trộn ngẫu nhiên các phần tử trong innerList
        //System.Random rand = new System.Random();
        //int n = innerList.Count;
        //while (n > 1)
        //{
        //    n--;
        //    int k = rand.Next(n + 1);
        //    ObjectDragParent value = innerList[k];
        //    innerList[k] = innerList[n];
        //    innerList[n] = value;
        //}

        //// Cập nhật lại các phần tử trong lstObjectDrag từ index 1 đến index 8
        //for (int i = 1; i <= lstObjectDrag.Count - 2; i++)
        //{
        //    lstObjectDrag[i] = innerList[i - 1]; // Đặt lại phần tử vào danh sách gốc
        //}

        //// Cập nhật lại vị trí của các phần tử trong danh sách
        //// Đặt vị trí Y cho các phần tử từ 1 đến 8
        //for (int i = 0; i < lstObjectDrag.Count; i++)
        //{
        //    Vector3 currentPos = lstObjectDrag[i].transform.position;

        //    // Tính toán vị trí Y cho phần tử hiện tại
        //    currentPos.y = lstObjectDrag[0].transform.position.y + (i * lstObjectDrag[i].GetMyCollider2D().size.y); // Tính giá trị Y theo thứ tự, mỗi lớp cách nhau 0.5f
        //    currentPos.z = currentPos.y * -0.001f;
        //    // Cập nhật lại vị trí
        //    lstObjectDrag[i].transform.position = currentPos;
        //}


        #endregion
        #region for sắp xếp lộn xộn

        lstObjectDrag.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            ObjectDragParent _objectDrag = transform.GetChild(i).GetComponent<ObjectDragParent>();
            if (_objectDrag != null)
            {
                _objectDrag.Load();
                lstObjectDrag.Add(_objectDrag);
            }
        }
        #endregion
    }

    public void Rotate()
    {
        #region for sắp xếp lộn xộn
        for (int i = 0; i < lstObjectDrag.Count; i++)
        {
            lstObjectDrag[i].Rotate();
        }
        #endregion
    }
    #endregion
}
#if UNITY_EDITOR
[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
    public bool isCheck;
    private LevelController myScript;

    private void OnSceneGUI()
    {
        myScript = (LevelController)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (myScript == null)
            myScript = (LevelController)target;

        GUIStyle SectionNameStyle = new GUIStyle();
        SectionNameStyle.fontSize = 16;
        SectionNameStyle.normal.textColor = Color.blue;

        if (myScript == null) return;

        EditorGUILayout.LabelField("----------\t----------\t----------\t----------\t----------", SectionNameStyle);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            if (GUILayout.Button("Load", GUILayout.Height(50)))
            {
                isCheck = true;
                myScript.Load();
            }
            if (GUILayout.Button("Rotate", GUILayout.Height(50)))
            {
                isCheck = true;
                myScript.Rotate();
            }
        }
        EditorGUILayout.EndVertical();
        if (isCheck)
        {
            EditorUtility.SetDirty(myScript);
            isCheck = false;
        }
        //
    }
}
#endif
