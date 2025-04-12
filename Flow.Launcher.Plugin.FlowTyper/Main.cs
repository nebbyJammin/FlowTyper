using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.FlowTyper.Typer;

namespace Flow.Launcher.Plugin.FlowTyper
{
    public class FlowTyper : IPlugin
    {
        public enum FlowTyperState {
            MAIN,           // Main Execution
            TYPING,
            SETTINGS
        }
        private FlowTyperState state = FlowTyperState.MAIN;
        private PluginInitContext _context;
        private TypingManager _typingManager;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _typingManager = new TypingManager();
        }

        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();

            switch (state) {
                case FlowTyperState.MAIN:
                    results = HandleMainQuery(query); 
                    break;
                case FlowTyperState.TYPING:
                    break;
                case FlowTyperState.SETTINGS:
                    break;
            }   

            return results;
        }

        public List<Result> HandleMainQuery(Query query) {
            List<Result> results = new List<Result>();

            if (query.IsReQuery) {
                Result r = new Result();
                r.Title = "This is a requery!!";
                results.Add(r);

                return results;
            }
            WordList wordlist = _typingManager.WordsList;            
            string[] words = wordlist.words;
            string[] wordsLists = _typingManager.listOfWordsLists.wordlists;

            foreach (string wordList in wordsLists) {
                Result r = new Result();
                r.Title = wordList;
                results.Add(r);

                r.Action = (ActionContext context) => {
                    _context.API.ChangeQuery("ft english");
                    _context.API.ReQuery();
                    return false;
                };
            }

            Result temp = new Result();
            temp.Title = " --- ";
            results.Add(temp);

            foreach (string word in words) {
                Result r = new Result();
                r.Title = word;
                results.Add(r);
            }

            return results;
        }
    }
}