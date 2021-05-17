using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SpawnCommandRestriction.Models;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace SpawnCommandRestriction
{
    public class Main : RocketPlugin<Configuration>
    {
        public static Main Inst;
        public static Configuration Conf;

        public static Dictionary<CSteamID, DateTime> ItemCooldowns;
        public static Dictionary<CSteamID, DateTime> VehicleCooldowns;
        public static Color MsgColor;
        
        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;

            ItemCooldowns = new Dictionary<CSteamID, DateTime>();
            VehicleCooldowns = new Dictionary<CSteamID, DateTime>();
            MsgColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.green);
            
            Logger.LogWarning("[SpawnCommandRestriction] Plugin loaded successfully!");
            Logger.LogWarning("[SpawnCommandRestriction] SpawnCommandRestriction v1.0.1");
            Logger.LogWarning("[SpawnCommandRestriction] Author: BarehSolok#2548");
            Logger.LogWarning("[SpawnCommandRestriction] Enjoy the plugin! ;)");
        }
        protected override void Unload()
        {
            Inst = null;
            Conf = null;

            ItemCooldowns = null;
            VehicleCooldowns = null;
            
            Logger.LogWarning("[SpawnCommandRestriction] Plugin unload successfully!");
        }
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "command_i_too_much","You have tried to spawn too many items! The limit is {0}." },
            { "command_i_blacklisted","This item is restricted!"},
            { "command_i_giving_console","Giving {0} item {1}:{2}"},
            { "command_i_giving_private","Giving you item {0}x {1} ({2})"},
            { "command_i_giving_failed_private","Failed giving you item {0}x {1} ({2})"},
            { "command_v_blacklisted","This vehicle is restricted!"},
            { "command_v_giving_console","Giving {0} vehicle {1}"},
            { "command_v_giving_private","Giving you vehicle {0} ({1})"},
            { "command_v_giving_failed_private","Failed giving you vehicle {0} ({1})"},
            { "command_cooldown", "You have to wait {0} before you can use this command again" },
            { "command_generic_invalid_parameter", "Invalid Parameter. Usage: /si <item> <amount>." },
            { "command_generic_v_invalid_parameter", "Invalid Parameter. Usage: /sv <vehicle>." },
            { "command_no_permission", "You don't have permission to do this command!" },
        };

        public static bool PlayerHasGroup(IRocketPlayer caller, Restriction restriction)
        {
            var groups = R.Permissions.GetGroups(caller, true).Select(g => g.Id).ToArray();
            for (ushort i = 0; i < groups.Length; i++)
            {
                if (string.Equals(restriction.ID, groups[i], StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }
        public static bool PlayerHasPermission(IRocketPlayer caller, Restriction restriction)
        {
            return caller.GetPermissions().Any(p =>
                string.Equals(p.Name, restriction.ID, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
