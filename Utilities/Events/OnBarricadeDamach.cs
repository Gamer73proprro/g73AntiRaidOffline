using AntiRaidOffline;
using AntiRaidOffline.Utilities.Database;
using AntiRaidOffline.Utilities.TimeUtilities;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace g73AntiRaidOffline.Utilities.Events
{
    public class OnBarricadeDamach
    {
        private static DataBaseSystem _system = new DataBaseSystem();

        

        public static void OnBarriadeDamage(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            
            List<DataBaseSystem.UserData> Users = _system.GetUsers();
            DataBaseSystem.BypassID bypassID = _system.GetBypassID();

            var drop = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);



            var serverData = drop.GetServersideData();

            if (serverData == null)
            {
                Rocket.Core.Logging.Logger.LogWarning("Error al obtener los datos de la barricada desde el servidor");
                return;
            }
            var BarricadeID = serverData.barricade.id;

            Rocket.Core.Logging.Logger.LogWarning(BarricadeID.ToString());

            if (bypassID.BypassIDs.Contains(BarricadeID))
            {
                return;
            }

            ulong owner = serverData.owner;

            try
            {
                CSteamID propSteamID = new CSteamID(owner);
                UnturnedPlayer propietario = UnturnedPlayer.FromCSteamID(propSteamID);
                UnturnedPlayer hostil = UnturnedPlayer.FromCSteamID(instigatorSteamID);

                if (propietario == null)
                {
                    Rocket.Core.Logging.Logger.LogWarning("El propietario no está conectado.");
                }

                if (hostil == null)
                {
                    Rocket.Core.Logging.Logger.LogWarning("El jugador hostil no está conectado.");
                    shouldAllow = true;
                    return;
                }


                DataBaseSystem.UserData propietarioData = Users.FirstOrDefault(u => u.SteamId == owner);
                if (propietarioData != null)
                {
                    if (propietarioData.IsOn)
                    {
                        shouldAllow = true;
                        return;
                    }

                    if (propietarioData.BrechaActiva)
                    {
                        DateTime.TryParse(propietarioData.RupturaDeAntiRaid, out DateTime momentoBrecha);
                        bool AunActiva = Funciones.ComprobarBrecha(momentoBrecha);

                        if (AunActiva)
                        {
                            shouldAllow = true;
                        }
                        else
                        {
                            propietarioData.BrechaActiva = false;
                            propietarioData.ProteccionActiva = true;
                            shouldAllow = false;
                            _system.SaveUsers(Users);
                        }

                    }

                    if (propietarioData.ProteccionActiva)
                    {
                        UnturnedChat.Say(hostil, Main.Instance.Translate("AntiRaidOn", propietarioData.Name), Color.red);
                        shouldAllow = false;
                        return;
                    }

                    if (!DateTime.TryParse(propietarioData.TiempoDeDesconeccion, out DateTime propietarioDesconeccion))
                    {
                        Rocket.Core.Logging.Logger.LogWarning("Peto la convercion de fechas");
                        return;
                    }


                    bool? PermitirDaño = Funciones.VerificarTiempo(propietarioDesconeccion);

                    if (PermitirDaño == null)
                    {
                        Rocket.Core.Logging.Logger.LogError("Error critico algo indebido esta sucediendo en la comprobacion horaria");
                        shouldAllow = false;
                        return;
                    }
                    else if (PermitirDaño == true)
                    {
                        if (propietarioData.RupturaDeAntiRaid == null)
                        {
                            propietarioData.RupturaDeAntiRaid = DateTime.Now.ToString();
                            UnturnedChat.Say(hostil, Main.Instance.Translate("BreachActivate", propietarioData.Name));
                        }
                        propietarioData.BrechaActiva = true;
                        shouldAllow = true;
                        _system.SaveUsers(Users);
                        return;
                    }
                    else
                    {
                        propietarioData.ProteccionActiva = true;
                        propietarioData.BrechaActiva = false;
                        propietarioData.RupturaDeAntiRaid = null;
                        shouldAllow = false;
                        _system.SaveUsers(Users);
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogError($"Error al obtener el propietario: {ex.Message}");
            }

            shouldAllow = true;

        }
    }
}
