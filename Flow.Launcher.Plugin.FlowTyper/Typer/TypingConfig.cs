using System;
using System.IO;
using System.Text.Json;

namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public class TypingConfig {
        private bool _showIncorrectCharacters = true;
        private string _language = "english";
        #region Punctuation
        private float _punctuationRate = 0f;
        #endregion

        #region Numbers
        private float _numbersRate = 0f;
        #endregion

        #region Capitalise
        private float _capitalizeRate = 0f;
        #endregion

        public bool ShowIncorrectCharacters {
            get { return _showIncorrectCharacters; }
            set { _showIncorrectCharacters = value; }
        }
        public string Language {
            get { return _language; }
            set { _language = value; }
        }

        #region Punctuation Getter/Setter

        public float PunctuationRate {
            get { return _punctuationRate; }
            set { _punctuationRate = value; }
        }
        #endregion

        #region Numbers Getter/Setter
        public float NumbersRate {
            get { return _numbersRate; }
            set { _numbersRate = value; }
        }
        #endregion

        #region Capitalise Getter/Setter

        public float CapitalizeRate {
            get { return _capitalizeRate; }
            set { _capitalizeRate = value; }
        }
        #endregion

        public Exception SaveConfig() {
            if (!Directory.Exists(Constants.CONFIG_DIR)) {
                Directory.CreateDirectory(Constants.CONFIG_DIR);
            }

            string path = Path.Combine(Constants.CONFIG_DIR, ".config.json");
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(path, json);

            return null;
        }

        public static TypingConfig LoadConfig() {
            string path = Path.Combine(Constants.CONFIG_DIR, ".config.json");
            if (File.Exists(path)) {
                
                FileStream configFile = new FileStream(path, FileMode.Open);

                using (StreamReader reader = new StreamReader(configFile))
                {
                    string json = reader.ReadToEnd();

                    return JsonSerializer.Deserialize<TypingConfig>(json);
                }

            }
            else {
                // If FileNotFound, create the file
                TypingConfig conf = new();
                conf.SaveConfig();

                return conf;
            }

        }
    }
}