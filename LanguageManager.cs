using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Csv2Xlsx3
{
    public sealed class LanguageOption
    {
        public string Code { get; private set; }
        public string DisplayName { get; private set; }

        public LanguageOption(string code, string displayName)
        {
            Code = code;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public static class LanguageManager
    {
        private const string DefaultLanguageCode = "EN";
        private const string LanguageConfigKey = "Language";
        private const string LanguageFileName = "CSV2XLSX.lang";
        private const string ConfigFileName = "CSVtoXLSX.cfg";

        private static readonly Dictionary<string, Dictionary<string, string>> Languages =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        private static string currentLanguageCode = DefaultLanguageCode;
        private static bool initialized;

        public static string CurrentLanguageCode
        {
            get
            {
                EnsureInitialized();
                return currentLanguageCode;
            }
        }

        public static string LanguageFilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LanguageFileName); }
        }

        public static string ConfigFilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName); }
        }

        public static void Initialize()
        {
            Languages.Clear();

            AddBuiltInEnglish();
            AddBuiltInGerman();

            EnsureLanguageFileExists();
            LoadLanguageFile();
            currentLanguageCode = LoadConfiguredLanguageCode();

            if (!Languages.ContainsKey(currentLanguageCode))
            {
                currentLanguageCode = DefaultLanguageCode;
            }

            initialized = true;
        }

        public static void Reload()
        {
            initialized = false;
            Initialize();
        }

        public static List<LanguageOption> GetAvailableLanguages()
        {
            EnsureInitialized();

            return Languages
                .Keys
                .OrderBy(GetLanguageSortIndex)
                .ThenBy(code => code, StringComparer.OrdinalIgnoreCase)
                .Select(code => new LanguageOption(code, GetLanguageDisplayName(code)))
                .ToList();
        }

        public static void SetCurrentLanguage(string languageCode)
        {
            EnsureInitialized();

            if (string.IsNullOrWhiteSpace(languageCode))
                return;

            languageCode = languageCode.Trim().ToUpperInvariant();

            if (!Languages.ContainsKey(languageCode))
                return;

            currentLanguageCode = languageCode;
            SaveConfiguredLanguageCode(languageCode);
        }

        public static string T(string key)
        {
            EnsureInitialized();

            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            string value;

            if (Languages.TryGetValue(currentLanguageCode, out var selectedLanguage) &&
                selectedLanguage.TryGetValue(key, out value))
            {
                return value;
            }

            if (Languages.TryGetValue(DefaultLanguageCode, out var defaultLanguage) &&
                defaultLanguage.TryGetValue(key, out value))
            {
                return value;
            }

            return key;
        }

        public static string T(string key, params object[] args)
        {
            string text = T(key);

            if (args == null || args.Length == 0)
                return text;

            return string.Format(CultureInfo.CurrentCulture, text, args);
        }

        private static void EnsureInitialized()
        {
            if (!initialized)
            {
                Initialize();
            }
        }

        private static int GetLanguageSortIndex(string code)
        {
            if (string.Equals(code, "EN", StringComparison.OrdinalIgnoreCase))
                return 0;

            if (string.Equals(code, "DE", StringComparison.OrdinalIgnoreCase))
                return 1;

            return 2;
        }

        private static string GetLanguageDisplayName(string code)
        {
            if (Languages.TryGetValue(code, out var language) &&
                language.TryGetValue("Language.Name", out var displayName) &&
                !string.IsNullOrWhiteSpace(displayName))
            {
                return displayName;
            }

            return code.ToUpperInvariant();
        }

        private static string LoadConfiguredLanguageCode()
        {
            var settings = LoadConfiguration();

            if (settings.TryGetValue(LanguageConfigKey, out var configuredLanguage) &&
                !string.IsNullOrWhiteSpace(configuredLanguage))
            {
                return configuredLanguage.Trim().ToUpperInvariant();
            }

            return DefaultLanguageCode;
        }

        private static void SaveConfiguredLanguageCode(string languageCode)
        {
            var settings = LoadConfiguration();
            settings[LanguageConfigKey] = languageCode;

            File.WriteAllLines(
                ConfigFilePath,
                settings.Select(setting => setting.Key + "=" + setting.Value));
        }

        private static Dictionary<string, string> LoadConfiguration()
        {
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(ConfigFilePath))
                return settings;

            foreach (var line in File.ReadAllLines(ConfigFilePath))
            {
                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                    continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                settings[key] = value;
            }

            return settings;
        }

        private static void LoadLanguageFile()
        {
            if (!File.Exists(LanguageFilePath))
                return;

            string currentSection = null;

            foreach (var rawLine in File.ReadAllLines(LanguageFilePath, Encoding.UTF8))
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("#", StringComparison.Ordinal) ||
                    line.StartsWith(";", StringComparison.Ordinal))
                    continue;

                if (line.StartsWith("[", StringComparison.Ordinal) &&
                    line.EndsWith("]", StringComparison.Ordinal))
                {
                    currentSection = line.Substring(1, line.Length - 2).Trim().ToUpperInvariant();

                    if (!string.IsNullOrWhiteSpace(currentSection) &&
                        !Languages.ContainsKey(currentSection))
                    {
                        Languages[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }

                    continue;
                }

                if (string.IsNullOrWhiteSpace(currentSection))
                    continue;

                int separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                string key = line.Substring(0, separatorIndex).Trim();
                string value = line.Substring(separatorIndex + 1).Trim();

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                Languages[currentSection][key] = DecodeValue(value);
            }
        }

        private static void EnsureLanguageFileExists()
        {
            if (File.Exists(LanguageFilePath))
                return;

            File.WriteAllText(LanguageFilePath, CreateDefaultLanguageFileContent(), Encoding.UTF8);
        }

        private static string DecodeValue(string value)
        {
            if (value == null)
                return string.Empty;

            return value
                .Replace("\\r\\n", "\n")
                .Replace("\\n", "\n")
                .Replace("\\t", "\t");
        }

        private static string EncodeValue(string value)
        {
            if (value == null)
                return string.Empty;

            return value
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        private static void AddBuiltInEnglish()
        {
            var language = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            language["Language.Name"] = "English";

            language["App.Title"] = "CSV2XLSX";
            language["App.NotifyIconText"] = "CSV2XLSX";
            language["App.NotifyBalloonText"] = "CSV2XLSX is running in the background";

            language["Button.OK"] = "OK";
            language["Button.Cancel"] = "Cancel";
            language["Button.Save"] = "Save";

            language["Main.Menu.File"] = "File";
            language["Main.Menu.BatchJob"] = "Batch job";
            language["Main.Menu.Settings"] = "Settings";
            language["Main.Menu.Exit"] = "Exit";
            language["Main.Menu.Help"] = "?";
            language["Main.Menu.About"] = "About";
            language["Main.DropText"] = "Drop .csv here";

            language["Dialog.SaveAsTitle"] = "Save as";
            language["Dialog.ExcelWorkbookFilter"] = "Excel Workbook|*.xlsx";
            language["Dialog.CsvFilesTitle"] = "Select multiple CSV files";
            language["Dialog.CsvFilesFilter"] = "CSV files (*.csv)|*.csv";

            language["Message.DoneCaption"] = "Done";
            language["Message.ErrorCaption"] = "Error";
            language["Message.BatchDoneCaption"] = "Batch job finished";
            language["Message.SaveSuccess"] = "Successfully saved!";
            language["Message.ConvertSuccess"] = "File was converted successfully:\\n{0}";
            language["Message.ConvertError"] = "Error while converting:\\n{0}";
            language["Message.GenericError"] = "Error: {0}";
            language["Message.InvalidColumns"] = "No valid columns selected!";
            language["Message.BatchDone"] = "All files have been processed.";

            language["CsvDelimiter.Title"] = "CSV delimiter";
            language["CsvDelimiter.Message"] = "The delimiter could not be detected reliably. Please select the CSV delimiter used.";
            language["CsvDelimiter.Comma"] = "Comma (,)";
            language["CsvDelimiter.Semicolon"] = "Semicolon (;)";
            language["CsvDelimiter.Tab"] = "Tab (\\t)";
            language["CsvDelimiter.Pipe"] = "Pipe (|)";

            language["ColumnSelection.Title"] = "Select columns";

            language["Settings.Title"] = "Settings";
            language["Settings.Systray"] = "Minimize application to system tray";
            language["Settings.Shell"] = "Enable shell context menu for CSV files";
            language["Settings.AlwaysOnTop"] = "Always on top";
            language["Settings.ShellInfo"] = "The shell context menu allows conversion directly by right-clicking CSV files in Windows Explorer.";
            language["Settings.CsvDelimiter"] = "CSV delimiter";
            language["Settings.DelimiterHint"] = "\",\" and \";\" are detected automatically";
            language["Settings.ConfigPathRuntime"] = "Path is set at runtime";
            language["Settings.ConfigPathText"] = "Settings are saved under:\\n{0}";
            language["Settings.Language"] = "Language";

            language["About.Title"] = "About";
            language["About.ProductName"] = "CSV2XLSX";
            language["About.Copyright"] = "(c) Daniel Capilla";
            language["About.Version"] = "Version: {0}";
            language["About.NoUpdates"] = "No new updates";
            language["About.UpdateAvailable"] = "Update available: {0}";
            language["About.Info"] = "CSV2XLSX is free to use.\\nIf this tool saves you time, you can support development here:";
            language["About.KoFiText"] = string.Empty;

            Languages["EN"] = language;
        }

        private static void AddBuiltInGerman()
        {
            var language = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            language["Language.Name"] = "Deutsch";

            language["App.Title"] = "CSV2XLSX";
            language["App.NotifyIconText"] = "CSV2XLSX";
            language["App.NotifyBalloonText"] = "CSV2XLSX läuft im Hintergrund";

            language["Button.OK"] = "OK";
            language["Button.Cancel"] = "Abbrechen";
            language["Button.Save"] = "Speichern";

            language["Main.Menu.File"] = "Datei";
            language["Main.Menu.BatchJob"] = "Batchjob";
            language["Main.Menu.Settings"] = "Einstellungen";
            language["Main.Menu.Exit"] = "Programm beenden";
            language["Main.Menu.Help"] = "?";
            language["Main.Menu.About"] = "Über";
            language["Main.DropText"] = "Hier .csv ablegen";

            language["Dialog.SaveAsTitle"] = "Speichern unter";
            language["Dialog.ExcelWorkbookFilter"] = "Excel Workbook|*.xlsx";
            language["Dialog.CsvFilesTitle"] = "Mehrere CSV-Dateien wählen";
            language["Dialog.CsvFilesFilter"] = "CSV-Dateien (*.csv)|*.csv";

            language["Message.DoneCaption"] = "Fertig";
            language["Message.ErrorCaption"] = "Fehler";
            language["Message.BatchDoneCaption"] = "Batchjob beendet";
            language["Message.SaveSuccess"] = "Erfolgreich gespeichert!";
            language["Message.ConvertSuccess"] = "Datei wurde erfolgreich konvertiert:\\n{0}";
            language["Message.ConvertError"] = "Fehler beim Konvertieren:\\n{0}";
            language["Message.GenericError"] = "Fehler: {0}";
            language["Message.InvalidColumns"] = "Keine gültigen Spalten zur Auswahl!";
            language["Message.BatchDone"] = "Alle Dateien wurden verarbeitet.";

            language["CsvDelimiter.Title"] = "CSV-Trennzeichen";
            language["CsvDelimiter.Message"] = "Das Trennzeichen konnte nicht eindeutig erkannt werden. Bitte wählen Sie das verwendete CSV-Trennzeichen.";
            language["CsvDelimiter.Comma"] = "Komma (,)";
            language["CsvDelimiter.Semicolon"] = "Semikolon (;)";
            language["CsvDelimiter.Tab"] = "Tabulator (\\t)";
            language["CsvDelimiter.Pipe"] = "Pipe (|)";

            language["ColumnSelection.Title"] = "Spalten auswählen";

            language["Settings.Title"] = "Einstellungen";
            language["Settings.Systray"] = "Programm bei Minimierung in den Systray";
            language["Settings.Shell"] = "Shell-Kontextmenü für CSV-Dateien aktivieren";
            language["Settings.AlwaysOnTop"] = "Programm immer im Vordergrund";
            language["Settings.ShellInfo"] = "Das Shell-Kontextmenü ermöglicht die Konvertierung direkt per Rechtsklick auf CSV-Dateien im Windows-Explorer.";
            language["Settings.CsvDelimiter"] = "CSV-Trennzeichen";
            language["Settings.DelimiterHint"] = "\",\" und \";\" werden automatisch erkannt";
            language["Settings.ConfigPathRuntime"] = "Pfad wird zur Laufzeit gesetzt";
            language["Settings.ConfigPathText"] = "Einstellungen werden gespeichert unter:\\n{0}";
            language["Settings.Language"] = "Sprache";

            language["About.Title"] = "About";
            language["About.ProductName"] = "CSV2XLSX";
            language["About.Copyright"] = "(c) Daniel Capilla";
            language["About.Version"] = "Version: {0}";
            language["About.NoUpdates"] = "Keine neuen Updates";
            language["About.UpdateAvailable"] = "Update verfügbar: {0}";
            language["About.Info"] = "CSV2XLSX ist kostenlos nutzbar.\\nWenn dir dieses Tool Zeit spart, kannst du die Entwicklung hier unterstützen:";
            language["About.KoFiText"] = string.Empty;

            Languages["DE"] = language;
        }

        private static string CreateDefaultLanguageFileContent()
        {
            var builder = new StringBuilder();

            builder.AppendLine("; CSV2XLSX language file");
            builder.AppendLine("; Simple format:");
            builder.AppendLine("; [EN]");
            builder.AppendLine("; Key=Text");
            builder.AppendLine("; Use \\n for line breaks and \\t for tabs.");
            builder.AppendLine("; Add a new language by adding a new section, e.g. [FR].");
            builder.AppendLine("; The value Language.Name is shown in the Settings dropdown.");
            builder.AppendLine();

            AppendLanguageSection(builder, "EN");
            builder.AppendLine();
            AppendLanguageSection(builder, "DE");

            return builder.ToString();
        }

        private static void AppendLanguageSection(StringBuilder builder, string languageCode)
        {
            builder.AppendLine("[" + languageCode + "]");

            foreach (var item in Languages[languageCode].OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                builder.AppendLine(item.Key + "=" + EncodeValue(item.Value));
            }
        }
    }
}