using TMPro;
using UnityEngine;

namespace KidzDev.Unity.Localization {
    /// <summary>
    /// Attach to any TMP_Text GameObject. Refreshes the text automatically
    /// whenever <see cref="LocalizationSystem.OnLanguageChanged"/> fires.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("KidzDev/Localization Handler")]
    public sealed class LocalizationHandler : MonoBehaviour {
        [SerializeField] string _key;

        [Tooltip("Optional single format argument — substituted into {0} in the localized string.")]
        [SerializeField] string _formatArg;

        [Tooltip("Text appended after the localized string.")]
        [SerializeField] string _suffix;

        TMP_Text _text;

        // ── Public API ────────────────────────────────────────────────────────────────

        public string Key {
            get => _key;
            set { _key = value; Refresh(); }
        }

        /// <summary>Sets key and optional suffix, then refreshes immediately.</summary>
        public void Set(string key, string suffix = null) {
            _key    = key;
            _suffix = suffix ?? _suffix;
            _formatArg = null;
            Refresh();
        }

        /// <summary>Sets key and a single format argument, then refreshes immediately.</summary>
        public void SetWithArg(string key, string arg) {
            _key       = key;
            _formatArg = arg;
            Refresh();
        }

        /// <summary>Sets key and multiple format args, applies immediately (no auto-refresh on change).</summary>
        public void SetFormat(string key, params object[] args) {
            if (_text == null) _text = GetComponent<TMP_Text>();
            _text.text = LocalizationSystem.GetTextFormat(key, args) + _suffix;
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────────────

        void Awake() => _text = GetComponent<TMP_Text>();

        void OnEnable() {
            LocalizationSystem.OnLanguageChanged += Refresh;
            Refresh();
        }

        void OnDisable() => LocalizationSystem.OnLanguageChanged -= Refresh;

        // ── Internals ─────────────────────────────────────────────────────────────────

        void Refresh() {
            if (_text == null) _text = GetComponent<TMP_Text>();
            if (string.IsNullOrEmpty(_key)) return;

            _text.text = string.IsNullOrEmpty(_formatArg)
                ? LocalizationSystem.GetText(_key) + _suffix
                : LocalizationSystem.GetTextFormat(_key, _formatArg) + _suffix;
        }
    }
}
