// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UniRx;
// using UniRx.Triggers;
//
// namespace JYL
// {
//     public class TestMapPresenter : MonoBehaviour
//     {
//         [SerializeField] private TestGameManager gameManager;
//         [SerializeField] private float posGap = 10f;
//         [SerializeField] private int mapCount = 3;
//
//         private Vector3 translatePos;
//         private List<float> hitList = new();
//         private int playerLayer;
//
//         private readonly Queue<Transform> mapQueue = new();
//
//         private void Awake()
//         {
//             var map = Resources.Load<GameObject>(gameManager.GetTestStageData().mapPrefabPath);
//             CreateMap(map);
//             playerLayer = LayerMask.NameToLayer("Player");
//         }
//
//         private void CreateMap(GameObject mapPrefab)
//         {
//             for (int i = 0; i < mapCount; i++)
//             {
//                 var map = 
//                     Instantiate(mapPrefab,
//                         new Vector3(posGap * i, 0, 0), 
//                         Quaternion.identity);
//                 mapQueue.Enqueue(map.transform);
//          
//                 BoxCollider col = map.GetComponent<BoxCollider>();
//                 col.OnTriggerEnterAsObservable()
//                     .Subscribe(TranslateMap)
//                     .AddTo(this);
//             }
//
//             translatePos = new Vector3(posGap * mapCount, 0, 0);
//         }
//    
//
//         private void TranslateMap(Collider hitCol) // hitCol은 충돌한 상대 물체
//         {
//             if (hitCol.gameObject.layer != playerLayer) return;
//       
//             hitList.Add(hitCol.bounds.center.x);
//             Debug.Log(hitCol.bounds.center.x);
//             if (hitList.Count <= 1) return;
//       
//             foreach (float x in hitList.SkipLast(1) )
//             {
//                 var abs = Math.Abs(x - hitCol.bounds.center.x);
//                 if (abs <= 1) return;
//             }
//       
//             var t = mapQueue.Dequeue();
//             t.Translate(translatePos);
//             mapQueue.Enqueue(t);
//         }
//     }
//
// }
