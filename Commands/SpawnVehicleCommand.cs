using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using SpawnCommandRestriction.Models;

namespace SpawnCommandRestriction.Commands
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
                UnturnedChat.Say(caller, Main.Inst.Translate("command_generic_v_invalid_parameter"), Main.MsgColor);
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
                    UnturnedChat.Say(player, Main.Inst.Translate("command_generic_v_invalid_parameter"), Main.MsgColor);
                    return;
                }

                id = vAsset.id;
                vehicleName = vAsset.vehicleName;
            }

            if (Assets.find(EAssetType.VEHICLE, id) == null)
            {
                UnturnedChat.Say(player, Main.Inst.Translate("command_generic_v_invalid_parameter"), Main.MsgColor);
                return;
            }

            if (vehicleName == null && id != 0)
            {
                vehicleName = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
            }

            if (Main.Conf.Restrictions.Any(g => Main.Conf.UseGroupInsteadOfPermission ? Main.PlayerHasGroup(caller, g) : Main.PlayerHasPermission(caller, g) && 
                                                g.BlackListVehicles.Any(i => i.ID == id)) && 
                !player.HasPermission(Main.Conf.VehicleBlacklistBypassPermission))
            {
                UnturnedChat.Say(player, Main.Inst.Translate("command_v_blacklisted"), Main.MsgColor);
                return;
            }

            if (!player.HasPermission(Main.Conf.VehicleCooldownBypassPermission) && 
                Main.VehicleCooldowns.TryGetValue(((UnturnedPlayer)caller).CSteamID, out var lastUse))
            {
                var cooldown = Main.Conf.Restrictions[0].VehicleCooldown;
                foreach (var g in Main.Conf.Restrictions.Where(g => Main.Conf.UseGroupInsteadOfPermission ? Main.PlayerHasGroup(caller, g) : Main.PlayerHasPermission(caller, g)).Where(g => g.ItemCooldown < cooldown))
                {
                    cooldown = g.VehicleCooldown;
                }
                var secondsElapsed = (DateTime.Now - lastUse).TotalSeconds;
                var timeLeft = Math.Round(cooldown - secondsElapsed);
                if (secondsElapsed < cooldown)
                {
                    UnturnedChat.Say(caller, Main.Inst.Translate("command_cooldown", timeLeft), Main.MsgColor);
                    return;
                }
            }

            if (VehicleTool.giveVehicle(player.Player, id))
            {
                Logger.Log(U.Translate("command_v_giving_console", player.CharacterName, 
                    id.ToString()));
                UnturnedChat.Say(caller, Main.Inst.Translate("command_v_giving_private",
                    vehicleName, id.ToString()), Main.MsgColor);
                Main.VehicleCooldowns[((UnturnedPlayer)caller).CSteamID] = DateTime.Now;
            }
            else
                UnturnedChat.Say(caller, Main.Inst.Translate("command_v_giving_failed_private", 
                    vehicleName, id.ToString()), Main.MsgColor);
        }
    }
}
