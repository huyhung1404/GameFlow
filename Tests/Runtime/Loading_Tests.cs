using UnityEngine;
using System.Collections;
using GameFlow.Internal;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class Loading_Tests
    {
        [UnityTest]
        public IEnumerator Simple_Show_Hide()
        {
            var controller = Initialization();
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
        }

        private static LoadingController Initialization()
        {
            var loadingController = Object.Instantiate(new GameObject()).AddComponent<LoadingController>();
            var controllers = new BaseLoadingTypeController[3];
            var displayLoading = Object.Instantiate(new GameObject(), loadingController.transform)
                .AddComponent<DisplayLoading>();
            displayLoading.gameObject.SetActive(false);
            controllers[0] = displayLoading;

            var fadeLoading = Object.Instantiate(new GameObject(), loadingController.transform)
                .AddComponent<CanvasGroup>()
                .gameObject.AddComponent<FadeLoading>();
            fadeLoading.gameObject.SetActive(false);
            controllers[1] = fadeLoading;

            var progressLoading = Object.Instantiate(new GameObject(), loadingController.transform)
                .AddComponent<CanvasGroup>()
                .gameObject.AddComponent<ProgressLoading>();

            progressLoading.progressSlider = Object.Instantiate(new GameObject(), progressLoading.transform).AddComponent<Slider>();
            progressLoading.gameObject.SetActive(false);
            controllers[2] = progressLoading;

            loadingController.OverriderControllers(controllers);
            return loadingController;
        }
    }
}