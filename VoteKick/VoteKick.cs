using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using MEC;

namespace Votekick
{
    public class Votekick : Plugin<Config>
    {
        private static Votekick singleton = new Votekick();
        public static Votekick Instance => singleton;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        public override Version RequiredExiledVersion { get; } = new Version(2, 10, 0);
        public override Version Version { get; } = new Version(1, 0, 4);

        public KickPoll ActiveKickPoll = null;

        private Votekick()
        { }

        public override void OnEnabled()
        {
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

            BroadcastToAllPlayers(BroadcastTime, Votekick.Instance.Config.VotekickStartedBroadcast.Replace("{name}", Target.Nickname).Replace("{reason}", Reason));
            EndKickPoll(KickPollDuration);
        }

        private void EndKickPoll(int delay)
        {
            ActiveCoro = Timing.CallDelayed(delay, () =>
            {
                switch (Votes[0] > Votes[1])
                {
                    case true:
                        Target.Kick(Votekick.Instance.Config.VotekickKickMessage.Replace("{reason}", Reason));
                        BroadcastToAllPlayers(BroadcastTime, Votekick.Instance.Config.VotekickSuccessBroadcast.Replace("{name}", Target.Nickname).Replace("{reason}", Reason));
                        break;

                    case false:
                        BroadcastToAllPlayers(BroadcastTime, Votekick.Instance.Config.VotekickFailBroadcast.Replace("{name}", Target.Nickname));
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