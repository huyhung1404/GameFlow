using GameFlow.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas")]
    public class GameFlowUICanvas : FlowListenerMonoBehaviour
    {
        [SerializeField, InternalDraw(DrawType.Element)] protected UIFlowElement element;
        [SerializeField, InternalDraw(DrawType.Canvas)] protected Canvas canvas;
        [SerializeField, HideInInspector] protected bool autoGetComponent = true;
        [SerializeField, HideInInspector] protected int offsetCanvasGroup;
        [SerializeField, InternalDraw(DrawType.SafeView)] protected RectTransform safeView;
        [SerializeField, HideInInspector] protected SafeAreaIgnore safeAreaIgnore;
        protected UIElementCallbackEvent delegates;
        protected CanvasScaler canvasScale;
        protected RectTransform rectTransform;
        private bool hasSafeView;
        private bool isGotComponents;

        protected void GetComponentsIfNeed()
        {
            if (isGotComponents) return;
            delegates = FlowObservable.UIEvent(element.elementType);
            if (autoGetComponent) canvas = GetComponent<Canvas>();
            canvasScale = canvas.GetComponent<CanvasScaler>();
            rectTransform = canvas.GetComponent<RectTransform>();
            hasSafeView = safeView != null;
            canvas.worldCamera = FlowUICamera.instance;
            isGotComponents = true;
        }

        protected virtual void Awake()
        {
            GetComponentsIfNeed();
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
            GetComponentsIfNeed();
            canvas.enabled = true;
            SetUpCanvas();
        }

        protected virtual void SetUpCanvas()
        {
            var sortingOrder = element.currentSortingOrder;
            canvas.sortingOrder = sortingOrder + offsetCanvasGroup;
            canvas.planeDistance = GameFlowRuntimeController.Manager().planeDistance;
            if (hasSafeView) safeView.ApplySafeArea(Screen.safeArea, safeAreaIgnore);
            OnBannerUpdate(FlowBannerController.CurrentBannerHeight);
        }

        protected virtual void OnBannerUpdate(float height)
        {
            GetComponentsIfNeed();
            safeView.offsetMin = height == 0 ? Vector2.zero : height / Screen.height * rectTransform.rect.size.y * Vector2.up;
        }

        protected virtual void HandleCanvasScaler()
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

        internal override void SetElement(GameFlowElement value, System.Type type)
        {
            if (element != null && type != element.GetType()) return;
            if (value is not UIFlowElement uiElement) return;
            element = uiElement;
        }
    }
}