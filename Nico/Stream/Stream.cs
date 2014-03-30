using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client.Stream
{
    // TODO GetStreamHashは"IPアドレス+ポート"をハッシュ値と称する手抜き仕様

    /// <summary>
    /// およそストリームのように操作可能な，TCP/IPセッションを提供します．
    /// 
    /// [注意事項]
    /// DisConnectメソッドによって切断したストリームは，再利用できません．
    /// </summary>
    class Stream
    {
        AsyncSocket controlSocket;
        StreamBuffer linkStreamBuffer;
        BufferSplitter linkBufferSplitter;

        /// <summary>
        /// データを受信しました．データには，EPSPOneLine型データと受信したストリームが入っています．
        /// </summary>
        public event EventHandler<TwoEventArgs> Read = delegate(object s, TwoEventArgs e) { };
        /// <summary>
        /// ストリームが切断されました．データには，切断したストリームが入っています．
        /// </summary>
        public event EventHandler<OneEventArgs> DisConnected = delegate(object s, OneEventArgs e) { };

        public Stream()
        {
            controlSocket = new AsyncSocket();
            linkStreamBuffer = new StreamBuffer();
            linkBufferSplitter = new BufferSplitter();

            controlSocket.Read += linkStreamBuffer.OnRead;
            controlSocket.Closed += OnClose;

            linkStreamBuffer.Added += linkBufferSplitter.OnAdded;
            linkBufferSplitter.LineRead += OnLineRead;
        }

        /// <summary>
        /// 接続します．
        /// </summary>
        /// <param name="remoteInformation">
        /// 接続先情報
        /// （IPEndPointまたは ipAddress string, port int の組み合わせ）
        /// </param>
        /// <returns>true: 成功, false: 失敗</returns>
        public bool Connect(params object[] remoteInformation)
        {
            string ipAddress;
            int port = 0;

            if (remoteInformation.Length == 1 && remoteInformation[0] is IPEndPoint)
            {
                ipAddress = ((IPEndPoint)remoteInformation[0]).Address.ToString();
                port = ((IPEndPoint)remoteInformation[0]).Port;
            }
            else if (remoteInformation.Length == 2 && remoteInformation[0] is string && remoteInformation[1] is int)
            {
                ipAddress = (string)remoteInformation[0];
                port = (int)remoteInformation[1];
            }
            else
            {
                throw new System.ArgumentException("サポートされていない型です．" + Trace.ObjectToTypeString.Array2Str(remoteInformation));
            }

            return controlSocket.Connect(ipAddress, port);
        }

        /// <summary>
        /// 接続済みソケットを参照し，ストリームとして利用します．
        /// </summary>
        /// <param name="referenceSocket">参照ソケット</param>
        /// <returns>true: 成功, false: 失敗</returns>
        public bool Set(params object[] referenceStream)
        {
            if (referenceStream.Length == 1 && referenceStream[0] is Socket)
            {
                return controlSocket.Set((Socket)referenceStream[0]);
            }
            else
            {
                throw new System.ArgumentException("サポートされていない長さまたは型です．" + Trace.ObjectToTypeString.Array2Str(referenceStream));
            }
        }

        /// <summary>
        /// 切断します．切断したストリームは，再利用できません．
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            return controlSocket.Close();
        }

        /// <summary>
        /// データを書き込みます．
        /// </summary>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public bool Write(byte[] writeData)
        {
            return controlSocket.Send(writeData, writeData.Length);
        }

        /// <summary>
        /// 接続先を表すハッシュを取得します．
        /// </summary>
        /// <param name="isPerfect">true: 完全（IPアドレス・ポートの組み合わせ）, false: 部分（IPアドレスのみ）</param>
        /// <returns>ハッシュ値</returns>
        public string GetStreamHash(bool isPerfect)
        {
            IPEndPoint socketRemoteEndPoint = controlSocket.GetRemoteEndPoint();

            if (null == socketRemoteEndPoint)
            {
                return "null";
            }
            else
            {
                if (isPerfect)
                    return socketRemoteEndPoint.Address.ToString() + "," + socketRemoteEndPoint.Port.ToString();
                else
                    return socketRemoteEndPoint.Address.ToString();
            }
        }

        /// <summary>
        /// 指定された接続先が，このストリームの接続先と同一かどうか調べます．
        /// </summary>
        /// <param name="remoteInformation"></param>
        /// <returns></returns>
        public bool isSamePoint(params object[] remoteInformation)
        {
            IPEndPoint socketRemoteEndPoint = controlSocket.GetRemoteEndPoint();
            if (null == socketRemoteEndPoint)
                return false;

            if (remoteInformation.Length == 1 && remoteInformation[0] is IPEndPoint)
            {
                // IPEndPointの比較は使わない 参照の等価を見ているらしいので．
                return socketRemoteEndPoint.Address.ToString().Equals(((IPEndPoint)remoteInformation[0]).Address.ToString()) &&
                       socketRemoteEndPoint.Port == ((IPEndPoint)remoteInformation[0]).Port;
            }
            else if (remoteInformation.Length == 2 && remoteInformation[0] is string && remoteInformation[1] is int)
            {
                return socketRemoteEndPoint.Address.ToString().Equals((string)remoteInformation[0]) && socketRemoteEndPoint.Port == (int)remoteInformation[1];
            }
            else if (remoteInformation.Length == 1 && remoteInformation[0] is string)
            {
                return socketRemoteEndPoint.Address.ToString().Equals((string)remoteInformation[0]);
            }
            else
            {
                throw new System.ArgumentException("サポートされていない型です．" + Trace.ObjectToTypeString.Array2Str(remoteInformation));
            }
        }

        private void OnClose(object sender, EventArgs e)
        {
            OneEventArgs disconnectedEventArgs = new OneEventArgs();
            disconnectedEventArgs.firstData = this;
            DisConnected(this, disconnectedEventArgs);
        }

        private void OnLineRead(object sender, ReadEventArgs e)
        {
            TwoEventArgs readedEventArgs = new TwoEventArgs();
            readedEventArgs.firstData = e.oneLine;
            readedEventArgs.secondData = this;
            Read(this, readedEventArgs);
        }
    }
}
