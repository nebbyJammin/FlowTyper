using System;
using System.IO;
using System.Text.Json;

namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public class TypingConfig {
        private bool _showIncorrectCharacters = true;
        private string _language = "english";
        private float _punctuationRate = 0f;
        private float _numbersRate = 0f;
        private float _capitalizeRate = 0f;
        public bool ShowIncorrectCharacters {
            get { return _showIncorrectCharacters; }
            set { _showIncorrectCharacters = value; }
        }
        public string Language {
            get { return _language; }
            set { _language = value; }
        }
        public float PunctuationRate {
            get { return _punctuationRate; }
            set { _punctuationRate = value; }
        }
        public float NumbersRate {
            get { return _numbersRate; }
            set { _numbersRate = value; }
        }
        public float CapitalizeRate {
            get { return _capitalizeRate; }
            set { _capitalizeRate = value; }
        }
    }
}