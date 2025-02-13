using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragAndRotate : ObjectDragParent
{
    [SerializeField] RotateObjectFunction rotate;
    public override void ChangePosition(Vector2 pos)
    {
        ChangeRotationFollowMouse();
    }
    public override void UpFunc()
    {

        isRotating = false;

        float currentAngle = transform.eulerAngles.z;

        if (currentAngle > 180f)
            currentAngle -= 360f;
        if (currentAngle >= -5f && currentAngle <= 5f)
        {
            transform.rotation = Quaternion.identity;
            GetMyCollider2D().enabled = false;
            GetIsDone = true;
            rotate.CheckRotateDoneObject();
        }
        else
        {
            GetMyCollider2D().enabled = true;
        }
    }
    public override void UpFuncBecauseEndTime()
    {
        base.UpFuncBecauseEndTime();

        isRotating = false;
        GetMyCollider2D().enabled = true;

    }
    public override void DownFunc()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos -= objectPos;

        initialAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - transform.eulerAngles.z;
        isRotating = true;
    }


    float initialAngle;
    bool isRotating = false;
    void ChangeRotationFollowMouse()
    {
        if (isRotating)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos -= objectPos;

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - initialAngle + 360);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }
}
