namespace KidzDev.Unity.Localization {
    public enum MissingKeyMode {
        BracketedKey,  // returns [key]  (default — easy to spot in UI)
        RawKey,        // returns key
        Empty,         // returns string.Empty
    }
}
