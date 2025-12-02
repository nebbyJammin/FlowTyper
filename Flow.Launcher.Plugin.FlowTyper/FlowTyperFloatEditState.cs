
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

        // TODO: Handle error state better
        private void updateParam(FloatField f, float val) {
            switch (f) {
                case FloatField.CAPITALIZE_RATE:
                _config.CapitalizeRate = Math.Clamp(val, 0, 1);
                break;
                case FloatField.PUNCTUATION_RATE:
                _config.PunctuationRate = Math.Clamp(val, 0, 1);
                break;
                case FloatField.NUMBERS_RATE:
                _config.NumbersRate = Math.Clamp(val, 0, 1);
                break;
            }
            _config.SaveConfig();
        }
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
                            // Func<float, bool> callback = floatFieldHandlers[currFloatField];
                            // bool success = callback.Invoke(val);

                            updateParam(currFloatField, val);

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