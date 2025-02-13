using ScratchCardAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTargetScratch : ObjectTargetParent
{
    [SerializeField] ScratchCardManager scratchCardManager;
    [SerializeField] float alphaBegin = 1f;
  
    bool waitCalculate = false;
    public bool GetWaitCalculate
    {
        get { return waitCalculate; }
        set
        {
            waitCalculate = value;
        }

    }
    public float GetProcessScratch()
    {
        return scratchCardManager.Progress.GetProgress();
    }
    public ScratchCard.ScratchMode GetModeScratch()
    {
        return scratchCardManager.Card.Mode;
    }
    public void DoneScratch()
    {
        
        if (scratchCardManager.Card.Mode == ScratchCard.ScratchMode.Erase)
        {
            scratchCardManager.Card.FillInstantly();
            gameObject.SetActive(false);
        }
        else
        {
            scratchCardManager.Card.ClearInstantly();
            gameObject.SetActive(true);
            scratchCardManager.SpriteCard.gameObject.SetActive(true);
        }
        GetMyCollider2D().enabled = false;
    }

    public override void OnAwake()
    {
        base.OnAwake();
        scratchCardManager.OnAwake();
        //   scratchCardManager.Card.ChangeColorSpriteRender(alphaBegin, true);
    }
    public void SetModeForScratch(ScratchCard.ScratchMode mode, Vector2 scaleBrush)
    {

        scratchCardManager.Card.Mode = mode;
        scratchCardManager.Card.BrushScale = scaleBrush;
        if (scratchCardManager.Card.Mode == ScratchCard.ScratchMode.Restore)
        {
            scratchCardManager.SpriteCard.gameObject.SetActive(false);
        }
        scratchCardManager.Card.ChangeColorSpriteRender(alphaBegin, true);
    }
    public void ChangeColorForScratch(float a)
    {
        scratchCardManager.Card.ChangeColorSpriteRender(a, false);
    }

    public void ActiveTargetScratch()
    {
        gameObject.SetActive(true);

        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return DataParamManager.GETTIME0_1S();
        if (scratchCardManager.Card.Mode == ScratchCard.ScratchMode.Restore)
        {
            scratchCardManager.Card.FillInstantly();
            scratchCardManager.Progress.ResetProgress();
            scratchCardManager.Progress.UpdateProgress();
            scratchCardManager.SpriteCard.SetActive(true);
        }
        else
        {
            scratchCardManager.Card.ClearInstantly();
            scratchCardManager.Progress.ResetProgress();
            scratchCardManager.Progress.UpdateProgress();
        }
        waitCalculate = false;
        //if(alphaBegin != 1)
        //{
        //    scratchCardManager.Card.ChangeColorSpriteRender(alphaBegin, true);
        //}    
    }
    public void SetPoint(GameObject _point)
    {
        scratchCardManager.Card.SetUpEarase(_point);
    }
    public override void OnStart()
    {
        //  Debug.LogError("============ :" + gameObject.name);
        scratchCardManager.OnStart();
    }

}
