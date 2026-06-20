using System.Collections.Generic;
using UnityEngine;

namespace KidzDev.Unity.Localization {
    [CreateAssetMenu(fileName = "LocalizationTable", menuName = "KidzDev/Localization/Table")]
    public sealed class LocalizationTable : ScriptableObject {
        public List<LocalizationEntry> entries = new();
    }
}
