using System.Text;
using Cysharp.Threading.Tasks;
using KidzDev.Unity.Localization;
using TMPro;
using UnityEngine;

/// <summary>
/// Walks through the localization workflow:
/// <c>Configure()</c> from settings → <c>SetLanguage</c> (sync or async) → <c>GetText</c> → <c>Release</c>.
/// The five buttons are wired to the public On* methods in the scene.
/// </summary>
public sealed class LocalizationDemoController : MonoBehaviour {
    [SerializeField] TMP_Text _statusText;
    [SerializeField] TMP_Text _outputText;

    // Keys shown in the output panel. "missing_key" demonstrates MissingKeyMode.
    static readonly string[] DemoKeys = { "greeting", "farewell", "app_name", "language", "missing_key" };

    void Start() {
        LocalizationSystem.Configure();   // reads Resources/LocalizationSettings
        Report("Configured. Press a button to load.");
    }

    // ── Button handlers (wired in scene) ─────────────────────────────────────────

    public void OnEnglishSync()  => LoadSync(GameLanguage.English);
    public void OnThaiSync()     => LoadSync(GameLanguage.Thai);
    public void OnEnglishAsync() => LoadAsync(GameLanguage.English).Forget();
    public void OnThaiAsync()    => LoadAsync(GameLanguage.Thai).Forget();

    public void OnRelease() {
        LocalizationSystem.Release();
        Report("Released — all entries cleared.");
    }

    // ── Workflow ─────────────────────────────────────────────────────────────────

    void LoadSync(GameLanguage language) {
        LocalizationSystem.SetLanguageSync(language);
        Report($"Loaded {language} (sync).");
    }

    async UniTaskVoid LoadAsync(GameLanguage language) {
        Report($"Loading {language} (async)…");
        await LocalizationSystem.SetLanguageAsync(language, this.GetCancellationTokenOnDestroy());
        Report($"Loaded {language} (async).");
    }

    // ── Output ───────────────────────────────────────────────────────────────────

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
