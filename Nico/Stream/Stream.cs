using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client.Stream
{
    // TODO GetStreamHash��"IP�A�h���X+�|�[�g"���n�b�V���l�Ə̂���蔲���d�l

    /// <summary>
    /// ���悻�X�g���[���̂悤�ɑ���\�ȁCTCP/IP�Z�b�V������񋟂��܂��D
    /// 
    /// [���ӎ���]
    /// DisConnect���\�b�h�ɂ���Đؒf�����X�g���[���́C�ė��p�ł��܂���D
    /// </summary>
    class Stream
    {
        AsyncSocket controlSocket;
        StreamBuffer linkStreamBuffer;
        BufferSplitter linkBufferSplitter;

        /// <summary>
        /// �f�[�^����M���܂����D�f�[�^�ɂ́CEPSPOneLine�^�f�[�^�Ǝ�M�����X�g���[���������Ă��܂��D
        /// </summary>
        public event EventHandler<TwoEventArgs> Read = delegate(object s, TwoEventArgs e) { };
        /// <summary>
        /// �X�g���[�����ؒf����܂����D�f�[�^�ɂ́C�ؒf�����X�g���[���������Ă��܂��D
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
        /// �ڑ����܂��D
        /// </summary>
        /// <param name="remoteInformation">
        /// �ڑ�����
        /// �iIPEndPoint�܂��� ipAddress string, port int �̑g�ݍ��킹�j
        /// </param>
        /// <returns>true: ����, false: ���s</returns>
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
                throw new System.ArgumentException("�T�|�[�g����Ă��Ȃ��^�ł��D" + Trace.ObjectToTypeString.Array2Str(remoteInformation));
            }

            return controlSocket.Connect(ipAddress, port);
        }

        /// <summary>
        /// �ڑ��ς݃\�P�b�g���Q�Ƃ��C�X�g���[���Ƃ��ė��p���܂��D
        /// </summary>
        /// <param name="referenceSocket">�Q�ƃ\�P�b�g</param>
        /// <returns>true: ����, false: ���s</returns>
        public bool Set(params object[] referenceStream)
        {
            if (referenceStream.Length == 1 && referenceStream[0] is Socket)
            {
                return controlSocket.Set((Socket)referenceStream[0]);
            }
            else
            {
                throw new System.ArgumentException("�T�|�[�g����Ă��Ȃ������܂��͌^�ł��D" + Trace.ObjectToTypeString.Array2Str(referenceStream));
            }
        }

        /// <summary>
        /// �ؒf���܂��D�ؒf�����X�g���[���́C�ė��p�ł��܂���D
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            return controlSocket.Close();
        }

        /// <summary>
        /// �f�[�^���������݂܂��D
        /// </summary>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public bool Write(byte[] writeData)
        {
            return controlSocket.Send(writeData, writeData.Length);
        }

        /// <summary>
        /// �ڑ����\���n�b�V�����擾���܂��D
        /// </summary>
        /// <param name="isPerfect">true: ���S�iIP�A�h���X�E�|�[�g�̑g�ݍ��킹�j, false: �����iIP�A�h���X�̂݁j</param>
        /// <returns>�n�b�V���l</returns>
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
        /// �w�肳�ꂽ�ڑ��悪�C���̃X�g���[���̐ڑ���Ɠ��ꂩ�ǂ������ׂ܂��D
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
                // IPEndPoint�̔�r�͎g��Ȃ� �Q�Ƃ̓��������Ă���炵���̂ŁD
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
                throw new System.ArgumentException("�T�|�[�g����Ă��Ȃ��^�ł��D" + Trace.ObjectToTypeString.Array2Str(remoteInformation));
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
