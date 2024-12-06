using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntiRaidOffline.Utilities.Events
{
    public class OnPlayerConected
    {
        private static Database.DataBaseSystem _database = new Database.DataBaseSystem();
        public static void OnPlayerConnected(UnturnedPlayer player)
        {

            List<Database.DataBaseSystem.UserData> Users = _database.GetUsers();


            bool EstaEnDataBase = Users.Any(u => u.SteamId == player.CSteamID.m_SteamID);


            if (EstaEnDataBase)
            {
                Database.DataBaseSystem.UserData user = Users.FirstOrDefault(u => u.SteamId == player.CSteamID.m_SteamID);

                user.ProteccionActiva = false;
                user.IsOn = true;
                user.TiempoDeDesconeccion = null;
                UnturnedChat.Say(player, "Se reinicio/inicio tu tiempo AntiRaid", Color.magenta);

                _database.SaveUsers(Users);
                return;
            }

            Database.DataBaseSystem.UserData newUser = new Database.DataBaseSystem.UserData
            {
                Name = player.CharacterName,
                SteamId = player.CSteamID.m_SteamID,
                ProteccionActiva = false,
                IsOn = true,
            };

            Users.Add(newUser);

            _database.SaveUsers(Users);
        }





    }
}
