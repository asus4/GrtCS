using System.Runtime.CompilerServices;

namespace GRT
{
    public class Log
    {

        static string MakeLog(string msg, string file = "", int line = 0, string member = "")
        {
            return $"[{file}:{line} - {member}] {msg}";
        }


        public static void Info(string msg,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            msg = MakeLog(msg, file, line, member);
#if UNITY_2017_1_OR_NEWER
            if (UnityEngine.Debug.isDebugBuild) UnityEngine.Debug.Log(msg);
#else
            System.Console.WriteLine(msg);
#endif
        }

        public static void Warning(string msg,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            msg = MakeLog(msg, file, line, member);
#if UNITY_2017_1_OR_NEWER
            if (UnityEngine.Debug.isDebugBuild) UnityEngine.Debug.LogWarning(msg);
#else
            System.Console.WriteLine(msg);
#endif
        }

        public static void Error(string msg,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            msg = MakeLog(msg, file, line, member);
#if UNITY_2017_1_OR_NEWER
            if (UnityEngine.Debug.isDebugBuild) UnityEngine.Debug.LogError(msg);
#else
            System.Console.WriteLine(msg);
#endif
        }

    }
}
