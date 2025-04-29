using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        private Result GetSettingsModeResult(Query query) {
            Result settingsResult = new TyperResult();
            settingsResult.Title = _context.API.GetTranslation("flowTyperSettingsTitle");
            settingsResult.SubTitle = _context.API.GetTranslation("flowTyperSettingsSubtitle");
            settingsResult.Action = (ActionContext context) => {
                state = FlowTyperState.SETTINGS;
                ResetQuery(query);

                return false;
            };
            settingsResult.Score = int.MinValue;
        
            return settingsResult;
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
            startTestResult.Score = 1000;

            return startTestResult;
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
    }
}