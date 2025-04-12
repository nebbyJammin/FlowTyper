using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Flow.Launcher.Plugin;
using Microsoft.VisualBasic;

namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public class TypingManager {
        private const string WORDLIST_PATH = "static/wordlists/";
        private const string LIST_OF_WORDLISTS_PATH = WORDLIST_PATH + "_wordlists.json";
        public WordList WordsList {get; private set;}
        public WordLists listOfWordsLists {get; init; }
        public TypingManager() {
            FileInfo file = new FileInfo(LIST_OF_WORDLISTS_PATH);
            using (StreamReader reader = new StreamReader(file.FullName)) {
                string Json = reader.ReadToEnd();

                listOfWordsLists = JsonSerializer.Deserialize<WordLists>(Json); 
            }

            LoadWordList();
        }

        public bool LoadWordList() {
            FileInfo file = new FileInfo(WORDLIST_PATH + "english.json");

            using (StreamReader reader = new StreamReader(file.FullName))  {
                string json = reader.ReadToEnd();

                // TODO: We may consider changing this to asynchronous...
                WordsList = JsonSerializer.Deserialize<WordList>(json);
            }

            return true;
        }
    }
}