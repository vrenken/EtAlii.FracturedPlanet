using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NavMeshPrefabInstance))]
    internal class NavMeshPrefabInstanceEditor : UnityEditor.Editor
    {
        private SerializedProperty _followTransformProp;
        private SerializedProperty _navMeshDataProp;

        public void OnEnable()
        {
            _followTransformProp = serializedObject.FindProperty("m_FollowTransform");
            _navMeshDataProp = serializedObject.FindProperty("m_NavMesh");
        }

        public override void OnInspectorGUI()
        {
            var instance = (NavMeshPrefabInstance) target;
            var go = instance.gameObject;

            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(_navMeshDataProp);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(_followTransformProp);

            EditorGUILayout.Space();

            OnInspectorGUIPrefab(go);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnInspectorGUIPrefab(GameObject go)
        {
            var prefab = PrefabUtility.GetPrefabInstanceHandle(go);
            var path = AssetDatabase.GetAssetPath(prefab);

            if (prefab && string.IsNullOrEmpty(path))
            {
                if (GUILayout.Button("Select the Prefab asset to bake or clear the navmesh", EditorStyles.helpBox))
                {
                    Selection.activeObject = PrefabUtility.GetCorrespondingObjectFromSource(go);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
            }

            if (string.IsNullOrEmpty(path))
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth);

            if (GUILayout.Button("Clear"))
                OnClear();

            if (GUILayout.Button("Bake"))
                OnBake();

            GUILayout.EndHorizontal();
        }

        private NavMeshData Build(NavMeshPrefabInstance instance)
        {
            var root = instance.transform;
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();

            UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                root, ~0, NavMeshCollectGeometry.RenderMeshes, 0, markups, instance.gameObject.scene, sources);
            var settings = NavMesh.GetSettingsByID(0);
            var bounds = new Bounds(Vector3.zero, 1000.0f * Vector3.one);
            var navmesh = NavMeshBuilder.BuildNavMeshData(settings, sources, bounds, root.position, root.rotation);
            navmesh.name = "Navmesh";
            return navmesh;
        }

        private void OnClear()
        {
            foreach (var tgt in targets)
            {
                var instance = (NavMeshPrefabInstance) tgt;
                var go = instance.gameObject;
                var prefab = PrefabUtility.GetPrefabInstanceHandle(go);
                var path = AssetDatabase.GetAssetPath(prefab);

                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("GameObject: " + go + " has no valid prefab path");
                    continue;
                }

                DestroyNavMeshData(path);
                AssetDatabase.SaveAssets();
            }
        }

        private void OnBake()
        {
            foreach (var tgt in targets)
            {
                var instance = (NavMeshPrefabInstance) tgt;
                var go = instance.gameObject;
                var prefab = PrefabUtility.GetPrefabInstanceHandle(go);
                var path = AssetDatabase.GetAssetPath(prefab);

                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("GameObject: " + go + " has no valid prefab path");
                    continue;
                }

                DestroyNavMeshData(path);

                // Store navmesh as a sub-asset of the prefab
                var navmesh = Build(instance);
                AssetDatabase.AddObjectToAsset(navmesh, prefab);

                instance.navMeshData = navmesh;
                AssetDatabase.SaveAssets();
            }
        }

        private void DestroyNavMeshData(string path)
        {
            // Destroy and remove all existing NavMeshData sub-assets
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var o in assets)
            {
                var data = o as NavMeshData;
                if (data != null)
                    DestroyImmediate(o, true);
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.Pickable)]
        private static void RenderGizmo(NavMeshPrefabInstance instance, GizmoType gizmoType)
        {
            if (!EditorApplication.isPlaying)
                instance.UpdateInstance();
        }
    }
}
