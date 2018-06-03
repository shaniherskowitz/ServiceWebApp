using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceWebApp.connection
{
    public class MessageEventArgs : EventArgs
    {
        public string message;

        public string receiver;

        /// <summary>
        /// Creates the messageEventArsg
        /// </summary>
        /// <param string="message"> The message from the server</param>
        /// <param string="reciever">who to send the message to - log or settings</param>
        public MessageEventArgs(string message, string receiver)
        {
            this.message = message;
            this.receiver = receiver;
        }
    }
}