using EXILED;
using EXILED.Extensions;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace SCPReq
{
    class EventHandlers
    {
        private Plugin Plugin;
        public EventHandlers(Plugin plugin) => Plugin = plugin;
        public class AwaitingPlayers { public static bool awaitingPlayers; };
        internal void OnWaitingForPlayers()
        {
            AwaitingPlayers.awaitingPlayers = true;
            if (CommandHandler.Lists.Cooldown.Count >= 1)
            {
                Timing.RunCoroutine(Cooldown(3f));
            }
        }

        public void OnRoundStart()
        {
            AwaitingPlayers.awaitingPlayers = false;
            if (CommandHandler.Lists.pids.Count >= 1)
            {
                Timing.RunCoroutine(SetRoles(3f));
            }
            else { return; }
        }

        public IEnumerator<float> Cooldown(float delay)
        {
            yield return Timing.WaitForSeconds(delay);
            foreach (string TimesReq in CommandHandler.Lists.Cooldown.Keys)
            {
                if (CommandHandler.Lists.Cooldown.TryGetValue(TimesReq, out int cooldown))
                {
                    if (cooldown <= 1)
                    {
                        if (cooldown == 0) { cooldown++; }
                        cooldown++;
                        CommandHandler.Lists.Cooldown.Remove(TimesReq);
                        CommandHandler.Lists.Cooldown.Add(TimesReq, cooldown); continue; 
                    }
                    else if (cooldown >= 2) {CommandHandler.Lists.Cooldown.Remove(TimesReq); continue; }
                }
                else { continue; }
            }
            if (CommandHandler.Lists.cooldown >= 2) { CommandHandler.Lists.cooldown = 0; Log.Info($"Cooldown reset: {CommandHandler.Lists.cooldown}"); }
        }

        public IEnumerator<float> SetRoles(float delay)
        {
            yield return Timing.WaitForSeconds(delay);
            Log.Info("Setting up roles...");
            yield return Timing.WaitForSeconds(0.5f);

            var playeramount = Player.GetHubs().Count();
            var NaturalScps = Player.GetHubs(Team.SCP);

            System.Random rand = new System.Random();

            List<RoleType> randRoles = new List<RoleType> { RoleType.ClassD, RoleType.ClassD, RoleType.ClassD, RoleType.Scientist };
            List<RoleType> randSCPS = new List<RoleType> { RoleType.Scp049, RoleType.Scp079, RoleType.Scp096, RoleType.Scp173, RoleType.Scp106, RoleType.Scp93953, RoleType.Scp93989 };

            foreach (var scp in NaturalScps)
            {
                if (Plugin.Lists.pids.Contains(scp))
                {
                    CommandHandler.Lists.AlreadySCP.Add(scp.GetUserId());
                    CommandHandler.Lists.existingScps.Add(scp.GetRole());
                    scp.Broadcast(4, "You are already <color=red>SCP</color>, removing request.", false); CommandHandler.Lists.pids.Remove(scp);
                }
                else { CommandHandler.Lists.existingScps.Add(scp.GetRole()); }
            }

            if (playeramount == 1) { Log.Info("Not changing role for naturally spawned SCPs - player count too low."); }
            else if (NaturalScps == null) { Log.Error("NaturalScps is null."); yield break; }
            // If playercount is greater than or equal to cap, and pids list is at max for that cap, and there is naturally spawned SCPs
            else if (playeramount >= 10 && CommandHandler.Lists.pids.Count == 2 && NaturalScps.Count() == 2 || playeramount >= 15 && CommandHandler.Lists.pids.Count == 3 && NaturalScps.Count() == 3 || playeramount == 20 && CommandHandler.Lists.pids.Count == 4 && NaturalScps.Count() == 4)
            {
                foreach (var scp in NaturalScps)
                {
                    if (CommandHandler.Lists.AlreadySCP.Contains(scp.GetUserId())) { continue; }
                    int randRole = rand.Next(randRoles.Count);
                    scp.SetRole(randRoles[randRole]); Log.Info($"{scp.GetNickname()}: {randRoles[randRole]}");
                    scp.Broadcast(4, $"You have been set to {randRoles[randRole]}\n to make room for <color=red>SCP</color> requests.", false);
                }
            }
            // If PID count doesn't hit the player cap, then only do it for one naturally spawned SCP
            else if (playeramount != 1 && CommandHandler.Lists.pids.Count == 1 && NaturalScps.Count() >= 1)
            {
                int randRole = rand.Next(randRoles.Count);
                int randNatSCP = rand.Next(NaturalScps.Count());
                ReferenceHub scp = NaturalScps.ElementAtOrDefault(randNatSCP);
                if (CommandHandler.Lists.AlreadySCP.Contains(scp.GetUserId())) { randNatSCP = rand.Next(NaturalScps.Count()); scp = NaturalScps.ElementAtOrDefault(randNatSCP); }
                scp.SetRole(randRoles[randRole]); Log.Info($"{scp.GetNickname()}: {randRoles[randRole]}");
                scp.Broadcast(4, $"You have been set to {randRoles[randRole]}\n to make room for <color=red>SCP</color> requests.", false);
            }
            // for really every other possibility ... also line 69 haha funny -- edit: fuck.. it's not line 69 anymore :(
            else if (playeramount == 20 && CommandHandler.Lists.pids.Count == 3 && NaturalScps.Count() >= 3 || playeramount >= 15 && CommandHandler.Lists.pids.Count == 2 && NaturalScps.Count() >= 2)
            {
                int xLoops = 0;
                if (CommandHandler.Lists.pids.Count == 3) { xLoops = 3; }
                else if (CommandHandler.Lists.pids.Count == 2) { xLoops = 2; }

                for (int i = 0; i < xLoops; i++)
                {
                    ReferenceHub GoToFuckingBed = NaturalScps.ElementAtOrDefault(i);
                    if (CommandHandler.Lists.AlreadySCP.Contains(GoToFuckingBed.GetUserId())) { xLoops++; i++; GoToFuckingBed = NaturalScps.ElementAtOrDefault(i); }
                    yield return Timing.WaitForSeconds(0.2f);
                    int randRole = rand.Next(randRoles.Count);
                    GoToFuckingBed.SetRole(randRoles[randRole]); Log.Info($"{GoToFuckingBed.GetNickname()}: {randRoles[randRole]}");
                    GoToFuckingBed.Broadcast(4, $"You have been set to {randRoles[randRole]}\n to make room for <color=red>SCP</color> requests.", false);
                }
            }

            if (NaturalScps.Count() == 1 && CommandHandler.Lists.pids.Count == 1)
            {
                randSCPS.RemoveAt(2);
            }
            // For every ID in the PIDS list (Players requested to be SCP)
            foreach (ReferenceHub id in CommandHandler.Lists.pids)
            {
                yield return Timing.WaitForSeconds(1f);
                CommandHandler.Lists.Cooldown.Add(id.GetUserId(), CommandHandler.Lists.cooldown);
                if (id.GetTeam() != Team.SCP)
                {
                    if (id == null) { Log.Error("id is null."); continue; }

                    int randSCP = rand.Next(randSCPS.Count);

                    // if scp already exists then reroll
                    if (CommandHandler.Lists.existingScps.Contains(randSCPS[randSCP]))
                    { 
                        Log.Info($"{randSCPS[randSCP]} already exists... Re-rolling...");
                        randSCPS.RemoveAt(randSCP);
                        randSCP = rand.Next(randSCPS.Count);
                    }
                    id.SetRole(randSCPS[randSCP]);
                    randSCPS.RemoveAt(randSCP);
                    id.Broadcast(3, "Successfully completed request to be <color=red>SCP</color>!", false);
                    Log.Info($"{id.GetNickname()} : {randSCPS[randSCP]}");

                    CommandHandler.Lists.pids.Remove(id);
                    if (!CommandHandler.Lists.pids.Contains(id)) { Log.Info($"Successfully purged {id.GetNickname()} from list. IDs left: {CommandHandler.Lists.pids.Count}"); }
                    else { Log.Error("List did not clear."); }
                }
                else { continue; }
            }
        }

        internal void OnRoundRestart()
        {
            CommandHandler.Lists.pids.Clear();
            CommandHandler.Lists.AlreadySCP.Clear();
            CommandHandler.Lists.existingScps.Clear();
            CommandHandler.Lists.cooldown++;
        }
    }
}
