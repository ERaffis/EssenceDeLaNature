using System;
using System.Linq;
using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using static AssetUsageFinder.PrefabUtilities;
using Object = UnityEngine.Object;

namespace AssetUsageFinder {
    [Serializable]
    class SearchTarget {
        public static SearchTarget CreateStage(Object target, string sceneOrStagePath = null) {
            return new SearchTarget(target, FindModeEnum.Stage, sceneOrStagePath);
        }

        public static SearchTarget CreateFile(Object target, string sceneOrStagePath = null) {
            return new SearchTarget(target, FindModeEnum.File, sceneOrStagePath);
        }

        public static SearchTarget CreateScene(Object target, string sceneOrStagePath = null) {
            return new SearchTarget(target, FindModeEnum.Scene, sceneOrStagePath);
        }

        public static SearchTarget CreateSceneNested(Object target) {
            return new SearchTarget(target, FindModeEnum.Scene) {IsNested = true};
        }

        public Object Target;
        public Option<Object[]> Nested;
        public Object Root;
        public Scene Scene;
        public UnityEditor.SceneManagement.PrefabStage Stage;
        public bool IsNested;

        SearchTarget(Object target, FindModeEnum findMode, string sceneOrStagePath = null) {
            Asr.IsNotNull(target, "Asset you're trying to search is corrupted");
            Target = target;
            var path = sceneOrStagePath ?? AssetDatabase.GetAssetPath(Target);
            switch (findMode) {
                case FindModeEnum.File: {
                    Asr.IsTrue(string.IsNullOrEmpty(sceneOrStagePath));
                    Root = AssetDatabase.LoadMainAssetAtPath(path);
                    if (!IsNested)
                        Nested = AufUtils.LoadAllAssetsAtPath(path);
                    if (AssetDatabase.GetMainAssetTypeAtPath(path).IsAssignableFrom(typeof(SceneAsset)))
                        Scene = SceneManager.GetSceneByPath(path);
                }
                    break;
                case FindModeEnum.Scene:
                case FindModeEnum.Stage: {
                    Root = Target;
                    var asset = AssetDatabase.GetAssetPath(target);
                    if (Target is GameObject go) {
                        switch (PrefabUtility.GetPrefabAssetType(go)) {
                            case PrefabAssetType.Regular:
                            case PrefabAssetType.Variant: {
                                if (!IsNested)
                                    Nested = string.IsNullOrEmpty(asset) ? go.GetComponents<Component>() : AssetDatabase.LoadAllAssetsAtPath(asset);

                                break;
                            }
                            case PrefabAssetType.Model: {
                                if (!IsNested)
                                    Nested = AssetDatabase.LoadAllAssetsAtPath(asset);
                                break;
                            }
                            case PrefabAssetType.MissingAsset:
                            case PrefabAssetType.NotAPrefab:
                                if (!IsNested)
                                    Nested = go.GetComponents<Component>().Union(Enumerable.Repeat(Target, 1)).ToArray();
                                break;
                        }

                        Stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                        if (findMode == FindModeEnum.Scene) {
                            if (string.IsNullOrEmpty(sceneOrStagePath))
                                sceneOrStagePath = go.scene.path;
                            Scene = SceneManager.GetSceneByPath(sceneOrStagePath);
                        }
                    }
                    else if (Target is Component c) {
                        // prefab instance
                        Nested = default;
                        if (findMode == FindModeEnum.Scene) {
                            if (string.IsNullOrEmpty(sceneOrStagePath))
                                sceneOrStagePath = c.gameObject.scene.path;
                            Scene = SceneManager.GetSceneByPath(sceneOrStagePath);
                        }
                    }
                    else {
                        if (!IsNested)
                            Nested = AssetDatabase.LoadAllAssetsAtPath(asset);

                        if (AssetDatabase.GetMainAssetTypeAtPath(path).IsAssignableFrom(typeof(SceneAsset)))
                            Scene = SceneManager.GetSceneByPath(path);
                    }
                }
                    break;
            }

            Asr.IsTrue(!IsNested || !Nested.HasValue);
        }

        public bool Check(Object arg) {
            if (arg == null || Target == null) return false;
            if (arg == Target) return true;
            if (!Nested.TryGet(out var n)) return false;

            var length = n.Length;
            for (var i = 0; i < length; i++)
                if (n[i] == arg)
                    return true;

            return false;
        }
    }
}