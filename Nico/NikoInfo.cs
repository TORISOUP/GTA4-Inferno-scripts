using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using System.Threading;

namespace PrjNikoNiko
{
    class NikoInfo
    {
        string URL_LOGIN = "https://secure.nicovideo.jp/secure/login?site=niconico";
        string URL_STATUS = "http://live.nicovideo.jp/api/getplayerstatus?v={0}";
        string URL_REGEX = "^http://live.nicovideo.jp/watch/(lv[0-9]+)$";
        string URL_SEND = "<thread thread=\"{0}\" version=\"20061206\" res_from=\"-{1}\"/>\0";

        bool mLogin = false;
        Encoding mEnc = null;
        CookieContainer mCookie = null;
        string mAddr = null;
        int mPort = 0;
        string mThreadID = null;

        //以下、マルチスレッド用データ
        Thread mThread = null;
        Hashtable mList = null;
        int mListMax = 0;
        int mMillSec = 1000;
        bool mKill = false;
        public static readonly object SYNC_OBJ = new object();  //mList mListMax を使用する場合は、この変数で排他処理を行ってください

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NikoInfo()
        {
            this.mLogin = false;
            this.mEnc = Encoding.UTF8;
            this.mCookie = new CookieContainer();
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="clMail">ID(メアド)</param>
        /// <param name="clPass">パスワード</param>
        /// <param name="clURL">取得したいURL(http://live.nicovideo.jp/watch/lv????????)</param>
        /// <returns>エラー内容(nullは正常)</returns>
        public string Login(string clMail, string clPass, string clURL)
        {
            Stream clStream = null;
            StreamReader clReader = null;

            try
            {
                //以下、ログイン処理
                string clParam = "mail=" + clMail + "&password=" + clPass;
                byte[] puchData = Encoding.ASCII.GetBytes(clParam);

                HttpWebRequest clRequest = (HttpWebRequest)WebRequest.Create(URL_LOGIN);
                clRequest.Method = "POST";
                clRequest.ContentType = "application/x-www-form-urlencoded";
                clRequest.ContentLength = puchData.Length;
                clRequest.CookieContainer = this.mCookie;

                clStream = clRequest.GetRequestStream();
                clStream.Write(puchData, 0, puchData.Length);
                clStream.Close();

                //以下、データ受信処理
                WebResponse clWebResponse = clRequest.GetResponse();
                clStream = clWebResponse.GetResponseStream();
                clReader = new StreamReader(clStream, this.mEnc);
                string clRecv = clReader.ReadToEnd();

                clReader.Close();
                clStream.Close();



                //以下、サーバー情報取得処理
                Regex clRegex;
                MatchCollection clMatch;
                clRegex = new Regex(URL_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                clMatch = clRegex.Matches(clURL);
                string clLiveID = clMatch[0].Groups[1].Value;

                string clUrl = string.Format(URL_STATUS, clLiveID);
                clRequest = (HttpWebRequest)WebRequest.Create(clUrl);
                clRequest.UserAgent = "NicoLiveCommentConsole1.0.0";
                clRequest.CookieContainer = this.mCookie;

                clWebResponse = clRequest.GetResponse();
                clStream = clWebResponse.GetResponseStream();
                clReader = new StreamReader(clStream, this.mEnc);
                string clBuffer = clReader.ReadToEnd();

                clReader.Close();
                clStream.Close();

                //以下、failチェック
                clRegex = new Regex("<getplayerstatus status=\"(.*?)\" time=\".*?\">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                clMatch = clRegex.Matches(clBuffer);
                string clResult = clMatch[0].Groups[1].Value;
                clResult = clResult.ToLower();
                if (clResult != "ok")
                {
                    clRegex = new Regex("<code>(.*?)</code>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    clMatch = clRegex.Matches(clBuffer);
                    string clErrorCode = clMatch[0].Groups[1].Value;
                    throw new Exception("ライブ情報取得に失敗しました\n" + clErrorCode);
                }

                //以下、正規表現処理
                clRegex = new Regex("<addr>(.*?)</addr>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                clMatch = clRegex.Matches(clBuffer);
                this.mAddr = clMatch[0].Groups[1].Value;

                clRegex = new Regex("<port>(.*?)</port>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                clMatch = clRegex.Matches(clBuffer);
                this.mPort = Convert.ToInt32(clMatch[0].Groups[1].Value);

                clRegex = new Regex("<thread>(.*?)</thread>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                clMatch = clRegex.Matches(clBuffer);
                this.mThreadID = clMatch[0].Groups[1].Value;

                //以下、フラグ設定処理
                this.mLogin = true;
            }
            catch (Exception err)
            {
                return (err.Message);
            }
            finally
            {
                if (clReader != null)
                {
                    clReader.Close();
                    clReader.Dispose();
                    clReader = null;
                }
                if (clStream != null)
                {
                    clStream.Close();
                    clStream.Dispose();
                    clStream = null;
                }
            }

            return (null);
        }

        string OldComment;
        public string GetNewComment()
        {
            string comment;

            GetComment(1, out comment);

            string patternStr = @"<.*?>";
            string newStr = Regex.Replace(comment, patternStr, string.Empty);
            if (newStr != OldComment)
            {
                OldComment = newStr;
                return newStr;
            }
            OldComment = newStr;
            return "";

        }


        /// <summary>
        /// コメント取得処理
        /// </summary>
        /// <param name="inCommentID">コメントID(1が最新のコメント 0を指定すると全てのコメント)</param>
        /// <param name="clComment">応答メッセージ</param>
        /// <returns>エラー内容(nullは正常)</returns>
        public string GetComment(int inCommentID, out string clComment)
        {
            clComment = "";

            TcpClient clTcp = null;
            MemoryStream clMemStream = null;

            try
            {
                if (!this.mLogin)
                {
                    throw new Exception("ログインしていません");
                }

                //以下、ソケット接続処理
                clTcp = new TcpClient(this.mAddr, this.mPort);
                NetworkStream clNetStream = clTcp.GetStream();

                //以下、送信処理
                if (inCommentID == 0) inCommentID = 999999999;
                string clSendMsg = string.Format(URL_SEND, this.mThreadID, inCommentID);
                byte[] puchData = this.mEnc.GetBytes(clSendMsg);
                clNetStream.Write(puchData, 0, puchData.Length);

                //以下、受信処理
                clMemStream = new MemoryStream();
                puchData = new byte[256];
                int inDataSize;
                do
                {
                    inDataSize = clNetStream.Read(puchData, 0, puchData.Length);
                    if (inDataSize == 0) break;
                    clMemStream.Write(puchData, 0, inDataSize);

                } while (clNetStream.DataAvailable);

                //受信したデータを文字列に変換
                clComment = this.mEnc.GetString(clMemStream.ToArray());
            }
            catch (Exception err)
            {
                return (err.Message);
            }
            finally
            {
                if (clMemStream != null)
                {
                    clMemStream.Close();
                    clMemStream.Dispose();
                    clMemStream = null;
                }
                if (clTcp != null)
                {
                    clTcp.Close();
                    clTcp = null;
                }
            }

            return (null);
        }

        //================================================================
        //　マルチスレッド処理
        //================================================================

        /// <summary>
        /// マルチスレッドを削除
        /// </summary>
        /// <returns>エラー内容(nullは正常)</returns>
        public string KillMultiThread()
        {
            this.mKill = true;

            if (this.mThread == null) return (null);

            while (this.mThread.IsAlive)
            {
                Thread.Sleep(100);
            }
            this.mThread = null;

            return (null);
        }

        /// <summary>
        /// マルチスレッドを作成
        /// </summary>
        /// <param name="inMillSec">更新間隔(ミリ秒)</param>
        /// <returns>エラー内容(nullは正常)</returns>
        public string SetMultiThread(int inMillSec)
        {
            try
            {
                if (this.mThread != null)
                {
                    if (this.mThread.IsAlive)
                    {
                        throw new Exception("スレッドを生成できませんでした２");
                    }
                }

                this.mMillSec = inMillSec;
                this.mKill = false;
                this.mList = new Hashtable();

                this.mThread = new Thread(new ThreadStart(ThreadFunction));
                this.mThread.Start();
            }
            catch (Exception err)
            {
                return (err.Message);
            }
            finally
            {
            }

            return (null);
        }

        /// <summary>
        /// コメント取得処理
        /// </summary>
        private void ThreadFunction()
        {
            string clComment;
            string clErrMsg;
            Regex clRegex;
            MatchCollection clMatch;
            int inCnt;
            string clKey, clVal;
            int inKey;

            while (!this.mKill)
            {
                clErrMsg = this.GetComment(10, out clComment);
                if (clErrMsg != null) break;

                //以下、正規表現処理
                Monitor.Enter(SYNC_OBJ); // ロック取得

                clRegex = new Regex("<thread .*? last_res=\"(.*?)\" .*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                clMatch = clRegex.Matches(clComment);
                if (clMatch.Count >= 1)
                {
                    this.mListMax = Convert.ToInt32(clMatch[0].Groups[1].Value);
                }

                clRegex = new Regex("<chat .*? no=\"(.*?)\" .*?>(.*?)</chat>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                clMatch = clRegex.Matches(clComment);
                try
                {
                    for (inCnt = 0; inCnt < clMatch.Count; inCnt++)
                    {
                        clKey = clMatch[inCnt].Groups[1].Value;
                        inKey = Convert.ToInt32(clKey);
                        clVal = clMatch[inCnt].Groups[2].Value;
                        this.mList[inKey] = clVal;
                    }
                }
                finally
                {
                    Monitor.Exit(SYNC_OBJ); // ロック解放
                }

                Thread.Sleep(this.mMillSec);
            }
        }

        /// <summary>
        /// コメント取得処理
        /// </summary>
        /// <param name="clList">コメントリスト(最新の10コメント)</param>
        /// <param name="inListMax">最新のコメントNo</param>
        /// <returns>エラー内容(nullは正常)</returns>
        public string GetComment(out Hashtable clList, out int inListMax)
        {
            clList = this.mList;
            inListMax = this.mListMax;

            return (null);
        }
    }
}