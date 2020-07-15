namespace SCPReq
{
    using EXILED;
    using System.Collections.Generic;

    public class Plugin : EXILED.Plugin
    {
        private EventHandlers EventHandlers;
        private CommandHandler CommandHandler;

        public static bool enabled;

        public override string getName => "SCPReq";

        public override void OnEnable()
        {
            LoadConfig();
            if (!enabled) return;

            Log.Info(message: "SCPReq Loading...");
            EventHandlers = new EventHandlers(this);
            CommandHandler = new CommandHandler(this);
            Events.RemoteAdminCommandEvent += CommandHandler.OnRACommand;
            Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
            Events.RoundStartEvent += EventHandlers.OnRoundStart;
            Events.RoundRestartEvent += EventHandlers.OnRoundRestart;
            Events.ConsoleCommandEvent += CommandHandler.OnConsoleCommand;
        }

        public void LoadConfig()
        {
            enabled = Config.GetBool(key: "SCPReq_enable", def: true);
        }

        public class Lists
        {
            public static List<ReferenceHub> pids = new List<ReferenceHub>();
            public static List<string> AlreadySCP = new List<string>();
            public static Dictionary<string, int> Cooldown = new Dictionary<string, int>();
            public static int cooldown = 0;
        }

        public override void OnDisable()
        {
            Events.RemoteAdminCommandEvent -= CommandHandler.OnRACommand;
            Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
            Events.RoundStartEvent -= EventHandlers.OnRoundStart;
            Events.RoundRestartEvent -= EventHandlers.OnRoundRestart;
            Events.ConsoleCommandEvent -= CommandHandler.OnConsoleCommand;

            EventHandlers = null;
            CommandHandler = null;

        }

        public override void OnReload()
        {

        }
    }
}
