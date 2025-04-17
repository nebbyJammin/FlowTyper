
namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public class TypingConfig {
        private bool _useOptimisticWordList = true;
        private string _language = "english";

        public bool UseOptimisticWordList {
            get { return _useOptimisticWordList; }
            set { _useOptimisticWordList = value; }
        }
        public string Language {
            get { return _language; }
            set { _language = value; }
        }
    }
}