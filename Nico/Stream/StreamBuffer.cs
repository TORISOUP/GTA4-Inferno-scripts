using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Stream
{
    /// <summary>
    /// �X�g���[���o�b�t�@�D
    /// �o�C�g�z��f�[�^���X�g���[���Ƃ��ĕێ����܂��D
    /// </summary>
    class StreamBuffer
    {
        List<byte[]> bufferList = new List<byte[]>();
        int bufferReadOffset;

        // �o�b�t�@�ǉ�
        private void internalAdd(byte[] addData, int addBytes)
        {
            byte[] addPerfectData = new byte[addBytes];
            Array.Copy(addData, 0, addPerfectData, 0, addBytes);

            lock (bufferList)
            {
                bufferList.Add(addPerfectData);
            }
        }

        // �o�b�t�@�ǂݏo��
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

        // �o�b�t�@�ꕔ�폜
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
                        // �ŌÃo�b�t�@�T�C�Y > �폜�T�C�Y
                        //   �ŌÃo�b�t�@�T�C�Y�ɃI�t�Z�b�g��������悤�ɂ��ďI���
                        // �ŌÃo�b�t�@�T�C�Y <= �폜�T�C�Y
                        //   �ŌÃo�b�t�@���폜���āC�폜�T�C�Y������������
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

        // �T�C�Y�v�Z
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
        /// �f�[�^�ǉ��C�x���g�D
        /// Get���\�b�h���g���Ď擾���Ă��������D
        /// </summary>
        public event EventHandler Added;

        /// <summary>
        /// �f�[�^��ǉ����܂��D
        /// </summary>
        /// <param name="addData">�ǉ��f�[�^</param>
        /// <param name="addBytes">�f�[�^��</param>
        public void Add(byte[] addData, int addBytes)
        {
            internalAdd(addData, addBytes);
            Added(this, EventArgs.Empty);
        }

        /// <summary>
        /// �f�[�^���擾���܂��D
        /// </summary>
        /// <param name="getBytes">�擾�o�C�g�� (-1: ���ׂ�)</param>
        /// <returns>�f�[�^</returns>
        public byte[] Get(int getBytes)
        {
            return internalGet();
        }

        /// <summary>
        /// �f�[�^�̈ꕔ���폜���܂��D
        /// �폜�Ɏ��s�����ꍇ�C�f�[�^�͕s���S�ȏ�ԂŎc��܂��D
        /// </summary>
        /// <param name="deleteBytes">�폜�o�C�g��</param>
        /// <returns>true: ����, false: ���s</returns>
        public bool Delete(int deleteBytes)
        {
            return internalDelete(deleteBytes);
        }

        /// <summary>
        /// �΃s�A�\�P�b�g(AsyncSocket)��M�C�x���g�ɑ΂��郁�\�b�h�D
        /// </summary>
        /// <param name="sender">���M��</param>
        /// <param name="e">�p�����^</param>
        public void OnRead(object sender, ByteArrayReadEventArgs e)
        {
            Add(e.readData, e.readBytes);
        }
    }
}
