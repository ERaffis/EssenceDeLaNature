using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetUsageFinder {
    public static class AufUtils {
        public static Object[] LoadAllAssetsAtPath (string assetPath) {
            // prevents error "Do not use readobjectthreaded on scene objects!"
            return typeof (SceneAsset) == AssetDatabase.GetMainAssetTypeAtPath (assetPath)
                ? new[] { AssetDatabase.LoadMainAssetAtPath (assetPath) }
                : AssetDatabase.LoadAllAssetsAtPath (assetPath);
        }

        public static T FirstOfType<T> () where T : Object {
            var typeName = typeof (T).Name;

            var guids = AssetDatabase.FindAssets (string.Format ("t:{0}", typeName));
            if (!guids.Any ()) {
                AssetDatabase.Refresh (ImportAssetOptions.ForceUpdate);
            }

            if (guids.Length == 0) {
                Asr.Fail ();
                Report (typeName);
                return null;
            }

            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath (guid);
                var asset = (T)AssetDatabase.LoadAssetAtPath (path, typeof (T));
                return asset;
            }

            Report (typeName);
            return null;
        }

        static void Report (string typeName) {
            Debug.LogError (string.Format ("Cannot find '{0}' (AUF style resource). Please try to remove AUF and import again, or restart Unity", typeName));
        }
    }
}