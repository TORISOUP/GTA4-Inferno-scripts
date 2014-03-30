using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Trace
{
    static class Logger
    {
        /// <summary>
        /// EMERG:�V�X�e�����g�p�s�\,
        /// ALERT:�����ɑΏ����K�v,
        /// CRIT:�v���I�ȏ��,
        /// ERR:�G���[���,
        /// WARNING:�x�����,
        /// NOTICE:�ʏ��Ԃ����厖�ȏ��,
        /// INFO:�ʒm,
        /// DEBUG,�f�o�b�O���x���̏��
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
