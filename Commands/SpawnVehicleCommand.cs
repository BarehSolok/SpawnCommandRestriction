using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RFSpawnCommand.Commands
{
    public class SpawnVehicleCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "spawnvehicle";
        public string Help => "Gives yourself a vehicle";
        public string Syntax => "<id>";
        public List<string> Aliases => new List<string> { "sv" };
        public List<string> Permissions => new List<string> { "spawnvehicle" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, Plugin.Inst.Translate("command_generic_v_invalid_parameter"), Plugin.MsgColor);
                return;
            }
            
            var player = (UnturnedPlayer)caller;
            var msg = command[0];
            string vehicleName = null;
            if (!ushort.TryParse(msg, out var id))
            {
                var array = Assets.find(EAssetType.VEHICLE);

                var vAsset = array.Cast<VehicleAsset>()
                    .FirstOrDefault(k => k?.vehicleName?.ToLower().Contains(command[0].ToLower()) == true);

                if (vAsset == null)
                {
                    UnturnedChat.Say(player, Plugin.Inst.Translate("command_generic_v_invalid_parameter"), Plugin.MsgColor);
                    return;
                }

                id = vAsset.id;
                vehicleName = vAsset.vehicleName;
            }

            if (Assets.find(EAssetType.VEHICLE, id) == null)
            {
                UnturnedChat.Say(player, Plugin.Inst.Translate("command_generic_v_invalid_parameter"), Plugin.MsgColor);
                return;
            }

            if (vehicleName == null && id != 0)
            {
                vehicleName = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
            }

            if (Plugin.Conf.Restrictions.Any(g => Plugin.Conf.UseGroupInsteadOfPermission ? Plugin.PlayerHasGroup(caller, g) : Plugin.PlayerHasPermission(caller, g) && 
                                                g.BlackListVehicles.Any(i => i.ID == id)) && 
                !player.HasPermission(Plugin.Conf.VehicleBlacklistBypassPermission))
            {
                UnturnedChat.Say(player, Plugin.Inst.Translate("command_v_blacklisted"), Plugin.MsgColor);
                return;
            }

            var playerCooldown = Plugin.Inst.Database.GetInternal(player.CSteamID.m_SteamID);
            if (!player.HasPermission(Plugin.Conf.VehicleCooldownBypassPermission) && playerCooldown.LastVehicleCommand.HasValue)
            {
                var cooldown = Plugin.Conf.Restrictions[0].VehicleCooldown;
                foreach (var g in Plugin.Conf.Restrictions.Where(g => Plugin.Conf.UseGroupInsteadOfPermission ? Plugin.PlayerHasGroup(caller, g) : Plugin.PlayerHasPermission(caller, g)).Where(g => g.ItemCooldown < cooldown))
                {
                    cooldown = g.VehicleCooldown;
                }
                var secondsElapsed = (DateTime.Now - playerCooldown.LastVehicleCommand.Value).TotalSeconds;
                var timeLeft = Math.Round(cooldown - secondsElapsed);
                if (secondsElapsed < cooldown)
                {
                    UnturnedChat.Say(caller, Plugin.Inst.Translate("command_cooldown", timeLeft), Plugin.MsgColor);
                    return;
                }
            }

            if (VehicleTool.giveVehicle(player.Player, id))
            {
                Logger.Log(U.Translate("command_v_giving_console", player.CharacterName, 
                    id.ToString()));
                UnturnedChat.Say(caller, Plugin.Inst.Translate("command_v_giving_private",
                    vehicleName, id.ToString()), Plugin.MsgColor);
                playerCooldown.LastVehicleCommand = DateTime.Now;
                Plugin.Inst.Database.Update(player.CSteamID.m_SteamID);
            }
            else
                UnturnedChat.Say(caller, Plugin.Inst.Translate("command_v_giving_failed_private", 
                    vehicleName, id.ToString()), Plugin.MsgColor);
        }
    }
}
