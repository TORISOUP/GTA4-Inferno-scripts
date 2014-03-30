using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

/*
 * TODO �C�x���g�Ăяo�����C��M�o�b�t�@(byte[])�����̂܂ܓn����
 *        �Q�Ƃ݂̂��n��\��������(�Ƃ����������؂�)�̂ŁC
 *         �f�[�^�𕡐�������œn������
 *       ��M�o�b�t�@�𒼐ڗp����StreamBuffer�N���X��
 *        ��M���ɂ��킹�ăf�[�^�R�s�[����悤�ɂȂ��Ă��邪�C
 *         �������������̎����𗊂�̂͂������_��
 */

namespace Client.Stream
{
    public class ByteArrayReadEventArgs : EventArgs
    {
        /// <summary>
        /// ��M�f�[�^
        /// </summary>
        public byte[] readData;
        /// <summary>
        /// ��M�f�[�^��
        /// </summary>
        public int readBytes;
    }

    /// <summary>
    /// �΃s�A/�T�[�o�\�P�b�g�D
    /// �񓯊��\�P�b�g���ȒP�Ɉ�����N���X�D
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
        /// �f�[�^��M�C�x���g�DReadEventArgs�I�u�W�F�N�g�Ƀf�[�^���i�[����Ă��܂��D
        /// </summary>
        public event EventHandler<ByteArrayReadEventArgs> Read = delegate(object s, ByteArrayReadEventArgs e) { };
        public event EventHandler Closed = delegate(object s, EventArgs e) { };

        public AsyncSocket()
        {
            mySocket =new Socket(AddressFamily.InterNetwork, 
                              SocketType.Stream, ProtocolType.Tcp );
        }

        // ��M���J�n���܂��D
        private void Start()
        {
            if (null == mySocket)
                return;

            mySocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None,
                new AsyncCallback(ReadCallback), mySocket);
        }

        /// <summary>
        /// TCP�ڑ����s���܂��D
        /// </summary>
        /// <param name="ipAddress">IP�A�h���X</param>
        /// <param name="port">�|�[�g</param>
        /// <returns>true: ����, false: ���s</returns>
        public bool Connect(string ipAddress, int port)
        {
            if (null == mySocket)
                return false;

            connectDone.Reset();
            IAsyncResult ar = mySocket.BeginConnect(ipAddress, port, new AsyncCallback(ConnectCallback), mySocket);

            Trace.Logger.Write("  try " + ipAddress + ":" + port.ToString(), Client.Trace.Logger.LogLevel.L7_DEBUG);

            if (connectDone.WaitOne(TimeOutMS, false))
            {
                // ����
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
                // ���s
                //   Close=>null��ConnectCallback=>false�Ԃ�
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
        /// �ڑ��ς݃\�P�b�g���Q�Ƃ��܂��D
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
        /// �f�[�^��񓯊��ő��M���܂��D�߂�l�́C���M�������J�n�ł������ǂ����ł��D
        /// </summary>
        /// <param name="sendData">���M����f�[�^</param>
        /// <param name="sendBytes">�f�[�^��</param>
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
        /// �ؒf���C���\�[�X��������܂��D���̃I�u�W�F�N�g�͍ė��p�ł��܂���D
        /// </summary>
        public bool Close()
        {
            try
            {
                // FIN(0�o�C�g��Read)�ɑ΂���Close�̏ꍇ�CShutdown�͕s�v
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

        // �ڑ��R�[���o�b�N
        private void ConnectCallback(IAsyncResult ar)
        {
            Socket connectSocket = (Socket)ar.AsyncState;

            if (null != connectSocket)
                connectDone.Set();
        }

        // ��M�R�[���o�b�N
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                Socket receiveSocket = (Socket)ar.AsyncState;
                int bytesRead = receiveSocket.EndReceive(ar);

                // ��M�����Ƃ��́C�C�x���g���Ăяo���đ��s����
                if (bytesRead > 0)
                {
                    ByteArrayReadEventArgs baReadEventArgs = new ByteArrayReadEventArgs();
                    baReadEventArgs.readData = buffer;
                    baReadEventArgs.readBytes = bytesRead;
                    Read(this, baReadEventArgs);
                    mySocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None,
                        new AsyncCallback(ReadCallback), mySocket);
                }
                // �G���[�����������Ƃ��́C�\�P�b�g����Đؒf�ʒm����
                else
                {
                    Close();
                    Closed(this, EventArgs.Empty);
                }
            }
            catch { }
        }

        // ���M�R�[���o�b�N
        private void SendCallback(IAsyncResult ar)
        {
            Socket sendSocket = (Socket)ar.AsyncState;
            try
            {
                int bytesSent = sendSocket.EndReceive(ar);
                if (bytesSent <= 0)
                    throw new ApplicationException("���M�G���[�ł��D");
            }
            catch
            {
                Close();
                Closed(this, EventArgs.Empty);
            }
        }
    }
}
