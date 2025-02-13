
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParent : MonoBehaviour
{
    public virtual void OnStart(Vector3 scale, Vector2 pos)
    {
        gameObject.transform.localScale = scale;
        gameObject.transform.position = pos;
        gameObject.SetActive(true);
    }
    public virtual void OnStart(Vector3 scale, Vector2 pos, string des)
    {
        OnStart(scale, pos);
    }
    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
}
