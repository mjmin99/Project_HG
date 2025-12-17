// using UnityEngine;
// using UnityEngine.UI;
// 
// public class HPBar3D : MonoBehaviour
// {
//     public Slider slider;
//     private Enemy target;
// 
//     // HP바 생성 → Enemy에게 반환
//     public static HPBar3D Create(Enemy enemy)
//     {
//         GameObject prefab = Resources.Load<GameObject>("UI/HPBar3D");
//         GameObject obj = Instantiate(prefab, enemy.transform.position + Vector3.up * 2f, Quaternion.identity);
// 
//         HPBar3D bar = obj.GetComponent<HPBar3D>();
//         bar.target = enemy;
//         bar.UpdateBar(1f);
// 
//         return bar;
//     }
// 
//     public void UpdateBar(float normalizedValue)
//     {
//         slider.value = normalizedValue;
//     }
// 
//     private void LateUpdate()
//     {
//         if (target == null)
//         {
//             Destroy(gameObject);
//             return;
//         }
// 
//         // Enemy 머리 위에서 따라다니기
//         transform.position = target.transform.position + Vector3.up * 2f;
// 
//         // 카메라를 바라보도록 Billboard 처리
//         transform.LookAt(Camera.main.transform);
//         transform.Rotate(0, 180, 0);
//     }
// }
