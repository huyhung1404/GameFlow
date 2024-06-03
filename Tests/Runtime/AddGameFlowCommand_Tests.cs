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
        public class TestScript___NoReference : GameFlowElement
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
        public IEnumerator _0_Single_Add_Execute_Command()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().LoadingId(0).OnCompleted((result) => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            var mono = PrefabTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono.onActiveCount == 1);
            Assert.IsTrue(mono.onEnable);
            Assert.IsTrue(!displayLoading.gameObject.activeSelf);
            GameFlowRuntimeController.CommandsIsEmpty();
        }

        [UnityTest]
        public IEnumerator _1_No_Reference_Add()
        {
            ErrorHandle.sendErrorIsLog = true;
            var next = false;
            GameCommand.Add<TestScript___NoReference>().OnCompleted((result) => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            yield return null;
            GameFlowRuntimeController.CommandsIsEmpty();
            ErrorHandle.sendErrorIsLog = false;
        }

        [UnityTest]
        public IEnumerator _2_Double_Add_Execute_Command()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().Build();
            GameCommand.Add<TestScript___ElementAddPrefab>().OnCompleted((result) => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            yield return null;
            var mono = PrefabTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono.onCloseCount == 1, "PrefabTestMonoBehaviour.onCloseCount = " + mono.onCloseCount);
            Assert.IsTrue(mono.onActiveCount == 2, "PrefabTestMonoBehaviour.onActiveCount = " + mono.onActiveCount);
            Assert.IsTrue(mono.onEnable);
            GameFlowRuntimeController.CommandsIsEmpty();
        }
        
        [UnityTest]
        public IEnumerator _3_Single_Add_Execute_Command_WithID_id()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).OnCompleted((result) => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            var mono = PrefabTestMonoBehaviour.GetWithID("id");
            Assert.IsTrue(mono.onActiveCount == 1);
            Assert.IsTrue(mono.onEnable);
            Assert.IsTrue(!displayLoading.gameObject.activeSelf);
            GameFlowRuntimeController.CommandsIsEmpty();
        }
        
        [UnityTest]
        public IEnumerator _4_Multi_Add_Execute_Command_WithID_id()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().LoadingId(0).Build();
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).OnCompleted((result) => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            var mono = PrefabTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono.onActiveCount == 1);
            Assert.IsTrue(mono.onEnable);
            
            var mono2 = PrefabTestMonoBehaviour.GetWithID("id");
            Assert.IsTrue(mono2.onActiveCount == 1);
            Assert.IsTrue(mono2.onEnable);
            Assert.IsTrue(!displayLoading.gameObject.activeSelf);
            GameFlowRuntimeController.CommandsIsEmpty();
        }
        
        [UnityTest]
        public IEnumerator _5_Multi_Add_Execute_Command_WithID_id()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().LoadingId(0).Build();
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).Build();
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).OnCompleted((result) => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }
            yield return null;

            var mono = PrefabTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono.onActiveCount == 1);
            Assert.IsTrue(mono.onEnable);
            
            var mono2 = PrefabTestMonoBehaviour.GetWithID("id");
            Assert.IsTrue(mono2.onActiveCount == 2);
            Assert.IsTrue(mono2.onCloseCount == 1);
            Assert.IsTrue(mono2.onEnable);
            Assert.IsTrue(!displayLoading.gameObject.activeSelf);
            GameFlowRuntimeController.CommandsIsEmpty();
        }
    }
}