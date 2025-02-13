using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStartGame : ObjectDragParent
{
    [SerializeField] ObjectDragParent[] lstObjectDragLinked;
    [SerializeField] Vector2 EndposBridFlyaway;
    public override void DownFunc()
    {
        base.DownFunc();
        transform.DOMove(EndposBridFlyaway, 3f);
        DoneMe();
        MusicManager.instance.PlaySoundLevelOneShot(true, GetIndexSoundDown());
    }
    public override void DragFunc()
    {

    }
    public override void ChangePosition(Vector2 pos)
    {

    }
    public override void DoneMe()
    {
        base.DoneMe();
        GetMyCollider2D().enabled = false;
        if (lstObjectDragLinked.Length == 0)
        {
            if (GetLstNextObjectActiveAfterDone.Count > 0)
                GamePlayManager.Instance.GetLevelController().ActiveNextObect();
            else
                GamePlayManager.Instance.CheckWin();
        }
        else
        {
            bool doneAllObjectLinked = true;

            for (int i = 0; i < lstObjectDragLinked.Length; i++)
            {
                if (!lstObjectDragLinked[i].GetIsDone)
                {
                    doneAllObjectLinked = false;
                    break;
                }
            }

            if (doneAllObjectLinked)
            {
                if (GetLstNextObjectActiveAfterDone.Count > 0)
                    GamePlayManager.Instance.GetLevelController().ActiveNextObect();
                else
                    GamePlayManager.Instance.CheckWin();
            }
        }
    }
}
