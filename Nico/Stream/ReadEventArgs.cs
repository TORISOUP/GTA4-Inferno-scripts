using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Stream
{
    class ReadEventArgs : EventArgs
    {
        /// <summary>
        /// ÉfÅ[É^1åè
        /// </summary>
        public string oneLine;
    }

    class OneEventArgs : EventArgs
    {
        public object firstData;
    }

    class TwoEventArgs : EventArgs
    {
        public object firstData;
        public object secondData;
    }
}
