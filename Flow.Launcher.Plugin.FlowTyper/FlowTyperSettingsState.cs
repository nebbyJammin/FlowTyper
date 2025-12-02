using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        public List<Result> HandleSettingsQuery(Query query) {
            List<Result> results = new List<Result>();
            List<Result> genericSettings = new List<Result>();

            // Add the result to go back to main menu
            Result mainResult = GetMainModeResult(query);
            mainResult.Score = 10000;
            results.Add(mainResult);

            // Settings
            bool hideGenericSettings = false;

            string[] searchTerms = query.SearchTerms;
            // Check for special settings such as language
            if (searchTerms.Length >= 1) {
                if ("language".StartsWith(searchTerms[0])) {
                    hideGenericSettings = true;
                    // Show language options
                    string[] wordlists = _typingManager.listOfWordsLists.wordlists;
                    for (int i = 0; i < wordlists.Length; i++) {
                        if (searchTerms.Length == 1 || wordlists[i].StartsWith(searchTerms[1])) {
                            string word = wordlists[i];
                            results.Add(new TyperResult() {
                                Title = $"language {wordlists[i]}",
                                SubTitle = $"Current language: {_config.Language}",
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
            }
            
            if (!hideGenericSettings) {
                genericSettings.Add(new TyperResult() {
                    Title = $"showIncorrectCharacters ({_config.ShowIncorrectCharacters.ToString().ToLower()})",
                    SubTitle = "When true, mistyped characters are shown over the true character.",
                    Action = (ActionContext _) => {
                        _config.ShowIncorrectCharacters ^= true;
                        saveConfig();
                        ResetQuery(query);

                        return false;
                    }
                });
                genericSettings.Add(new TyperResult() {
                    Title = $"{_context.API.GetTranslation("flowTyperParamsLanguageTitle")} ({_config.Language})",
                    SubTitle = _context.API.GetTranslation("flowTyperParamsLanguageSubtitle"),
                    Action = (ActionContext _) => {
                        ResetQuery(query, "language ");
                        return false;
                    }
                });

                #region Capitalise
                genericSettings.Add(new TyperResult() {
                    Title = $"{_context.API.GetTranslation("flowTyperParamsCapitalizeRateTitle")} ({_config.CapitalizeRate})",
                    SubTitle = _context.API.GetTranslation("flowTyperParamsCapitalizeRateSubtitle"),
                    Action = (ActionContext _) => {
                        ResetQuery(query);
                        changeFloatCallback = floatFieldHandlers[FloatField.CAPITALIZE_RATE];
                        state = FlowTyperState.SETTINGS_FLOAT_EDIT;
                        return false;
                    }
                });
                #endregion

                #region Punctuation
                genericSettings.Add(new TyperResult() {
                    Title = $"{_context.API.GetTranslation("flowTyperParamsPunctuationTitle")} ({_config.PunctuationRate})",
                    SubTitle = "The rate at which punctuation is added to a word.",
                    Action = (ActionContext _) => {
                        // TODO: HOW TO IMPLEMENT? (SEE Capitalise Rate)
                        saveConfig();
                        ResetQuery(query);
                        return false;
                    }
                });
                #endregion

                #region Numbers
                // - NumbersRate
                #endregion

                foreach (Result result in genericSettings) {
                    if (result.Title.StartsWith(query.FirstSearch)) {
                        results.Add(result);
                    }
                }
            }

            // This is used to update the word list. Reset if we are not in typing mode
            previousTypingQuery = "";

            return results;
        }
    }
}