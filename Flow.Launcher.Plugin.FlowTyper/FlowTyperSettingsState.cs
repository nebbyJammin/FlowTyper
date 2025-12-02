using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        private void SetSettingsEditState(Query q, FloatField f) {
            ResetQuery(q);
            currFloatField = f;
            state = FlowTyperState.SETTINGS_FLOAT_EDIT;
        }

        private void SetSettingsEditState(Query q, IntField f) {
            ResetQuery(q);
            currIntField = f;
            state = FlowTyperState.SETTINGS_INT_EDIT;
        }

        private List<Result> HandleSettingsQuery(Query query) {
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
                    string[] wordlists = _typingManager.listOfWordsList.wordlists;
                    for (int i = 0; i < wordlists.Length; i++) {
                        if (searchTerms.Length == 1 || searchTerms.Length == 2 && wordlists[i].StartsWith(searchTerms[1])) {
                            string word = wordlists[i];
                            Result r = new TyperResult() {
                                Title = $"language {wordlists[i]}",
                                SubTitle = $"Current language: {_config.Language}",
                                Action = (ActionContext actionContext) => {
                                    _typingManager.SetActiveWordList(word);
                                    ResetQuery(query, "language ");

                                    return false;
                                },
                            };

                            if (searchTerms.Length == 2 && wordlists[i] == searchTerms[1]) {
                                r.Score = 20000;
                            }

                            results.Add(r);
                        }
                    }
                }
            }
            
            if (!hideGenericSettings) {
                genericSettings.Add(new TyperResult() {
                    Title = $"{_context.API.GetTranslation("flowTyperParamsShowIncorrectCharactersTitle")} ({_config.ShowIncorrectCharacters.ToString().ToLower()})",
                    SubTitle = _context.API.GetTranslation("flowTyperParamsShowIncorrectCharactersSubtitle"),
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
                        SetSettingsEditState(query, FloatField.CAPITALIZE_RATE);
                        return false;
                    }
                });
                #endregion

                #region Punctuation
                genericSettings.Add(new TyperResult() {
                    Title = $"{_context.API.GetTranslation("flowTyperParamsPunctuationTitle")} ({_config.PunctuationRate})",
                    SubTitle = _context.API.GetTranslation("flowTyperParamsPunctuationSubtitle"),
                    Action = (ActionContext _) => {
                        SetSettingsEditState(query, FloatField.PUNCTUATION_RATE);
                        return false;
                    }
                });
                #endregion

                #region Numbers
                genericSettings.Add(new TyperResult() {
                    Title = $"{_context.API.GetTranslation("flowTyperParamsNumbersTitle")} ({_config.NumbersRate})",
                    SubTitle = _context.API.GetTranslation("flowTyperParamsNumbersSubtitle"),
                    Action = (ActionContext _) => {
                        SetSettingsEditState(query, FloatField.NUMBERS_RATE);
                        return false;
                    }
                });
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