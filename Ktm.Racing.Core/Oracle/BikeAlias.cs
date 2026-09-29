namespace Ktm.Racing.Core
{
    /// <summary>
    /// Normalisiert Bike-/Rider-Aliasse auf die kanonische Form ohne Leerzeichen
    /// (z.B. <c>"AA 01"</c> → <c>"AA01"</c>).
    ///
    /// <para>Ein Leerzeichen im Device-Ordnernamen ist ein bekannter Datenfehler: Er führt sonst zu
    /// einer fehlerhaften <c>RiderAbbreviation</c> (<c>"AA "</c> statt <c>"AA"</c>) — wodurch die
    /// KOMPASS-Fahrersuche fehlschlägt — und zu Leerzeichen in den geschriebenen
    /// <c>Name</c>/<c>Alias</c>-Properties, die downstream (MotoDB/Tableau) Probleme verursachen.</para>
    /// </summary>
    public static class BikeAlias
    {
        /// <summary>Entfernt alle Leerraum-Zeichen aus dem Alias (<c>"AA 01"</c> → <c>"AA01"</c>).</summary>
        public static string Normalize(string? alias)
            => string.IsNullOrEmpty(alias)
                ? string.Empty
                : new string(alias.Where(c => !char.IsWhiteSpace(c)).ToArray());

        /// <summary>True, wenn der Alias bereits kanonisch ist (leer oder ohne Leerraum).</summary>
        public static bool IsCanonical(string? alias)
            => string.IsNullOrEmpty(alias) || !alias.Any(char.IsWhiteSpace);
    }
}
