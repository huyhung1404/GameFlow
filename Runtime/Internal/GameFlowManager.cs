using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow.Internal
{
    internal sealed class GameFlowManager : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("elementCollection")] internal ElementCollection ElementCollection;
        [SerializeField, FormerlySerializedAs("sortingOrderOffset")] internal int SortingOrderOffset = 5;
        [SerializeField, FormerlySerializedAs("planeDistance")] internal int PlaneDistance = 100;
        [SerializeField, FormerlySerializedAs("loadingShieldSortingOrder")] internal int LoadingShieldSortingOrder = 100;
        [SerializeField, FormerlySerializedAs("referenceResolution")] internal Vector2 ReferenceResolution = new Vector2(1080f, 2280f);
    }
}