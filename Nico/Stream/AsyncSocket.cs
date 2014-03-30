using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

/*
 * TODO イベント呼び出し時，受信バッファ(byte[])をそのまま渡すと
 *        参照のみが渡る可能性がある(というか未検証な)ので，
 *         データを複製した上で渡すこと
 *       受信バッファを直接用いるStreamBufferクラスは
 *        受信長にあわせてデータコピーするようになっているが，
 *         そうした相手先の実装を頼るのはもちろんダメ
 */

namespace Client.Stream
{
    public class ByteArrayReadEventArgs : EventArgs
    {
        /// <summary>
        /// 受信データ
        /// </summary>
        public byte[] readData;
        /// <summary>
        /// 受信データ長
        /// </summary>
        public int readBytes;
    }

    /// <summary>
    /// 対ピア/サーバソケット．
    /// 非同期ソケットを簡単に扱えるクラス．
    /// </summary>
    public class AsyncSocket
    {
        protected const int TimeOutMS = 2000;
        protected ManualResetEvent connectDone = new ManualResetEvent(false);

        protected const int BufferSize = 1024;
        protected byte[] buffer = new byte[BufferSize];
        
        protected Socket mySocket;

        // public delegate void ReadEventHandler(object sender, ReadEventArgs e);
        /// <summary>
        /// データ受信イベント．ReadEventArgsオブジェクトにデータが格納されています．
        /// </summary>
        public event EventHandler<ByteArrayReadEventArgs> Read = delegate(object s, ByteArrayReadEventArgs e) { };
        public event EventHandler Closed = delegate(object s, EventArgs e) { };

        public AsyncSocket()
        {
            mySocket =new Socket(AddressFamily.InterNetwork, 
                              SocketType.Stream, ProtocolType.Tcp );
        }

        // 受信を開始します．
        private void Start()
        {
            if (null == mySocket)
                return;

            mySocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None,
                new AsyncCallback(ReadCallback), mySocket);
        }

        /// <summary>
        /// TCP接続を行います．
        /// </summary>
        /// <param name="ipAddress">IPアドレス</param>
        /// <param name="port">ポート</param>
        /// <returns>true: 成功, false: 失敗</returns>
        public bool Connect(string ipAddress, int port)
        {
            if (null == mySocket)
                return false;

            connectDone.Reset();
            IAsyncResult ar = mySocket.BeginConnect(ipAddress, port, new AsyncCallback(ConnectCallback), mySocket);

            Trace.Logger.Write("  try " + ipAddress + ":" + port.ToString(), Client.Trace.Logger.LogLevel.L7_DEBUG);

            if (connectDone.WaitOne(TimeOutMS, false))
            {
                // 成功
                try
                {
                    mySocket.EndConnect(ar);
                    Start();
                    Trace.Logger.Write("  ok.", Client.Trace.Logger.LogLevel.L7_DEBUG);
                    return true;
                }
                catch (Exception e)
                {
                    Close();
                    Trace.Logger.Write("  ng.", Client.Trace.Logger.LogLevel.L7_DEBUG);
                    return false;
                }
            }
            else
            {
                // 失敗
                //   Close=>nullのConnectCallback=>false返し
                Close();
                try
                {
                    mySocket.EndConnect(ar);
                }
                catch (Exception e)
                {

                }
                Trace.Logger.Write("  ng.", Client.Trace.Logger.LogLevel.L7_DEBUG);
                return false;
            }

        }

        /// <summary>
        /// 接続済みソケットを参照します．
        /// </summary>
        /// <param name="referenceSocket"></param>
        /// <returns></returns>
        public bool Set(object referenceSocket)
        {
            if (null == mySocket)
                return false;

            if (referenceSocket is Socket)
            {
                mySocket = (Socket)referenceSocket;
                Start();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// データを非同期で送信します．戻り値は，送信処理が開始できたかどうかです．
        /// </summary>
        /// <param name="sendData">送信するデータ</param>
        /// <param name="sendBytes">データ長</param>
        /// <returns></returns>
        public bool Send(byte[] sendData, int sendBytes)
        {
            try
            {
                mySocket.BeginSend(sendData, 0, sendBytes, SocketFlags.None, new AsyncCallback(SendCallback), mySocket);
                return true;
            }
            catch (Exception es)
            {
                Close();
                Closed(this, EventArgs.Empty);
                return false;
            }
        }


        public IPEndPoint GetRemoteEndPoint()
        {
            try
            {
                return (IPEndPoint)mySocket.RemoteEndPoint;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 切断し，リソースを解放します．このオブジェクトは再利用できません．
        /// </summary>
        public bool Close()
        {
            try
            {
                // FIN(0バイトのRead)に対するCloseの場合，Shutdownは不要
                mySocket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            try
            {
                mySocket.Close();
            }
            catch { }
            
            mySocket = null;
            return true;
        }

        // 接続コールバック
        private void ConnectCallback(IAsyncResult ar)
        {
            Socket connectSocket = (Socket)ar.AsyncState;

            if (null != connectSocket)
                connectDone.Set();
        }

        // 受信コールバック
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                Socket receiveSocket = (Socket)ar.AsyncState;
                int bytesRead = receiveSocket.EndReceive(ar);

                // 受信したときは，イベントを呼び出して続行する
                if (bytesRead > 0)
                {
                    ByteArrayReadEventArgs baReadEventArgs = new ByteArrayReadEventArgs();
                    baReadEventArgs.readData = buffer;
                    baReadEventArgs.readBytes = bytesRead;
                    Read(this, baReadEventArgs);
                    mySocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None,
                        new AsyncCallback(ReadCallback), mySocket);
                }
                // エラーが発生したときは，ソケットを閉じて切断通知する
                else
                {
                    Close();
                    Closed(this, EventArgs.Empty);
                }
            }
            catch { }
        }

        // 送信コールバック
        private void SendCallback(IAsyncResult ar)
        {
            Socket sendSocket = (Socket)ar.AsyncState;
            try
            {
                int bytesSent = sendSocket.EndReceive(ar);
                if (bytesSent <= 0)
                    throw new ApplicationException("送信エラーです．");
            }
            catch
            {
                Close();
                Closed(this, EventArgs.Empty);
            }
        }
    }
}
