﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace ArduinoPhone
{
    public partial class Main : Form
    {
        SerialPort serial;
        bool gotIncomingNum = false;
        bool answeredCall = false;
        bool gotOwnNumber = false;
        string numberToReply;
        int unreadMessages = 0;
        SmsDb smsDb;
        string myNum;
        Dictionary<string, int> unreadInConvo;

        public Main()
        {
            InitializeComponent();
            replyBox.GotFocus += ReplyBox_GotFocus;
            OpenPort();
            GetOwnNumber();
            unreadInConvo = new Dictionary<string, int>();
            smsDb = new SmsDb(myNum);
            ShowAllMessages();
        }

        //Events
        private void NewMessage(object sender, MessageEventArgs e)
        {
            string num = FormatNumber(e.Number);
            string message = e.Message;
            unreadMessages++;
            smsDb.StoreIncomingMessage(num, message, e.Time);

            if (num == numberToReply)
            {
                // Incoming text from current sender, add message to conversation
                Invoke(new Action(() =>
                {
                    messageViewer.Add(message, MessageControl.BubblePositionEnum.Left, copyMessageText);
                }));
                unreadMessages--;
            }
            else
            {
                if (!unreadInConvo.ContainsKey(num))
                    unreadInConvo.Add(num, 1);
                else
                    unreadInConvo[num]++;
                string numUnread = (unreadMessages == 0) ? "" : " (" + unreadMessages.ToString() + ")";
                Invoke(new Action(() =>
                {
                    messageTitle.Text = "Messages" + numUnread;
                    ShowAllMessages();
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
                    Console.WriteLine(line);
                    if (line.StartsWith("+CMT: ") && !line.Contains("??"))
                    {
                        string data = line + serial.ReadLine();
                        MessageEventHandler msg = new MessageEventHandler();
                        msg.MessageReceived += NewMessage;
                        msg.IncomingMessage = data; // This triggers the event handler to be called
                    }
                    else if (line.StartsWith("+CLIP: ") && !gotIncomingNum)
                    {
                        Invoke(new Action(() =>
                        {
                            newCallNumber.Text = "Incoming call: " + line.Split('"')[1];
                            Util.Animate(callNotification, Util.Effect.Slide, 150, 90);
                        }));
                        gotIncomingNum = true;
                    }
                    else if (line.StartsWith("NO CARRIER"))
                    {
                        gotIncomingNum = false;
                    }
                    else if (line == "OK")
                    {
                        if (answeredCall)
                        {
                            answeredCall = false;
                        }
                    }
                    else if (line.StartsWith("+CNUM: "))
                    {
                        string[] data = line.Split('"');
                        myNum = FormatNumber(data[3]);
                        Console.WriteLine("Number: " + myNum);
                        gotOwnNumber = true;
                    }
                }
            }
            catch (IOException io)
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

            updateCharCount(replyBox.Text.Length);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serial != null && serial.IsOpen)
                serial.Close();
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
            Conversation.Entry entry = GetEntryFromConvoClick(sender);
            ShowMessagesForNumber(entry.ConvoNumber);
        }


        //Methods
        private void OpenPort()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 1)
                MessageBox.Show("Many many COM ports...");
            else
            {
                string comPort = ports[0];
                try
                {
                    serial = new SerialPort(comPort, 115200, Parity.None, 8, StopBits.One);
                    serial.DataReceived += DataReceived;
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
            messageViewer.Controls.Clear();
            numberToReply = number;
            recipientNumber.Text = number;
            if (unreadInConvo.ContainsKey(number))
            {
                unreadInConvo[number]--;
                if (unreadInConvo[number] == 0)
                    unreadInConvo.Remove(number);

                unreadMessages--;
                string numUnread = (unreadMessages == 0) ? "" : " (" + unreadMessages.ToString() + ")";
                messageTitle.Text = "Messages" + numUnread;
            }
            foreach(KeyValuePair<string, string> msg in smsDb.GetMessagesForNumber(messageViewer, number))
            {
                if (msg.Key == myNum)
                    messageViewer.Add(msg.Value, MessageControl.BubblePositionEnum.Right, copyMessageText);
                else
                    messageViewer.Add(msg.Value, MessageControl.BubblePositionEnum.Left, copyMessageText);
            }
        }

        private void ShowAllMessages()
        {
            conversationView.RemoveAll();
            foreach (KeyValuePair<string, Dictionary<string, string>> kv in smsDb.GetAllDbMessages())
            {
                string num = kv.Key;
                string lastMessage = "";
                string lastTimestamp = "";
                foreach (KeyValuePair<string, string> details in kv.Value)
                {
                    lastMessage = details.Key;
                    lastTimestamp = details.Value;
                }
                conversationView.Add(num, lastMessage, lastTimestamp);
            }
        }

        private void updateCharCount(int length)
        {
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
            serial.WriteLine("AT+CLIP=1,1");
            Thread.Sleep(500);

            // Set clock yy/MM/dd,HH:mm:ss+-zz
            DateTime dt = DateTime.Now;
            string formatted = dt.ToString("yy/MM/dd,HH:mm:sszz");
            serial.WriteLine("AT+CCLK=" + formatted);
            Thread.Sleep(500);
        }

        private void CallNumber(string num)
        {
            serial.WriteLine("ATD" + num + ";");
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
            CallNumber(numberToReply);
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
            }
            numberToReply = null;
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
                messageViewer.Controls.Clear();
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
            smsDb.StoreOutgoingMessage(
                FormatNumber(numberToReply),
                message,
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:sszz")
                );

            /*serial.Write("AT+CMGS=\"" + numberToReply + "\"\r");
            Thread.Sleep(500);
            serial.Write(message + (char)26);
            Thread.Sleep(500);*/

            messageViewer.Add(replyBox.Text, MessageControl.BubblePositionEnum.Right, copyMessageText);
            replyBox.Clear();
            btnSend.Enabled = false;
            ShowAllMessages();
        }

        private void copyText_Click(object sender, EventArgs e)
        {
            Control sourceControl = GetSourceControl(sender);
            MessageControl.Message msg = sourceControl as MessageControl.Message;
            Clipboard.SetText(msg.Text);
        }
    }
}
