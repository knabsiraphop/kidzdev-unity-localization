# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-06-20

### Changed
- Promoted to stable v1.0.0 — API is complete and unchanged from 0.1.0.

### Added
- `LocalizationHandler.SetFormat(key, params object[] args)` — applies a multi-arg format string immediately without subscribing to language changes.
- `TmpTextExtensions.SetLocalizedKeyWithArg(text, key, arg)` — attaches (or reuses) a `LocalizationHandler` with a single format argument that auto-refreshes.
- `LocalizationSettings.SourceConfig` nested type with `sourceName`, `loaderType`, and `pathTemplate` fields configurable in the Inspector.
- `LocalizationLoaderType` enum (`Json`, `Table`) used by `LocalizationSettings`.
- `ResourcesJsonLoader` accepts both bare JSON arrays (`[…]`) and wrapped objects (`{"items":[…]}`).

## [0.1.0] - 2026-06-20

### Added
- `GameLanguage` enum (English, Thai, Chinese, Korean, Japanese, Spanish).
- `LocalizationEntry` data model and `LocalizationTable` ScriptableObject.
- `ILocalizationLoader` seam with sync (`Load`) and async (`LoadAsync`) methods.
- `LocalizationContainer` — internal keyed string dictionary.
- `LocalizationManager` — owns language state, named sources, UniTask parallel loading, `OnLanguageChanged` event.
- `LocalizationSystem` — static facade delegating to `LocalizationManager.Default`.
- `ResourcesJsonLoader` — loads JSON arrays (`[{key, content}]`) from `Resources`.
- `ResourcesTableLoader` — loads `LocalizationTable` ScriptableObjects from `Resources`.
- `MissingKeyMode` — `BracketedKey` (default), `RawKey`, `Empty`.
- `LocalizationHandler` MonoBehaviour — auto-refreshes TMP_Text on language change.
- `TmpTextExtensions` — `SetLocalizedKey`, `SetLocalizedKeyFormat`, `SetLocalizedText`, `SetLocalizedTextFormat`.
- Demo sample: JSON data files, runtime controller, and Editor setup script.
