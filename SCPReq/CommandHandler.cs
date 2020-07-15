using EXILED;
using EXILED.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SCPReq
{
    class CommandHandler
    {
        public Plugin plugin;
        public CommandHandler(Plugin plugin) => this.plugin = plugin;

        public class Lists 
        { 
            public static List<ReferenceHub> pids = new List<ReferenceHub>();
            public static List<string> AlreadySCP = new List<string>();
            public static List<RoleType> existingScps = new List<RoleType>();
            public static Dictionary<string, int> Cooldown = new Dictionary<string, int>(); 
            public static int cooldown = 0; 
        }

        internal void OnRACommand(ref RACommandEvent ev)
        {
            if (string.IsNullOrEmpty(ev.Command)) return;

            string args = ev.Command.ToLower();

            ReferenceHub sender = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? PlayerManager.localPlayer.GetPlayer() : Player.GetPlayer(ev.Sender.SenderId);
            if (sender == null)
            {
                sender = PlayerManager.localPlayer.GetComponent<ReferenceHub>();
            }

            if (args.StartsWith("scpreq") || args.StartsWith("req"))
            {
                ev.Allow = false;
                if (!sender.CheckPermission("SCPReq.req")) { ev.Sender.RAMessage("Permission denied.", false); return; }
                if (EventHandlers.AwaitingPlayers.awaitingPlayers)
                {
                    ReferenceHub id = Player.GetPlayer(sender.GetPlayerId());
                    string timesReq = id.GetUserId();
                    var playeramount = Player.GetHubs().Count();
                    Log.Info($"Player Amount: {playeramount}");
                    if (Lists.pids.Contains(id)) { ev.Sender.RAMessage("You have already requested to be SCP!", false); return; }

                    else if (playeramount <= 9 && Lists.pids.Count() == 1 || playeramount >= 10 && Lists.pids.Count() == 2 || playeramount >= 15 && Lists.pids.Count() == 3 || playeramount == 20 && Lists.pids.Count() == 4)
                    { ev.Sender.RAMessage("Max amount of SCP requests for this player cap!", false); return; }

                    else
                    {
                        if (Lists.Cooldown.ContainsKey(timesReq))
                        {
                            ev.Sender.RAMessage("You requested to be SCP recently, please wait one round to request again.");
                            return;
                        }

                        ev.Sender.RAMessage($"{ev.Sender.Nickname} has requested to be SCP.", true);
                        Lists.pids.Add(id);
                        foreach (ReferenceHub ids in Lists.pids) { Log.Info($"{Lists.pids.Count}: {ids.GetNickname()}"); }
                    }
                }
                else { ev.Sender.RAMessage("You are currently in a round! You must use this command on the |Waiting For Players| screen.", false); return; }
            }
        }

        internal void OnConsoleCommand(ConsoleCommandEvent ev)
        {
            string cmd = ev.Command.ToLower();
            if (cmd.StartsWith("scpreq") || cmd.StartsWith("req"))
            {
                if (!ev.Player.CheckPermission("SCPReq.req")) { ev.ReturnMessage = ("Permission denied."); return; }
                if (EventHandlers.AwaitingPlayers.awaitingPlayers)
                {
                    ReferenceHub id = Player.GetPlayer(ev.Player.GetPlayerId());
                    string timesReq = id.GetUserId();
                    var playeramount = Player.GetHubs().Count();
                    Log.Info($"Player Amount: {playeramount}");
                    if (Lists.pids.Contains(id)) { ev.ReturnMessage = ("You have already requested to be SCP!"); return; }

                    else if (playeramount <= 9 && Lists.pids.Count() == 1 || playeramount >= 10 && Lists.pids.Count() == 2 || playeramount >= 15 && Lists.pids.Count() == 3 || playeramount == 20 && Lists.pids.Count() == 4)
                    { ev.ReturnMessage = ("Max amount of SCP requests for this player cap!"); return; }

                    else
                    {
                        if (Lists.Cooldown.ContainsKey(timesReq))
                        {
                            ev.ReturnMessage = ("You requested to be SCP recently, please wait one round to request again.");
                            return;
                        }
                        ev.ReturnMessage = ($"{ev.Player.GetNickname()} has requested to be SCP.");
                        Lists.pids.Add(id);
                        foreach (ReferenceHub ids in Lists.pids) { Log.Info($"{Lists.pids.Count}: {ids.GetNickname()}"); }
                    }
                }
                else { ev.ReturnMessage = ("You are currently in a round! You must use this command on the |Waiting For Players| screen."); return; }
            }
        }
    }
}
