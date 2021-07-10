using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace Commands.VotekickCommand
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class VotekickCommand : ICommand
    {
        private static Votekick.KickPoll ActiveVotekick => Votekick.Votekick.Instance.ActiveKickPoll;

        public string Command { get; } = "votekick";

        public string[] Aliases { get; } = { "vk" };

        public string Description { get; } = "Used for starting votekicks or voting to kick players.";

        public bool Execute(ArraySegment<string> Arguments, ICommandSender sender, out string response)
        {
            if (Arguments.Count == 0) { response = "Incorrect usage! \"votekick start\" or \"votekick yes\" or \"votekick no\""; return false; }

            switch (Arguments.At(0).ToLower())
            {
                case "start":
                    if (Arguments.Count < 2)
                    {
                        response = "Incorrect usage! \"votekick start <player> [reason]\"";
                        return false;
                    }
                    return Start(Arguments, sender, out response);

                case "yes":
                case "no":
                case "y":
                case "n":
                    return Vote(Arguments, sender, out response);

                default:
                    response = "Incorrect usage! Valid sub-commands are \"start\", \"yes\", \"no\"";
                    return false;
            }
        }

        public bool Start(ArraySegment<string> Arguments, ICommandSender sender, out string response)
        {
            if (!CanStart(sender as CommandSender))
            {
                response = "You're not allowed to use this command!";
                return false;
            }

            if (!(Votekick.Votekick.Instance.ActiveKickPoll is null))
            {
                response = "You cannot start a vote kick whilst one is currently active!";
                return false;
            }

            Player TargetPlayer = Player.Get(Arguments.At(1));

            if (CannotBeVotekicked(TargetPlayer))
            {
                response = "The player you entered cannot be votekicked or they do not exist!";
                return false;
            }

            string reason = ExtractReason(Arguments);

            Votekick.Votekick.Instance.ActiveKickPoll = new Votekick.KickPoll(reason, TargetPlayer);

            response = "Succesfully stared the votekick!";
            return true;
        }

        public bool Vote(ArraySegment<string> Arguments, ICommandSender sender, out string response)
        {
            var playerSender = Player.Get((sender as CommandSender)?.SenderId);

            if (ActiveVotekick is null) { response = "There is no currently active votekick!"; return false; }
            if (ActiveVotekick.AlreadyVoted.Contains(playerSender)) { response = "You've already voted on this votekick!"; return false; }
            if (!CanVote(sender as CommandSender)) { response = "You're not allowed to use this command!"; return false; }

            switch (Arguments.At(0))
            {
                case "yes":
                case "y":
                    ActiveVotekick.Votes[0]++;
                    ActiveVotekick.AlreadyVoted.Add(playerSender);
                    response = "Voted yes!";
                    return true;

                case "no":
                case "n":
                    ActiveVotekick.Votes[1]++;
                    ActiveVotekick.AlreadyVoted.Add(playerSender);
                    response = "Voted no!";
                    return true;
            }

            response = "Something went wrong";
            return false;
        }

        public bool CanStart(CommandSender player)
        {
            bool Banned = player.CheckPermission("vk.banned");

            return (!Banned) || player.CheckPermission("vk.master"); //Mainly so players with the *.* permission can run the command
        }

        public bool CanVote(CommandSender player)
        {
            bool Banned = (player.CheckPermission("vk.votebanned") || player.CheckPermission("vk.banned"));

            return (!Banned) || player.CheckPermission("vk.master");
        }

        public bool CannotBeVotekicked(Player player)
        {
            return player?.CheckPermission("vk.immune") ?? true;
        }

        private string ExtractReason(ArraySegment<string> Arguments)
        {
            if (Arguments.Count >= 3)
            {
                string output = "";

                for (int i = 2; i < Arguments.Count; i++)
                {
                    output += Arguments.At(i) + " ";
                }

                return output;
            }

            return "No reason given";
        }
    }
}