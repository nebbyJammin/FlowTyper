using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
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
            if (searchTerms.Length > 0 && _config.ShowIncorrectCharacters) {
                string nextWordLeftOver = _typingManager.NextWord.Substring(Math.Min(searchTerms[0].Length, _typingManager.NextWord.Length)) + " ";

                typingTest.Title = new string(' ', TEST_WHITESPACE_PADDING)
                                + searchTerms[0] + nextWordLeftOver
                                + _typingManager.CurrentWordsExceptFirstString;
            }
            else {
                typingTest.Title = new string(' ', TEST_WHITESPACE_PADDING)
                                + _typingManager.CurrentWordsString;
            }
            typingTest.SubTitle = _context.API.GetTranslation("flowTyperTypingExitTestMode");
             
            typingTest.Action = (ActionContext context) =>
            {
                state = FlowTyperState.MAIN;
                _typingManager.EndTest();
                _typingManager.PopulateWordQueue();
                ResetQuery(query);

                return false;
            };

            results.Add(new TyperResult() {
                Title = $"{_context.API.GetTranslation("flowTyperTypingResetTestTitle")}",
                SubTitle = $"{_context.API.GetTranslation("flowTyperTypingResetTestTitle")}",
                Score = int.MinValue,
                Action = (ActionContext _) => {
                    _typingManager.EndTest();
                    _typingManager.PopulateWordQueue();
                    ResetQuery(query, whitespace: TEST_WHITESPACE_PADDING + 1);

                    return false;
                }
            });

            results.Add(typingTest);

            Result wpm = new TyperResult() {
                Title = $"{_context.API.GetTranslation("flowTyperTypingWPM")}: {_typingManager.WPM:0.}",
                SubTitle = $"{_context.API.GetTranslation("flowTyperTypingRawWPM")}: {_typingManager.RawWPM:0.}",
                Score = int.MinValue,
            };
            results.Add(wpm);

            Result accuracy = new TyperResult() {
                Title = $"{_context.API.GetTranslation("flowTyperTypingAccuracy")}: {_typingManager.Accuracy:0.##}%",
                Score = int.MinValue,
            };
            results.Add(accuracy);

            Result charsTyped = new TyperResult() {
                Title = $"{_context.API.GetTranslation("flowTyperTypingCharactersTyped")}: {_typingManager.CorrectCharacters}/{_typingManager.TotalCharacters}",
                Score = int.MinValue,
            };
            results.Add(charsTyped);

            Result wordsTyped = new TyperResult() {
                Title = $"{_context.API.GetTranslation("flowTyperTypingWordsTyped")}: {_typingManager.WordsTyped} ({_typingManager.TotalCharacters / 5f})",
                Score = int.MinValue,
            };
            results.Add(wordsTyped);
            
            int testTime = _typingManager.TestStart != DateTimeOffset.UnixEpoch ? (int)(DateTime.Now - _typingManager.TestStart).TotalSeconds : 0;
            Result testLength = new TyperResult() {
                Title = $"{_context.API.GetTranslation("flowTyperTypingTestTime")}: {testTime}",
                Score = int.MinValue,
            };
            results.Add(testLength);

            previousTypingQuery = searchTerms.Length >= 1 ? searchTerms[searchTerms.Length - 1] : "";

            return results;
        }

    }
}