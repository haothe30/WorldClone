using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataLevel", menuName = "DataLevel")]
public class DataLevel : ScriptableObject
{
    public LstDataLevel[] lstDataLevel;
    [System.Serializable]
    public class LstDataLevel
    {
        public LevelInfo[] levelInfo;
    }
    [System.Serializable]
    public class LevelInfo
    {
        public int indexLevel;
        public int indexPrefab;
        public double maxTime;
        public int totalStarToUnlock;
    }
}
