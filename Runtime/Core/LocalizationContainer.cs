using System.Collections.Generic;

namespace KidzDev.Unity.Localization {
    internal sealed class LocalizationContainer {
        readonly Dictionary<string, string> _entries = new();

        internal bool TryGet(string key, out string value) =>
            _entries.TryGetValue(key, out value);

        internal void Add(IReadOnlyList<LocalizationEntry> entries) {
            foreach (var e in entries) {
                if (!string.IsNullOrEmpty(e.key))
                    _entries[e.key] = e.content ?? string.Empty;
            }
        }

        internal void Clear() => _entries.Clear();
    }
}
