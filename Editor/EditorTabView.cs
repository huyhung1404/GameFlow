using UnityEditor;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    public partial class EditorTabView : VisualElement
    {
        [UxmlAttribute("default-tab-id")] public string DefaultTabId { get; set; } = "";
#else
    public class EditorTabView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EditorTabView, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_defaultTabId = new UxmlStringAttributeDescription { name = "default-tab-id", defaultValue = "" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var view = (EditorTabView)ve;
                view.DefaultTabId = m_defaultTabId.GetValueFromBag(bag, cc);
            }
        }

        public string DefaultTabId { get; set; } = "";
#endif

        private VisualElement _indicator;
        private string _currentTabId = "";
        private bool _isInitialized;

        private string GetPrefKey() => $"GameFlow_TabView_{name ?? "Default"}_LastTabId";

        public EditorTabView()
        {
            AddToClassList("gameflow-tabview");
            RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            _indicator = this.Query<VisualElement>("", "gameflow-tabview__indicator").First();

            EditorTab tabToActivate = null;

            string savedTabId = EditorPrefs.GetString(GetPrefKey(), DefaultTabId);

            if (!string.IsNullOrEmpty(savedTabId))
            {
                tabToActivate = this.Query<EditorTab>().Where(t => t.TargetId == savedTabId).First();
            }

            if (tabToActivate == null && !string.IsNullOrEmpty(DefaultTabId))
            {
                tabToActivate = this.Query<EditorTab>().Where(t => t.TargetId == DefaultTabId).First();
            }

            if (tabToActivate == null)
            {
                tabToActivate = this.Query<EditorTab>().First();
            }

            if (tabToActivate != null)
            {
                _currentTabId = "";
                SelectTab(tabToActivate.TargetId, false);
            }
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (_isInitialized || _indicator == null || string.IsNullOrEmpty(_currentTabId)) return;

            var activeTab = this.Query<EditorTab>().Where(t => t.TargetId == _currentTabId).First();
            if (activeTab != null && activeTab.layout.width > 0)
            {
                _isInitialized = true;
                UpdateIndicatorPosition(activeTab, false);
            }
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.target is not VisualElement clickedElement) return;

            var tab = clickedElement.GetFirstAncestorOfType<EditorTab>();
            if (tab == null && clickedElement is EditorTab directTab) tab = directTab;

            if (tab != null)
            {
                SelectTab(tab.TargetId);
                evt.StopPropagation();
            }
        }

        public void SelectTab(string targetId, bool animate = true)
        {
            if (string.IsNullOrEmpty(targetId) || _currentTabId == targetId) return;

            _currentTabId = targetId;

            EditorPrefs.SetString(GetPrefKey(), _currentTabId);

            var allTabs = this.Query<EditorTab>().ToList();
            var allContents = this.Query<EditorTabContent>().ToList();
            EditorTab activeTabElement = null;

            foreach (var tab in allTabs)
            {
                if (tab.TargetId == targetId)
                {
                    tab.AddToClassList("gameflow-tab--active");
                    activeTabElement = tab;
                }
                else
                {
                    tab.RemoveFromClassList("gameflow-tab--active");
                }
            }

            foreach (var content in allContents)
            {
                if (content.TargetId == targetId)
                    content.AddToClassList("gameflow-tab-content--visible");
                else
                    content.RemoveFromClassList("gameflow-tab-content--visible");
            }

            if (_isInitialized && activeTabElement != null)
            {
                UpdateIndicatorPosition(activeTabElement, animate);
            }
        }

        private void UpdateIndicatorPosition(EditorTab activeTabElement, bool animate)
        {
            if (_indicator == null) return;

            _indicator.schedule.Execute(() =>
            {
                float targetLeft = activeTabElement.layout.x;
                float targetWidth = activeTabElement.layout.width;

                if (animate)
                {
                    _indicator.experimental.animation.Start(_indicator.resolvedStyle.left, targetLeft, 200, (ve, val) => ve.style.left = val);
                    _indicator.experimental.animation.Start(_indicator.resolvedStyle.width, targetWidth, 200, (ve, val) => ve.style.width = val);
                }
                else
                {
                    _indicator.style.left = targetLeft;
                    _indicator.style.width = targetWidth;
                }
            });
        }
    }
}