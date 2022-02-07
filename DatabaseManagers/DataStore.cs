using System;
using System.IO;
using Newtonsoft.Json;
using Rocket.Core.Logging;

namespace RFSpawnCommand.DatabaseManagers
{
    internal class DataStore<T> where T : class
    {
        private string DataPath { get; set; }

        internal DataStore(string dir, string fileName)
        {
            DataPath = Path.Combine(dir, fileName);
        }

        internal bool Save(T obj)
        {
            try
            {
                var objData = JsonConvert.SerializeObject(obj, Formatting.Indented);

                using (var stream = new StreamWriter(DataPath, false))
                {
                    stream.Write(objData);
                }

                return true;
            }
            catch (Exception exception)
            {
                Logger.LogError($"[ERROR] JSON Save: {exception}");
                return false;
            }
        }

        internal T Load()
        {
            try
            {
                if (!File.Exists(DataPath))
                    return null;
                string dataText;
                using (var stream = File.OpenText(DataPath))
                {
                    dataText = stream.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<T>(dataText);
            }
            catch (Exception exception)
            {
                Logger.LogError($"[ERROR] JSON Load: {exception}");
                return null;
            }
        }
    }
}