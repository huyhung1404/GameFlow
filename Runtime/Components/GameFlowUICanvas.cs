using GameFlow.Internal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas")]
    public class GameFlowUICanvas : FlowListenerMonoBehaviour
    {
        [SerializeField, Element, FormerlySerializedAs("element")] protected UIFlowElement m_element;
        [SerializeField, HideInInspector, FormerlySerializedAs("canvas")] protected Canvas m_canvas;
        [SerializeField, HideInInspector, FormerlySerializedAs("autoGetComponent")] protected bool m_autoGetComponent = true;
        [SerializeField, HideInInspector, FormerlySerializedAs("offsetCanvasGroup")] protected int m_offsetCanvasGroup;
        [SerializeField, HideInInspector, FormerlySerializedAs("safeView")] protected RectTransform m_safeView;
        [SerializeField, HideInInspector, FormerlySerializedAs("safeAreaIgnore")] protected SafeAreaIgnore m_safeAreaIgnore;
        protected UIElementCallbackEvent _delegates;
        protected CanvasScaler _canvasScale;
        protected RectTransform _rectTransform;
        private bool _hasSafeView;
        private bool _isGotComponents;

        protected void GetComponentsIfNeed()
        {
            if (_isGotComponents) return;
            _delegates = FlowObservable.UIEvent(m_element.ElementType);
            if (m_autoGetComponent) m_canvas = GetComponent<Canvas>();
            _canvasScale = m_canvas.GetComponent<CanvasScaler>();
            _rectTransform = m_canvas.GetComponent<RectTransform>();
            _hasSafeView = m_safeView != null;
            m_canvas.worldCamera = FlowUICamera.Instance;
            _isGotComponents = true;
        }

        protected virtual void Awake()
        {
            GetComponentsIfNeed();
            HandleCanvasScaler();
        }

        protected virtual void OnEnable()
        {
            m_canvas.enabled = false;
            _delegates.RegisterListener(this);
            FlowBannerController.OnBannerUpdate += OnBannerUpdate;
        }

        public override void OnActive()
        {
            GetComponentsIfNeed();
            m_canvas.enabled = true;
            SetUpCanvas();
        }

        protected virtual void SetUpCanvas()
        {
            var sortingOrder = m_element.CurrentSortingOrder;
            m_canvas.sortingOrder = sortingOrder + m_offsetCanvasGroup;
            m_canvas.planeDistance = GameFlowRuntimeController.Manager().PlaneDistance;
            if (_hasSafeView) m_safeView.ApplySafeArea(Screen.safeArea, m_safeAreaIgnore);
            OnBannerUpdate(FlowBannerController.CurrentBannerHeight);
        }

        protected virtual void OnBannerUpdate(float height)
        {
            GetComponentsIfNeed();
            m_safeView.offsetMin = height == 0 ? Vector2.zero : height / Screen.height * _rectTransform.rect.size.y * Vector2.up;
        }

        protected virtual void HandleCanvasScaler()
        {
            _canvasScale.referenceResolution = GameFlowRuntimeController.Manager().ReferenceResolution;
            _canvasScale.matchWidthOrHeight = (float)Screen.width / Screen.height < _canvasScale.referenceResolution.x / _canvasScale.referenceResolution.y ? 0 : 1;
        }

        protected virtual void OnDisable()
        {
            _delegates.UnregisterListener(this);
            FlowBannerController.OnBannerUpdate -= OnBannerUpdate;
        }

        public override void OnKeyBack()
        {
            ReleaseCanvas();
        }

        public void ReleaseCanvas()
        {
            GameCommand.Release(m_element.ElementType).Build();
        }

        internal Canvas GetCanvas()
        {
            return m_autoGetComponent ? GetComponent<Canvas>() : m_canvas;
        }

        internal override void SetElement(GameFlowElement value, System.Type type)
        {
            if (m_element != null && type != m_element.GetType()) return;
            if (value is not UIFlowElement uiElement) return;
            m_element = uiElement;
        }
    }
}