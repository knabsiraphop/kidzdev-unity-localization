using TMPro;

namespace KidzDev.Unity.Localization {
    public static class TmpTextExtensions {
        // ── With LocalizationHandler (auto-refreshes on language change) ────────────

        /// <summary>
        /// Attaches (or reuses) a <see cref="LocalizationHandler"/> and sets <paramref name="key"/>.
        /// The text auto-refreshes on every language change.
        /// </summary>
        public static void SetLocalizedKey(this TMP_Text text, string key) {
            GetOrAddHandler(text).Set(key);
        }

        /// <summary>
        /// Attaches (or reuses) a <see cref="LocalizationHandler"/> and sets <paramref name="key"/>
        /// with a single format argument. The text auto-refreshes on every language change.
        /// </summary>
        public static void SetLocalizedKeyWithArg(this TMP_Text text, string key, string arg) {
            GetOrAddHandler(text).SetWithArg(key, arg);
        }

        // ── One-shot (no auto-refresh) ────────────────────────────────────────────────

        /// <summary>Sets the text to the localized value for <paramref name="key"/> immediately.
        /// Does not subscribe to language changes.</summary>
        public static void SetLocalizedText(this TMP_Text text, string key) {
            text.text = LocalizationSystem.GetText(key);
        }

        /// <summary>Sets the text to a formatted localized string immediately.
        /// Does not subscribe to language changes.</summary>
        public static void SetLocalizedTextFormat(this TMP_Text text, string key, params object[] args) {
            text.text = LocalizationSystem.GetTextFormat(key, args);
        }

        // ── Internals ─────────────────────────────────────────────────────────────────

        static LocalizationHandler GetOrAddHandler(TMP_Text text) =>
            text.GetComponent<LocalizationHandler>()
            ?? text.gameObject.AddComponent<LocalizationHandler>();
    }
}
