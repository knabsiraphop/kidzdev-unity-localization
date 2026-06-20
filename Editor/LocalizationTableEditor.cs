using UnityEditor;
using UnityEngine;

namespace KidzDev.Unity.Localization.Editor {
    [CustomEditor(typeof(LocalizationTable))]
    public sealed class LocalizationTableEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var table = (LocalizationTable)target;
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(
                $"Entry count: {table.entries?.Count ?? 0}",
                EditorStyles.miniLabel);
        }
    }
}
