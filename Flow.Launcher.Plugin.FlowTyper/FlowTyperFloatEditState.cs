
using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Typer;
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
            _context.API.SaveSettingJsonStorage<TypingConfig>();
            _typingManager.PopulateWordQueue(); // Populate the word queue after any change
        }

        private float getParam(FloatField f) {
            switch (f) {
                case FloatField.CAPITALIZE_RATE:
                    return _config.CapitalizeRate;
                case FloatField.PUNCTUATION_RATE:
                    return _config.PunctuationRate;
                case FloatField.NUMBERS_RATE:
                    return _config.NumbersRate;
                default:
                    // TODO: This is probably bad
                    return -1;
            }
        }

        FloatField currFloatField = FloatField.NONE;
        private List<Result> HandleFloatEditQuery(Query query) {
            Tuple<string, string> translationKey = floatFieldTranslationKeys[currFloatField];

            List<Result> results = new List<Result>() {
                new TyperResult() {
                    Title = $"{_context.API.GetTranslation(translationKey.Item1)} {getParam(currFloatField)}",
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