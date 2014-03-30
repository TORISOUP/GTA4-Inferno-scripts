using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Stream
{
    /// <summary>
    /// �o�b�t�@�X�v���b�^�D
    /// �X�g���[���o�b�t�@�̃f�[�^���CNULL�X�v���b�^�ɂ���č��݂܂��D
    /// </summary>
    class BufferSplitter
    {
        NullSplitter nullSplitter = new NullSplitter();

        /// <summary>
        /// ���C����M�C�x���g�D
        /// </summary>
        public event EventHandler<ReadEventArgs> LineRead = delegate(object s, ReadEventArgs e) { };

        /// <summary>
        /// StreamBuffer����̃f�[�^�ǉ��C�x���g���D
        /// </summary>
        public void OnAdded(object sender, EventArgs e)
        {
            StreamBuffer senderSB = (StreamBuffer)sender;

            byte[] readBuffer = senderSB.Get(-1);
            nullSplitter.Set(readBuffer);

            // CRLF�����邾�����C����M�C�x���g���N����
            foreach (byte[] oneLine in nullSplitter)
            {
                ReadEventArgs readEA = new ReadEventArgs();
                readEA.oneLine = Encoding.UTF8.GetString(oneLine);

                if (null != readEA.oneLine)
                    LineRead(this, readEA);
            }

            // �ǂݎ�������͍̂폜
            senderSB.Delete(nullSplitter.getReadBytes());
        }
    }
}
