using UnityEngine;

/// <summary>
/// 모든 UI의 공통 베이스
/// </summary>
public abstract class UIBase : MonoBehaviour
{
    /// UI 종류 (Panel / Popup)
    public abstract UIType Type { get; }

    /// ESC로 닫을 수 있는지
    public virtual bool CanCloseByESC => true;

    public virtual void OnOpen() { }
    public virtual void OnClose() { }
}


public enum UIType
{
    Panel,
    Popup
}

public class UIPanel : UIBase
{
    public override UIType Type => UIType.Panel;
}

public class UIPopup : UIBase
{
    public override UIType Type => UIType.Popup;
}