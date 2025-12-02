
using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        enum FloatField {
            NONE,
            CAPITALIZE_RATE,
            PUNCTUATION_RATE,
            NUMBERS_RATE,
        }
        Dictionary<FloatField, Tuple<string, string>> floatFieldTranslationKeys = new Dictionary<FloatField, Tuple<string, string>>() {
            {
                FloatField.CAPITALIZE_RATE, 
                new Tuple<string, string>(
                    "flowTyperParamsEditCapitalizeRateTitle",
                    "flowTyperParamsEditCapitalizeRateSubtitle"
                )
            },
            {
                FloatField.PUNCTUATION_RATE,
                new Tuple<string, string>(
                    "flowTyperParamsEditPunctuationRateTitle",
                    "flowTyperParamsEditPunctuationRateSubtitle"
                )
            },
            {
                FloatField.NUMBERS_RATE,
                new Tuple<string, string>(
                    "flowTyperParamsEditNumbersRateTitle",
                    "flowTyperParamsEditNumbersRateSubtitle"
                )
            }
        };
        Dictionary<FloatField, Func<float, bool>> floatFieldHandlers = new Dictionary<FloatField, Func<float, bool>>() {
            { 
                FloatField.CAPITALIZE_RATE, (float val) => {
                    _config.CapitalizeRate = Math.Clamp(val, 0, 1);
                    saveConfig();
                    return true;
                }
            },
            {
                FloatField.PUNCTUATION_RATE, (float val) => {
                    _config.PunctuationRate = Math.Clamp(val, 0, 1);
                    saveConfig();
                    return true;
                }
            },
            {
                FloatField.NUMBERS_RATE, (float val) => {
                    _config.NumbersRate = Math.Clamp(val, 0, 1);
                    saveConfig();
                    return true;
                }
            }
        };
        FloatField currFloatField = FloatField.NONE;
        private List<Result> HandleFloatEditQuery(Query query) {
            Tuple<string, string> translationKey = floatFieldTranslationKeys[currFloatField];

            List<Result> results = new List<Result>() {
                new TyperResult() {
                    Title = $"{_context.API.GetTranslation(translationKey.Item1)} {_config.CapitalizeRate}",
                    SubTitle = query.Search,
                    Action = (ActionContext _) => {
                        try {
                            if (query.SearchTerms.Length == 0) {
                                ResetQuery(query);
                                state = FlowTyperState.SETTINGS;
                                return false;
                            }

                            string s = query.Search;
                            float val = float.Parse(s);
                            // TODO: Handle an error better. Shouldn't be an issue most the time though.
                            Func<float, bool> callback = floatFieldHandlers[currFloatField];
                            bool success = callback.Invoke(val);
                            ResetQuery(query);
                            state = FlowTyperState.SETTINGS;
                        }
                        catch (Exception) {
                            // Silently fail
                        }

                        return false;
                    }
                }
            };
            return results;
        }
    }
}