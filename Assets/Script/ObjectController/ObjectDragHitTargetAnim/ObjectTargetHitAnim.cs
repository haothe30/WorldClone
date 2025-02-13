using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ObjectTargetHitAnim : ObjectTargetParent
{
    [SerializeField] GameObject effect;
    [SerializeField] bool iamChildOfDragObject,disableMe;
    [SerializeField] SpriteRenderer sp;
    [SerializeField] Sprite[] sps;

    int currentStep;
    public override void OnStart()
    {
        base.OnStart();
        if (effect != null)
        {
            effect.SetActive(false);
            effect.transform.position = transform.position;
        }
    }
   

    public void HitDragObject()
    {
        if (effect != null)
            effect.SetActive(true);

        if (iamChildOfDragObject)
        {
            transform.parent = GetMyObjectDrag.transform;
            transform.localPosition = new Vector3(GetMyObjectDrag.GetPoint().transform.localPosition.x + Random.Range(-0.5f, 0.5f), GetMyObjectDrag.GetPoint().transform.localPosition.y + Random.Range(-0.5f, 0.5f), -0.001f);
            transform.localScale = new Vector3(0.2f, 0.2f, 1);
            transform.DOScale(new Vector3(0.6f, 0.6f, 1), 0.5f).SetEase(Ease.Linear);
            GetMyCollider2D().enabled = false;
        }
        else
        {
            if (sp != null)
            {
                currentStep++;
                sp.sprite = sps[currentStep];
                GetMyCollider2D().enabled = false;
            }
        }    
        if(disableMe)
        {
            gameObject.SetActive(false);
        }    
    }
}
