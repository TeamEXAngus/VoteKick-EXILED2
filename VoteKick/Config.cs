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
    }
}