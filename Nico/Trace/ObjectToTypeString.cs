using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Trace
{
    /// <summary>
    /// デバッグ用クラス
    /// </summary>
    static class ObjectToTypeString
    {
        /// <summary>
        /// オブジェクト配列内の長さと型を出力します．
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
