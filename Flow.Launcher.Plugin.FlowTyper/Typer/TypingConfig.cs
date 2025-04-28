
namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public class TypingConfig {
        private bool _showIncorrectCharacters = true;
        private string _language = "english";

        public bool ShowIncorrectCharacters {
            get { return _showIncorrectCharacters; }
            set { _showIncorrectCharacters = value; }
        }
        public string Language {
            get { return _language; }
            set { _language = value; }
        }
    }
}