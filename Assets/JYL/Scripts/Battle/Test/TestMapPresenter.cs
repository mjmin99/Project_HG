using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TestMapPresenter : MonoBehaviour
{
   [SerializeField] private TestGameManager gameManager;
   [SerializeField] private float posGap = 10f;
   [SerializeField] private int mapCount = 3;

   private Vector3 translatePos;
   private List<float> hitList = new();

   private readonly Queue<Transform> mapQueue = new();

   private void Awake()
   {
      var map = Resources.Load<GameObject>(gameManager.GetTestStageData().mapPrefabPath);
      CreateMap(map);
   }

   private void CreateMap(GameObject mapPrefab)
   {
      for (int i = 0; i < mapCount; i++)
      {
         var map = 
            Instantiate(mapPrefab, 
               new Vector3(posGap * i, 0, 0), 
               Quaternion.identity);
         mapQueue.Enqueue(map.transform);
         
         BoxCollider col = map.GetComponent<BoxCollider>();
         col.OnTriggerEnterAsObservable().Skip(i == 0 ? 1 : 0)
            .Subscribe(TranslateMap).AddTo(this);
      }

      translatePos = new Vector3(posGap * mapCount, 0, 0);
   }
   

   private void TranslateMap(Collider hitCol) // hitCol은 충돌한 상대 물체
   {
      if (!hitCol.CompareTag("Player")) return;
      if (hitList.Count > 0)
      {
         foreach (float x in hitList)
         {
            var abs = Math.Abs(x - hitCol.bounds.center.x);
            if (abs <= 1) return;
         }
      }
      hitList.Add(hitCol.bounds.center.x);
      var t = mapQueue.Dequeue();
      t.Translate(translatePos);
      mapQueue.Enqueue(t);
   }
}
