using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using MEC;

namespace Votekick
{
    public class Votekick : Plugin<Config>
    {
        public static Votekick Instance;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        public override Version RequiredExiledVersion { get; } = new Version(2, 10, 0);
        public override Version Version { get; } = new Version(1, 0, 2);

        public KickPoll ActiveKickPoll = null;

        public Votekick()
        { }

        public override void OnEnabled()
        {
            Instance = this;
        }

        public override void OnDisabled()
        {
            if (!(ActiveKickPoll is null)) { Timing.KillCoroutines(ActiveKickPoll.ActiveCoro); }
        }
    }

    public class KickPoll
    {
        public string Reason;
        public Player Target;
        public int[] Votes;
        public List<Player> AlreadyVoted;
        public CoroutineHandle ActiveCoro;
        private readonly ushort BroadcastTime;
        private readonly int KickPollDuration;

        public KickPoll(string reason, Player target)
        {
            BroadcastTime = Votekick.Instance.Config.VotekickTextDuration;
            KickPollDuration = Votekick.Instance.Config.VotekickDuration;

            Reason = reason;
            Target = target;
            Votes = new int[2] { 0, 0 };
            AlreadyVoted = new List<Player>();

            BroadcastToAllPlayers(BroadcastTime, $"Votekick: {Target.Nickname} for {Reason}\nType \".votekick yes\" or \".votekick no\" in the console to vote!");
            EndKickPoll(KickPollDuration);
        }

        private void EndKickPoll(int delay)
        {
            ActiveCoro = Timing.CallDelayed(delay, () =>
            {
                switch (Votes[0] > Votes[1])
                {
                    case true:
                        Target.Kick($"Votekick: {Reason}");
                        BroadcastToAllPlayers(BroadcastTime, $"{Target.Nickname} was votekicked for {Reason}!");
                        break;

                    case false:
                        BroadcastToAllPlayers(BroadcastTime, $"{Target.Nickname} was not votekicked!");
                        break;
                }

                Votekick.Instance.ActiveKickPoll = null;
            });
        }

        private void BroadcastToAllPlayers(ushort time, string message)
        {
            foreach (var player in Player.List)
            {
                player.ClearBroadcasts();
                player.Broadcast(time, message);
            }
        }
    }
}