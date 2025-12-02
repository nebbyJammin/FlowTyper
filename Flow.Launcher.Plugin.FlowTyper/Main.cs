using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.FlowTyper.Typer;
using Flow.Launcher.Plugin.FlowTyper.Utils;

namespace Flow.Launcher.Plugin.FlowTyper
{
    /// <summary>
    /// The entry point for the Flow Typer Plugin.
    /// </summary>
    public partial class FlowTyper : IPlugin, IPluginI18n
    {
        /// <summary>
        /// Represents the state of the program.
        /// </summary>
        public enum FlowTyperState {
            /// <summary>
            /// The main menu state
            /// </summary>
            MAIN,
            /// <summary>
            /// The state that represents when the program is in a typing test
            /// </summary>
            TYPING,
            /// <summary>
            /// The state that represents when the program is browsing the list of settings
            /// </summary>
            SETTINGS,
            /// <summary>
            /// The state that represents when the program is prompting the user to edit an integer config parameter
            /// </summary>
            SETTINGS_INT_EDIT,
            /// <summary>
            /// The state that represents when the program is prompting the user to edit an float config parameter
            /// </summary>
            SETTINGS_FLOAT_EDIT,
            /// <summary>
            /// The bad, error state
            /// </summary>
            ERROR
        }
        private FlowTyperState state = FlowTyperState.MAIN;
        private PluginInitContext _context;
        private TypingConfig _config {
            get {
                return _typingManager._conf;
            } 
        }
        private TypingManager _typingManager;
        private const int TEST_WHITESPACE_PADDING = 5;
        private string previousTypingQuery = "";
        private Exception exception;

        private void saveConfig() {
            _typingManager._conf.SaveConfig();
        }

        void IPlugin.Init(PluginInitContext context)
        {
            _context = context;
            TypingConfig conf = null;
            try {
                conf = TypingConfig.LoadConfig();
            } catch (Exception e) {
                exception = e;
            }

            if(conf == null) {
                state = FlowTyperState.ERROR;
            }
            else {
                state = FlowTyperState.MAIN;
                _typingManager = new TypingManager(conf);
            }
        }

        List<Result> IPlugin.Query(Query query)
        {
            List<Result> results = new List<Result>();

            switch (state) {
                case FlowTyperState.MAIN:
                    results = HandleMainQuery(query); 
                    break;
                case FlowTyperState.TYPING:
                    results = HandleTypingQuery(query);
                    break;
                case FlowTyperState.SETTINGS:
                    results = HandleSettingsQuery(query);
                    break;
                case FlowTyperState.ERROR:
                    results = HandleErrorQuery(query);
                    break;
                case FlowTyperState.SETTINGS_INT_EDIT:
                    results = HandleIntEditQuery(query);
                    break;
                case FlowTyperState.SETTINGS_FLOAT_EDIT:
                    results = HandleFloatEditQuery(query);
                    break;
            }   

            return results;
        }
        /// <summary>
        /// Provides translated plugin title as per flow launcher API
        /// </summary>
        public string GetTranslatedPluginTitle()
        {
            return _context.API.GetTranslation("flowTyperTitle");
        }

        /// <summary>
        /// Provides translated plugin description as per flow launcher API
        /// </summary>
        public string GetTranslatedPluginDescription()
        {
            return _context.API.GetTranslation("flowTyperDescription");
        }

        private List<Result> HandleErrorQuery(Query query) {
            List<Result> results = new List<Result>() {
                new Result() {
                    Title = "Oops, something went wrong...",
                    SubTitle = "If you think this is a bug please report this!",

                },
                new Result() {
                    Title = "Click me to see the crash report!",
                    Action = (ActionContext actionContext) => {
                        throw exception;
                    },
                }
            };

            return results;
        }

        private void ResetQuery(Query query, string suffix = "", bool requery = true, int whitespace = 1) {
            _context.API.ChangeQuery(query.ActionKeyword + new string(' ', whitespace) + suffix);
            _context.API.ReQuery(requery);
        }

        private Result GetMainModeResult(Query query) {
            Result returnMainResult = new TyperResult();
            returnMainResult.Title = _context.API.GetTranslation("flowTyperReturnToMainTitle");
            returnMainResult.SubTitle = _context.API.GetTranslation("flowTyperReturnToMainSubtitle");
            returnMainResult.Action = (ActionContext context) =>
            {
                state = FlowTyperState.MAIN;
                ResetQuery(query);

                return false;
            };

            return returnMainResult;
        }

    }
}