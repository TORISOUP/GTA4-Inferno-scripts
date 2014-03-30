using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Client.Stream
{
    /// <summary>
    /// CRLFスプリッタ．
    /// バイト配列を受けて，順次CRLFまでの切り出しを支援する．
    /// </summary>
    class NullSplitter
    {
        int forwardBytes;
        byte[] byteArray;

        /// <summary>
        /// セット
        /// </summary>
        /// <param name="byteArray">バイト配列</param>
        public void Set(byte[] byteArray)
        {
            forwardBytes = 0;
            this.byteArray = byteArray;
        }

        /// <summary>
        /// foreach的読込サポート
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            byte[] returnArray;
            for (int i = 0; i < byteArray.Length ; i++)
            {
                // CRLFが存在するならば
                if (byteArray[i] == 0)
                {
                    returnArray = new byte[(i - forwardBytes) + 1 - 1];
                    Array.Copy(byteArray, forwardBytes, returnArray, 0, (i - forwardBytes) + 1 - 1);
                    forwardBytes = i + 1;

                    yield return returnArray;
                }
            }
        }

        /// <summary>
        /// 読み取った総バイト数を取得します．
        /// </summary>
        /// <returns>総バイト数</returns>
        public int getReadBytes()
        {
            return forwardBytes;
        }
    }
}
