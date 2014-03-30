using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.IO;
using Client.Stream;
using System.Text.RegularExpressions;

namespace LiveNicoAPIAlert.LiveNicoAPIs
{
    class LiveEventArgs : EventArgs
    {
        public string liveAddress;
        public string liveCommunity;
        public string liveUser;
    }

    class LiveNicoAPI
    {
        public event EventHandler<LiveEventArgs> LiveStarted = delegate(object s, LiveEventArgs e) { };
        Client.Stream.Stream stream;

        public LiveNicoAPI()
        {

        }

        public bool Start()
        {
            // 初期化
            if (null != stream)
                stream.DisConnect();
            
            stream = new Client.Stream.Stream();
            stream.Read += new EventHandler<TwoEventArgs>(stream_Read);

            // コメントサーバとスレッドIDを取得
            string serverAddress = "", threadNumber = "";
            int serverPort = -1;

            WebRequest req = WebRequest.Create("http://live.nicovideo.jp/api/getalertinfo");
            WebResponse rsp = req.GetResponse();
            System.IO.Stream stm = rsp.GetResponseStream();
            if (stm != null)
            {
                StreamReader reader = new StreamReader(stm, Encoding.UTF8);
                string readData = reader.ReadToEnd();
                stm.Close();

                Match match = Regex.Match(readData, "<addr>(.+)</addr>");
                if (match.Success)
                    serverAddress = match.Groups[1].Value;

                match = Regex.Match(readData, "<port>(.+)</port>");
                if (match.Success)
                    serverPort = int.Parse(match.Groups[1].Value);

                match = Regex.Match(readData, "<thread>(.+)</thread>");
                if (match.Success)
                    threadNumber = match.Groups[1].Value;
            }
            rsp.Close();

            if ("" == serverAddress || -1 == serverPort || "" == threadNumber)
                return false;

            // 接続・送信
            if (stream.Connect(serverAddress, serverPort)) {
                string sendString = "<thread thread=\"" + threadNumber + "\" version=\"20061206\" res_from=\"-1\"/>";
                byte[] sendBytes = Encoding.UTF8.GetBytes(sendString);

                if (stream.Write(sendBytes) && stream.Write(new byte[] {0}))
                {
                    // OK.
                }
                else
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }

            return true;
        }

        public bool Stop()
        {
            stream.DisConnect();
            return true;
        }

        void stream_Read(object sender, TwoEventArgs e)
        {
            string lineString = (string)e.firstData;

            Match match = Regex.Match(lineString, ">(.+),(.+),(.+)<");
            if (match.Success)
            {
                LiveEventArgs liveEventArgs = new LiveEventArgs();
                liveEventArgs.liveAddress   = match.Groups[1].Value;
                liveEventArgs.liveCommunity = match.Groups[2].Value;
                liveEventArgs.liveUser      = match.Groups[3].Value;
                LiveStarted(this, liveEventArgs);
            }
        }
    }
}
