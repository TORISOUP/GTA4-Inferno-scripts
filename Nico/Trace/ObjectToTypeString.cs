using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Trace
{
    /// <summary>
    /// �f�o�b�O�p�N���X
    /// </summary>
    static class ObjectToTypeString
    {
        /// <summary>
        /// �I�u�W�F�N�g�z����̒����ƌ^���o�͂��܂��D
        /// </summary>
        /// <param name="ObjectArray"></param>
        /// <returns></returns>
        public static string Array2Str(object[] ObjectArray)
        {
            string typeString;
            typeString = "(Length: " + ObjectArray.Length;

            foreach (object checkObject in ObjectArray)
            {
                typeString += ", " + checkObject.GetType().ToString();
            }
            typeString += ")";

            return typeString;
        }
    }
}
