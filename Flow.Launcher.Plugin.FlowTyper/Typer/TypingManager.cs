using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Media.Effects;

namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public class TypingManager {
        private PluginInitContext _context;
        public TypingConfig _conf {get; private set;}
        private readonly string WORDLIST_PATH;
        private readonly string LIST_OF_WORDLISTS_PATH;
        private const int WORD_QUEUE_LENGTH = 40;
        private const double TEST_TIMEOUT_LENGTH = 5;
        private Queue<string> wordQueue;
        public string[] CurrentWords {
            get {
                return wordQueue.ToArray();
            }
        }

        public string NextWord {
            get {
                return wordQueue.Peek();
            }
        }

        public string CurrentWordsString {
            get {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (string word in CurrentWords) {
                    stringBuilder.Append(word);
                    stringBuilder.Append(" ");
                } 

                return stringBuilder.ToString();
            }
        }

        public string CurrentWordsExceptFirstString {
            get {
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 1; i < CurrentWords.Length; i++) {
                    stringBuilder.Append(CurrentWords[i]);
                    stringBuilder.Append(" ");
                }

                return stringBuilder.ToString();
            }
        }
        private Random random = new Random();
        public WordList WordsList {get; private set;}
        public WordLists listOfWordsList {get; init; }
        public double WPM {get; private set;}
        public double RawWPM {get; private set;}
        public double Accuracy {get; private set;}
        public int WordsTyped {get; private set;}
        public int CorrectCharacters {get; private set;}
        public int TotalCharacters {get; private set;}
        public DateTime TestStart {get; private set;}
        public DateTime LastTimeToType {get; private set;}
        public bool ForceResetTestStartTime {get; private set;}
        public TypingManager(PluginInitContext context) {
            this._context = context;
            this._conf = _context.API.LoadSettingJsonStorage<TypingConfig>();

            this.WORDLIST_PATH = Path.Combine(_context.CurrentPluginMetadata.PluginDirectory, "Static\\", "Wordlists\\");
            this.LIST_OF_WORDLISTS_PATH = Path.Combine(WORDLIST_PATH, "_wordlists.json");

            FileStream file = new FileStream(LIST_OF_WORDLISTS_PATH, FileMode.Open);
            using (StreamReader reader = new StreamReader(file))
            {
                string Json = reader.ReadToEnd();

                listOfWordsList = JsonSerializer.Deserialize<WordLists>(Json);
            }


            TestStart = DateTime.UnixEpoch; 

            random = new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            LoadWordList();
        }

        public void SetActiveWordList(string wordList) {
            _conf.Language = wordList;
            _context.API.SaveSettingJsonStorage<TypingConfig>();
            LoadWordList();
        }

        private FileStream getWordListJSON(string wordListName) {
            return new FileStream(WORDLIST_PATH + wordListName + ".json", FileMode.Open);
        }
        
        public void UpdateStatistics(int charsTyped = 0, int correctChars = 0) {
            CorrectCharacters += correctChars;
            TotalCharacters += charsTyped;

            TimeSpan span = DateTime.Now - TestStart;
            TimeSpan lastTypeSpan = DateTime.Now - LastTimeToType;

            if (lastTypeSpan.TotalSeconds > TEST_TIMEOUT_LENGTH || ForceResetTestStartTime) {
                TestStart = DateTime.Now;
                ForceResetTestStartTime = false;
                CorrectCharacters = 0;
                TotalCharacters = 0;
            }

            if (span.TotalMinutes > 0) {
                WPM = CorrectCharacters / 5.0 / span.TotalMinutes;
                RawWPM = TotalCharacters / 5.0 / span.TotalMinutes;
            }

            if (TotalCharacters > 0) {
                Accuracy = (double) CorrectCharacters / TotalCharacters * 100;
            }
        }

        public void EndTest() {
            ForceResetTestStartTime = true;
            CorrectCharacters = 0;
            TotalCharacters = 0;
            WordsTyped = 0;
            WPM = 0;
            RawWPM = 0;
        }

        public void UpdateLastTimeToType() {
            LastTimeToType = DateTime.Now;
        }

        public void confirmWord(string userInputtedWord) {
            // Calculate how many correct characters there are
            int numCorrect = 0;
            string currentWord = wordQueue.Peek();
            int chars = Math.Min(userInputtedWord.Length, currentWord.Length);
            for (int i = 0; i < chars; i++) {
                if (userInputtedWord[i] == currentWord[i]) {
                    numCorrect++;
                }
            }
            // Space character counts as a character, so 1 more than the number of correct chars.
            WordsTyped++;
            UpdateStatistics(currentWord.Length + 1, numCorrect + 1);
            dequeueWord();
        }

        private void dequeueWord() {
            wordQueue.Dequeue();
            enqueueRandomWord();
        }

        private void enqueueRandomWord() {
            // First check if the next word is a number or a "word"
            if (random.NextDouble() < _conf.NumbersRate) {
                int randNum = random.Next(1000);
                wordQueue.Enqueue(randNum.ToString());
            } else {
                int max = WordsList.words.Length;
                string word = WordsList.words[random.Next(max)];

                // Randomize capitalization rate
                if (random.NextDouble() < _conf.CapitalizeRate) {
                    word = char.ToUpper(word[0]) + word.Substring(1);
                }

                // Randomize punctuation rate
                if (random.NextDouble() < _conf.PunctuationRate) {
                    // TODO: Have better punctuation randomization
                    double puncRand = random.NextDouble();
                    if (puncRand < 0.1) {
                        word = $"\"{word}\"";
                    } else if (puncRand < 0.4) {
                        word = $"{word}.";
                    } else if (puncRand < 0.6) {
                        word = $"{word},";
                    } else if (puncRand < 0.8) {
                        word = $"{word}?";
                    } else if (puncRand < 0.9) {
                        word = $"{word}!";
                    } else if (puncRand < 0.95) {
                        word = $"{word}*";
                    } else {
                        word = $"{word}...";
                    }
                }

                wordQueue.Enqueue(word);
            }
        }

        public void PopulateWordQueue() {
            wordQueue.Clear();
            for (int i = 0; i < WORD_QUEUE_LENGTH; i++) {
                enqueueRandomWord();
            }
        }

        public bool LoadWordList() {
            FileStream file = getWordListJSON(this._conf.Language);

            using (StreamReader reader = new StreamReader(file))  {
                string json = reader.ReadToEnd();

                WordsList = JsonSerializer.Deserialize<WordList>(json);
            }

            wordQueue = new Queue<string>();

            PopulateWordQueue();

            return true;
        }
    }
}