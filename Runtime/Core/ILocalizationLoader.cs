using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace KidzDev.Unity.Localization {
    /// <summary>
    /// Seam for loading localization entries from any data source.
    /// Implement this interface to plug in Addressables, streaming assets, web requests, etc.
    /// </summary>
    public interface ILocalizationLoader {
        /// <summary>Synchronously loads entries from <paramref name="path"/>.</summary>
        IReadOnlyList<LocalizationEntry> Load(string path);

        /// <summary>Asynchronously loads entries from <paramref name="path"/>.</summary>
        UniTask<IReadOnlyList<LocalizationEntry>> LoadAsync(string path, CancellationToken ct = default);
    }
}
