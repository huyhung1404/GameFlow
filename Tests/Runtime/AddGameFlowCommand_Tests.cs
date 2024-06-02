using System.Collections;
using System.Collections.Generic;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class AddGameFlowCommand_Tests
    {
        public class TestScript___ElementAddPrefab : GameFlowElement
        {
        }

        private LoadingController loadingController;
        private DisplayLoading displayLoading;
        private FadeLoading fadeLoading;
        private ProgressLoading progressLoading;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var controller = Builder.CreateMono<GameFlowRuntimeController>();
            loadingController = controller.CreateChildMono<LoadingController>();

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
        public IEnumerator Single_Add_Execute_Command()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().OnCompleted((result) => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            Assert.IsTrue(PrefabTestMonoBehaviour.onActiveCount == 1);
        }
    }
}