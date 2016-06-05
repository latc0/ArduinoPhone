using System;

namespace ArduinoPhone
{
    class MessageEventHandler
    {
        public delegate void MessageHandler(object myObject, MessageEventArgs mArgs);
        public event MessageHandler MessageReceived;

        public string IncomingMessage
        {
            set
            {
                string line = value;
                string[] items = line.Split('"');
                string num = items[1];
                string time = items[5];
                string message = items[6];
                MessageEventArgs mArgs = new MessageEventArgs(num, message, time);
                MessageReceived(this, mArgs);
            }
        }

        public MessageEventHandler()
        {
        }
    }

    public class MessageEventArgs : EventArgs
    {
        private string number;
        private string message;
        private string time;

        public MessageEventArgs(string number, string message, string time)
        {
            this.number = number;
            this.message = message;
            this.time = time;
        }

        public string Number { get { return number; } }
        public string Message { get { return message; } }
        public string Time { get { return time; } }
    }
}
