using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using SpawnCommandRestriction.Models;

namespace SpawnCommandRestriction.Commands
{
    public class SpawnItemCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "spawnitem";
        public string Help => "Gives yourself an item";
        public string Syntax => "<id> [amount]";
        public List<string> Aliases => new List<string> { "si" };
        public List<string> Permissions => new List<string> { "spawnitem" };
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, Main.Inst.Translate("command_generic_invalid_parameter"), Main.MsgColor);
                throw new WrongUsageOfCommandException(caller, this);
            }
            
            var player = (UnturnedPlayer)caller;

            byte amount = 1;

            var itemString = command[0];

            if (!ushort.TryParse(itemString, out var id))
            {
                var sortedAssets = new List<ItemAsset>(Assets.find(EAssetType.ITEM).Cast<ItemAsset>());
                var asset = sortedAssets.Where(i => i.itemName != null).OrderBy(i => i.itemName.Length).FirstOrDefault(i => i.itemName.ToLower().Contains(itemString.ToLower()));
                if (asset != null) id = asset.id;
                if (string.IsNullOrEmpty(itemString.Trim()) || id == 0)
                {
                    UnturnedChat.Say(player, Main.Inst.Translate("command_generic_invalid_parameter"), Main.MsgColor);
                    throw new WrongUsageOfCommandException(caller, this);
                }
            }

            var a = Assets.find(EAssetType.ITEM, id);

            if (command.Length == 2 && !byte.TryParse(command[1], out amount) || a == null)
            {
                UnturnedChat.Say(player, Main.Inst.Translate("command_generic_invalid_parameter"), Main.MsgColor);
                throw new WrongUsageOfCommandException(caller, this);
            }

            var assetName = ((ItemAsset)a).itemName;

            if (!player.HasPermission(Main.Inst.Configuration.Instance.ItemLimitBypassPermission))
            {
                var limit = Main.Inst.Configuration.Instance.Restrictions[0].ItemAmountLimit;
                foreach (var g in Main.Inst.Configuration.Instance.Restrictions.Where(g => Main.Conf.UseGroupInsteadOfPermission ? Main.PlayerHasGroup(caller, g) : Main.PlayerHasPermission(caller, g)).Where(g => g.ItemAmountLimit > limit))
                {
                    limit = g.ItemAmountLimit;
                }
                if (amount > limit)
                {
                    UnturnedChat.Say(player, Main.Inst.Translate("command_i_too_much", limit), Main.MsgColor);
                    return;
                }
            }

            if (Main.Inst.Configuration.Instance.Restrictions.Any(g => Main.Conf.UseGroupInsteadOfPermission ? Main.PlayerHasGroup(caller, g) : Main.PlayerHasPermission(caller, g)
                                                                    && g.BlackListItems.Any(i => i.ID == id)) && !player.HasPermission(Main.Inst.Configuration.Instance.ItemBlacklistBypassPermission))
            {
                UnturnedChat.Say(player, Main.Inst.Translate("command_i_blacklisted"), Main.MsgColor);
                return;
            }

            if (!player.HasPermission(Main.Conf.ItemCooldownBypassPermission) && Main.ItemCooldowns.TryGetValue(((UnturnedPlayer)caller).CSteamID, out var lastUse))
            {
                var cooldown = Main.Conf.Restrictions[0].ItemCooldown;
                foreach (var g in Main.Conf.Restrictions.Where(g => Main.Conf.UseGroupInsteadOfPermission ? Main.PlayerHasGroup(caller, g) : Main.PlayerHasPermission(caller, g)).Where(g => g.ItemCooldown < cooldown))
                {
                    cooldown = g.ItemCooldown;
                }
                var secondsElapsed = (DateTime.Now - lastUse).TotalSeconds;
                var timeLeft = Math.Round(cooldown - secondsElapsed);
                if (secondsElapsed < cooldown)
                {
                    UnturnedChat.Say(caller, Main.Inst.Translate("command_cooldown", timeLeft), Main.MsgColor);
                    return;
                }
            }

            var item = new Item(id, true);
            if (GiveItem(player, item, amount, true))
            {
                Logger.Log(Main.Inst.Translate("command_i_giving_console", player.DisplayName, id, amount));
                UnturnedChat.Say(player, Main.Inst.Translate("command_i_giving_private", amount, assetName, id), Main.MsgColor);
                Main.ItemCooldowns[((UnturnedPlayer)caller).CSteamID] = DateTime.Now;
            }
            else
            {
                UnturnedChat.Say(player, Main.Inst.Translate("command_i_giving_failed_private", amount, assetName, id), Main.MsgColor);
            }
        }

        private static bool GiveItem(UnturnedPlayer player, Item item, ushort amount, bool dropIfInventoryIsFull = false) {
            var added = false;

            for (var i = 0; i < amount; i++) {
                var clone = new Item(item.id, item.amount, item.durability, item.metadata);

                added = player.Inventory.tryAddItem(clone, true);

                if (!added && dropIfInventoryIsFull) {
                    ItemManager.dropItem(clone, player.Position, true, Dedicator.isDedicated, true);
                }
            }

            return added;
        }
    }
}
