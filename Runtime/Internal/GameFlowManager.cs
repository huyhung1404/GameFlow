using UnityEngine;

namespace GameFlow.Internal
{
    internal sealed class GameFlowManager : ScriptableObject
    {
        [SerializeField] internal ElementCollection ElementCollection;
        [SerializeField] internal int SortingOrderOffset = 5;
        [SerializeField] internal int PlaneDistance = 100;
        [SerializeField] internal int LoadingShieldSortingOrder = 100;
        [SerializeField] internal Vector2 ReferenceResolution = new Vector2(1080f, 2280f);
    }
}