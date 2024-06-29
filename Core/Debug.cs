using log4net;
using System;

namespace Coralite.Core
{
    public static class DEBUGHelper
    {
        public static void LogFancy(string prefix, Exception e)
        {
            LogFancy(prefix, null, e);
        }

        public static void LogFancy(string prefix, string logText, Exception e = null)
        {
            ILog logger = LogManager.GetLogger("Terraria");
            if (e != null)
            {
                logger.Info(">---------<");
                logger.Error(prefix + e.Message);
                logger.Error(e.StackTrace);
                logger.Info(">---------<");
                //ErrorLogger.Log(prefix + e.Message); ErrorLogger.Log(e.StackTrace);	ErrorLogger.Log(">---------<");	
            }
            else
            {
                logger.Info(">---------<");
                logger.Info(prefix + logText);
                logger.Info(">---------<");
                //ErrorLogger.Log(prefix + logText);
            }
        }

    }
}
