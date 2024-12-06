using g73AntiRaidOffline.Utilities.Events;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;
using System;

namespace AntiRaidOffline
{
    public class Main : RocketPlugin
    {
        public static Main Instance {  get; private set; }
        protected override void Load()
        {
            Instance = this;
            Console.WriteLine(@"
╔╦╗┌─┐┌┬┐┌─┐  ╔╗ ╦ ╦  ╔═╗┌─┐┌┬┐┌─┐┬─┐┌─┐┬─┐┌─┐┌─┐┬─┐┬─┐┌─┐
║║║├─┤ ││├┤   ╠╩╗╚╦╝  ║ ╦├─┤│││├┤ ├┬┘├─┘├┬┘│ │├─┘├┬┘├┬┘│ │
╩ ╩┴ ┴─┴┘└─┘  ╚═╝ ╩   ╚═╝┴ ┴┴ ┴└─┘┴└─┴  ┴└─└─┘┴  ┴└─┴└─└─┘
", Console.ForegroundColor = ConsoleColor.DarkCyan);


            U.Events.OnPlayerConnected += Utilities.Events.OnPlayerConected.OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Utilities.Events.OnPlayerDisconect.OnPlayerDisconnected;

            StructureManager.onDamageStructureRequested += StructureDamage.OnStructureDamage;
            BarricadeManager.onDamageBarricadeRequested += OnBarricadeDamach.OnBarriadeDamage;
            
            
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Utilities.Events.OnPlayerConected.OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Utilities.Events.OnPlayerDisconect.OnPlayerDisconnected;

            StructureManager.onDamageStructureRequested -= StructureDamage.OnStructureDamage;
            BarricadeManager.onDamageBarricadeRequested -= OnBarricadeDamach.OnBarriadeDamage;

            Rocket.Core.Logging.Logger.LogWarning("Gracias por utilizar el plugin Atte gamer73proprro");
        }


        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "AntiRaidOn", "El jugador {0} tiene Activa su proteccion temporal por 24 horas, intentalo mas tarde!" },
            { "BreachActivate", "¡Wow! detonaste la proteccion AntiRaid de {0} Tienes 20 minutos a contar de ahora para quitarle todo!" }
        };
    }
}
