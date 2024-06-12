using System;
using System.Collections;
using GameFlow.Tests.Build;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = GameFlow.Internal.Assert;
using Random = UnityEngine.Random;

namespace GameFlow.Tests
{
    public class LoadingTests : BaseTests
    {
        [UnityTest]
        public IEnumerator Loading_AllHide_ShowAllType()
        {
            var display = ResourcesInstance.loadingController.LoadingOn(0);
            yield return null;
            Assert.IsTrue(display.gameObject.activeSelf);
            ResourcesInstance.loadingController.LoadingOff(0);
            yield return null;
            Assert.IsTrue(!display.gameObject.activeSelf);

            var fade = (FadeLoading)ResourcesInstance.loadingController.LoadingOn(1);
            yield return new WaitForSeconds(0.5f);
            fade.LoadingIsShow();
            ResourcesInstance.loadingController.LoadingOff(1);
            yield return new WaitForSeconds(0.5f);
            fade.LoadingIsHide();

            var progress = (ProgressLoading)ResourcesInstance.loadingController.LoadingOn(2);
            yield return null;
            progress.UpdateProgress(0.3f);
            yield return new WaitForSeconds(0.2f);
            progress.UpdateProgress(0.6f);
            Assert.IsGameObjectEnable(progress);
            yield return new WaitForSeconds(0.2f);
            progress.UpdateProgress(1f);
            yield return new WaitForSeconds(0.3f);
            Assert.IsGameObjectDisable(progress);
        }

        [UnityTest]
        public IEnumerator Loading_Hide_FadeShowStraightWay()
        {
            var time = Time.time;
            float timeOn;
            float timeOff;
            var fade = (FadeLoading)ResourcesInstance.loadingController.LoadingOn(1).OnCompleted(() =>
            {
                timeOn = Time.time - time;
                Assert.IsTrue(timeOn >= 0.5, timeOn.ToString("0.0000"));
            }).SetTime(0.5f);
            ResourcesInstance.loadingController.LoadingOff(1).OnCompleted(() =>
            {
                timeOff = Time.time - time;
                Assert.IsTrue(timeOff > 1f, timeOff.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(1.2f);
            Assert.IsTrue(!fade.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Loading_Show_FadeDoubleShowStraightWay()
        {
            var time = Time.time;
            float time1;
            float time2;
            var fade = (FadeLoading)ResourcesInstance.loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time1 = Time.time - time;
                Assert.IsTrue(time1 < 0.32f, time1.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.3f);
            ResourcesInstance.loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time2 = Time.time - time;
                Assert.IsTrue(time2 < 0.52f, time2.ToString("0.0000"));
            }).SetTime(0.5f);

            yield return new WaitForSeconds(0.6f);
            Assert.IsTrue(fade.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Loading_Hide_Fade2Show2HideStraightWay()
        {
            var time = Time.time;
            float time1;
            float time2;
            float time3;
            float time4;
            var fade = (FadeLoading)ResourcesInstance.loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time1 = Time.time - time;
                Assert.IsTrue(time1 < 0.32f, time1.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.3f);
            ResourcesInstance.loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time2 = Time.time - time;
                Assert.IsTrue(time2 < 0.52f, time2.ToString("0.0000"));
            }).SetTime(0.5f);

            ResourcesInstance.loadingController.LoadingOff(1).OnCompleted(() =>
            {
                time3 = Time.time - time;
                Assert.IsTrue(time3 < 0.82f, time3.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.3f);
            ResourcesInstance.loadingController.LoadingOff(1).OnCompleted(() =>
            {
                time4 = Time.time - time;
                Assert.IsTrue(time4 < 1.02f, time4.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.72f);
            Assert.IsTrue(!fade.gameObject.activeSelf);
        }

        private static readonly int[] FadeCounts = { 1, 10, 20 };

        [UnityTest]
        public IEnumerator Loading_Random_Fade([ValueSource(nameof(FadeCounts))] int fadeTime)
        {
            for (var i = 0; i < fadeTime; i++)
            {
                var time1 = Random.Range(0, 0.5f);
                var time2 = Random.Range(0, 0.5f);
                var time3 = Random.Range(0, 0.5f);
                var time4 = Random.Range(0, 0.5f);
                var timeExecute1 = Random.Range(0, 0.5f);
                var timeExecute2 = Random.Range(0, 0.5f);
                var timeExecute3 = Random.Range(0, 0.5f);
                var timeExecute4 = Random.Range(0, 0.5f);
                Debug.Log($"var time1 = {time1}f;\n                " +
                          $"var time2 = {time2}f;\n                " +
                          $"var time3 = {time3}f;\n                " +
                          $"var time4 = {time4}f;\n                " +
                          $"var timeExecute1 = {timeExecute1}f;\n                " +
                          $"var timeExecute2 = {timeExecute2}f;\n                " +
                          $"var timeExecute3 = {timeExecute3}f;\n                " +
                          $"var timeExecute4 = {timeExecute4}f;");
                var lastIsHide = true;
                var execute = 0;
                ResourcesInstance.loadingController.StartCoroutine(CreateFadeOn(time1, timeExecute1, () =>
                {
                    lastIsHide = false;
                    execute++;
                }));
                ResourcesInstance.loadingController.StartCoroutine(CreateFadeOn(time2, timeExecute2, () =>
                {
                    lastIsHide = false;
                    execute++;
                }));
                ResourcesInstance.loadingController.StartCoroutine(CreateFadeOff(time3, timeExecute3, () =>
                {
                    lastIsHide = true;
                    execute++;
                }));
                ResourcesInstance.loadingController.StartCoroutine(CreateFadeOff(time4, timeExecute4, () =>
                {
                    lastIsHide = true;
                    execute++;
                }));
                yield return new WaitForSeconds(time1 + time2 + time3 + time4 + 1.5f + timeExecute1 + timeExecute2 + timeExecute3 + timeExecute4);
                Assert.AreEqual(execute, 4, "Execute is not 4");
                if (lastIsHide)
                {
                    ResourcesInstance.fadeLoading.LoadingIsHide();
                }
                else
                {
                    ResourcesInstance.fadeLoading.LoadingIsShow();
                }
            }
        }

        private static IEnumerator CreateFadeOn(float time, float timeExecute, Action onCompleted)
        {
            yield return new WaitForSeconds(time);
            ResourcesInstance.loadingController.LoadingOn(1).OnCompleted(onCompleted).SetTime(timeExecute);
        }

        private static IEnumerator CreateFadeOff(float time, float timeExecute, Action onCompleted)
        {
            yield return new WaitForSeconds(time);
            ResourcesInstance.loadingController.LoadingOff(1).OnCompleted(onCompleted).SetTime(timeExecute);
        }

        [UnityTest]
        public IEnumerator Loading_Hide_ProgressLoading()
        {
            var onCompleted = false;
            var progress = (ProgressLoading)ResourcesInstance.loadingController.LoadingOn(2)
                .OnCompleted(() => onCompleted = true)
                .SetTime(0.15f);
            yield return null;
            progress.UpdateProgress(0.5f);
            ResourcesInstance.loadingController.LoadingOff(2);
            yield return new WaitForSeconds(0.1f);
            Assert.IsGameObjectEnable(progress);
            yield return new WaitForSeconds(0.6f);
            Assert.IsGameObjectDisable(progress);
            Assert.IsTrue(onCompleted);
            yield return new WaitForSeconds(0.1f);
            ResourcesInstance.loadingController.LoadingOff(2);
        }

        [UnityTest]
        public IEnumerator Loading_Hide_ProgressDoubleLoading()
        {
            var onCompleted = false;
            var onCompleted2 = false;
            var progress = (ProgressLoading)ResourcesInstance.loadingController.LoadingOn(2)
                .OnCompleted(() => onCompleted = true)
                .SetTime(0.15f);
            yield return null;
            progress.UpdateProgress(0.5f);
            yield return null;
            ResourcesInstance.loadingController.LoadingOn(2)
                .OnCompleted(() => onCompleted2 = true)
                .SetTime(0.15f);

            yield return null;
            progress.UpdateProgress(1f);
            yield return null;
            yield return new WaitForSeconds(0.1f);
            Assert.IsGameObjectEnable(progress);
            yield return new WaitForSeconds(0.6f);
            Assert.IsGameObjectDisable(progress);
            Assert.IsTrue(onCompleted);
            Assert.IsTrue(onCompleted2);
        }

        [UnityTest]
        public IEnumerator Loading_Hide_ProgressDoubleLoadingNoCallback()
        {
            var onCompleted = false;

            var progress = (ProgressLoading)ResourcesInstance.loadingController.LoadingOn(2)
                .OnCompleted(() => onCompleted = true)
                .SetTime(0.15f);

            yield return null;
            progress.UpdateProgress(0.5f);
            yield return null;

            ResourcesInstance.loadingController.LoadingOn(2);

            yield return null;
            progress.UpdateProgress(1f);
            yield return null;

            yield return new WaitForSeconds(0.1f);
            Assert.IsGameObjectEnable(progress);
            yield return new WaitForSeconds(0.6f);
            Assert.IsGameObjectDisable(progress);
            Assert.IsTrue(onCompleted);
        }

        [UnityTest]
        public IEnumerator Loading_Hide_ProgressLoadingCallback()
        {
            var onCompleted = false;
            var onCompleted2 = false;
            var progress = (ProgressLoading)ResourcesInstance.loadingController.LoadingOn(2)
                .OnCompleted(() => onCompleted = true)
                .SetTime(0.15f);
            yield return null;
            progress.UpdateProgress(0.5f);
            ResourcesInstance.loadingController.LoadingOff(2).OnCompleted(() => onCompleted2 = true);
            yield return new WaitForSeconds(0.1f);
            Assert.IsGameObjectEnable(progress);
            yield return new WaitForSeconds(0.6f);
            Assert.IsGameObjectDisable(progress);
            Assert.IsTrue(onCompleted);
            Assert.IsTrue(onCompleted2);
        }
    }
}