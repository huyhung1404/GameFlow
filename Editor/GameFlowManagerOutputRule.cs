#if AUTO_GROUP_GENERATOR
using AutoGroupGenerator;
using GameFlow.Internal; 
using UnityEngine;

namespace GameFlow
{
    [CreateAssetMenu(menuName = "GameFlow/Manager OutputRule")]
    public class GameFlowManagerOutputRule : OutputRule
    {
        protected override bool DoesMatchSelectionCriteria(GroupLayout _)
        {
            return true;
        }

        public override void Refine()
        {
            var targetAssetPath = PackagePath.ManagerPath();

            foreach (var groupLayout in m_Selection)
            {
                if (groupLayout == null || groupLayout.Nodes == null) continue;
                var isTargetGroup = false;
                foreach (var node in groupLayout.Nodes)
                {
                    if (node == null) continue;
                    if (node.AssetPath == targetAssetPath)
                    {
                        isTargetGroup = true;
                        break;
                    }
                }

                if (isTargetGroup)
                {
                    Rename(groupLayout, targetAssetPath);
                }
            }

            base.Refine();
        }
    }
}
#endif