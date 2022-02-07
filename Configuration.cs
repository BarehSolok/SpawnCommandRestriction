using System.Collections.Generic;
using System.Xml.Serialization;
using RFSpawnCommand.Models;
using Rocket.API;

namespace RFSpawnCommand
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string ItemCooldownBypassPermission;
        public string ItemLimitBypassPermission;
        public string ItemBlacklistBypassPermission;
        public string VehicleCooldownBypassPermission;
        public string VehicleBlacklistBypassPermission;
        public string MessageColor;
        public bool UseGroupInsteadOfPermission;
        [XmlArrayItem("Restriction")]
        public List<Restriction> Restrictions;
        public void LoadDefaults()
        {
            ItemCooldownBypassPermission = "itemcooldown.bypass";
            ItemLimitBypassPermission = "itemspawnlimit.bypass";
            ItemBlacklistBypassPermission = "itemblacklist.bypass";
            VehicleCooldownBypassPermission = "vehiclecooldown.bypass";
            VehicleBlacklistBypassPermission = "vehicleblacklist.bypass";
            MessageColor = "white";
            UseGroupInsteadOfPermission = false;
            Restrictions = new List<Restriction>
            {
                new Restriction()
                {
                    ID = "default",
                    ItemCooldown = 300,
                    ItemAmountLimit = 1,
                    BlackListItems = new List<AssetModel> { new AssetModel(1394), new AssetModel(519), new AssetModel(1241) },
                    VehicleCooldown = 300,
                    BlackListVehicles = new List<AssetModel> { new AssetModel(1394), new AssetModel(519), new AssetModel(1241), },
                },
                new Restriction()
                {
                    ID = "vip",
                    ItemCooldown = 0,
                    ItemAmountLimit = 10,
                    BlackListItems = new List<AssetModel> { new AssetModel(1394), new AssetModel(519), new AssetModel(1241) },
                    VehicleCooldown = 0,
                    BlackListVehicles = new List<AssetModel> { new AssetModel(1394), new AssetModel(519), new AssetModel(1241), },
                }
            };
        }
    }
}