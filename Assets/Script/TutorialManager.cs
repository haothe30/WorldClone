using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager tutorial;

    [SerializeField] Animator handHintAnimator;
    [SerializeField] GameObject[] objectTuts;
    private void Awake()
    {
        tutorial = this;
    }
    public int GetTutCount()
    {
        return tutCount;
    }
    int tutCount;
    public void ActiveTut()
    {
        if (DataParamManager.isTuroring)
        {
            if (tutCount == 0)
            {
                MovingHand(true, GamePlayManager.Instance.GetObjectGroupTras().position, Vector3.zero);
            }
            else if (tutCount == 1)
            {
                MovingHand(false, GamePlayManager.Instance.GetLevelController().GetLstObjectDrag()[0].transform.position, objectTuts[0].transform.position);
            }
            else if (tutCount == 2)
            {
                MovingHand(true, GamePlayManager.Instance.GetObjectGroupTras().position, Vector3.zero);
            }
            else if (tutCount == 3)
            {
                objectTuts[1].SetActive(true);
                MovingHand(false, GamePlayManager.Instance.GetLevelController().GetLstObjectDrag()[1].transform.position, objectTuts[1].transform.position);
            }
            else if (tutCount == 4)
            {
                DataParamManager.isTuroring = false;
            }
            tutCount++;
        }
    }

    public void ActiveHandhint(bool active, string nameAnim)
    {
        if (active)
        {
            handHintAnimator.gameObject.SetActive(true);
            handHintAnimator.Play(nameAnim);
        }
        else
            handHintAnimator.gameObject.SetActive(false);
    }
    void MovingHand(bool choosing, Vector3 oripos, Vector3 goalPos)
    {
        if (choosing)
        {
            handHintAnimator.transform.position = oripos;
            ActiveHandhint(true, "HandChoosing");
            handHintAnimator.transform.DOKill();
        }
        else
        {
            handHintAnimator.transform.position = oripos;
            ActiveHandhint(true, "HandIdle");
            handHintAnimator.transform.DOMove(goalPos, 1f).SetLoops(-1, LoopType.Restart);
        }
    }
}
