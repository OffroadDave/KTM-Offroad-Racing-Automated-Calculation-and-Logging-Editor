using System.Security.Cryptography;
using System.Text.Json;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Describes the version of an input file (KOMPASS.json, loadChannels.xlsx) for log and status:
    /// timestamp, size, SHA-256 (shortened) and – for KOMPASS – author/time from the KODE sidecar
    /// <c>KOMPASS.meta.json</c>. This makes it traceable which data version a run was processed with.
    /// </summary>
    public static class InputFingerprint
    {
        /// <summary>Short description, e.g. <c>KOMPASS.json 2026-10-12T12:07:04Z, 5.1 KB, sha256 1a2b3c4d5e6f, saved by reicheltd 2026-10-12T12:07:04Z</c>.</summary>
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
