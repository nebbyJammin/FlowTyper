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
    }
}