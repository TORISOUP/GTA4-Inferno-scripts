using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Trace
{
    static class Logger
    {
        /// <summary>
        /// EMERG:システムが使用不能,
        /// ALERT:直ちに対処が必要,
        /// CRIT:致命的な状態,
        /// ERR:エラー状態,
        /// WARNING:警告状態,
        /// NOTICE:通常状態だが大事な状態,
        /// INFO:通知,
        /// DEBUG,デバッグレベルの情報
        /// </summary>
        public enum LogLevel
        {
            L0_EMERG = 0,
            L1_ALERT = 1,
            L2_CRIT = 2,
            L3_ERR = 3,
            L4_WARNING = 4,
            L5_NOTICE = 5,
            L6_INFO = 6,
            L7_DEBUG = 7
        }

        public static void Write(string writeData, LogLevel logLevel)
        {
            // Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " >> " + writeData); // + ((int)logLevel).ToString() + "<>"
        }
    }
}
