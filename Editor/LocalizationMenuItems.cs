using System.IO;
using UnityEditor;
using UnityEngine;

namespace KidzDev.Unity.Localization.Editor {
    internal static class LocalizationMenuItems {
        [MenuItem("Tools/Localization/Create Settings")]
        static void CreateSettings() {
            const string dir = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(dir)) {
                Directory.CreateDirectory(dir);
                AssetDatabase.Refresh();
            }

            const string path = dir + "/LocalizationSettings.asset";
            var existing = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(path);
            if (existing != null) {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = existing;
                return;
            }

            var settings = ScriptableObject.CreateInstance<LocalizationSettings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = settings;
        }
    }
}
