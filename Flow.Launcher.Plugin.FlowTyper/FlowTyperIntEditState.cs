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
                // TODO: This function is not currently used by any setting
                // See the implementation for float field
            };
            return results;
        }
    }
}