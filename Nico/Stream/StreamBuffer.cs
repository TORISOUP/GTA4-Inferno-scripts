using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Stream
{
    /// <summary>
    /// ストリームバッファ．
    /// バイト配列データをストリームとして保持します．
    /// </summary>
    class StreamBuffer
    {
        List<byte[]> bufferList = new List<byte[]>();
        int bufferReadOffset;

        // バッファ追加
        private void internalAdd(byte[] addData, int addBytes)
        {
            byte[] addPerfectData = new byte[addBytes];
            Array.Copy(addData, 0, addPerfectData, 0, addBytes);

            lock (bufferList)
            {
                bufferList.Add(addPerfectData);
            }
        }

        // バッファ読み出し
        private byte[] internalGet()
        {
            byte[] returnData;

            lock (bufferList)
            {
                returnData = new byte[internalGetSize()];
                int offset = 0, size = 0;

                foreach (byte[] buffer in bufferList)
                {
                    size = buffer.Length;
                    if (offset == 0)
                    {
                        Array.Copy(buffer, bufferReadOffset, returnData, offset, size - bufferReadOffset);
                        offset += (size - bufferReadOffset);
                    }
                    else
                    {
                        Array.Copy(buffer, 0, returnData, offset, size);
                        offset += size;
                    }
                }
            }

            return returnData;
        }

        // バッファ一部削除
        private bool internalDelete(int deleteBytes)
        {
            bufferReadOffset = 0;

            lock (bufferList)
            {
                while (deleteBytes >= 0)
                {
                    if (bufferList.Count == 0)
                        return false;
                    else
                    {
                        // 最古バッファサイズ > 削除サイズ
                        //   最古バッファサイズにオフセットがかかるようにして終わる
                        // 最古バッファサイズ <= 削除サイズ
                        //   最古バッファを削除して，削除サイズを減少させる
                        if (bufferList[0].Length > deleteBytes)
                        {
                            bufferReadOffset = deleteBytes;
                            return true;
                        }
                        else
                        {
                            deleteBytes -= bufferList[0].Length;
                            bufferList.RemoveAt(0);
                        }
                    }
                }
            }
            return true;
        }

        // サイズ計算
        private int internalGetSize()
        {
            int size = 0;
            foreach (byte[] buffer in bufferList)
            {
                size += buffer.Length;
            }
            size -= bufferReadOffset;

            return size;
        }

        /// <summary>
        /// データ追加イベント．
        /// Getメソッドを使って取得してください．
        /// </summary>
        public event EventHandler Added;

        /// <summary>
        /// データを追加します．
        /// </summary>
        /// <param name="addData">追加データ</param>
        /// <param name="addBytes">データ長</param>
        public void Add(byte[] addData, int addBytes)
        {
            internalAdd(addData, addBytes);
            Added(this, EventArgs.Empty);
        }

        /// <summary>
        /// データを取得します．
        /// </summary>
        /// <param name="getBytes">取得バイト数 (-1: すべて)</param>
        /// <returns>データ</returns>
        public byte[] Get(int getBytes)
        {
            return internalGet();
        }

        /// <summary>
        /// データの一部を削除します．
        /// 削除に失敗した場合，データは不完全な状態で残ります．
        /// </summary>
        /// <param name="deleteBytes">削除バイト数</param>
        /// <returns>true: 成功, false: 失敗</returns>
        public bool Delete(int deleteBytes)
        {
            return internalDelete(deleteBytes);
        }

        /// <summary>
        /// 対ピアソケット(AsyncSocket)受信イベントに対するメソッド．
        /// </summary>
        /// <param name="sender">送信者</param>
        /// <param name="e">パラメタ</param>
        public void OnRead(object sender, ByteArrayReadEventArgs e)
        {
            Add(e.readData, e.readBytes);
        }
    }
}
