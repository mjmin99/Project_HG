// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using System.Collections.Generic;
//
// public class StageSelectPanelController : MonoBehaviour,
//     IBeginDragHandler,
//     IEndDragHandler
// {
//     private bool isDraggingStage = false;
//
//     [Header("Stage Page Scroll (Top)")]
//     [SerializeField] private ScrollRect stageScrollRect;
//     [SerializeField] private RectTransform stageContent;
//     [SerializeField] private StagePageUI stagePagePrefab;
//
//     [Header("World Bar Scroll (Bottom)")]
//     [SerializeField] private ScrollRect worldScrollRect;
//     [SerializeField] private RectTransform worldContent;
//     [SerializeField] private WorldButtonUI worldButtonPrefab;
//
//     [Header("Config")]
//     [SerializeField] private int maxWorld = 5;          // StageDatabase 기준으로 나중에 자동화 가능
//     [SerializeField] private int stagesPerWorld = 5;   // 월드당 스테이지 수
//
//     private readonly List<StagePageUI> pages = new();
//
//     private int currentWorld = 1;
//     private int currentPageIndex = 0;
//
//     #region Unity Lifecycle
//
//     private void Awake()
//     {
//         BuildWorldButtons();
//     }
//
//     private void OnEnable()
//     {
//         // 패널이 열릴 때 현재 진행도 기준 월드로 자동 포커싱
//         int savedWorld = Manager.Save.CurrentData.clearedWorld;
//         SelectWorld(savedWorld);
//     }
//
//     private void Update()
//     {
//         HandleStagePageSnap();
//     }
//
//     #endregion
//
//     #region World Buttons
//
//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         isDraggingStage = true;
//     }
//
//     public void OnEndDrag(PointerEventData eventData)
//     {
//         isDraggingStage = false;
//     }
//
//     void BuildWorldButtons()
//     {
//         // 기존 제거
//         for (int i = worldContent.childCount - 1; i >= 0; i--)
//             Destroy(worldContent.GetChild(i).gameObject);
//
//         for (int world = 1; world <= maxWorld; world++)
//         {
//             int w = world;
//             WorldButtonUI btn = Instantiate(worldButtonPrefab, worldContent);
//             btn.Init(w, () => SelectWorld(w));
//         }
//     }
//
//     void SelectWorld(int world)
//     {
//         currentWorld = world;
//
//         RebuildStagePages(world);
//
//         // 진입 가능한 첫 스테이지로 자동 이동
//         int firstEnterableStage = FindFirstEnterableStage(world);
//         currentPageIndex = Mathf.Max(0, firstEnterableStage - 1);
//
//         SnapToPage(currentPageIndex, true);
//     }
//
//     int FindFirstEnterableStage(int world)
//     {
//         for (int stage = 1; stage <= stagesPerWorld; stage++)
//         {
//             if (StageProgressUtil.CanEnter(world, stage))
//                 return stage;
//         }
//
//         // 전부 잠겨있으면 첫 페이지
//         return 1;
//     }
//
//     #endregion
//
//     #region Stage Pages
//
//     void RebuildStagePages(int world)
//     {
//         // 기존 페이지 제거
//         for (int i = stageContent.childCount - 1; i >= 0; i--)
//             Destroy(stageContent.GetChild(i).gameObject);
//
//         pages.Clear();
//
//         for (int stage = 1; stage <= stagesPerWorld; stage++)
//         {
//             StageId id = new StageId(world, stage);
//
//             StagePageUI page = Instantiate(stagePagePrefab, stageContent);
//             page.Init(id);
//
//             pages.Add(page);
//         }
//
//         // 레이아웃 강제 갱신 (첫 프레임 위치 꼬임 방지)
//         LayoutRebuilder.ForceRebuildLayoutImmediate(stageContent);
//
//         // 스크롤 위치 초기화
//         stageScrollRect.horizontalNormalizedPosition = 0f;
//     }
//
//     #endregion
//
//     #region Page Snap Logic
//
//     void HandleStagePageSnap()
//     {
//         if (pages.Count == 0)
//             return;
//
//         // 드래그 중이면 아무것도 안 함
//         if (isDraggingStage)
//             return;
//
//         int nearest = GetNearestPageIndex();
//         if (nearest != currentPageIndex)
//         {
//             currentPageIndex = nearest;
//         }
//
//         SnapToPage(currentPageIndex, false);
//     }
//
//     int GetNearestPageIndex()
//     {
//         int lastIndex = pages.Count - 1;
//         if (lastIndex <= 0)
//             return 0;
//
//         float t = stageScrollRect.horizontalNormalizedPosition;
//         int index = Mathf.RoundToInt(t * lastIndex);
//         return Mathf.Clamp(index, 0, lastIndex);
//     }
//
//     void SnapToPage(int pageIndex, bool immediate)
//     {
//         int lastIndex = pages.Count - 1;
//         if (lastIndex <= 0)
//         {
//             stageScrollRect.horizontalNormalizedPosition = 0f;
//             return;
//         }
//
//         float target = (float)pageIndex / lastIndex;
//
//         if (immediate)
//         {
//             stageScrollRect.horizontalNormalizedPosition = target;
//         }
//         else
//         {
//             stageScrollRect.horizontalNormalizedPosition = Mathf.Lerp(
//                 stageScrollRect.horizontalNormalizedPosition,
//                 target,
//                 Time.deltaTime * 10f
//             );
//         }
//     }
//
//     #endregion
// }
