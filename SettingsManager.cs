using System.IO;
using System.Text.Json;

namespace DesktopClockWidget
{
    internal class SettingsManager
    {
        private const string PathFile = "clock_settings.json";
        public double? Left { get; set; }
        public double? Top { get; set; }
        public bool ClickThrough { get; set; }
        public bool RunAtStartup { get; set; }

        public void Load()
        {
            if (File.Exists(PathFile))
            {
                var json = File.ReadAllText(PathFile);
                var obj = JsonSerializer.Deserialize<SettingsManager>(json);
                Left = obj.Left;
                Top = obj.Top;
                ClickThrough = obj.ClickThrough;
                RunAtStartup = obj.RunAtStartup;
            }
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this);
            File.WriteAllText(PathFile, json);
        }
    }
}
