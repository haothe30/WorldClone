using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectFunction : MonoBehaviour
{
    [SerializeField] ListObjectRotate[] listObjectRotates;
    [SerializeField] bool cammoveStart, deactiveAfterDone;

    float camMax;
    Camera cam;
    float camSize;
    [SerializeField] float[] camOrthorSizeMaxTemp;
    private void Start()
    {
        cam = Camera.main;
        camSize = cam.orthographicSize;
        float temp = 0;
        if (camSize == 7.2f)
        {
            temp = camOrthorSizeMaxTemp[1];
        }
        else if (camSize == 8f)
        {
            temp = camOrthorSizeMaxTemp[2];
        }
        else if (camSize == 8.2f)
        {
            temp = camOrthorSizeMaxTemp[3];
        }
        else
        {
            temp = camOrthorSizeMaxTemp[0];
        }

        camMax = camSize + temp;
        if (cammoveStart)
        {
            cam.orthographicSize = camMax;
            camMovePoint = listObjectRotates[countDone].slotRotatePictures.transform.position;
            cam.transform.DOMoveX(camMovePoint.x, 1f);
            cam.transform.DOMoveY(camMovePoint.y, 1f);
            DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, camSize, 1f);
        }
    }
    Vector2 camMovePoint;
    int countDone;
    public void CheckRotateDoneObject()
    {
        bool doneObject = true;
        for (int i = 0; i < listObjectRotates[countDone].objectDragAndRotates.Length; i++)
        {
            if (!listObjectRotates[countDone].objectDragAndRotates[i].GetIsDone)
            {
                doneObject = false;
                break;
            }
            else
            {
                if (deactiveAfterDone)
                    listObjectRotates[countDone].objectDragAndRotates[i].gameObject.SetActive(false);
            }
        }
        if (doneObject)
        {
            if (countDone < listObjectRotates.Length - 1)
            {
                countDone++;
                camMovePoint = listObjectRotates[countDone].slotRotatePictures.transform.position;
                listObjectRotates[countDone].slotRotatePictures.SetActive(transform);
                cam.transform.DOMoveX(camMovePoint.x, 2f);
                cam.transform.DOMoveY(camMovePoint.y, 2f);
                DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, camMax, 1f).OnComplete(() =>
                {
                    DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, camSize, 1f);
                });
            }
            else
            {
                GamePlayManager.Instance.CheckWin();
            }
        }
    }
    public float GetMaxCamZoom()
    {
        return camMax;
    }
    public void SkipFunc()
    {
        for (int i = 0; i < listObjectRotates.Length; i++)
        {
            listObjectRotates[i].slotRotatePictures.SetActive(transform);

            for (int j = 0; j < listObjectRotates[i].objectDragAndRotates.Length; j++)
            {
                listObjectRotates[i].objectDragAndRotates[j].SkipFunc();
            }
        }
    }
}
[Serializable]
public class ListObjectRotate
{
    public GameObject slotRotatePictures;
    public ObjectDragParent[] objectDragAndRotates;
}
