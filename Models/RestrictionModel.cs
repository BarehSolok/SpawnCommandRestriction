using System.Collections.Generic;
using System.Xml.Serialization;

namespace RFSpawnCommand.Models
{
    public class RestrictionModel
    {
        [XmlAttribute]
        public string ID;
        [XmlAttribute]
        public uint ItemCooldown;
        [XmlAttribute]
        public uint VehicleCooldown;
        [XmlAttribute]
        public ushort ItemAmountLimit;
        [XmlArrayItem(ElementName = "Item")]
        public List<AssetModel> BlackListItems;
        [XmlArrayItem(ElementName = "Vehicle")]
        public List<AssetModel> BlackListVehicles;
        public RestrictionModel()
        {

        }
    }
}