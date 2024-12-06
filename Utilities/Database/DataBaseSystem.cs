using Newtonsoft;
using Newtonsoft.Json;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;

namespace AntiRaidOffline.Utilities.Database
{
    public class DataBaseSystem
    {
        private readonly string DataBaseFilePath = "/home/container/Servers/unturned/Rocket/Plugins/g73AntiRaidOffline/UsersDataBase.json";
        private readonly string BypassIdsFilePath = "/home/container/Servers/unturned/Rocket/Plugins/g73AntiRaidOffline/BypassIds.json";
        public class UserData
        {
            public string Name { get; set; }
            public ulong SteamId { get; set; }
            public string TiempoDeDesconeccion { get; set; }
            public string RupturaDeAntiRaid {  get; set; }
            public bool BrechaActiva {  get; set; }
            public bool ProteccionActiva { get; set; }
            public bool IsOn {  get; set; }
        }
        public class BypassID
        {
            public List<ushort> BypassIDs { get; set; }

        }
        public BypassID GetBypassID()
        {
            if (!File.Exists(BypassIdsFilePath))
            {
                var initialData = new BypassID();

                initialData.BypassIDs = new List<ushort>()
                {
                    330,  
                    336,  
                    339,  
                    343,  
                    341,  
                    345,  
                    1404, 
                    1105, 
                    1106, 
                    1107,
                    1108,
                    1109,
                    1110
                };

                var json = JsonConvert.SerializeObject(initialData, Formatting.Indented);

                File.WriteAllText(BypassIdsFilePath, json);

                return initialData;
            }


            var Json = File.ReadAllText(BypassIdsFilePath);
            return JsonConvert.DeserializeObject<BypassID>(Json);
        }
        public List<UserData> GetUsers()
        {
            if (!File.Exists(DataBaseFilePath))
            {
                var initialData = new List<UserData>();
                var json = JsonConvert.SerializeObject(initialData, Formatting.Indented);

                File.WriteAllText(DataBaseFilePath, json);

                return initialData;
            }


            var Json = File.ReadAllText(DataBaseFilePath);
            return JsonConvert.DeserializeObject<List<UserData>>(Json);
        }

        public void SaveUsers(List<UserData> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(DataBaseFilePath, json);
        }
    }
}
