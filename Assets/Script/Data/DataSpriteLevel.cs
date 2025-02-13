using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DataSpriteLevel", menuName = "DataSpriteLevel")]
public class DataSpriteLevel : ScriptableObject
{
    public LstDataSpriteLevel[] lstDataSpriteLevel;
    [System.Serializable]
    public struct LstDataSpriteLevel
    {
        public Sprite hint, selectLevel;
    }
}
