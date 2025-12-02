using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper {
    public partial class FlowTyper {
        enum IntField {
            NONE,
        }
        Dictionary<IntField, Func<int, bool>> intFieldHandlers = new Dictionary<IntField, Func<int, bool>>() {
        };
        IntField currIntField = IntField.NONE;
        private List<Result> HandleIntEditQuery(Query query) {
            List<Result> results = new List<Result>() {
                new TyperResult() {
                    Title = $"Test capitalise Rate enter value, current is {_config.CapitalizeRate}",
                    SubTitle = query.Search,
                    Action = (ActionContext _) => {
                        try {
                            if (query.SearchTerms.Length == 0) {
                                ResetQuery(query);
                                state = FlowTyperState.SETTINGS;
                                return false;
                            }

                            string s = query.Search;
                            int val = int.Parse(s);
                            Func<int, bool> callback = intFieldHandlers[currIntField];
                            bool success = callback.Invoke(val);
                            ResetQuery(query);
                            state = FlowTyperState.SETTINGS;
                        }
                        catch (Exception) {
                            
                        }

                        return false;
                    }
                }
            };
            return results;
        }
    }
}