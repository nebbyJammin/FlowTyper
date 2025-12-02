
using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        enum FloatField {
            CAPITALIZE_RATE
        }
        Dictionary<FloatField, Func<float, bool>> floatFieldHandlers = new Dictionary<FloatField, Func<float, bool>>() {
            { FloatField.CAPITALIZE_RATE, (float val) => {
                _config.CapitalizeRate = val;
                saveConfig();
                return true;
            }}
        };
        private Func<float, bool> changeFloatCallback;
        private List<Result> HandleFloatEditQuery(Query query) {
            List<Result> results = new List<Result>() {
                new TyperResult() {
                    Title = $"Test capitalise Rate enter value, current is {_config.CapitalizeRate}",
                    SubTitle = query.Search,
                    Action = (ActionContext _) => {
                        try {
                            string s = query.Search;
                            float val = float.Parse(s);
                            bool success = changeFloatCallback.Invoke(val);
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