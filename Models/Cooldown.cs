using System;

namespace RFSpawnCommand.Models
{
    public class Cooldown
    {
        public ulong SteamId { get; set; }
        public DateTime? LastItemCommand { get; set; }
        public DateTime? LastVehicleCommand { get; set; }

        public Cooldown()
        {
            
        }
    }
}