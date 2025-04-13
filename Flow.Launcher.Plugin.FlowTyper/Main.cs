using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.FlowTyper.Typer;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.FlowTyper
{
    public class FlowTyper : IPlugin, IPluginI18n
    {
        public enum FlowTyperState {
            MAIN,           // Main Execution
            TYPING,
            SETTINGS
        }
        private FlowTyperState state = FlowTyperState.MAIN;
        private PluginInitContext _context;
        private TypingManager _typingManager;
        private const int TEST_WHITESPACE_PADDING = 5;
        private string previousTypingQuery = "";

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
                    results = HandleTypingQuery(query);
                    break;
                case FlowTyperState.SETTINGS:
                    results = HandleSettingsQuery(query);
                    break;
            }   

            return results;
        }
        public string GetTranslatedPluginTitle()
        {
            return _context.API.GetTranslation("flowTyperTitle");
        }

        public string GetTranslatedPluginDescription()
        {
            return _context.API.GetTranslation("flowTyperDescription");
        }

        private void ResetQuery(Query query, string suffix = "", bool requery = true, int whitespace = 1) {
            _context.API.ChangeQuery(query.ActionKeyword + new string(' ', whitespace) + suffix);
            _context.API.ReQuery(requery);
        }

        private Result GetTestModeResult(Query query) {
            Result startTestResult = new Result();
            startTestResult.Title = _context.API.GetTranslation("flowTyperChangeTestTitle");
            startTestResult.SubTitle = _context.API.GetTranslation("flowTyperChangeTestSubtitle");
            startTestResult.Action = (ActionContext context) =>
            {
                state = FlowTyperState.TYPING;
                ResetQuery(query, whitespace: TEST_WHITESPACE_PADDING + 1);

                return false;
            };

            return startTestResult;
        }

        private Result GetMainModeResult(Query query) {
            Result returnMainResult = new Result();
            returnMainResult.Title = _context.API.GetTranslation("flowTyperReturnToMainTitle");
            returnMainResult.SubTitle = _context.API.GetTranslation("flowTyperReturnToMainSubtitle");
            returnMainResult.Action = (ActionContext context) =>
            {
                state = FlowTyperState.MAIN;
                ResetQuery(query);

                return false;
            };

            return returnMainResult;
        }

        private Result GetSettingsModeResult(Query query) {
            Result settingsResult = new Result();
            settingsResult.Title = _context.API.GetTranslation("flowTyperSettingsTitle");
            settingsResult.SubTitle = _context.API.GetTranslation("flowTyperSettingsSubtitle");
            settingsResult.Action = (ActionContext context) => {
                state = FlowTyperState.SETTINGS;
                ResetQuery(query);

                return false;
            };
        
            return settingsResult;
        }

        public List<Result> HandleMainQuery(Query query) {
            List<Result> results = new List<Result>();      

            Result testMode = GetTestModeResult(query);
            testMode.Score = int.MaxValue;
            if (testMode.Title.StartsWith(query.Search)) results.Add(testMode);
            
            Result settingsMode = GetSettingsModeResult(query);
            settingsMode.Score = int.MinValue;
            if (settingsMode.Title.StartsWith(query.Search)) results.Add(settingsMode);

            // This is used to update the word list. Reset if we are not in typing mode
            previousTypingQuery = "";

            return results;
        }

        public List<Result> HandleTypingQuery(Query query) {
            List<Result> results = new List<Result>();

            // Update last time to type
            _typingManager.UpdateStatistics();
            _typingManager.UpdateLastTimeToType();

            // Checking if the user pressed space
            string[] searchTerms = query.SearchTerms;

            if (previousTypingQuery != "" && searchTerms.Length >= 1 && searchTerms[searchTerms.Length - 1] == previousTypingQuery) {
                _typingManager.confirmWord(searchTerms[0]);
                // TODO: Sometimes, if you type too fast, you can lose some of the searchterms beyond the first.
                ResetQuery(query, whitespace: TEST_WHITESPACE_PADDING + 1);
            }

            Result result = new Result();
            result.Title = new string(' ', TEST_WHITESPACE_PADDING) + _typingManager.CurrentWordsString;
            result.SubTitle = _context.API.GetTranslation("flowTyperExitTestMode");
             
            result.Action = (ActionContext context) =>
            {
                state = FlowTyperState.MAIN;
                _typingManager.EndTest();
                ResetQuery(query);

                return false;
            };

            results.Add(result);

            Result wpm = new Result();
            wpm.Title = "WPM: " + _typingManager.WPM;
            wpm.SubTitle = "Raw WPM: " + _typingManager.RawWPM;
            wpm.Score = -1;
            results.Add(wpm);

            previousTypingQuery = searchTerms.Length >= 1 ? searchTerms[searchTerms.Length - 1] : "";

            return results;
        }

        public List<Result> HandleSettingsQuery(Query query) {
            List<Result> results = new List<Result>();

            Result result = new Result();
            result.Title = "We are in settings mode now!";
            result.Score = int.MaxValue;

            results.Add(result);

            Result mainResult = GetMainModeResult(query);
            mainResult.Score = int.MinValue;
            results.Add(mainResult);

            // This is used to update the word list. Reset if we are not in typing mode
            previousTypingQuery = "";

            return results;
        }

    }
}