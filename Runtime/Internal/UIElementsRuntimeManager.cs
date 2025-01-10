using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFlow.Internal
{
    public static class UIElementsRuntimeManager
    {
        internal static List<UIFlowElement> elementsRuntime
#if UNITY_EDITOR
        {
            get;
        }
#else
        ;
#endif

        static UIElementsRuntimeManager()
        {
            elementsRuntime = new List<UIFlowElement>();
        }

        internal static void AddUserInterfaceElement(UIFlowElement userInterfaceFlowElement)
        {
            userInterfaceFlowElement.currentSortingOrder = GetSortingOrder();
            elementsRuntime.Add(userInterfaceFlowElement);
        }

        internal static Type GetTopElement()
        {
            var elementCount = elementsRuntime.Count;
            return elementCount == 0 ? null : elementsRuntime[elementCount - 1].elementType;
        }

        internal static int GetSortingOrder()
        {
            var elementCount = elementsRuntime.Count;
            if (elementCount == 0) return 0;
            return elementsRuntime[elementCount - 1].currentSortingOrder + GameFlowRuntimeController.Manager().sortingOrderOffset;
        }

        internal static UIFlowElement GetElement(Type type)
        {
            for (var i = elementsRuntime.Count - 1; i >= 0; i--)
            {
                if (elementsRuntime[i].elementType != type) continue;
                return elementsRuntime[i];
            }

            return null;
        }

        internal static void RemoveElement(UIFlowElement element)
        {
            elementsRuntime.Remove(element);
        }

        internal static void ReleaseAllElement(Action onReleaseCompleted)
        {
            var elementCount = elementsRuntime.Count;
            Debug.LogError(elementsRuntime.Count);
            var releaseCount = new ReleaseCount(onReleaseCompleted, elementCount);
            for (var i = elementsRuntime.Count - 1; i >= 0; i--)
            {
                Debug.LogError(elementsRuntime[i].GetFullInfo());
                ReleaseElement(elementsRuntime[i], releaseCount);
            }
        }

        private static void ReleaseElement(UIFlowElement element, ReleaseCount releaseCount)
        {
            FlowObservable.Event(element.GetType()).RaiseOnRelease(true);
            switch (element.releaseMode)
            {
                default:
                case ElementReleaseMode.RELEASE_ON_CLOSE:
                case ElementReleaseMode.RELEASE_ON_CLOSE_INCLUDE_CALLBACK:
                    element.reference.ReleaseHandlePrefab(element.runtimeInstance, releaseCount);
                    break;
                case ElementReleaseMode.NONE_RELEASE:
                    element.runtimeInstance.SetActive(false);
                    releaseCount.Count();
                    break;
            }
        }

        internal static void OnKeyBack()
        {
            var type = GetTopElement();
            if (type == null) return;
            FlowObservable.UIEvent(type).RaiseOnKeyBack();
        }

        internal class ReleaseCount : IReleaseCompleted
        {
            private readonly Action onCompleted;
            private readonly int totalCount;
            private int current;

            internal ReleaseCount(Action onCompleted, int totalCount)
            {
                this.onCompleted = onCompleted;
                this.totalCount = totalCount;
            }

            void IReleaseCompleted.UnloadCompleted(bool isSuccess)
            {
                Count();
            }

            internal void Count()
            {
                if (++current == totalCount) onCompleted.Invoke();
            }
        }
    }
}