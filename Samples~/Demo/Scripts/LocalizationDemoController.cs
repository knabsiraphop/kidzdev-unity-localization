using System.Text;
using Cysharp.Threading.Tasks;
using KidzDev.Unity.Localization;
using TMPro;
using UnityEngine;

/// <summary>
/// Demonstrates three things:
///   1. Manual GetText — call SetLanguage then read keys yourself (output panel).
///   2. LocalizationHandler — TMP_Text components with the handler attached refresh
///      automatically on OnLanguageChanged with no code here at all.
///   3. OnLanguageChanged event — subscribe in code to react to language switches
///      (used here to drive the change-count label).
/// </summary>
public sealed class LocalizationDemoController : MonoBehaviour {
    [Header("Manual GetText output")]
    [SerializeField] TMP_Text _statusText;
    [SerializeField] TMP_Text _outputText;

    [Header("LocalizationHandler — auto-refresh (no code needed)")]
    [SerializeField] TMP_Text _handlerGreeting;  // LocalizationHandler(key=greeting) attached in scene
    [SerializeField] TMP_Text _handlerFarewell;  // LocalizationHandler(key=farewell) attached in scene
    [SerializeField] TMP_Text _handlerLanguage;  // LocalizationHandler(key=language)  attached in scene

    [Header("OnLanguageChanged event")]
    [SerializeField] TMP_Text _changeCountLabel;

    static readonly string[] DemoKeys = { "greeting", "farewell", "app_name", "language", "missing_key" };

    int _changeCount;

    // ── Lifecycle ─────────────────────────────────────────────────────────────────

    void Start() {
        LocalizationSystem.Configure();
        Report("Configured. Press a button to load.");
    }

    void OnEnable()  => LocalizationSystem.OnLanguageChanged += OnLanguageChanged;
    void OnDisable() => LocalizationSystem.OnLanguageChanged -= OnLanguageChanged;

    // ── OnLanguageChanged callback ────────────────────────────────────────────────

    void OnLanguageChanged() {
        _changeCount++;
        if (_changeCountLabel != null)
            _changeCountLabel.text = $"OnLanguageChanged fired  ×{_changeCount}";
    }

    // ── Button handlers ───────────────────────────────────────────────────────────

    public void OnEnglishSync()  => LoadSync(GameLanguage.English);
    public void OnThaiSync()     => LoadSync(GameLanguage.Thai);
    public void OnEnglishAsync() => LoadAsync(GameLanguage.English).Forget();
    public void OnThaiAsync()    => LoadAsync(GameLanguage.Thai).Forget();

    public void OnRelease() {
        LocalizationSystem.Release();
        Report("Released — all entries cleared.");
    }

    // ── Workflow ──────────────────────────────────────────────────────────────────

    void LoadSync(GameLanguage language) {
        LocalizationSystem.SetLanguageSync(language);
        Report($"Loaded {language} (sync).");
    }

    async UniTaskVoid LoadAsync(GameLanguage language) {
        Report($"Loading {language} async…");
        await LocalizationSystem.SetLanguageAsync(language, this.GetCancellationTokenOnDestroy());
        Report($"Loaded {language} (async).");
    }

    // ── Output ────────────────────────────────────────────────────────────────────

    void Report(string status) {
        if (_statusText != null) _statusText.text = status;
        if (_outputText != null) _outputText.text = BuildOutput();
        Debug.Log($"[LocalizationDemo] {status}");
    }

    static string BuildOutput() {
        var sb = new StringBuilder();
        foreach (var key in DemoKeys)
            sb.AppendLine($"{key,-12} →  {LocalizationSystem.GetText(key)}");
        return sb.ToString();
    }
}
