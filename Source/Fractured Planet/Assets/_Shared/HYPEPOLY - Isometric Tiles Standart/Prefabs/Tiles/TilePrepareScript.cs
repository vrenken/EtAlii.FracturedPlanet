using UnityEngine;

namespace EtAlii.FracturedPlanet
{
    using UnityEditor;

    public class TilePrepareScript
    {
        [MenuItem("Assets/Fractal Planet/Prepare tiles")]
        public static void EditPrefab()
        {
            var paths = new []
            {
                "Assets/_Shared/HYPEPOLY - Isometric Tiles Standart/Prefabs/Tiles/ReadyToUse/With Filling",
                "Assets/_Shared/HYPEPOLY - Isometric Tiles Standart/Prefabs/Tiles/ReadyToUse/Without Filling",
            } ;
            var prefabs = AssetDatabase.FindAssets("t:prefab", paths);

            foreach (var prefab in prefabs)
            {
                var path = AssetDatabase.GUIDToAssetPath(prefab);
                Debug.Log($"Preparing prefab: {path}");
                Debug.Log($"Making prefab static...");

                using var editingScope = new PrefabUtility.EditPrefabContentsScope(path);

                var prefabRoot = editingScope.prefabContentsRoot;
                var tileTop = prefabRoot.transform.Find("Tile Top Part");
                var staticFlags = StaticEditorFlags.BatchingStatic |
                                  StaticEditorFlags.NavigationStatic |
                                  StaticEditorFlags.OccludeeStatic |
                                  StaticEditorFlags.OccluderStatic |
                                  StaticEditorFlags.ContributeGI |
                                  StaticEditorFlags.ReflectionProbeStatic |
                                  StaticEditorFlags.OffMeshLinkGeneration;
                GameObjectUtility.SetStaticEditorFlags(tileTop.gameObject, staticFlags);

                var tileFillPart = prefabRoot.transform.Find("Tile Fill Part");
                if (tileFillPart != null)
                {
                    Debug.Log($"Configuring fill colliders...");
                    var colliders = tileFillPart.GetComponentsInChildren<Collider>(true);
                    foreach (var collider in colliders)
                    {
                        collider.enabled = false;
                    }
                }
            }
        }
    }
}
