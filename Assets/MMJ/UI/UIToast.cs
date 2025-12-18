using System.Collections;
using UnityEngine;
using TMPro;

public class UIToast : MonoBehaviour
{
    [SerializeField] protected TMP_Text messageText;

    public void Show(string message, float duration = 2f)
    {
        messageText.text = message;
        gameObject.SetActive(true);
        StartCoroutine(AutoHide(duration));
    }

    private IEnumerator AutoHide(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
