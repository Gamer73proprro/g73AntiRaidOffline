using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiRaidOffline.Utilities.Events
{
    public class OnPlayerDisconect
    {
        private static Database.DataBaseSystem _database = new Database.DataBaseSystem();
        public static void OnPlayerDisconnected(UnturnedPlayer player)
        {
            List<Database.DataBaseSystem.UserData> Users = _database.GetUsers();

            Database.DataBaseSystem.UserData user = Users.FirstOrDefault(u => u.SteamId == player.CSteamID.m_SteamID);

            user.TiempoDeDesconeccion = DateTime.Now.ToString();
            user.IsOn = false;

            _database.SaveUsers(Users);
        }


    }
}
