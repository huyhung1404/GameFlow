﻿using System;

namespace GameFlow
{
    public class ReleaseUserInterfaceCommand : ReleaseCommand
    {
        public ReleaseUserInterfaceCommand(Type elementType) : base(elementType)
        {
        }

        internal override void PreUpdate()
        {
        }

        protected override void ReleaseOnClose()
        {
        }

        internal override void UnloadCompleted(bool isSuccess)
        {
        }

        protected override void NoneRelease()
        {
        }

        protected override void OnLoadResult(bool canRelease)
        {
        }
    }
}