namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public class TypingConfig {
        private bool _showIncorrectCharacters = true;
        private string _language = "english";
        #region Punctuation
        private float _punctuationRate = 0.05f;
        private bool _punctuation = false;
        #endregion

        #region Numbers
        private float _numbersRate = 0.1f;
        private int _numbersMin = 0;
        private int _numbersMax = 100;
        private bool _numbers = false;
        #endregion

        #region Capitalise
        private float _capitaliseRate = 0.05f;
        private bool _capitalise = false;
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
        public bool Punctuation {
            get { return _punctuation; }
            set { _punctuation = value; }
        }

        public float PunctuationRate {
            get { return _punctuationRate; }
            set { _punctuationRate = value; }
        }
        #endregion

        #region Numbers Getter/Setter
        public bool Numbers {
            get { return _numbers; }
            set { _numbers = value; }
        }

        public float NumbersRate {
            get { return _numbersRate; }
            set { _numbersRate = value; }
        }

        public int NumbersMin {
            get { return _numbersMin; }
            set { _numbersMin = value; }
        }

        public int NumbersMax {
            get { return _numbersMax; }
            set { _numbersMax = value; }
        }
        #endregion

        #region Capitalise Getter/Setter
        public bool Capitalise {
            get { return _capitalise; }
            set { _capitalise = value; }
        }

        public float CapitaliseRate {
            get { return _capitaliseRate; }
            set { _capitaliseRate = value; }
        }
        #endregion
    }
}