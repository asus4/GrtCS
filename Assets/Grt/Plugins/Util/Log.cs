using UnityEngine;

namespace GRT
{
    public class Log
    {
        public static void Info(string msg)
        {
            Debug.Log(msg);
        }

        public static void Warning(string msg)
        {
            Debug.LogWarning(msg);
        }

        public static void Error(string msg)
        {
            Debug.LogError(msg);
        }

    }
}
