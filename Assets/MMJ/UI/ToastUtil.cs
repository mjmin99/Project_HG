public static class ToastUtil
{
    public static void Info(string message)
    {
        UIManager.Instance.ShowToast("SimpleToast", message, 2f);
    }

    public static void Success(string message)
    {
        UIManager.Instance.ShowToast("SimpleToast", message, 2f);
    }

    public static void Error(string message)
    {
        UIManager.Instance.ShowToast("SimpleToast", message, 2f);
    }
}