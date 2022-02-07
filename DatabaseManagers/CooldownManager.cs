using System.Collections.Generic;
using RFSpawnCommand.Models;

namespace RFSpawnCommand.DatabaseManagers
{
    internal class CooldownManager
    {
        internal List<Cooldown> Collection { get; set; } = new List<Cooldown>();

        private static readonly string Json_FileName = "_cooldown.json";
        private DataStore<List<Cooldown>> Json_DataStore { get; }

        internal CooldownManager()
        {
            Json_DataStore =
                new DataStore<List<Cooldown>>(Plugin.Inst.Directory, Json_FileName);
            JSON_Reload();
        }

        private void JSON_Reload()
        {
            Collection = Json_DataStore.Load();
            if (Collection != null)
                return;
            Collection = new List<Cooldown>();
            Json_DataStore.Save(Collection);
        }

        internal void Add(Cooldown cooldown)
        {
            var index = Collection.FindIndex(x => x.SteamId == cooldown.SteamId);
            if (index != -1)
                return;
            Collection.Add(cooldown);
            Json_DataStore.Save(Collection);
        }

        internal Cooldown GetInternal(ulong steamId)
        {
            return Collection.Find(x => x.SteamId == steamId);
        }

        internal void Update(ulong steamId)
        {
            var index = Collection.FindIndex(x => x.SteamId == steamId);
            if (index == -1)
                return;
            Json_DataStore.Save(Collection);
        }
    }
}