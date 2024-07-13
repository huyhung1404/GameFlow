using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas")]
    public class GameFlowUICanvas : FlowListenerMonoBehaviour
    {
        [SerializeField] protected bool autoGetComponent = true;
        [SerializeField] protected UIFlowElement element;
        [SerializeField] protected RectTransform safeView;
        [SerializeField] protected SafeAreaIgnore safeAreaIgnore;
        [SerializeField] protected Canvas canvas;
        protected UIElementCallbackEvent delegates;
        protected CanvasScaler canvasScale;
        protected RectTransform rectTransform;
        private bool hasSafeView;

        protected virtual void Awake()
        {
            delegates = FlowObservable.UIEvent(element.elementType);
            if (autoGetComponent) canvas = GetComponent<Canvas>();
            canvasScale = canvas.GetComponent<CanvasScaler>();
            rectTransform = canvas.GetComponent<RectTransform>();
            hasSafeView = safeView != null;
            canvas.worldCamera = FlowUICamera.instance;
            HandleCanvasScaler();
        }

        protected virtual void OnEnable()
        {
            canvas.enabled = false;
            delegates.RegisterListener(this);
            FlowBannerController.OnBannerUpdate += OnBannerUpdate;
        }

        public override void OnActive()
        {
            canvas.enabled = true;
            SetUpCanvas();
        }

        protected virtual void SetUpCanvas()
        {
            var sortingOrder = element.currentSortingOrder;
            canvas.sortingOrder = sortingOrder;
            if (hasSafeView) safeView.ApplySafeArea(Screen.safeArea, safeAreaIgnore);
            OnBannerUpdate(FlowBannerController.CurrentBannerHeight);
        }

        protected virtual void OnBannerUpdate(float height)
        {
            safeView.offsetMin = height == 0 ? Vector2.zero : height / Screen.height * rectTransform.rect.size.y * Vector2.up;
        }

        private void HandleCanvasScaler()
        {
            canvasScale.matchWidthOrHeight = (float)Screen.width / Screen.height < canvasScale.referenceResolution.x / canvasScale.referenceResolution.y ? 0 : 1;
        }

        protected virtual void OnDisable()
        {
            delegates.UnregisterListener(this);
            FlowBannerController.OnBannerUpdate -= OnBannerUpdate;
        }

        public override void OnKeyBack()
        {
            ReleaseCanvas();
        }

        public void ReleaseCanvas()
        {
            GameCommand.Release(element.elementType).Build();
        }

        internal Canvas GetCanvas()
        {
            return autoGetComponent ? GetComponent<Canvas>() : canvas;
        }
    }
}