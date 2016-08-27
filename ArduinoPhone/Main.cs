using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace ArduinoPhone
{
    public partial class Main : Form
    {
        bool gotIncomingNum = false;
        bool answeredCall = false;
        bool gotOwnNumber = false;
        string numberToReply;
        int unreadMessages = 0;
        SmsDb smsDb;
        string myNum;
        Dictionary<string, int> unreadInConvo;
        const string mainTitle = "ArduinoPhone";
        Conversation.Entry prevEntry;

        public Main()
        {
            InitializeComponent();
            replyBox.GotFocus += ReplyBox_GotFocus;
            OpenPort();
            // SetupModem();
            GetOwnNumber();
            unreadInConvo = new Dictionary<string, int>();
            smsDb = new SmsDb(myNum);
            ShowAllMessages();
        }

        private enum ATCommandType
        {
            ReceivedMessage,
            CallingLineIdentification,
            ConnectedLineIdentification,
            CallEnd,
            OK,
            SubscriberNumber,
            Unknown,
        }

        //Events
        private void NotifyNewMessage(string number, string message, string time)
        {
            string[] tz = time.Split('+');
            int tzi = Convert.ToInt32(tz[1]) / 4;
            time = tz[0] + "+0" + tzi;
            time = DateTime.ParseExact(time, "yy/MM/dd,HH:mm:sszz",CultureInfo.CurrentCulture).ToString("dd/MM/yyyy,HH:mm:sszz");
            unreadMessages++;
            Invoke(new Action(() =>
            {
                Util.FlashWindowEx(this);
                Text = mainTitle + " (" + unreadMessages + ")";
            }));

            smsDb.StoreIncomingMessage(number, message, time);

            if (number == numberToReply)
            {
                // Incoming text from current sender, add message to conversation
                Invoke(new Action(() =>
                {
                    messageViewer.AddReceived(message);
                }));
                unreadMessages--;
            }
            else
            {
                if (!unreadInConvo.ContainsKey(number))
                    unreadInConvo[number] = 1;
                else
                    unreadInConvo[number]++;

                string numUnread = (unreadMessages == 0) ? "" : " (" + unreadMessages.ToString() + ")";
                Invoke(new Action(() =>
                {
                    foreach (Conversation.Entry ce in conversationView.Controls)
                    {
                        if (ce.ConvoNumber == number)
                        {
                            ce.SetUnread();
                            ce.LastMessage = message;
                        }
                    }
                    messageTitle.Text = "Messages" + numUnread;
                    //ShowAllMessages();
                }));
            }
        }

        private void ReplyBox_GotFocus(object sender, EventArgs e)
        {
            if (newNumber.Visible)
            {
                string newNum = newNumber.Text;
                newNumber.Hide();
                newMsg.Text = "New";
                string formatNum = FormatNumber(newNum);
                recipientNumber.Text = formatNum;
                numberToReply = formatNum;
                if (smsDb.NumberInPreviousRecipients(formatNum))
                {
                    ShowMessagesForNumber(formatNum);
                }
                else
                {
                    messageViewer.Controls.Clear();
                }
                btnCall.Show();
            }
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = "";
                while ((line = serial.ReadLine()) != null)
                {
                    ATCommandType cmdType = GetCmdType(line);
                    switch (cmdType)
                    {
                        case ATCommandType.ReceivedMessage:
                            string meta = line;
                            string data = serial.ReadLine();
                            string[] items = meta.Remove(0, 6).Replace("\"", "").Split(',');
                            string num = FormatNumber(items[0]);
                            string time = items[2] + "," + items[3];
                            NotifyNewMessage(num, data, time);
                            break;
                        case ATCommandType.CallingLineIdentification:
                            if (!gotIncomingNum)
                            {
                                Invoke(new Action(() =>
                                {
                                    newCallNumber.Text = "Incoming call: " + line.Split('"')[1];
                                    Util.Animate(callNotification, Util.Effect.Slide, 150, 90);
                                }));
                                gotIncomingNum = true;
                            }
                            break;
                        case ATCommandType.ConnectedLineIdentification:
                            Invoke(new Action(() =>
                            {
                                newCallNumber.Text = numberToReply;
                            }));
                            break;
                        case ATCommandType.CallEnd:
                            Invoke(new Action(() =>
                            {
                                Util.Animate(callNotification, Util.Effect.Slide, 150, 90);
                            }));
                            gotIncomingNum = false;
                            break;
                        case ATCommandType.OK:
                            if (answeredCall)
                            {
                                answeredCall = false;
                            }
                            break;
                        case ATCommandType.SubscriberNumber:
                            string[] newdata = line.Split('"');
                            myNum = FormatNumber(newdata[3]);
                            gotOwnNumber = true;
                            break;
                    }
                }
            }
            catch(System.IO.IOException io)
            {
                Console.WriteLine("Error: " + io.Message);
            }
        }

        private void replyBox_TextChanged(object sender, EventArgs e)
        {
            if (replyBox.Text.Length > 0)
                btnSend.Enabled = true;
            else
                btnSend.Enabled = false;
            int oldY = replyBox.Location.Y;
            int oldX = replyBox.Location.X;
            int oldHeight = replyBox.Height;
            // amount of padding to add
            const int padding = 3;
            // get number of lines (first line is 0, so add 1)
            int numLines = replyBox.GetLineFromCharIndex(replyBox.TextLength) + 1;
            // get border thickness
            int border = replyBox.Height - replyBox.ClientSize.Height;
            // set height (height of one line * number of lines + spacing)
            int newHeight = replyBox.Font.Height * numLines + padding + border;
            if (newHeight < 300)
            {
                replyBox.Height = newHeight;
                replyBox.Location = new Point(oldX, oldY - (newHeight - oldHeight));

                int mOldH = messageViewer.Height;
                messageViewer.Height = mOldH - (newHeight - oldHeight);
            }

            updateCharCount();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (smsDb != null)
                smsDb.CloseConnection();
        }

        private void MessageViewer_Resize(object sender, EventArgs e)
        {
            messageViewer.ScrollControlIntoView(messageViewer);
        }

        private void MessageViewer_ControlAdded(object sender, ControlEventArgs e)
        {
            messageViewer.ScrollControlIntoView(e.Control);
            MessageControl.Message m = e.Control as MessageControl.Message;
            m.ContextMenuStrip = copyMessageText;
        }

        private void ConversationView_ControlAdded(object sender, ControlEventArgs e)
        {
            Conversation.Entry en = e.Control as Conversation.Entry;
            en.Click += ConvoClick;
            en.ContextMenuStrip = deleteConvo;
            foreach (Label l in en.Controls)
            {
                l.Click += ConvoClick;
                l.ContextMenuStrip = deleteConvo;
            }
        }

        private void ConvoClick(object sender, EventArgs e)
        {
            if (prevEntry != null)
                prevEntry.PubBackColor = Color.White;
            Conversation.Entry entry = GetEntryFromConvoClick(sender);
            if (entry.hasUnread)
                entry.SetRead();
            entry.PubBackColor = Color.LightGray;
            prevEntry = entry;
            ShowMessagesForNumber(entry.ConvoNumber);
        }


        //Methods
        private void OpenPort()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 1)
                MessageBox.Show("Many many COM ports...");
            else if (ports.Length == 1)
            {
                string comPort = ports[0];
                try
                {
                    serial.NewLine = "\r\n";
                    serial.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(comPort + " is in use.");
                }
            }
        }

        private string FormatNumber(string number)
        {
            //Format an 07 number to an international +44 one
            string modNum = number;
            if (number.StartsWith("0"))
            {
                string temp = number.Remove(0, 1);
                modNum = "+44" + temp;
            }
            return modNum;
        }

        private void GetOwnNumber()
        {
            // Wait until we have got our own number before proceeding
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                serial.WriteLine("AT+CNUM");
            }).Start();

            Stopwatch st = new Stopwatch();
            st.Start();
            while (!gotOwnNumber && st.Elapsed.Seconds <= 5) { }
            st.Stop();
            if (st.Elapsed.Seconds > 5)
            {
                MessageBox.Show("Timeout occurred while connecting. Is the shield powered on?");
                Environment.Exit(1);
                Application.Exit();
            }
        }

        private void ShowMessagesForNumber(string number)
        {
            replyBox.Enabled = true;
            btnCall.Show();
            messageViewer.RemoveAll();
            numberToReply = number;
            recipientNumber.Text = number;
            if (unreadInConvo.ContainsKey(number))
            {
                int num = unreadInConvo[number];
                unreadInConvo.Remove(number);
                unreadMessages -= num;
                string numUnread = (unreadMessages == 0) ? "" : " (" + unreadMessages.ToString() + ")";
                messageTitle.Text = "Messages" + numUnread;
                Text = mainTitle + numUnread;
            }
            smsDb.ShowMessagesForNumber(messageViewer, number);
            replyBox.Focus();
        }

        private void ShowAllMessages()
        {
            conversationView.RemoveAll();
            smsDb.GetAllDbMessages(conversationView);
        }

        private void updateCharCount()
        {
            int length = replyBox.Text.Length;
            if (length <= 160)
                charCount.Text = length + " / 160";
            else
            {
                int rem = length % 160;
                int numTexts = (length - rem) / 160;
                if (rem == 0)
                {
                    rem = 160;
                    numTexts--;
                }
                charCount.Text = rem + " / 160 (" + numTexts + ")";
            }
        }

        private void SetupModem()
        {
            // Enable status message when caller hangs up
            serial.WriteLine("ATQ0");
            Thread.Sleep(500);

            // Set text message format
            serial.WriteLine("AT+CMGF=1");
            Thread.Sleep(500);

            // Enable text message notifications
            serial.WriteLine("AT+CNMI=2,2,0,0,0");
            Thread.Sleep(500);

            // Enable phone number in incoming calls
            serial.WriteLine("AT+CLIP=1");
            Thread.Sleep(500);

            // Enable connected line notification
            serial.WriteLine("AT+COLP=1");
            Thread.Sleep(500);

            // Use verbose values for errors
            serial.WriteLine("AT+CMEE=2");
            Thread.Sleep(500);

            // Use long format for informational messages
            serial.WriteLine("ATV1");
            Thread.Sleep(500);

            // Enable USSD result codes
            serial.WriteLine("AT+CUSD=1");
            Thread.Sleep(500);

            // Set clock yy/MM/dd,HH:mm:ss+-zz
            DateTime dt = DateTime.Now;
            string formatted = dt.ToString("yy/MM/dd,HH:mm:sszz");
            serial.WriteLine("AT+CCLK=" + "\"" + formatted + "\"");
            Thread.Sleep(500);
        }

        private Conversation.Entry GetEntryFromConvoClick(object sender)
        {
            Control c = sender as Control;
            if (c.HasChildren)
            {
                // Groupbox
                return c as Conversation.Entry;
            }
            else
            {
                // Label
                return c.Parent as Conversation.Entry;
            }
        }

        private Control GetSourceControl(object sender)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    return owner.SourceControl;
                }
            }
            return null;
        }

        private ATCommandType GetCmdType(string line)
        {
            if (line.StartsWith("+CMT: "))
                return ATCommandType.ReceivedMessage;
            else if (line.StartsWith("+CLIP: "))
                return ATCommandType.CallingLineIdentification;
            else if (line.StartsWith("+COLP: "))
                return ATCommandType.ConnectedLineIdentification;
            else if (line.StartsWith("OK"))
                return ATCommandType.OK;
            else if (line.StartsWith("+CNUM: "))
                return ATCommandType.SubscriberNumber;
            else if (line.StartsWith("NO CARRIER"))
                return ATCommandType.CallEnd;
            else
                return ATCommandType.Unknown;
        }


        //Buttons
        private void decline_Click(object sender, EventArgs e)
        {
            serial.WriteLine("ATH");
            gotIncomingNum = false;
            Util.Animate(callNotification, Util.Effect.Slide, 150, 90);
        }

        private void answer_Click(object sender, EventArgs e)
        {
            serial.WriteLine("ATA");
            answer.Hide();
            answeredCall = true;
            gotIncomingNum = false;
            endCall.Show();
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            newCallNumber.Text = "Dialing " + numberToReply;
            Util.Animate(callNotification, Util.Effect.Slide, 150, 90);
            answer.Hide();
            decline.Hide();
            endCall.Show();
            serial.WriteLine("ATD" + numberToReply + ";");
        }

        private void deleteItem_Click(object sender, EventArgs e)
        {
            Control sourceControl = GetSourceControl(sender);
            Conversation.Entry entry = GetEntryFromConvoClick(sourceControl);
            if (entry.ConvoNumber == numberToReply)
            {
                messageViewer.RemoveAll();
                replyBox.Enabled = false;
                recipientNumber.Text = "";
                btnCall.Hide();
                numberToReply = null;
            }
            smsDb.DeleteConversation(entry.ConvoNumber);
            conversationView.Remove(entry);
        }

        private void endCall_Click(object sender, EventArgs e)
        {
            serial.WriteLine("ATH");
            Util.Animate(callNotification, Util.Effect.Slide, 150, 90);
        }

        private void newMsg_Click(object sender, EventArgs e)
        {
            if (newMsg.Text == "New")
            {
                messageViewer.RemoveAll();
                btnCall.Hide();
                replyBox.Enabled = true;
                newNumber.Clear();
                newNumber.Show();
                newMsg.Text = "Cancel";
                newNumber.Focus();
            }
            else
            {
                // Cancel
                if (numberToReply != null)
                    ShowMessagesForNumber(numberToReply);
                replyBox.Enabled = false;
                newNumber.Hide();
                newMsg.Text = "New";
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = replyBox.Text;
            string fNum = FormatNumber(numberToReply);
            string fTime = DateTime.Now.ToString("dd/MM/yyyy,HH:mm:sszz");
            smsDb.StoreOutgoingMessage(fNum, message, fTime);
            string part = message;
            while (message.Length > 0)
            {
                int toRemove = 0;
                if (message.Length >= 160)
                    toRemove = 160;
                else
                    toRemove = message.Length;
                part = message.Substring(0, toRemove);
                Console.WriteLine("Sending: {0} with length {1}", part, part.Length);
                message = message.Remove(0, toRemove);
                Console.WriteLine("Message: {0} with length {1}", message, message.Length);
                /*serial.Write("AT+CMGS=\"" + numberToReply + "\"\r");
                Thread.Sleep(100);
                serial.Write(message + (char)26);
                Thread.Sleep(100);*/
            }

            messageViewer.AddSent(message);
            replyBox.Clear();
            btnSend.Enabled = false;
            conversationView.Add(fNum, message, fTime);
            replyBox.Focus();
        }

        private void copyText_Click(object sender, EventArgs e)
        {
            Control sourceControl = GetSourceControl(sender);
            MessageControl.Message msg = sourceControl as MessageControl.Message;
            Clipboard.SetText(msg.Text);
        }
    }
}
