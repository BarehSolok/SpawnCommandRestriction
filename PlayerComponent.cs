using RFSpawnCommand.Models;
using Rocket.Unturned.Player;

namespace RFSpawnCommand
{
    public class PlayerComponent : UnturnedPlayerComponent
    {
        protected override void Load()
        {
            Plugin.Inst.Database.Add(new Cooldown
            {
                SteamId = Player.CSteamID.m_SteamID
            });
        }

        protected override void Unload()
        {
            base.Unload();
        }
    }
}