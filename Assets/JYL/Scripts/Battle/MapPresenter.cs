using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MapPresenter : MonoBehaviour
{
    [Header("Set Refs")]
    [SerializeField] private Transform mapParent;
    
    [Header("Set Values")]
    [SerializeField] private float posGap = 10f;
    [SerializeField] private int mapCount = 3;

    private Vector3 translatePos;
    private readonly List<float> hitList = new();
    private int playerLayer;

    private readonly Queue<Transform> mapQueue = new();
    private readonly string testPrefabPath = "Test/TestTerrain";

    public void Init()
    {
        var map = Resources.Load<GameObject>(Manager.Game.GetStageData().mapPrefabPath);
        if (map == null)
        {
            Debug.LogWarning("맵 프리팹 경로 설정 안되어 있음");
            map = Resources.Load<GameObject>(testPrefabPath);
        }
        CreateMap(map);
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void CreateMap(GameObject mapPrefab)
    {
        for (int i = 0; i < mapCount; i++)
        {
            var map = 
                Instantiate(mapPrefab,
                    new Vector3(posGap * i, 0, 0), 
                    Quaternion.identity);
            map.transform.SetParent(mapParent);
            mapQueue.Enqueue(map.transform);
         
            BoxCollider col = map.GetComponent<BoxCollider>();
            col.OnTriggerEnterAsObservable()
                .Subscribe(TranslateMap)
                .AddTo(this);
        }

        translatePos = new Vector3(posGap * mapCount, 0, 0);
    }
    
    private void TranslateMap(Collider hitCol) // hitCol은 충돌한 상대 물체
    {
        if (hitCol.gameObject.layer != playerLayer) return;
      
        hitList.Add(hitCol.bounds.center.x);
        if (hitList.Count <= 1) return;
      
        foreach (float x in hitList.SkipLast(1) )
        {
            var abs = Mathf.Abs(x - hitCol.bounds.center.x);
            if (abs <= 1) return;
        }
      
        var t = mapQueue.Dequeue();
        t.Translate(translatePos);
        mapQueue.Enqueue(t);
    }
}
