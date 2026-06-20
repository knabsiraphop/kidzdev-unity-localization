using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KidzDev.Unity.Localization {
    /// <summary>
    /// Owns localization state: named sources, current language, and the entry dictionary.
    /// Use <see cref="LocalizationSystem"/> for the default singleton instance.
    /// </summary>
    public sealed class LocalizationManager : IDisposable {
        readonly struct Source {
            public readonly ILocalizationLoader Loader;
            public readonly string PathTemplate;   // e.g. "Localization/{language}MainData"
            public Source(ILocalizationLoader loader, string pathTemplate) {
                Loader = loader;
                PathTemplate = pathTemplate;
            }
        }

        // Preserves insertion order so later sources override earlier ones on key collision.
        readonly Dictionary<string, Source> _sources = new();
        readonly LocalizationContainer _container = new();

        UniTask _loadTask;
        bool _isLoading;
        CancellationTokenSource _loadCts;
        GameLanguage _loadingLanguage;

        public GameLanguage CurrentLanguage { get; private set; } = GameLanguage.English;

        public MissingKeyMode MissingKeyMode { get; set; } = MissingKeyMode.BracketedKey;

        public event Action OnLanguageChanged;

        // ── Sources ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Registers a named data source. <paramref name="pathTemplate"/> may contain
        /// <c>{language}</c> which is replaced by <see cref="GameLanguage"/> name at load time.
        /// </summary>
        public void AddSource(string name, ILocalizationLoader loader, string pathTemplate) {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (loader == null) throw new ArgumentNullException(nameof(loader));
            _sources[name] = new Source(loader, pathTemplate ?? string.Empty);
        }

        public bool RemoveSource(string name) => _sources.Remove(name);

        // ── Loading ──────────────────────────────────────────────────────────────────

        /// <summary>Loads all sources synchronously for <paramref name="language"/>.</summary>
        public void LoadSync(GameLanguage language) {
            _container.Clear();
            foreach (var (_, source) in _sources)
                LoadSourceSync(source, language);
            CurrentLanguage = language;
        }

        /// <summary>Loads all sources in parallel for <paramref name="language"/>.</summary>
        public async UniTask LoadAsync(GameLanguage language, CancellationToken ct = default) {
            // Join an in-flight load for the same language rather than racing it.
            if (_isLoading && _loadingLanguage == language) {
                await _loadTask;
                return;
            }

            // Cancel any previous in-flight load for a different language.
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            _loadingLanguage = language;
            _isLoading = true;
            _loadTask = LoadAsyncCore(language, _loadCts.Token);
            try {
                await _loadTask;
            } finally {
                _isLoading = false;
            }
        }

        async UniTask LoadAsyncCore(GameLanguage language, CancellationToken ct) {
            var tasks = new List<UniTask<IReadOnlyList<LocalizationEntry>>>(_sources.Count);
            foreach (var (_, source) in _sources)
                tasks.Add(LoadSourceAsync(source, language, ct));

            var results = await UniTask.WhenAll(tasks);

            // Only commit if not cancelled — avoids partial-language state.
            ct.ThrowIfCancellationRequested();
            _container.Clear();
            foreach (var entries in results)
                _container.Add(entries);
            CurrentLanguage = language;
        }

        // ── Language switch (load + fire event) ──────────────────────────────────────

        public void SetLanguageSync(GameLanguage language) {
            LoadSync(language);
            OnLanguageChanged?.Invoke();
        }

        public async UniTask SetLanguageAsync(GameLanguage language, CancellationToken ct = default) {
            await LoadAsync(language, ct);
            OnLanguageChanged?.Invoke();
        }

        // ── Lookup ────────────────────────────────────────────────────────────────────

        public bool TryGetText(string key, out string value) =>
            _container.TryGet(key, out value);

        public string GetText(string key) {
            if (_container.TryGet(key, out var value)) return value;
            return MissingKeyMode switch {
                MissingKeyMode.RawKey => key,
                MissingKeyMode.Empty  => string.Empty,
                _                     => $"[{key}]",
            };
        }

        /// <summary>Returns <paramref name="fallback"/> when the key is not found.</summary>
        public string GetText(string key, string fallback) =>
            _container.TryGet(key, out var value) ? value : fallback;

        public string GetTextFormat(string key, params object[] args) {
            var text = GetText(key);
            try { return string.Format(text, args); } catch { return text; }
        }

        // ── Settings ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies <paramref name="settings"/>: sets <see cref="MissingKeyMode"/> and registers
        /// all configured sources. Existing sources are not cleared; call
        /// <see cref="RemoveSource"/> first if you want a clean slate.
        /// </summary>
        public void Apply(LocalizationSettings settings) {
            if (settings == null) return;
            MissingKeyMode = settings.MissingKeyMode;
            foreach (var cfg in settings.Sources)
                AddSource(cfg.sourceName, CreateLoader(cfg.loaderType), cfg.pathTemplate);
        }

        static ILocalizationLoader CreateLoader(LocalizationLoaderType type) => type switch {
            LocalizationLoaderType.Json  => new ResourcesJsonLoader(),
            LocalizationLoaderType.Table => new ResourcesTableLoader(),
            _                            => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        // ── Release ───────────────────────────────────────────────────────────────────

        public void Release() {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = null;
            _isLoading = false;
            _container.Clear();
        }

        public void Dispose() => Release();

        // ── Internals ─────────────────────────────────────────────────────────────────

        string ResolvePath(string template, GameLanguage language) =>
            template.Replace("{language}", language.ToString());

        void LoadSourceSync(Source source, GameLanguage language) {
            var path = ResolvePath(source.PathTemplate, language);
            try {
                var entries = source.Loader.Load(path);
                if (entries != null) _container.Add(entries);
            } catch (Exception ex) {
                Debug.LogError($"[Localization] Failed to load '{path}': {ex.Message}");
            }
        }

        async UniTask<IReadOnlyList<LocalizationEntry>> LoadSourceAsync(
            Source source, GameLanguage language, CancellationToken ct) {
            var path = ResolvePath(source.PathTemplate, language);
            try {
                return await source.Loader.LoadAsync(path, ct);
            } catch (OperationCanceledException) {
                throw;
            } catch (Exception ex) {
                Debug.LogError($"[Localization] Failed to load '{path}': {ex.Message}");
                return Array.Empty<LocalizationEntry>();
            }
        }
    }
}
