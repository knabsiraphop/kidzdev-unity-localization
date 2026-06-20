using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KidzDev.Unity.Localization {
    /// <summary>
    /// Loads a JSON array of <see cref="LocalizationEntry"/> from <c>Resources</c>.
    /// Accepted formats:
    ///   Array:   <c>[{"key":"…","content":"…"}, …]</c>
    ///   Wrapped: <c>{"items":[{"key":"…","content":"…"}, …]}</c>
    /// </summary>
    public sealed class ResourcesJsonLoader : ILocalizationLoader {
        [Serializable]
        sealed class Wrapper { public LocalizationEntry[] items; }

        public IReadOnlyList<LocalizationEntry> Load(string path) {
            var asset = Resources.Load<TextAsset>(path);
            if (asset == null) {
                Debug.LogWarning($"[Localization] JSON not found at Resources/{path}");
                return Array.Empty<LocalizationEntry>();
            }
            return Parse(asset.text);
        }

        // Resources.Load is synchronous; UniTask.FromResult avoids allocating a state machine.
        public UniTask<IReadOnlyList<LocalizationEntry>> LoadAsync(string path, CancellationToken ct = default)
            => UniTask.FromResult(Load(path));

        static IReadOnlyList<LocalizationEntry> Parse(string json) {
            var trimmed = json.TrimStart();
            if (trimmed.StartsWith("["))
                trimmed = "{\"items\":" + trimmed + "}";
            var wrapper = JsonUtility.FromJson<Wrapper>(trimmed);
            return wrapper?.items ?? Array.Empty<LocalizationEntry>();
        }
    }
}
