using System.Xml.Linq;

namespace SENTINEL
{
    /// <summary>
    /// Generates download.done XML files conforming to the KRAKEN XSD schema.
    /// 
    /// <para><b>Schema structure (from download.done.xsd):</b></para>
    /// <code>
    /// &lt;Done&gt;
    ///   &lt;UserName&gt;SENTINEL&lt;/UserName&gt;
    ///   &lt;ComputerName&gt;WORKSTATION-01&lt;/ComputerName&gt;
    ///   &lt;UserDomain&gt;KTM&lt;/UserDomain&gt;
    ///   &lt;DateTime&gt;2025-06-15T14:32:00&lt;/DateTime&gt;
    ///   &lt;LocalPath&gt;C:\...\Run_205&lt;/LocalPath&gt;
    ///   &lt;RemotePath/&gt;
    ///   &lt;Run path="Run_205" size="524288" subfolders="21" files="23"&gt;
    ///     &lt;File size="2048"&gt;Session.xml&lt;/File&gt;
    ///     &lt;Lap path="Lap_001" size="24576" subfolders="0" files="1"&gt;
    ///       &lt;File size="24576"&gt;cableData.ztx&lt;/File&gt;
    ///     &lt;/Lap&gt;
    ///   &lt;/Run&gt;
    /// &lt;/Done&gt;
    /// </code>
    /// </summary>
    public static class DownloadDoneWriter
    {
        private const string FileName = "download.done";

        /// <summary>
        /// Generates and writes the download.done file into the run folder.
        /// </summary>
        /// <param name="snapshot">The stable snapshot of the run folder</param>
        /// <returns>Path to the created download.done file</returns>
        public static string Write(RunSnapshot snapshot)
        {
            var runDir = new DirectoryInfo(snapshot.RunPath);
            var runName = runDir.Name;

            var totalSubfolders = CountSubfolders(snapshot);
            var totalFiles = snapshot.FileCount;

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Done",
                    new XElement("UserName", "SENTINEL"),
                    new XElement("ComputerName", Environment.MachineName),
                    new XElement("UserDomain", Environment.UserDomainName),
                    new XElement("DateTime", FormatTimestamp(DateTime.Now)),
                    new XElement("LocalPath", snapshot.RunPath),
                    new XElement("RemotePath", string.Empty),
                    BuildRunElement(runName, snapshot, totalSubfolders, totalFiles)
                )
            );

            // Write completely to a temp file first, then rename: KRAKEN never sees a partial XML.
            var outputPath = Path.Combine(snapshot.RunPath, FileName);
            var tempPath = outputPath + ".tmp";
            try
            {
                doc.Save(tempPath);
                File.Move(tempPath, outputPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            return outputPath;
        }

        /// <summary>
        /// Local time in the format <c>yyyy-MM-ddTHH:mm:ss</c> (xs:dateTime without offset, as before),
        /// formatted culture-invariantly — otherwise e.g. a dot could appear as the time separator.
        /// </summary>
        internal static string FormatTimestamp(DateTime value) =>
            value.ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Checks whether a download.done already exists for the given run folder.
        /// </summary>
        public static bool Exists(string runPath)
        {
            return File.Exists(Path.Combine(runPath, FileName));
        }

        private static XElement BuildRunElement(string runName, RunSnapshot snapshot, int subfolders, int files)
        {
            var runElement = new XElement("Run",
                new XAttribute("path", runName),
                new XAttribute("size", snapshot.TotalSizeBytes),
                new XAttribute("subfolders", subfolders),
                new XAttribute("files", files)
            );

            // Files at run level (e.g. Session.xml)
            foreach (var file in snapshot.RunFiles)
            {
                runElement.Add(new XElement("File",
                    new XAttribute("size", file.SizeBytes),
                    file.FileName
                ));
            }

            // Lap folders
            foreach (var lap in snapshot.Laps)
            {
                var lapElement = new XElement("Lap",
                    new XAttribute("path", lap.LapName),
                    new XAttribute("size", lap.SizeBytes),
                    new XAttribute("subfolders", lap.SubfolderCount),
                    new XAttribute("files", lap.FileCount)
                );

                foreach (var file in lap.Files)
                {
                    lapElement.Add(new XElement("File",
                        new XAttribute("size", file.SizeBytes),
                        file.FileName
                    ));
                }

                runElement.Add(lapElement);
            }

            return runElement;
        }

        /// <summary>
        /// Counts all subfolders in the run (Lap_* folders + their subfolders).
        /// </summary>
        private static int CountSubfolders(RunSnapshot snapshot)
        {
            int count = snapshot.LapCount;
            foreach (var lap in snapshot.Laps)
            {
                count += lap.SubfolderCount;
            }
            return count;
        }
    }
}
