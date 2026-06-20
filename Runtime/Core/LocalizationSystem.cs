using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KidzDev.Unity.Localization {
    /// <summary>
    /// Static facade over <see cref="LocalizationManager.Default"/>.
    /// Swap <see cref="Default"/> for testing or multi-manager setups.
    /// </summary>
    public static class LocalizationSystem {
        static LocalizationManager _default;

        public static LocalizationManager Default {
            get => _default ??= new LocalizationManager();
            set => _default = value;
        }

        // ── Settings ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Loads <c>Resources/LocalizationSettings</c> and applies it to <see cref="Default"/>.
        /// Safe to call when the asset does not exist (no-op).
        /// </summary>
        public static void Configure() => Default.Apply(Resources.Load<LocalizationSettings>("LocalizationSettings"));

        /// <summary>Applies an explicit <paramref name="settings"/> asset to <see cref="Default"/>.</summary>
        public static void Configure(LocalizationSettings settings) => Default.Apply(settings);

        // ── Forwarded properties ────────────────────────────────────────────────────

        public static GameLanguage CurrentLanguage => Default.CurrentLanguage;

        public static MissingKeyMode MissingKeyMode {
            get => Default.MissingKeyMode;
            set => Default.MissingKeyMode = value;
        }

        public static event Action OnLanguageChanged {
            add    => Default.OnLanguageChanged += value;
            remove => Default.OnLanguageChanged -= value;
        }

        // ── Sources ─────────────────────────────────────────────────────────────────

        public static void AddSource(string name, ILocalizationLoader loader, string pathTemplate)
            => Default.AddSource(name, loader, pathTemplate);

        public static bool RemoveSource(string name) => Default.RemoveSource(name);

        // ── Loading ──────────────────────────────────────────────────────────────────

        public static void LoadSync(GameLanguage language) => Default.LoadSync(language);

        public static UniTask LoadAsync(GameLanguage language, CancellationToken ct = default)
            => Default.LoadAsync(language, ct);

        public static void SetLanguageSync(GameLanguage language) => Default.SetLanguageSync(language);

        public static UniTask SetLanguageAsync(GameLanguage language, CancellationToken ct = default)
            => Default.SetLanguageAsync(language, ct);

        // ── Lookup ────────────────────────────────────────────────────────────────────

        public static bool TryGetText(string key, out string value) => Default.TryGetText(key, out value);

        public static string GetText(string key) => Default.GetText(key);

        public static string GetText(string key, string fallback) => Default.GetText(key, fallback);

        public static string GetTextFormat(string key, params object[] args) => Default.GetTextFormat(key, args);

        // ── Release ───────────────────────────────────────────────────────────────────

        public static void Release() => Default.Release();
    }
}
