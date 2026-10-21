using System.Security.Cryptography;
using System.Text.Json;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Beschreibt die Version einer Eingabedatei (KOMPASS.json, loadChannels.xlsx) für Log und Status:
    /// Zeitstempel, Größe, SHA-256 (gekürzt) und – für KOMPASS – Autor/Zeit aus dem KODE-Sidecar
    /// <c>KOMPASS.meta.json</c>. Damit ist nachvollziehbar, mit welchem Datenstand ein Run verarbeitet wurde.
    /// </summary>
    public static class InputFingerprint
    {
        /// <summary>Kurzbeschreibung, z.B. <c>KOMPASS.json 2026-10-12T12:07:04Z, 5.1 KB, sha256 1a2b3c4d5e6f, saved by reicheltd 2026-10-12T12:07:04Z</c>.</summary>
        public static string Describe(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return $"{path} (missing)";

            try
            {
                var info = new FileInfo(path);
                string hash;
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    hash = Convert.ToHexString(SHA256.HashData(stream))[..12].ToLowerInvariant();

                var text = $"{info.Name} {info.LastWriteTimeUtc:yyyy-MM-ddTHH:mm:ssZ}, {info.Length / 1024.0:0.0} KB, sha256 {hash}";
                var meta = Path.Combine(info.DirectoryName ?? string.Empty, Path.GetFileNameWithoutExtension(info.Name) + ".meta.json");
                if (File.Exists(meta))
                {
                    using var document = JsonDocument.Parse(File.ReadAllText(meta));
                    var root = document.RootElement;
                    var by = root.TryGetProperty("updatedBy", out var u) ? u.GetString() : null;
                    var at = root.TryGetProperty("lastUpdatedUtc", out var t) ? t.GetString() : null;
                    if (!string.IsNullOrEmpty(by) || !string.IsNullOrEmpty(at))
                        text += $", saved by {by} {at}";
                }
                return text;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                return $"{path} (not readable: {ex.Message})";
            }
        }
    }
}
