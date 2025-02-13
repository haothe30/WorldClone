using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;

public class ObjectBeginLevel : MonoBehaviour
{
    [SerializeField] SkeletonAnimation sa;
    [SerializeField] string nameIdle, nameAnim, nameAnimSkip,nameAnimAfterIdle;
    [SerializeField] Collider2D myCollider2D;
    [SerializeField] int indexSound;
    [SerializeField] GameObject objDisplayAfterplayAnimStart;
    [SerializeField] bool loopBegin = true;
    int layerOfSa;
    public int GetLayerOfSA()
    {
        return layerOfSa;
    }
    public string GetNameAnimSkip()
    {
        return nameAnimSkip;
    }    
    public SkeletonAnimation GetSa()
    {
        return sa;
    }
    public void OnStart()
    {
        if (myCollider2D != null)
            myCollider2D.enabled = true;
        gameObject.SetActive(true);
        sa.AnimationState.Complete += Complete;
        PlayAnim(nameIdle, false, null);

        layerOfSa = sa.GetComponent<Renderer>().sortingOrder;
    }

    private void Complete(TrackEntry trackEntry)
    {
        if (objDisplayAfterplayAnimStart != null)
        {
            objDisplayAfterplayAnimStart.SetActive(true);   
        }

        if (trackEntry.Animation.Name == nameIdle)
        {
            if (!string.IsNullOrEmpty(nameAnimAfterIdle))
            {
                PlayAnim(nameAnimAfterIdle, loopBegin, null);
            }
        }
    }

    public void PlayAnim(string nameAnim, bool loop, Action ac)
    {
        sa.AnimationState.SetAnimation(0, nameAnim, loop).Event += (TrackEntry trackEntry, Spine.Event e) =>
        {
            if (ac != null)
            {
                ac();
                ac = null;
            }

        };
    }
    public void OnMouseDown()
    {
        if (DataManager.instance.CanNotAction() || DataParamManager.state != DataParamManager.STATEGAMEPLAY.PLAY)
        {
            return;
        }
        myCollider2D.enabled = false;
        MusicManager.instance.PlaySoundLevelOneShot(true, indexSound);
        PlayAnim(nameAnim, false, GamePlayManager.Instance.GetLevelController().ActionAfterPlayAnimBegin);
    }
    public void SkipFunc()
    {
        if (myCollider2D != null)
            myCollider2D.enabled = false;
        PlayAnim(nameAnimSkip, false, null);
    }
    public void PlayAnimEnd()
    {
        PlayAnim(nameAnimSkip, false, null);
    }
}
