using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        Dictionary<string, Func<int, bool>> intFieldHandlers = new Dictionary<string, Func<int, bool>>() {
        };
        private Func<int, bool> changeIntCallback;
        private List<Result> HandleIntEditQuery(Query query) {
            List<Result> results = new List<Result>() {
                new TyperResult() {
                    Title = $"Test capitalise Rate enter value, current is {_config.CapitaliseRate}",
                    SubTitle = query.Search,
                    Action = (ActionContext _) => {
                        try {
                            string s = query.Search;
                            int val = int.Parse(s);
                            bool success = changeIntCallback.Invoke(val);
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