using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFlow.Tests
{
    public static class Builder
    {
        public static T CreateMono<T>() where T : MonoBehaviour
        {
            return Object.Instantiate(new GameObject()).AddComponent<T>();
        }

        public static T1 CreateChildMono<T1>(this MonoBehaviour parent) where T1 : MonoBehaviour
        {
            return Object.Instantiate(new GameObject(), parent.transform).AddComponent<T1>();
        }

        public static T1 CreateChildMono<T1, T2>(this T1 parent, Action<T2> callback) where T1 : MonoBehaviour where T2 : MonoBehaviour
        {
            callback.Invoke(Object.Instantiate(new GameObject(), parent.transform).AddComponent<T2>());
            return parent;
        }

        public static T Disable<T>(this T mono) where T : MonoBehaviour
        {
            mono.gameObject.SetActive(false);
            return mono;
        }

        public static T AddCanvasGroup<T>(this T mono, float alpha) where T : MonoBehaviour
        {
            mono.gameObject.AddComponent<CanvasGroup>().alpha = Mathf.Clamp01(alpha);
            return mono;
        }
    }
}