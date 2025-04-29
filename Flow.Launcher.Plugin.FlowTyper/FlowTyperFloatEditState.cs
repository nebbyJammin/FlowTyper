
using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        Dictionary<string, Func<float, bool>> floatFieldHandlers = new Dictionary<string, Func<float, bool>>() {
            { "capitaliseRate", (float val) => {
                _config.CapitaliseRate = val;
                saveConfig();
                return true;
            }}
        };
        private Func<float, bool> changeFloatCallback;
        private List<Result> HandleFloatEditQuery(Query query) {
            List<Result> results = new List<Result>() {
                new TyperResult() {
                    Title = $"Test capitalise Rate enter value, current is {_config.CapitaliseRate}",
                    SubTitle = query.Search,
                    Action = (ActionContext _) => {
                        try {
                            string s = query.Search;
                            float val = float.Parse(s);
                            bool success = changeFloatCallback.Invoke(val);
                            ResetQuery(query);
                            state = FlowTyperState.SETTINGS;
                        }
                        catch (Exception e) {
                            
                        }

                        return false;
                    }
                }
            };
            return results;
        }
    }
}