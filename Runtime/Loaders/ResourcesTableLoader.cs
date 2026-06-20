using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KidzDev.Unity.Localization {
    /// <summary>
    /// Loads a <see cref="LocalizationTable"/> ScriptableObject from <c>Resources</c>.
    /// </summary>
    public sealed class ResourcesTableLoader : ILocalizationLoader {
        public IReadOnlyList<LocalizationEntry> Load(string path) {
            var table = Resources.Load<LocalizationTable>(path);
            if (table == null) {
                Debug.LogWarning($"[Localization] LocalizationTable not found at Resources/{path}");
                return Array.Empty<LocalizationEntry>();
            }
            return table.entries;
        }

        public UniTask<IReadOnlyList<LocalizationEntry>> LoadAsync(string path, CancellationToken ct = default)
            => UniTask.FromResult(Load(path));
    }
}
