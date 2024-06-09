using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class GameFlowUICanvas : MonoBehaviour
    {
        [SerializeField] protected UserInterfaceFlowElement element;
        [SerializeField] protected RectTransform safeView;
        [SerializeField] protected SafeAreaIgnore safeAreaIgnore;
        protected Canvas canvas;
        protected CanvasScaler canvasScale;
        protected RectTransform rectTransform;
        private bool hasSafeView;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasScale = GetComponent<CanvasScaler>();
            rectTransform = GetComponent<RectTransform>();
            hasSafeView = safeView != null;
        }

        private void OnEnable()
        {
            FlowSubject.UIEvent(element.elementType).OnActive += SetUpCanvas;
            FlowBannerController.OnBannerUpdate += OnBannerUpdate;
        }

        private void SetUpCanvas()
        {
            var sortingOrder = element.currentSortingOrder;
            canvas.sortingOrder = sortingOrder;
            canvas.worldCamera = FlowUICamera.instance;
            HandleCanvasScaler();
            if (hasSafeView) safeView.ApplySafeArea(Screen.safeArea, safeAreaIgnore);
        }

        private void OnBannerUpdate(float height)
        {
            safeView.offsetMin = height == 0 ? Vector2.zero : height / Screen.height * rectTransform.rect.size.y * Vector2.up;
        }

        private void HandleCanvasScaler()
        {
            canvasScale.matchWidthOrHeight = (float)Screen.width / Screen.height < canvasScale.referenceResolution.x / canvasScale.referenceResolution.y ? 0 : 1;
        }

        private void OnDisable()
        {
            FlowSubject.UIEvent(element.elementType).OnActive -= SetUpCanvas;
            FlowBannerController.OnBannerUpdate -= OnBannerUpdate;
        }
    }
}