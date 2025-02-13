using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    Vector3 difference, dragOrigin;
    Camera cam;
    float mapMinX, mapMaxX;
    [SerializeField] GameObject claimMaxXObj, claimMinXObj;
    float camWidth;
    void Start()
    {
        cam = Camera.main;
        mapMinX = claimMinXObj.transform.position.x;
        mapMaxX = claimMaxXObj.transform.position.x;
        camWidth = cam.orthographicSize * cam.aspect;
    }
    public float XMin()
    {
        return claimMinXObj.transform.position.x;
    }
    public float XMax()
    {
        return claimMaxXObj.transform.position.x;
    }
    void LateUpdate()
    {
        if (DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY || DataParamManager.dragging)
            return;
      //  Debug.LogError("===================== " + DataParamManager.dragging);
        PanCamera();
    }
    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin.x = cam.ScreenToWorldPoint(Input.mousePosition).x;
        }
        if (Input.GetMouseButton(0))
        {
            difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            //if (difference.x != 0)
            //{
            //    Debug.LogError("============== true bien bool de ko bam duoc");
            //}
            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }

    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        return new Vector3(newX, cam.transform.position.y, cam.transform.position.z);
    }
}
