// namespace Unity.StarterAssets.ThirdPersonController.Editor
// {
//     using System;
//     using System.Linq;
//     using Cinemachine;
//     using StarterAssetsPackageChecker;
//     using UnityEditor;
//     using UnityEngine;
//
//     // This class needs to be a scriptable object to support dynamic determination of StarterAssets install path
//     public partial class StarterAssetsDeployMenu
//     {
//         public const string MenuRoot = "Tools/Starter Assets";
//
//         // prefab names
//         private const string MainCameraPrefabName = "MainCamera";
//         private const string PlayerCapsulePrefabName = "PlayerCapsule";
//
//         // names in hierarchy
//         private const string CinemachineVirtualCameraName = "PlayerFollowCamera";
//
//         // tags
//         private const string PlayerTag = "Player";
//         private const string MainCameraTag = "MainCamera";
//         private const string CinemachineTargetTag = "CinemachineTarget";
//
//         private static GameObject _cinemachineVirtualCamera;
//
//         /// <summary>
//         /// Deletes the scripting define set by the Package Checker.
//         /// See Assets/Editor/PackageChecker/PackageChecker.cs for more information
//         /// </summary>
//         [MenuItem(MenuRoot + "/Reinstall Dependencies", false)]
//         private static void ResetPackageChecker()
//         {
//             PackageChecker.RemovePackageCheckerScriptingDefine();
//         }
//
// #if STARTER_ASSETS_PACKAGES_CHECKED
//         private static void CheckCameras(Transform targetParent, string prefabFolder)
//         {
//             CheckMainCamera(prefabFolder);
//
//             var vcam = GameObject.Find(CinemachineVirtualCameraName);
//
//             if (!vcam)
//             {
//                 if (TryLocatePrefab(CinemachineVirtualCameraName, new[]{prefabFolder}, new[] { typeof(CinemachineVirtualCamera) }, out var vcamPrefab, out var _))
//                 {
//                     HandleInstantiatingPrefab(vcamPrefab, out vcam);
//                     _cinemachineVirtualCamera = vcam;
//                 }
//                 else
//                 {
//                     Debug.LogError("Couldn't find Cinemachine Virtual Camera prefab");
//                 }
//             }
//             else
//             {
//                 _cinemachineVirtualCamera = vcam;
//             }
//
//             var targets = GameObject.FindGameObjectsWithTag(CinemachineTargetTag);
//             var target = targets.FirstOrDefault(t => t.transform.IsChildOf(targetParent));
//             if (target == null)
//             {
//                 target = new GameObject("PlayerCameraRoot");
//                 target.transform.SetParent(targetParent);
//                 target.transform.localPosition = new Vector3(0f, 1.375f, 0f);
//                 target.tag = CinemachineTargetTag;
//                 Undo.RegisterCreatedObjectUndo(target, "Created new cinemachine target");
//             }
//
//             CheckVirtualCameraFollowReference(target, _cinemachineVirtualCamera);
//         }
//
//         private static void CheckMainCamera(string inFolder)
//         {
//             var mainCameras = GameObject.FindGameObjectsWithTag(MainCameraTag);
//
//             if (mainCameras.Length < 1)
//             {
//                 // if there are no MainCameras, add one
//                 if (TryLocatePrefab(MainCameraPrefabName, new[]{inFolder}, new[] { typeof(CinemachineBrain), typeof(Camera) }, out var camera, out var _))
//                 {
//                     HandleInstantiatingPrefab(camera, out _);
//                 }
//                 else
//                 {
//                     Debug.LogError("Couldn't find Starter Assets Main Camera prefab");
//                 }
//             }
//             else
//             {
//                 // make sure the found camera has a cinemachine brain (we only need 1)
//                 if (!mainCameras[0].TryGetComponent(out CinemachineBrain _))
//                 {
//                     mainCameras[0].AddComponent<CinemachineBrain>();
//                 }
//             }
//         }
//
//         private static void CheckVirtualCameraFollowReference(GameObject target,
//             GameObject cinemachineVirtualCamera)
//         {
//             var serializedObject =
//                 new SerializedObject(cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>());
//             var serializedProperty = serializedObject.FindProperty("m_Follow");
//             serializedProperty.objectReferenceValue = target.transform;
//             serializedObject.ApplyModifiedProperties();
//         }
//
//         private static bool TryLocatePrefab(string name, string[] inFolders, Type[] requiredComponentTypes, out GameObject prefab, out string path)
//         {
//             // Locate the player armature
//             var allPrefabs = AssetDatabase.FindAssets("t:Prefab", inFolders);
//             for (var i = 0; i < allPrefabs.Length; ++i)
//             {
//                 var assetPath = AssetDatabase.GUIDToAssetPath(allPrefabs[i]);
//
//                 if (assetPath.Contains("/StarterAssets/"))
//                 {
//                     var loadedObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
//
//                     if (PrefabUtility.GetPrefabAssetType(loadedObj) != PrefabAssetType.NotAPrefab &&
//                         PrefabUtility.GetPrefabAssetType(loadedObj) != PrefabAssetType.MissingAsset)
//                     {
//                         var loadedGo = loadedObj as GameObject;
//                         var hasRequiredComponents = true;
//                         foreach (var componentType in requiredComponentTypes)
//                         {
//                             if (!loadedGo.TryGetComponent(componentType, out _))
//                             {
//                                 hasRequiredComponents = false;
//                                 break;
//                             }
//                         }
//
//                         if (hasRequiredComponents)
//                         {
//                              if (loadedGo.name == name)
//                              {
//                                  prefab = loadedGo;
//                                  path = assetPath;
//                                  return true;
//                              }
//                         }
//                     }
//                 }
//             }
//
//             prefab = null;
//             path = null;
//             return false;
//         }
//
//         private static void HandleInstantiatingPrefab(GameObject prefab, out GameObject prefabInstance)
//         {
//             prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
//             Undo.RegisterCreatedObjectUndo(prefabInstance, "Instantiate Starter Asset Prefab");
//
//             prefabInstance.transform.localPosition = Vector3.zero;
//             prefabInstance.transform.localEulerAngles = Vector3.zero;
//             prefabInstance.transform.localScale = Vector3.one;
//         }
// #endif
//     }
// }