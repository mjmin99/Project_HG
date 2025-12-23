// using UnityEngine;
// using UnityEngine.UI;
// 
// public class PlayerHPBar3D : MonoBehaviour
// {
//     public Slider slider;
//     private PlayerBattleUnit target;
// 
//     // Player용 HPBar 생성
//     public static PlayerHPBar3D Create(PlayerBattleUnit player)
//     {
//         GameObject prefab = Resources.Load<GameObject>("UI/PlayerHPBar3D");
//         GameObject obj = Instantiate(prefab, player.transform.position + Vector3.up * 2f, Quaternion.identity);
// 
//         PlayerHPBar3D bar = obj.GetComponent<PlayerHPBar3D>();
//         bar.target = player;
//         bar.UpdateBar(1f);
// 
//         return bar;
//     }
// 
//     public void UpdateBar(float normalizedValue)
//     {
//         slider.value = Mathf.Clamp01(normalizedValue);
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
//         // 플레이어 머리 위에서 따라다니기
//         transform.position = target.transform.position + Vector3.up * 2f;
// 
//         // 카메라 바라보도록 빌보드 처리
//         transform.LookAt(Camera.main.transform);
//         transform.Rotate(0, 180f, 0f);
//     }
// }
