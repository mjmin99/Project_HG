using UnityEngine;

public static class ToastUtil
{
    public static void Info(string message)
    {
        UIManager.Instance.ShowToast("SimpleToast", message, 2f);
    }

    public static void Success(string message)
    {
        Manager.Audio.PlaySfx("SFX_ToastSuccess");
        Debug.Log("로직 넘어가기");
        UIManager.Instance.ShowToast("SimpleToast", message, 2f);
    }

    public static void Error(string message)
    {
        Manager.Audio.PlaySfx("SFX_ToastError");
        UIManager.Instance.ShowToast("SimpleToast", message, 2f);
    }
}