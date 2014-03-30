using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Stream
{
    /// <summary>
    /// バッファスプリッタ．
    /// ストリームバッファのデータを，NULLスプリッタによって刻みます．
    /// </summary>
    class BufferSplitter
    {
        NullSplitter nullSplitter = new NullSplitter();

        /// <summary>
        /// ライン受信イベント．
        /// </summary>
        public event EventHandler<ReadEventArgs> LineRead = delegate(object s, ReadEventArgs e) { };

        /// <summary>
        /// StreamBufferからのデータ追加イベント時．
        /// </summary>
        public void OnAdded(object sender, EventArgs e)
        {
            StreamBuffer senderSB = (StreamBuffer)sender;

            byte[] readBuffer = senderSB.Get(-1);
            nullSplitter.Set(readBuffer);

            // CRLFがあるだけライン受信イベントを起こす
            foreach (byte[] oneLine in nullSplitter)
            {
                ReadEventArgs readEA = new ReadEventArgs();
                readEA.oneLine = Encoding.UTF8.GetString(oneLine);

                if (null != readEA.oneLine)
                    LineRead(this, readEA);
            }

            // 読み取ったものは削除
            senderSB.Delete(nullSplitter.getReadBytes());
        }
    }
}
