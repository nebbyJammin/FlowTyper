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
                Score = int.MinValue,
            };

            results.Add(wpm);

            Result accuracy = new TyperResult() {
                Title = $"Accuracy: {_typingManager.Accuracy:0.##}%",
                Score = int.MinValue,
            };

            results.Add(accuracy);

            previousTypingQuery = searchTerms.Length >= 1 ? searchTerms[searchTerms.Length - 1] : "";

            return results;
        }

    }
}