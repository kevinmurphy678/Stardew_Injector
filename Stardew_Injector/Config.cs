using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Stardew_Injector
{
    public static class Config
    {
        public static bool EnableDebugMode
        {
            get
            {
                bool val = false;
                bool.TryParse(ConfigurationManager.AppSettings["EnableDebugMode"], out val);
                return val;
            }
        }

        public static bool EnableAlwaysSpawnFishingBubble
        {
            get
            {
                bool val = false;
                bool.TryParse(ConfigurationManager.AppSettings["EnableAlwaysSpawnFishingBubble"], out val);
                return val;
            }
        }

        public static bool EnableEasyFishing
        {
            get
            {
                bool val = false;
                bool.TryParse(ConfigurationManager.AppSettings["EnableEasyFishing"], out val);
                return val;
            }
        }

        public static int SecondsPerTenMinutes
        {
            get
            {
                int val = 7;
                int.TryParse(ConfigurationManager.AppSettings["SecondsPerTenMinutes"], out val);
                return val;
            }
        }

        public static float RunSpeed
        {
            get
            {
                float val = 1f;
                float.TryParse(ConfigurationManager.AppSettings["RunSpeed"], out val);
                return val;
            }
        }

        public static bool EnableTweakedDiagonalMovement
        {
            get
            {
                bool val = false;
                bool.TryParse(ConfigurationManager.AppSettings["EnableTweakedDiagonalMovement"], out val);
                return val;
            }
        }

    }
}
