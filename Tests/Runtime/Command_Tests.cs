﻿using System.Collections;
using System.Collections.Generic;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class Command_Tests
    {
        private class AutoReleaseCommand : Command
        {
            internal bool isExecute;
            private readonly int executeFrame;

            public AutoReleaseCommand(int executeFrame)
            {
                this.executeFrame = executeFrame;
            }

            internal override void Execute()
            {
                isExecute = true;
                FlowController.instance.StartCoroutine(IERelease());
            }

            private IEnumerator IERelease()
            {
                yield return DelayFrame(executeFrame);
                Release();
            }
        }

        private class DelayCommand : AutoReleaseCommand
        {
            private readonly CommandData data;
            private readonly List<DelayCommand> listAdd;
            private readonly List<DelayCommand> listExecute;

            public DelayCommand(CommandData data, List<DelayCommand> listAdd, List<DelayCommand> listExecute) : base(data.executeFrame)
            {
                this.data = data;
                this.listAdd = listAdd;
                this.listExecute = listExecute;
                FlowController.instance.StartCoroutine(IEDelay());
            }

            internal override void Execute()
            {
                base.Execute();
                listExecute.Add(this);
            }

            private IEnumerator IEDelay()
            {
                yield return DelayFrame(data.delayFrame);
                FlowController.instance.AddCommand(this);
                listAdd.Add(this);
            }
        }

        private class CommandData
        {
            public readonly int executeFrame;
            public readonly int delayFrame;

            public CommandData(int executeFrame, int delayFrame)
            {
                this.executeFrame = executeFrame;
                this.delayFrame = delayFrame;
            }
        }

        private static IEnumerator DelayFrame(int frame)
        {
            for (var i = 0; i < frame; i++)
            {
                yield return null;
            }
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var controller = Builder.CreateMono<FlowController>();
            controller.loadingController = controller.CreateChildMono<LoadingController>();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Single_Add_Execute_Command()
        {
            var command = new AutoReleaseCommand(Random.Range(1, 15));
            FlowController.instance.AddCommand(command);
            yield return DelayFrame(16);
            Assert.IsTrue(command.isExecute);
            FlowController.CommandsIsEmpty();
        }

        [UnityTest,]
        public IEnumerator Multi_Add_Execute_Command()
        {
            yield return MultiExecuteOrder(
                new CommandData(0, 0),
                new CommandData(0, 0),
                new CommandData(0, 0),
                new CommandData(0, 0));
            for (var i = 0; i < 1000; i++)
            {
                yield return MultiExecuteOrder(
                    new CommandData(Random.Range(0, 15), Random.Range(0, 15)),
                    new CommandData(Random.Range(0, 15), Random.Range(0, 15)),
                    new CommandData(Random.Range(0, 15), Random.Range(0, 15)),
                    new CommandData(Random.Range(0, 15), Random.Range(0, 15)));
                yield return DelayFrame(10);
            }
        }

        private static IEnumerator MultiExecuteOrder(
            CommandData commandData1,
            CommandData commandData2,
            CommandData commandData3,
            CommandData commandData4
        )
        {
            Debug.Log($"Execute [{commandData1.executeFrame} - {commandData1.delayFrame}]  " +
                      $"[{commandData2.executeFrame} - {commandData2.delayFrame}]  " +
                      $"[{commandData3.executeFrame} - {commandData3.delayFrame}]  " +
                      $"[{commandData4.executeFrame} - {commandData4.delayFrame}]");
            var listAdd = new List<DelayCommand>();
            var listExecute = new List<DelayCommand>();
            var command1 = new DelayCommand(commandData1, listAdd, listExecute);
            var command2 = new DelayCommand(commandData2, listAdd, listExecute);
            var command3 = new DelayCommand(commandData3, listAdd, listExecute);
            var command4 = new DelayCommand(commandData4, listAdd, listExecute);
            yield return DelayFrame(
                commandData1.delayFrame + commandData1.executeFrame +
                commandData2.delayFrame + commandData2.executeFrame +
                commandData3.delayFrame + commandData3.executeFrame +
                commandData4.delayFrame + commandData4.executeFrame +
                10);

            Assert.IsTrue(command1.isExecute && command2.isExecute && command3.isExecute && command4.isExecute);
            Assert.IsTrue(listAdd.Count == 4);
            Assert.IsTrue(listExecute.Count == 4);
            Assert.IsTrue(listAdd[0] == listExecute[0]);
            Assert.IsTrue(listAdd[1] == listExecute[1]);
            Assert.IsTrue(listAdd[2] == listExecute[2]);
            Assert.IsTrue(listAdd[3] == listExecute[3]);
            FlowController.CommandsIsEmpty();
        }
    }
}