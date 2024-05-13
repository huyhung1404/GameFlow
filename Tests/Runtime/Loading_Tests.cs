using System;
using UnityEngine;
using System.Collections;
using GameFlow.Internal;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Assert = GameFlow.Internal.Assert;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GameFlow.Tests
{
    public class Loading_Tests
    {
        private static LoadingController Initialization(out FadeLoading fadeLoading)
        {
            var loadingController = Object.Instantiate(new GameObject()).AddComponent<LoadingController>();
            var controllers = new BaseLoadingTypeController[3];
            var displayLoading = Object.Instantiate(new GameObject(), loadingController.transform)
                .AddComponent<DisplayLoading>();
            displayLoading.gameObject.SetActive(false);
            controllers[0] = displayLoading;

            fadeLoading = Object.Instantiate(new GameObject(), loadingController.transform)
                .AddComponent<CanvasGroup>()
                .gameObject.AddComponent<FadeLoading>();
            fadeLoading.GetComponent<CanvasGroup>().alpha = 0;
            fadeLoading.gameObject.SetActive(false);
            controllers[1] = fadeLoading;

            var progressLoading = Object.Instantiate(new GameObject(), loadingController.transform)
                .AddComponent<CanvasGroup>()
                .gameObject.AddComponent<ProgressLoading>();

            progressLoading.GetComponent<CanvasGroup>().alpha = 0;
            progressLoading.progressSlider = Object.Instantiate(new GameObject(), progressLoading.transform).AddComponent<Slider>();
            progressLoading.gameObject.SetActive(false);
            controllers[2] = progressLoading;

            loadingController.OverriderControllers(controllers);
            return loadingController;
        }

        [UnityTest]
        public IEnumerator Simple_Show_Hide()
        {
            var controller = Initialization(out _);
            yield return null;
            var display = controller.LoadingOn(0);
            yield return null;
            Assert.IsTrue(display.gameObject.activeSelf);
            controller.LoadingOff(0);
            yield return null;
            Assert.IsTrue(!display.gameObject.activeSelf);

            var fade = (FadeLoading)controller.LoadingOn(1);
            yield return new WaitForSeconds(0.5f);
            fade.LoadingIsShow();
            controller.LoadingOff(1);
            yield return new WaitForSeconds(0.5f);
            fade.LoadingIsHide();

            var progress = (ProgressLoading)controller.LoadingOn(2);
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
        public IEnumerator Fade_Show_Hide_StraightWay()
        {
            var controller = Initialization(out _);
            yield return null;
            var time = Time.time;
            float timeOn;
            float timeOff;
            var fade = (FadeLoading)controller.LoadingOn(1).OnCompleted(() =>
            {
                timeOn = Time.time - time;
                Assert.IsTrue(timeOn >= 0.5, timeOn.ToString("0.0000"));
            }).SetTime(0.5f);
            controller.LoadingOff(1).OnCompleted(() =>
            {
                timeOff = Time.time - time;
                Assert.IsTrue(timeOff > 1f, timeOff.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(1.2f);
            Assert.IsTrue(!fade.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Fade_Show_Show_StraightWay()
        {
            var controller = Initialization(out _);
            yield return null;
            var time = Time.time;
            float time1;
            float time2;
            var fade = (FadeLoading)controller.LoadingOn(1).OnCompleted(() =>
            {
                time1 = Time.time - time;
                Assert.IsTrue(time1 < 0.32f, time1.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.3f);
            controller.LoadingOn(1).OnCompleted(() =>
            {
                time2 = Time.time - time;
                Assert.IsTrue(time2 < 0.52f, time2.ToString("0.0000"));
            }).SetTime(0.5f);

            yield return new WaitForSeconds(0.6f);
            Assert.IsTrue(fade.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Fade_Show_Show_Hide_Hide_StraightWay()
        {
            var controller = Initialization(out _);
            yield return null;
            var time = Time.time;
            float time1;
            float time2;
            float time3;
            float time4;
            var fade = (FadeLoading)controller.LoadingOn(1).OnCompleted(() =>
            {
                time1 = Time.time - time;
                Assert.IsTrue(time1 < 0.32f, time1.ToString("0.0000"));
            }).SetTime(0.5f);
            yield return new WaitForSeconds(0.3f);
            controller.LoadingOn(1).OnCompleted(() =>
            {
                time2 = Time.time - time;
                Assert.IsTrue(time2 < 0.52f, time2.ToString("0.0000"));
            }).SetTime(0.5f);

            controller.LoadingOff(1).OnCompleted(() =>
            {
                time3 = Time.time - time;
                Assert.IsTrue(time3 < 0.82f, time3.ToString("0.0000"));
            }).SetTime(0.5f);

            yield return new WaitForSeconds(0.3f);
            controller.LoadingOff(1).OnCompleted(() =>
            {
                time4 = Time.time - time;
                Assert.IsTrue(time4 < 1.02f, time4.ToString("0.0000"));
            }).SetTime(0.5f);

            yield return new WaitForSeconds(0.72f);
            Assert.IsTrue(!fade.gameObject.activeSelf);
        }


        [UnityTest]
        public IEnumerator Fade_1000()
        {
            var controller = Initialization(out var fade);
            yield return null;
            for (var i = 0; i < 1000; i++)
            {
                var time1 = Random.Range(0, 0.5f);
                var time2 = Random.Range(0, 0.5f);
                var time3 = Random.Range(0, 0.5f);
                var time4 = Random.Range(0, 0.5f);
                var timeExecute1 = Random.Range(0, 0.5f);
                var timeExecute2 = Random.Range(0, 0.5f);
                var timeExecute3 = Random.Range(0, 0.5f);
                var timeExecute4 = Random.Range(0, 0.5f);
                // [0.01414388 - 0.239598]  [0.2966626 - 0.2277192]  [0.08744401 - 0.1012809]  [0.02651149 - 0.2941101]
                Debug.Log($"[{time1} - {timeExecute1}]  " +
                          $"[{time2} - {timeExecute2}]  " +
                          $"[{time3} - {timeExecute3}]  " +
                          $"[{time4} - {timeExecute4}]  ");
                bool lastIsHide = true;
                var execute = 0;
                controller.StartCoroutine(CreateFadeOn(controller, time1, timeExecute1, () =>
                {
                    lastIsHide = false;
                    execute++;
                }));
                controller.StartCoroutine(CreateFadeOn(controller, time2, timeExecute2, () =>
                {
                    lastIsHide = false;
                    execute++;
                }));
                controller.StartCoroutine(CreateFadeOff(controller, time3, timeExecute3, () =>
                {
                    lastIsHide = true;
                    execute++;
                }));
                controller.StartCoroutine(CreateFadeOff(controller, time4, timeExecute4, () =>
                {
                    lastIsHide = true;
                    execute++;
                }));
                yield return new WaitForSeconds(time1 + time2 + time3 + time4 + 0.2f + timeExecute1 + timeExecute2 + timeExecute3 + timeExecute4);
                Assert.IsTrue(execute == 4, "execute == 4");
                if (lastIsHide)
                {
                    fade.LoadingIsHide();
                }
                else
                {
                    fade.LoadingIsShow();
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