using System;
using System.Collections.Generic;
using UnityEngine;

namespace KidzDev.Unity.Localization {
    /// <summary>
    /// Resources-loaded ScriptableObject that configures <see cref="LocalizationManager"/>.
    /// Place one asset at <c>Resources/LocalizationSettings.asset</c> and call
    /// <see cref="LocalizationSystem.Configure"/> (or pass it to <see cref="LocalizationManager.Apply"/>).
    /// Use <b>KidzDev &gt; Localization &gt; Create Settings</b> to create it.
    /// </summary>
    [CreateAssetMenu(menuName = "KidzDev/Localization/Settings", fileName = "LocalizationSettings")]
    public sealed class LocalizationSettings : ScriptableObject {
        [Serializable]
        public sealed class SourceConfig {
            [Tooltip("Unique name for this source. Later sources override earlier ones on key collision.")]
            public string sourceName = "default";

            [Tooltip("Loader type: Json = ResourcesJsonLoader, Table = ResourcesTableLoader.")]
            public LocalizationLoaderType loaderType = LocalizationLoaderType.Json;

            [Tooltip("Resources path with {language} token, e.g. Localization/{language}Data")]
            public string pathTemplate = "Localization/{language}Data";
        }

        [SerializeField] MissingKeyMode _missingKeyMode = MissingKeyMode.BracketedKey;
        [SerializeField] List<SourceConfig> _sources = new();

        public MissingKeyMode MissingKeyMode => _missingKeyMode;
        public IReadOnlyList<SourceConfig> Sources => _sources;
    }
}
