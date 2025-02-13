using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTargetOfObjectDragToTargetAndPlayAnimOfTarget : ObjectTargetParent
{
    [SerializeField] SkeletonAnimation sa;
    [SerializeField] string nameAnim, nameIdle = "Idle";
    [SerializeField] int layerWhenPlayAnim = 2;
    [SerializeField] GameObject[] lstGameObjectDisableWhenPlayAnim;
    Renderer render;
    int layerOriginal;
    string nameSkin;
    public string GetNameSkinTarget
    {
        get { return nameSkin; }
        set { nameSkin = value; }
    }
    public void DisableLstObjectWhenPlayAnim()
    {
        for (int i = 0; i < lstGameObjectDisableWhenPlayAnim.Length; i++)
        {
            lstGameObjectDisableWhenPlayAnim[i].SetActive(false);
        }
    }
    public void Start()
    {
        render = sa.GetComponent<Renderer>();
        layerOriginal = render.sortingOrder;
    }
    public void PlayAnimNear(bool play, string nameAnim)
    {
        if (play)
        {
            sa.AnimationState.SetAnimation(0, nameAnim, false);
        }
        else
        {
            if (sa.AnimationName != nameAnim)
                sa.AnimationState.SetAnimation(0, nameIdle, true);
        }
    }
    public void PlayAnim(Action ac, string nameSkin = "")
    {
        DataParamManager.playingAnim = true;
        if (!string.IsNullOrEmpty(nameSkin))
            ChangeSkinTarget(nameSkin);
        GetMyCollider2D().enabled = false;
        render.sortingOrder = layerWhenPlayAnim;
        sa.AnimationState.SetAnimation(0, nameAnim, false).Complete += (TrackEntry trackEntry) =>
        {
            DataParamManager.playingAnim = false;
            GetMyCollider2D().enabled = true;

            if (ac != null)
            {
                ac();
                ac = null;
            }
            sa.AnimationState.SetAnimation(0, nameIdle, true);
            render.sortingOrder = layerOriginal;
        };
    }
    void ChangeSkinTarget(string nameSkin)
    {
        sa.Skeleton.SetSkin(nameSkin);
        sa.Skeleton.SetSlotsToSetupPose();
        sa.Update(0);
    }
}
