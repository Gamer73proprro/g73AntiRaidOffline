using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiRaidOffline.Utilities.TimeUtilities
{
    public class Funciones
    {
        public static bool? VerificarTiempo(DateTime userDateTime)
        { 
            DateTime serverTimeNow = DateTime.Now;

            if(serverTimeNow.Day > userDateTime.Day)
            {
                return true;
            }

            TimeSpan diferencia = serverTimeNow -  userDateTime;

            if(diferencia.TotalMinutes >= 10)
            {
                return false;
            }
            else if (diferencia.TotalMinutes < 10)
            {
                return true;
            }

            return null;
        }

        public static bool ComprobarBrecha(DateTime userDateTime)
        {
            DateTime serverTimeNow = DateTime.Now;

            if (serverTimeNow.Day > userDateTime.Day)
            {
                return true;
            }

            TimeSpan diferencia = serverTimeNow - userDateTime;

            if (diferencia.TotalMinutes >= 20)
            {
                return false;
            }

            return true;
        }

    }
}
