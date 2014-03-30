using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Client.Stream
{
    /// <summary>
    /// CRLF�X�v���b�^�D
    /// �o�C�g�z����󂯂āC����CRLF�܂ł̐؂�o�����x������D
    /// </summary>
    class NullSplitter
    {
        int forwardBytes;
        byte[] byteArray;

        /// <summary>
        /// �Z�b�g
        /// </summary>
        /// <param name="byteArray">�o�C�g�z��</param>
        public void Set(byte[] byteArray)
        {
            forwardBytes = 0;
            this.byteArray = byteArray;
        }

        /// <summary>
        /// foreach�I�Ǎ��T�|�[�g
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            byte[] returnArray;
            for (int i = 0; i < byteArray.Length ; i++)
            {
                // CRLF�����݂���Ȃ��
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
        /// �ǂݎ�������o�C�g�����擾���܂��D
        /// </summary>
        /// <returns>���o�C�g��</returns>
        public int getReadBytes()
        {
            return forwardBytes;
        }
    }
}
