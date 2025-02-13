using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[System.Serializable]
public class UnitPoolerHaveScript
{
    public UnitParent unit;
    public int amount;
}
public class ObjectPoolManagerHaveScript : MonoBehaviour
{
    public static ObjectPoolManagerHaveScript Instance;
    [HideInInspector]
    public List<ObjectPoolerHaveScript> listPooler = new List<ObjectPoolerHaveScript>();
    public UnitPoolerHaveScript[] unitPooler;

    [HideInInspector]
    public List<ObjectPoolerHaveScript> AllPool = new List<ObjectPoolerHaveScript>();
    void Awake()
    {
        if (Instance == null)
        {
            Init();
            Instance = this;
            DontDestroyOnLoad(gameObject);
         // Debug.LogError("=========== start object pool");
        }
        else
        {
            DestroyImmediate(gameObject);
        }
     //   Debug.LogError("=========== start object pool");
    }
    public void ClearAllPool()
    {

        for (int i = 0; i < AllPool.Count; i++)
        {
            for (int j = 0; j < AllPool[i].transform.childCount; j++)
            {
                AllPool[i].transform.GetChild(j).gameObject.SetActive(false);
            }
        }
    }


    public void Init()
    {
        for (int i = 0; i < unitPooler.Length; i++)
        {
            ObjectPoolerHaveScript _objectPoolerHaveScript = new ObjectPoolerHaveScript();
            go = new GameObject(unitPooler[i].unit.name);
            _objectPoolerHaveScript = go.AddComponent<ObjectPoolerHaveScript>();
            _objectPoolerHaveScript.unitPooledObject = unitPooler[i].unit;
            go.transform.parent = this.gameObject.transform;
            _objectPoolerHaveScript.InitializeUnit(unitPooler[i].amount);
            _objectPoolerHaveScript.name.Replace("(Clone)", "");
            listPooler.Add(_objectPoolerHaveScript);
            AllPool.Add(_objectPoolerHaveScript);
        }
    }

    public void SpawnResources(int indexResources, Vector2 startPos, Vector2 scale, TextMeshProUGUI textDisplayResources, int amount, bool randomPos = true, float timeDelay = 0.5f, float speedMove = 0.1f, Action _callback = null)
    {
     //   MusicController.instance.PlaySoundOther(true, 2);
        StartCoroutine(delaySpawnResources(indexResources, startPos, scale, textDisplayResources, amount, randomPos, timeDelay, speedMove, _callback));
    }

    void DisplayTextResources(int indexResources, TextMeshProUGUI textDisplayResources)
    {
        int resourcesBefore = 0;
        int resourcesTarget = 0;
        if (indexResources == 0)
        {

        }
        else
        {

        }
        //  textDisplayResources.text = "" + resourcesBefore;
        //   Debug.LogError("======= resourcesBefore:" + resourcesBefore + "_resourcesTarget:" + resourcesTarget);
        DOTween.To(() => resourcesBefore, x => resourcesBefore = x, resourcesTarget, 0.5f)
.OnUpdate(() =>
{
    // Debug.Log("========:" + power);
    textDisplayResources.text = "" + resourcesBefore;
}).OnComplete(() =>
{
    textDisplayResources.text = "" + resourcesTarget;
});

        //  Debug.LogError("============ display text resources");
    }
    IEnumerator delaySpawnResources(int indexResources, Vector2 startPos, Vector2 scale, TextMeshProUGUI textDisplayResources, int amount, bool randomPos = true, float timeDelay = 0.5f, float speedMove = 0.1f, Action _callback = null)
    {
        int countAmount = 0;
        int countResourceEndFly = 0;



        while (true)
        {
            UnitParent _unit = listPooler[indexResources].GetUnitPooledObject();
            _unit.transform.position = startPos;
            _unit.transform.localScale = scale;
            _unit.gameObject.SetActive(true);
            Vector2 posRandom = _unit.transform.position;
            Vector2 endPos = textDisplayResources.transform.position;
            if (randomPos)
            {
                posRandom.x = startPos.x + UnityEngine.Random.Range(-1f, 1f);
                posRandom.y = startPos.y + UnityEngine.Random.Range(-1f, 1f);
            }
            else
            {
                posRandom.x = startPos.x + UnityEngine.Random.Range(-0.3f, 0.3f);
                posRandom.y = startPos.y;
            }

            _unit.transform.DOMove(posRandom, Vector2.Distance(posRandom, startPos) /*/ 100*/ * speedMove).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _unit.transform.DOMove(endPos, Vector2.Distance(posRandom, endPos) /*/ 100*/ * speedMove).SetEase(Ease.InBack).OnComplete(() =>
                {
                    countResourceEndFly++;

                    if (countResourceEndFly == 1)
                    {
                      //  MusicManager.instance.PlaySoundOther(true, 8);
                        //if (_callback != null)
                        //{
                        //    _callback();
                        //}
                        DisplayTextResources(indexResources, textDisplayResources);
                    }
                    else if (countResourceEndFly == amount)
                    {
                        if (_callback != null)
                        {
                            _callback();
                        }
                    }
                    _unit.OnClose();

                }).SetDelay(timeDelay);
            });
            countAmount++;
            if (countAmount == amount)
            {

                break;
            }
            //  yield return null;

            yield return DataParamManager.GETTIME0_1S();
        }
    }
    GameObject go;
}
