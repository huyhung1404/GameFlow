// using UnityEngine;
//
// namespace GameFlow.Tests
// {
//     [AddComponentMenu("")]
//     public class SceneTestMonoBehaviour : TestMonoBehaviour<AddGameFlowCommand_Tests.TestScript___ElementAddScene>
//     {
//         public static SceneTestMonoBehaviour GetWithID(string idSearch)
//         {
//             foreach (var mono in FindObjectsOfType<SceneTestMonoBehaviour>())
//             {
//                 if (string.IsNullOrEmpty(mono.id) && string.IsNullOrEmpty(idSearch)) return mono;
//                 if (mono.id == idSearch) return mono;
//             }
//
//             return null;
//         }
//     }
// }