using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DataShop", menuName = "DataShop")]
public class DataShop : ScriptableObject
{
    public ShopInfo[] shopInfo;
    [System.Serializable]
    public struct ShopInfo
    {
        public DataParamManager.TYPERESOURCES typeResource;
        public int reward;
        public string price;
        public Sprite icon;
    }
}
