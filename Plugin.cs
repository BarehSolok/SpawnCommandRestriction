﻿using System;
using System.Linq;
using RFSpawnCommand.DatabaseManagers;
using RFSpawnCommand.Models;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RFSpawnCommand
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Inst;
        public static Configuration Conf;
        public static Color MsgColor;
        internal CooldownManager Database;
        
        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;
            MsgColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.green);
            Database = new CooldownManager();
            
            Logger.LogWarning("[RFSpawnCommand] Plugin loaded successfully!");
            Logger.LogWarning("[RFSpawnCommand] RFSpawnCommand v1.0.2");
            Logger.LogWarning("[RFSpawnCommand] Made with 'rice' by RiceField Plugins!");
        }
        protected override void Unload()
        {
            Inst = null;
            Conf = null; 
            Logger.LogWarning("[RFSpawnCommand] Plugin unload successfully!");
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
