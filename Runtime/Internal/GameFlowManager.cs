using UnityEngine;

namespace GameFlow.Internal
{
    internal sealed class GameFlowManager : ScriptableObject
    {
        [SerializeField] internal ElementCollection elementCollection;
        [SerializeField] internal int sortingOrderOffset = 5;
        [SerializeField] internal int planeDistance = 100;
        [SerializeField] internal int loadingShieldSortingOrder = 100;
    }
}