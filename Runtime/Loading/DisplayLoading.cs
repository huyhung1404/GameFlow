using System;
using UnityEngine;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/Loading/Display")]
    public class DisplayLoading : BaseLoadingTypeController
    {
        protected override void OnShow()
        {
            IsShow = true;
        }

        protected override void OnHide()
        {
            IsShow = false;
        }

        public override BaseLoadingTypeController OnCompleted(Action onCompleted)
        {
            onCompleted?.Invoke();
            return this;
        }
    }
}