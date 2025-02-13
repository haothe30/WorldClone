using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragHavePhysic : ObjectDragParent
{
    private Rigidbody2D rb;
    [SerializeField] float claimY = -4.5f;
    public override void OnStart()
    {
        base.OnStart();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f; // Giảm tốc độ rơi
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Khóa xoay
        rb.mass = 1; // Giữ khối lượng bình thường
        rb.drag = 1; // Thêm ma sát để giảm tốc độ
    }
    public override void DownFunc()
    {
        base.DownFunc();
        GetIsDone = false;
        rb.bodyType = RigidbodyType2D.Kinematic; // Vô hiệu lực vật lý tạm thời
        if (GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Contains(this))
        {
            GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Remove(this);
            GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
        }
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDown());
    }
    public override void ChangePosition(Vector2 pos)
    {
        if (pos.y <= claimY)
            pos.y = claimY;
        base.ChangePosition(pos);
    }
    private void Update()
    {
        if (DataParamManager.state == DataParamManager.STATEGAMEPLAY.RESULT)
            return;
        Vector3 myPos = new Vector3(transform.position.x, transform.position.y, transform.position.y * -0.001f);
        transform.position = myPos;
    }
    public override void GetIndexOf()
    {
        base.GetIndexOf();
        GetIntValue = GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().IndexOf(this);
        if (GetIntValue == GetOriginalIndex() && Mathf.Abs(transform.position.x - 0) <= 1f)
        {
            GetIsDone = true;
        }
        else
        {
            GetIsDone = false;
        }
    }
    public override void UpFunc()
    {
        base.UpFunc();
        ChangeSortingLayer(GetOrderLayerUp);
        GetMyCollider2D().enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        //if (transform.position.y >= 0.5f)
        //    return;

        if (!GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Contains(this))
        {
            GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Add(this);
            GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
        }

        if (GamePlayManager.Instance.GetObjectDrag().transform.position.y < GamePlayManager.Instance.GetLevelController().GetLstObjectDrag()[0].transform.position.y)
        {
            GamePlayManager.Instance.GetLevelController().CheckAndInsertLayer(this);
        }
        else
        {
            rb.gravityScale = Mathf.Clamp(Mathf.RoundToInt(transform.position.y), 1, 4);
        }

        GamePlayManager.Instance.GetLevelController().CheckDoneSxHumbuger();

        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundUp());
    }

    public override void UpFuncBecauseEndTime()
    {
        base.UpFuncBecauseEndTime();

        rb.bodyType = RigidbodyType2D.Dynamic;

        //if (transform.position.y >= 0.5f)
        //    return;

        if (!GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Contains(this))
        {
            GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Add(this);
            GamePlayManager.Instance.GetLevelController().GetLstObjectDrag().Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
        }

        if (GamePlayManager.Instance.GetObjectDrag().transform.position.y < GamePlayManager.Instance.GetLevelController().GetLstObjectDrag()[0].transform.position.y)
            GamePlayManager.Instance.GetLevelController().CheckAndInsertLayer(this);
    }

    public override void SkipFunc()
    {
        base.SkipFunc();
        ChangeSortingLayer(GetOrderLayerUp);
        //GetMyCollider2D().enabled = false;
        //rb.bodyType = RigidbodyType2D.Dynamic;
    }

}
