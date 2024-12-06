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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace g73AntiRaidOffline.Utilities.Events
{
    public class StructureDamage
    {
        private static DataBaseSystem _system = new DataBaseSystem();
        public static void OnStructureDamage(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            List<DataBaseSystem.UserData> Users = _system.GetUsers();

            if (structureTransform == null)
            {
                Rocket.Core.Logging.Logger.LogError("structureTransform es null");
                shouldAllow = false;
                return;
            }

            // Obtener la estructura y verificar que no sea null
            var drop = StructureManager.FindStructureByRootTransform(structureTransform);
            if (drop == null)
            {
                Rocket.Core.Logging.Logger.LogError("No se pudo encontrar la estructura");
                shouldAllow = false;
                return;
            }

            // Obtener los datos del servidor y verificar
            var serverData = drop.GetServersideData();
            if (serverData == null)
            {
                Rocket.Core.Logging.Logger.LogError("No se pudieron obtener los datos del servidor");
                shouldAllow = false;
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

                    if(propietarioData.ProteccionActiva)
                    {
                        UnturnedChat.Say(hostil, Main.Instance.Translate("AntiRaidOn", propietarioData.Name), Color.red);
                        shouldAllow = false;
                        return;
                    }

                    if(!DateTime.TryParse(propietarioData.TiempoDeDesconeccion, out DateTime propietarioDesconeccion))
                    {
                        Rocket.Core.Logging.Logger.LogWarning("Peto la convercion de fechas");
                        return;
                    }


                    bool? PermitirDaño = Funciones.VerificarTiempo(propietarioDesconeccion);

                    if(PermitirDaño == null)
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
