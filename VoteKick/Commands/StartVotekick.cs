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

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0) { response = "Incorrect usage! \"votekick start\" or \"votekick yes\" or \"votekick no\""; }

            switch (arguments.At(0).ToLower())
            {
                case "start":
                    if (arguments.Count < 2)
                    {
                        response = "Incorrect usage! \"votekick start <player> [reason]\"";
                        return false;
                    }
                    return Start(arguments, sender, out response);

                case "yes":
                case "no":
                case "y":
                case "n":
                    return Vote(arguments, sender, out response);

                default:
                    response = "Incorrect usage! Valid sub-commands are \"start\", \"yes\", \"no\"";
                    return false;
            }
        }

        public bool Start(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (not(CanStart(sender as CommandSender)))
            {
                response = "You're not allowed to use this command!";
                return false;
            }

            Player TargetPlayer = Player.Get(arguments.At(1));

            if (CannotBeVotekicked(TargetPlayer))
            {
                response = "The player you entered cannot be votekicked or they do not exist!";
                return false;
            }

            string reason = "";

            for (int i = 2; i < arguments.Count; i++)
            {
                reason += arguments.At(i) + " ";
            }

            Votekick.Votekick.Instance.ActiveKickPoll = new Votekick.KickPoll(reason, TargetPlayer);

            response = "Succesfully stared the votekick!";
            return true;
        }

        public bool Vote(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var playerSender = sender as Player;

            if (ActiveVotekick is null) { response = "There is no currently active votekick!"; return false; }
            if (ActiveVotekick.AlreadyVoted.Contains(playerSender)) { response = "You've already voted on this votekick!"; return false; }
            if (not(CanVote(sender as CommandSender))) { response = "You're not allowed to use this command!"; return false; }

            switch (arguments.At(0))
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
            return not(player.CheckPermission("vk.banned")) || player.CheckPermission("vk.master"); //Mainly so players with the *.* permission can run the command
        }

        public bool CanVote(CommandSender player)
        {
            return not(player.CheckPermission("vk.votebanned") || player.CheckPermission("vk.banned")) || player.CheckPermission("vk.master");
        }

        public bool CannotBeVotekicked(Player player)
        {
            return player?.CheckPermission("vk.immune") ?? true;
        }

        public bool not(bool boolean) => !boolean;
    }
}