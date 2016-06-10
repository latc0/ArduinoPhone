using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class Conversation : ScrollableControl
{
    // A custom conversation scrollable control, whos controls are of type Entry (a conversation entry)
    public List<Entry> Conversations { get; private set; }

    public Conversation()
    {
        Conversations = new List<Entry>();
        SetStyle(ControlStyles.AllPaintingInWmPaint | 
            ControlStyles.OptimizedDoubleBuffer| 
            ControlStyles.SupportsTransparentBackColor, true);
        DoubleBuffered = true;
        BackColor = Color.White;
        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        AutoScroll = true;
    }

    public void Add(string number, string message, string time)
    {
        string[] data = time.Split(',')[1].Split(':');
        time = data[0] + ":" + data[1];

        bool match = false;
        foreach (Entry en in Conversations)
        {
            if (en.ConvoNumber == number)
            {
                en.LastMessage = message;
                match = true;
                break;
            }
        }
        if (!match)
        {
            Entry e = new Entry(number, message, time);
            int convos = Conversations.Count;
            if (convos > 0)
                e.Top = Conversations[convos - 1].Top + Conversations[convos - 1].Height;

            Conversations.Add(e);
            Controls.Add(e);
        }
    }

    public void RedrawControls()
    {
        int count = 0;
        Entry last = null;
        SuspendLayout();

        foreach (Entry e in Conversations)
        {
            if (count > 0)
            {
                e.Top = last.Top + last.Height;
            }
            else
            {
                e.Top = 0;
            }
            last = e;
            count++;
        }
        ResumeLayout();
        Invalidate();
    }

    public void Remove(Entry entry)
    {
        Conversations.Remove(entry);
        Controls.Remove(entry);
        RedrawControls();
    }

    public void RemoveAll()
    {
        Conversations.Clear();
        Controls.Clear();
    }

    public class Entry : GroupBox
    {
        private Label Number;
        private Label Message;
        private Label Time;

        public bool hasUnreadMsgs = false;

        public Entry(string number, string message, string time)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            Size = new Size(280, 100);
            BackColor = Color.White;
            ForeColor = Color.Black;
            Left = -1;
            Width = 322;
            Height = 80;
            Top = 0;
            Text = "";
            Font = new Font("Segoe UI", 9);
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Number = new Label();
            Number.Text = number;
            Number.Width = 150;
            Number.Location = new Point(10, 15);

            Message = new Label();
            Message.BackColor = Color.White;
            Message.ForeColor = Color.Black;
            if (message.Length > 70)
                message = message.Substring(0, 68) + "..";
            Message.Text = message;
            Message.Width = 270;
            Message.Height = 40;
            Message.Location = new Point(10, 35);

            Time = new Label();
            Time.Text = time + " >";
            Time.Location = new Point(260, 15);
            Time.Anchor = AnchorStyles.Right;
            Time.Width = 50;

            Controls.Add(Number);
            Controls.Add(Message);
            Controls.Add(Time);
        }

        public string ConvoNumber
        {
            get { return Number.Text; }
            set { Number.Text = value; }
        }
        public string LastMessage
        {
            set { Message.Text = value; }
        }
    }
}
