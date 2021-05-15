using System.Xml.Serialization;

namespace SpawnCommandRestriction.Models
{
    public class AssetID
    {
        [XmlAttribute]
        public ushort ID;

        public AssetID()
        {
        }
        public AssetID(ushort id)
        {
            ID = id;
        }
    }
}