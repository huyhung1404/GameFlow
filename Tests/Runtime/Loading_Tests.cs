using System;
using UnityEngine;
using System.Collections;
using GameFlow.Internal;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Assert = GameFlow.Internal.Assert;
using Random = UnityEngine.Random;

namespace GameFlow.Tests
{
    public class Loading_Tests
    {
        private LoadingController loadingController;
        private DisplayLoading displayLoading;
        private FadeLoading fadeLoading;
        private ProgressLoading progressLoading;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            loadingController = Builder.CreateMono<LoadingController>();

            displayLoading = loadingController.CreateChildMono<DisplayLoading>()
                .Disable();

            fadeLoading = loadingController.CreateChildMono<FadeLoading>()
                .AddCanvasGroup(0)
                .Disable();

            progressLoading = loadingController.CreateChildMono<ProgressLoading>()
                .AddCanvasGroup(0)
                .CreateChildMono<ProgressLoading, Slider>((loading, slider) => loading.progressSlider = slider)
                .Disable();

            loadingController.RegisterControllers(displayLoading, fadeLoading, progressLoading);
            yield return null;
        }

        [UnityTest]
        public IEnumerator _0_Simple_Show_Hide()
        {
            var display = loadingController.LoadingOn(0);
            yield return null;
            Assert.IsTrue(display.gameObject.activeSelf);
            loadingController.LoadingOff(0);
            yield return null;
            Assert.IsTrue(!display.gameObject.activeSelf);

            var fade = (FadeLoading)loadingController.LoadingOn(1);
            yield return new WaitForSeconds(0.5f);
            fade.LoadingIsShow();
            loadingController.LoadingOff(1);
            yield return new WaitForSeconds(0.5f);
            fade.LoadingIsHide();

            var progress = (ProgressLoading)loadingController.LoadingOn(2);
            yield return null;
            progress.UpdateProgress(0.3f);
            yield return new WaitForSeconds(0.2f);
            progress.UpdateProgress(0.6f);
            Assert.IsTrue(progress.gameObject.activeSelf);
            yield return new WaitForSeconds(0.2f);
            progress.UpdateProgress(1f);
            yield return new WaitForSeconds(0.3f);
            Assert.IsTrue(!progress.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator _1_Fade_Show_Hide_StraightWay()
        {
            var time = Time.time;
            float timeOn;
            float timeOff;
            var fade = (FadeLoading)loadingController.LoadingOn(1).OnCompleted(() =>
            {
                timeOn = Time.time - time;
                Assert.IsTrue(timeOn >= 0.5, timeOn.ToString("0.0000"));
            }).SetTime(0.5f);
            loadingController.LoadingOff(1).OnCompleted(() =>
            {
                timeOff = Time.time - time;
                Assert.IsTrue(timeOff > 1f, timeOff.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(1.2f);
            Assert.IsTrue(!fade.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator _2_Fade_Show_Show_StraightWay()
        {
            var time = Time.time;
            float time1;
            float time2;
            var fade = (FadeLoading)loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time1 = Time.time - time;
                Assert.IsTrue(time1 < 0.32f, time1.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.3f);
            loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time2 = Time.time - time;
                Assert.IsTrue(time2 < 0.52f, time2.ToString("0.0000"));
            }).SetTime(0.5f);

            yield return new WaitForSeconds(0.6f);
            Assert.IsTrue(fade.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator _3_Fade_Show_Show_Hide_Hide_StraightWay()
        {
            var time = Time.time;
            float time1;
            float time2;
            float time3;
            float time4;
            var fade = (FadeLoading)loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time1 = Time.time - time;
                Assert.IsTrue(time1 < 0.32f, time1.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.3f);
            loadingController.LoadingOn(1).OnCompleted(() =>
            {
                time2 = Time.time - time;
                Assert.IsTrue(time2 < 0.52f, time2.ToString("0.0000"));
            }).SetTime(0.5f);

            loadingController.LoadingOff(1).OnCompleted(() =>
            {
                time3 = Time.time - time;
                Assert.IsTrue(time3 < 0.82f, time3.ToString("0.0000"));
            }).SetTime(0.5f);

            yield return new WaitForSeconds(0.3f);
            loadingController.LoadingOff(1).OnCompleted(() =>
            {
                time4 = Time.time - time;
                Assert.IsTrue(time4 < 1.02f, time4.ToString("0.0000"));
            }).SetTime(0.5f);

            yield return new WaitForSeconds(0.72f);
            Assert.IsTrue(!fade.gameObject.activeSelf);
        }

        private static readonly int[] FadeCounts = { 1, 10, 20 };

        [UnityTest]
        public IEnumerator _Fade([ValueSource(nameof(FadeCounts))] int fadeTime)
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
                loadingController.StartCoroutine(CreateFadeOn(loadingController, time1, timeExecute1, () =>
                {
                    lastIsHide = false;
                    execute++;
                }));
                loadingController.StartCoroutine(CreateFadeOn(loadingController, time2, timeExecute2, () =>
                {
                    lastIsHide = false;
                    execute++;
                }));
                loadingController.StartCoroutine(CreateFadeOff(loadingController, time3, timeExecute3, () =>
                {
                    lastIsHide = true;
                    execute++;
                }));
                loadingController.StartCoroutine(CreateFadeOff(loadingController, time4, timeExecute4, () =>
                {
                    lastIsHide = true;
                    execute++;
                }));
                yield return new WaitForSeconds(time1 + time2 + time3 + time4 + 1.5f + timeExecute1 + timeExecute2 + timeExecute3 + timeExecute4);
                Assert.AreEqual(execute, 4, "Execute is not 4");
                if (lastIsHide)
                {
                    fadeLoading.LoadingIsHide();
                }
                else
                {
                    fadeLoading.LoadingIsShow();
                }
            }
        }

        private static IEnumerator CreateFadeOn(LoadingController controller, float time, float timeExecute, Action onCompleted)
        {
            yield return new WaitForSeconds(time);
            controller.LoadingOn(1).OnCompleted(onCompleted).SetTime(timeExecute);
        }

        private static IEnumerator CreateFadeOff(LoadingController controller, float time, float timeExecute, Action onCompleted)
        {
            yield return new WaitForSeconds(time);
            controller.LoadingOff(1).OnCompleted(onCompleted).SetTime(timeExecute);
        }
    }
}