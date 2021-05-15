using System.Collections.Generic;
using System.Xml.Serialization;

namespace SpawnCommandRestriction.Models
{
    public class Restriction
    {
        [XmlAttribute]
        public string ID;
        [XmlAttribute]
        public ushort ItemCooldown;
        [XmlAttribute]
        public ushort VehicleCooldown;
        [XmlAttribute]
        public ushort ItemAmountLimit;
        [XmlArrayItem(ElementName = "Item")]
        public List<AssetID> BlackListItems;
        [XmlArrayItem(ElementName = "Vehicle")]
        public List<AssetID> BlackListVehicles;
        public Restriction()
        {

        }
    }
}