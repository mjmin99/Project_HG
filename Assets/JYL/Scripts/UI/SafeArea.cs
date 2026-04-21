using UnityEngine;

namespace JYL.Scripts.UI
{
    public class SafeArea : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Rect lastSafeArea = Rect.zero;
        private Vector2 lastScreenSize = Vector2.zero;
        private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Refresh();
        }

        void Update()
        {
            // 화면 크기나 방향이 바뀌었을 때만 갱신 (최적화)
            if (lastSafeArea != Screen.safeArea || 
                !Mathf.Approximately(lastScreenSize.x, Screen.width) || 
                !Mathf.Approximately(lastScreenSize.y, Screen.height) || 
                lastOrientation != Screen.orientation)
            {
                Refresh();
            }
        }

        void Refresh()
        {
            Rect safeArea = Screen.safeArea;

            if (safeArea != lastSafeArea)
            {
                ApplySafeArea(safeArea);
            }
        }

        void ApplySafeArea(Rect r)
        {
            lastSafeArea = r;
            lastScreenSize.x = Screen.width;
            lastScreenSize.y = Screen.height;
            lastOrientation = Screen.orientation;

            // 0~1 사이의 정규화된 값으로 변환
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // RectTransform에 적용
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}