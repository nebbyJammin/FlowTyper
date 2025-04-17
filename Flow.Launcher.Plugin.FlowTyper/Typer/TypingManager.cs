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
        private static readonly string WORDLIST_PATH = Constants.PLUGIN_DIR + @"Static\Wordlists\";
        private static readonly string LIST_OF_WORDLISTS_PATH = WORDLIST_PATH + @"_wordlists.json";
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
        public WordLists listOfWordsLists {get; init; }
        public double WPM {get; private set;}
        public double RawWPM {get; private set;}
        public double Accuracy {get; private set;}
        public int CorrectCharacters {get; private set;}
        public int CharactersTyped {get; private set;}
        public DateTime TestStart {get; set;}
        public DateTime LastTimeToType {get; set;}
        public bool ForceResetTestStartTime {get; set;}
        public string ActiveWordListName {get; private set;}
        public TypingManager(string language = "english") {
            this.ActiveWordListName = language;

            FileStream file = new FileStream(LIST_OF_WORDLISTS_PATH, FileMode.Open);
            using (StreamReader reader = new StreamReader(file))
            {
                string Json = reader.ReadToEnd();

                listOfWordsLists = JsonSerializer.Deserialize<WordLists>(Json);
            }


            TestStart = DateTime.Now; 

            random = new Random();
            LoadWordList();
        }

        public void SetActiveWordList(string wordList) {
            this.ActiveWordListName = wordList;
            LoadWordList();
        }

        private FileStream getWordListJSON(string wordListName) {
            return new FileStream(WORDLIST_PATH + wordListName + ".json", FileMode.Open);
        }
        
        public void UpdateStatistics(int charsTyped = 0, int correctChars = 0) {
            CorrectCharacters += correctChars;
            CharactersTyped += charsTyped;

            TimeSpan span = DateTime.Now - TestStart;
            TimeSpan lastTypeSpan = DateTime.Now - LastTimeToType;

            if (lastTypeSpan.TotalSeconds > TEST_TIMEOUT_LENGTH || ForceResetTestStartTime) {
                TestStart = DateTime.Now;
                ForceResetTestStartTime = false;
                CorrectCharacters = 0;
                CharactersTyped = 0;
            }

            if (span.TotalMinutes > 0) {
                WPM = CorrectCharacters / 5.0 / span.TotalMinutes;
                RawWPM = CharactersTyped / 5.0 / span.TotalMinutes;
            }

            if (CharactersTyped > 0) {
                Accuracy = (double) CorrectCharacters / CharactersTyped * 100;
            }
        }

        public void EndTest() {
            ForceResetTestStartTime = true;
            CorrectCharacters = 0;
            CharactersTyped = 0;
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
            UpdateStatistics(userInputtedWord.Length + 1, numCorrect + 1);
            dequeueWord();
        }

        private void dequeueWord() {
            wordQueue.Dequeue();
            enqueueRandomWord();
        }

        private void enqueueRandomWord() {
            int max = WordsList.words.Length;
            string word = WordsList.words[random.Next(max)];

            wordQueue.Enqueue(word);
        }

        private void populateWordQueue() {
            for (int i = 0; i < WORD_QUEUE_LENGTH; i++) {
                enqueueRandomWord();
            }
        }

        public bool LoadWordList() {
            FileStream file = getWordListJSON(ActiveWordListName);

            using (StreamReader reader = new StreamReader(file))  {
                string json = reader.ReadToEnd();

                WordsList = JsonSerializer.Deserialize<WordList>(json);
            }

            wordQueue = new Queue<string>();

            populateWordQueue();

            return true;
        }
    }
}