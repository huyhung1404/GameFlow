using System.Collections;
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

        public class TestScript___ElementAddScene : GameFlowElement
        {
        }

        public class TestScript___NoReference : GameFlowElement
        {
        }

        private FadeLoading fadeLoading;
        private ProgressLoading progressLoading;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            Builder.CreateMono<GameFlowRuntimeController>();
            yield return null;
            var loadingController = LoadingController.instance;
            fadeLoading = loadingController.CreateChildMono<FadeLoading>()
                .AddCanvasGroup(0)
                .Disable();

            progressLoading = loadingController.CreateChildMono<ProgressLoading>()
                .AddCanvasGroup(0)
                .CreateChildMono<ProgressLoading, Slider>((loading, slider) => loading.progressSlider = slider)
                .Disable();

            loadingController.RegisterControllers(fadeLoading, progressLoading);
            yield return null;
        }

        [UnityTest]
        public IEnumerator _0_Single_Add_Execute_Command()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().OnCompleted(_ =>
            {
                LoadingController.IsTransparentOn();
                next = true;
            }).Build();
            while (!next)
            {
                yield return null;
            }

            var mono = PrefabTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono.onActiveCount == 1);
            Assert.IsTrue(mono.onEnable);
            GameFlowRuntimeController.CommandsIsEmpty();

            var next2 = false;
            GameCommand.Add<TestScript___ElementAddScene>().OnCompleted(_ =>
            {
                LoadingController.IsTransparentOn();
                next2 = true;
            }).Build();
            while (!next2)
            {
                yield return null;
            }

            yield return null;
            var mono2 = SceneTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono2.onActiveCount == 1);
            Assert.IsTrue(mono2.onEnable);
            GameFlowRuntimeController.CommandsIsEmpty();
            LoadingController.IsTransparentOff();
        }

        [UnityTest]
        public IEnumerator _1_No_Reference_Add()
        {
            ErrorHandle.sendErrorIsLog = true;
            var next = false;
            GameCommand.Add<TestScript___NoReference>().OnCompleted(_ =>
            {
                next = true;
            }).Build();
            while (!next)
            {
                yield return null;
            }

            yield return null;
            GameFlowRuntimeController.CommandsIsEmpty();
            ErrorHandle.sendErrorIsLog = false;
            LoadingController.IsTransparentOff();
        }

        [UnityTest]
        public IEnumerator _2_Double_Add_Execute_Command()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().Build();
            GameCommand.Add<TestScript___ElementAddPrefab>().OnCompleted(_ => { next = true; }).Build();
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

            var next2 = false;
            GameCommand.Add<TestScript___ElementAddScene>().Build();
            GameCommand.Add<TestScript___ElementAddScene>().OnCompleted(_ => { next2 = true; }).Build();
            while (!next2)
            {
                yield return null;
            }

            yield return null;
            var mono2 = SceneTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono2.onCloseCount == 1, "PrefabTestMonoBehaviour.onCloseCount = " + mono2.onCloseCount);
            Assert.IsTrue(mono2.onActiveCount == 2, "PrefabTestMonoBehaviour.onActiveCount = " + mono2.onActiveCount);
            Assert.IsTrue(mono.onEnable);
            GameFlowRuntimeController.CommandsIsEmpty();
        }

        [UnityTest]
        public IEnumerator _3_Single_Add_Execute_Command_WithID_id()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).OnCompleted(_ => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            var mono = PrefabTestMonoBehaviour.GetWithID("id");
            Assert.IsTrue(mono.onActiveCount == 1);
            Assert.IsTrue(mono.onEnable);
            GameFlowRuntimeController.CommandsIsEmpty();
        }

        [UnityTest]
        public IEnumerator _4_Multi_Add_Execute_Command_WithID_id()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().LoadingId(0).Build();
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).OnCompleted(_ => { next = true; }).Build();
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
            GameFlowRuntimeController.CommandsIsEmpty();
        }

        [UnityTest]
        public IEnumerator _5_Multi_Add_Execute_Command_WithID_id()
        {
            var next = false;
            GameCommand.Add<TestScript___ElementAddPrefab>().LoadingId(0).Build();
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).Build();
            GameCommand.Add<TestScript___ElementAddPrefab>("id").LoadingId(0).OnCompleted(_ => { next = true; }).Build();
            while (!next)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            var mono = PrefabTestMonoBehaviour.GetWithID("");
            Assert.IsTrue(mono.onActiveCount == 1);
            Assert.IsTrue(mono.onEnable);

            var mono2 = PrefabTestMonoBehaviour.GetWithID("id");
            Assert.IsTrue(mono2.onActiveCount == 2);
            Assert.IsTrue(mono2.onCloseCount == 1);
            Assert.IsTrue(mono2.onEnable);
            fadeLoading.LoadingIsHide();
            GameFlowRuntimeController.CommandsIsEmpty();
        }
    }
}