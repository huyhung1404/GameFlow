using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [AddComponentMenu("Game Flow/UI Canvas")]
    public class GameFlowUICanvas : MonoBehaviour
    {
        [SerializeField] protected UserInterfaceFlowElement element;
        [SerializeField] protected RectTransform safeView;
        [SerializeField] protected SafeAreaIgnore safeAreaIgnore;
        protected Canvas canvas;
        protected CanvasScaler canvasScale;
        protected RectTransform rectTransform;
        private bool hasSafeView;

        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasScale = GetComponent<CanvasScaler>();
            rectTransform = GetComponent<RectTransform>();
            hasSafeView = safeView != null;
        }

        protected virtual void OnEnable()
        {
            RegisterDelegates(FlowObservable.UIEvent(element.elementType));
            FlowBannerController.OnBannerUpdate += OnBannerUpdate;
        }

        protected virtual void RegisterDelegates(UIElementCallbackEvent delegates)
        {
            delegates.OnActive += SetUpCanvas;
            delegates.OnKeyBack += OnKeyBack;
        }

        protected virtual void SetUpCanvas()
        {
            var sortingOrder = element.currentSortingOrder;
            canvas.sortingOrder = sortingOrder;
            canvas.worldCamera = FlowUICamera.instance;
            HandleCanvasScaler();
            if (hasSafeView) safeView.ApplySafeArea(Screen.safeArea, safeAreaIgnore);
            OnBannerUpdate(FlowBannerController.CurrentBannerHeight);
        }

        private void OnBannerUpdate(float height)
        {
            safeView.offsetMin = height == 0 ? Vector2.zero : height / Screen.height * rectTransform.rect.size.y * Vector2.up;
        }

        private void HandleCanvasScaler()
        {
            canvasScale.matchWidthOrHeight = (float)Screen.width / Screen.height < canvasScale.referenceResolution.x / canvasScale.referenceResolution.y ? 0 : 1;
        }

        protected virtual void OnDisable()
        {
            UnregisterDelegates(FlowObservable.UIEvent(element.elementType));
            FlowBannerController.OnBannerUpdate -= OnBannerUpdate;
        }

        protected virtual void UnregisterDelegates(UIElementCallbackEvent delegates)
        {
            delegates.OnActive -= SetUpCanvas;
            delegates.OnKeyBack -= OnKeyBack;
        }

        protected virtual void OnKeyBack()
        {
            ReleaseCanvas();
        }

        public void ReleaseCanvas()
        {
            GameCommand.Release(element.elementType).Build();
        }
    }
}