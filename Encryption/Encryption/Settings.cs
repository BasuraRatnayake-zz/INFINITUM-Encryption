using System;
using System.Text;
using System.Threading;

namespace Infinitum.Encryption {
    public class SettingData {
        public const string Seperator = "#IR#";

        public string ViewLimit { get; set; }
        public string DateExpire { get; set; }

        public SettingData() {
            ViewLimit = "UUUU";
            DateExpire = "UU/UU/UUUU";
        }

        public string compileSettings() {
            string _settings = $"numberofviews:'{ViewLimit}',expire:'{DateExpire}'";
            return $"{Seperator}{_settings}{Seperator}";
        }

        public void decompileSettings(byte[] jsonSettings) {
            string _jsonSettings = Encoding.UTF8.GetString(jsonSettings);            

            _jsonSettings = _jsonSettings
            .Replace(Seperator, string.Empty)
            .Replace("\r\n", string.Empty)
            .Replace("{", string.Empty)
            .Replace("}", string.Empty)
            .Replace("'", string.Empty);

            string[] _setCollection = _jsonSettings.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            ViewLimit = _setCollection[0].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1];
            DateExpire = _setCollection[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1];
        }
    }

    public class Settings {
        private const int settingsLength = 48;

        public SettingData settingsJSON;

        public int Length {
            get {
                return settingsLength;
            }
        }

        public byte[] writeSettings(SettingData settingsJSON, byte[] plainTextBytes) {
            this.settingsJSON = settingsJSON;

            byte[] _settings = Encoding.UTF8.GetBytes(settingsJSON.compileSettings());

            byte[] dataWithSettings = new byte[plainTextBytes.Length + _settings.Length];
            for (int i = 0;i < settingsLength;i++) {
                dataWithSettings[i] = _settings[i];
            }

            int dataWithSettingsLength = dataWithSettings.Length;

            for (int i = settingsLength;i < dataWithSettingsLength;i++) {
                dataWithSettings[i] = plainTextBytes[i - settingsLength];
            }

            return dataWithSettings;
        }

        public byte[] readSettings(byte[] cipherTextBytes) {
            byte[] dataWithoutSettings = new byte[cipherTextBytes.Length - Length];

            byte[] _settings = new byte[Length];
            int cipherTextLength = cipherTextBytes.Length;

            for (int i = 0;i < cipherTextLength;i++) {
                if (i <= Length - 1) {
                    _settings[i] = cipherTextBytes[i];
                } else {
                    dataWithoutSettings[i - Length] = cipherTextBytes[i];
                }
            }            

            new Thread(()=> {
                try {
                    settingsJSON.decompileSettings(_settings);
                    Thread.Sleep(1000);
                } catch { }
            }).Start();

            return dataWithoutSettings;
        }        
    }
}