// using UnityEngine;
// using TMPro;
// 
// public class DamageText : MonoBehaviour
// {
//     public TMP_Text text;
//     public float lifetime = 1f;
//     public float floatSpeed = 1f;
// 
//     public static void Create(Vector3 worldPos, float dmg)
//     {
//         GameObject prefab = Resources.Load<GameObject>("UI/DamageText");
//         GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);
// 
//         obj.GetComponent<DamageText>().SetText(dmg);
//     }
// 
//     public void SetText(float dmg)
//     {
//         text.text = dmg.ToString("0");
//     }
// 
//     private void Update()
//     {
//         transform.position += Vector3.up * floatSpeed * Time.deltaTime;
//         lifetime -= Time.deltaTime;
//         if (lifetime < 0)
//             Destroy(gameObject);
//     }
// }
