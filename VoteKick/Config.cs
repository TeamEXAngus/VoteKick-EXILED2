using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Votekick
{
    public sealed class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled on this server.")]
        public bool IsEnabled { get; set; } = true;

        [Description("The number of seconds which a poll will be displayed on top of players' screens.")]
        public ushort VotekickTextDuration { get; set; } = 5;

        [Description("The number of seconds which polls should last for.")]
        public int VotekickDuration { get; set; } = 30;

        public string VotekickStartedBroadcast { get; set; } = "Votekick: {name} for {reason}\nType \".votekick yes\" or \".votekick no\" in the console to vote!";
        public string VotekickSuccessBroadcast { get; set; } = "{name} was votekicked for {reason}!";
        public string VotekickFailBroadcast { get; set; } = "{name} was not votekicked!";
        public string VotekickKickMessage { get; set; } = "Votekick: {reason}";
    }
}