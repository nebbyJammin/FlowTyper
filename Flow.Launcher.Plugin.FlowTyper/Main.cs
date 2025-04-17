using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.FlowTyper.Typer;
using Flow.Launcher.Plugin.FlowTyper.Utils;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.FlowTyper
{
    /// <summary>
    /// The entry point for the Flow Typer Plugin.
    /// </summary>
    public class FlowTyper : IPlugin, IPluginI18n
    {
        /// <summary>
        /// Represents the state of the program.
        /// </summary>
        public enum FlowTyperState {
            /// <summary>
            /// The main menu state
            /// </summary>
            MAIN,
            /// <summary>
            /// The state that represents when the program is in a typing test
            /// </summary>
            TYPING,
            SETTINGS,
            ERROR
        }
        private FlowTyperState state = FlowTyperState.MAIN;
        private PluginInitContext _context;
        private TypingConfig _config;
        private TypingManager _typingManager;
        private const int TEST_WHITESPACE_PADDING = 5;
        private string previousTypingQuery = "";
        private Exception exception;

        private void saveConfig() {
            if (!Directory.Exists(Constants.CONFIG_DIR)) {
                Directory.CreateDirectory(Constants.CONFIG_DIR);
            }

            string path = Constants.CONFIG_DIR + ".config.json";
            string json = JsonSerializer.Serialize(_config);
            File.WriteAllText(path, json);
        }

        private bool loadConfig() {
            string path = Constants.CONFIG_DIR + ".config.json";
            if (File.Exists(path)) {
                try
                {
                    FileStream configFile = new FileStream(Constants.CONFIG_DIR + ".config.json", FileMode.Open);

                    using (StreamReader reader = new StreamReader(configFile))
                    {
                        string json = reader.ReadToEnd();

                        _config = JsonSerializer.Deserialize<TypingConfig>(json);
                    }

                    return true;
                }
                catch(Exception e)
                {
                    exception = e;
                    return false;
                }

            }
            else {
                // If FileNotFound, create the file
                _config = new();
                saveConfig();

                return true;
            }
        }

        void IPlugin.Init(PluginInitContext context)
        {
            _context = context;
            if(loadConfig()) {
                state = FlowTyperState.MAIN;
            }
            else {
                state = FlowTyperState.ERROR;
            }
          
            _typingManager = new TypingManager(_config.Language);
        }

        List<Result> IPlugin.Query(Query query)
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
                case FlowTyperState.ERROR:
                    results = HandleErrorQuery(query);
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

        private List<Result> HandleErrorQuery(Query query) {
            List<Result> results = new List<Result>() {
                new Result() {
                    Title = "Oops, something went wrong...",
                    SubTitle = "If you think this is a bug please report this!",

                },
                new Result() {
                    Title = "Click me to see the crash report!",
                    Action = (ActionContext actionContext) => {
                        throw exception;
                    },
                }
            };

            return results;
        }

        private void ResetQuery(Query query, string suffix = "", bool requery = true, int whitespace = 1) {
            _context.API.ChangeQuery(query.ActionKeyword + new string(' ', whitespace) + suffix);
            _context.API.ReQuery(requery);
        }

        private Result GetTestModeResult(Query query) {
            Result startTestResult = new TyperResult();
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
            Result returnMainResult = new TyperResult();
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
            Result settingsResult = new TyperResult();
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

            if (previousTypingQuery != "" && searchTerms.Length >= 1 && searchTerms[searchTerms.Length - 1] == previousTypingQuery || searchTerms.Length >= 2) {
                _typingManager.confirmWord(searchTerms[0]);

                string leftOver = "";
                if (searchTerms.Length >= 2) leftOver = searchTerms[1];
                ResetQuery(query, whitespace: TEST_WHITESPACE_PADDING + 1, suffix: leftOver);
            }

            Result typingTest = new TyperResult();
            if (searchTerms.Length > 0 && _config.UseOptimisticWordList) {
                string nextWordLeftOver = _typingManager.NextWord.Substring(Math.Min(searchTerms[0].Length, _typingManager.NextWord.Length)) + " ";

                typingTest.Title = new string(' ', TEST_WHITESPACE_PADDING)
                                + searchTerms[0] + nextWordLeftOver
                                + _typingManager.CurrentWordsExceptFirstString;
            }
            else {
                typingTest.Title = new string(' ', TEST_WHITESPACE_PADDING)
                                + _typingManager.CurrentWordsString;
            }
            typingTest.SubTitle = _context.API.GetTranslation("flowTyperExitTestMode");
             
            typingTest.Action = (ActionContext context) =>
            {
                state = FlowTyperState.MAIN;
                _typingManager.EndTest();
                ResetQuery(query);

                return false;
            };

            results.Add(typingTest);

            Result wpm = new TyperResult() {
                Title = $"WPM: {_typingManager.WPM:0.}",
                SubTitle = $"Raw WPM: {_typingManager.RawWPM:0.}",
                Score = -1,
            };

            results.Add(wpm);

            Result accuracy = new TyperResult() {
                Title = $"Accuracy: {_typingManager.Accuracy:0.##}%",
                Score = -2,
            };

            results.Add(accuracy);

            previousTypingQuery = searchTerms.Length >= 1 ? searchTerms[searchTerms.Length - 1] : "";

            return results;
        }

        public List<Result> HandleSettingsQuery(Query query) {
            List<Result> results = new List<Result>();

            string[] searchTerms = query.SearchTerms;
            if (searchTerms.Length >= 1) {
                if ("language".StartsWith(searchTerms[0])) {
                    // Show language options
                    string[] wordlists = _typingManager.listOfWordsLists.wordlists;
                    for (int i = 0; i < wordlists.Length; i++) {
                        if (searchTerms.Length == 1 || wordlists[i].StartsWith(searchTerms[1])) {
                            String word = wordlists[i];
                            results.Add(new TyperResult() {
                                Title = $"language {wordlists[i]}",
                                SubTitle = $"Current language is {_config.Language}",
                                Action = (ActionContext actionContext) => {
                                    _config.Language = word;
                                    saveConfig();
                                    _typingManager.SetActiveWordList(word);
                                    ResetQuery(query, "language ");

                                    return false;
                                },
                            });
                        }
                    }
                }
                results.Add(new TyperResult()
                {
                    Title = $"useOptimisticWordList (currently {_config.UseOptimisticWordList.ToString().ToLower()})",
                    SubTitle = "When true, mistyped characters are shown over the true character.",
                    Action = (ActionContext context) =>
                    {
                        _config.UseOptimisticWordList ^= true;
                        saveConfig();
                        ResetQuery(query);

                        return false;
                    }
                });
            }
            else {
                results.Add(new TyperResult() {
                    Title = $"useOptimisticWordList (currently {_config.UseOptimisticWordList.ToString().ToLower()})",
                    SubTitle = "When true, mistyped characters are shown over the true character.",
                    Action = (ActionContext context) => {
                        _config.UseOptimisticWordList ^= true;
                        saveConfig();
                        ResetQuery(query);

                        return false;
                    }
                });
                results.Add(new TyperResult() {
                    Title = $"language <language>",
                    SubTitle = "Specify the typing language",
                    Action = (ActionContext context) => {
                        ResetQuery(query, "language ");
                        return false;
                    }
                });
            }
            Result mainResult = GetMainModeResult(query);
            mainResult.Score = int.MinValue;
            results.Add(mainResult);

            // This is used to update the word list. Reset if we are not in typing mode
            previousTypingQuery = "";

            return results;
        }
    }
}