using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ArduinoPhone
{
    public class MessageControl : ScrollableControl
    {
        public List<Message> Messages { get; private set; }

        private Color _LeftBubbleColor = Color.LightGray;
        private Color _RightBubbleColor = Color.LimeGreen;
        private Color _LeftBubbleTextColor = Color.Black;
        private Color _RightBubbleTextColor = Color.White;
        private int _BubbleIndent = 40;
        private int _BubbleSpacing = 10;
        public enum BubblePositionEnum { Left, Right }
        private const int MAX_LEN = 260;

        public MessageControl()
        {
            Messages = new List<Message>();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            BackColor = Color.White;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            AutoScroll = true;
        }

        public void RemoveAll()
        {
            Messages.Clear();
            Controls.Clear();
        }

        private Message CreateCommonMessage(Message b, string message)
        {
            if (Messages.Count > 0)
            {
                b.Top = Messages[Messages.Count - 1].Top + Messages[Messages.Count - 1].Height + _BubbleSpacing;
            }
            else
            {
                b.Top = _BubbleSpacing + AutoScrollPosition.Y;
            }

            b.Text = message;
            using (Graphics G = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF s = G.MeasureString(b.Text, Font, Width);
                int textW = (int)s.Width;
                b.Width = (textW > MAX_LEN) ? MAX_LEN : textW;
                b.Height = (int)(Math.Floor(s.Height) * 1.3);
            }
            return b;
        }

        public void AddSent(string message)
        {
            Message b = new Message(BubblePositionEnum.Right);
            b = CreateCommonMessage(b, message);
            b.Left = Width - b.Width;
            if (VerticalScroll.Visible)
                b.Left -= SystemInformation.VerticalScrollBarWidth;
            b.BubbleColor = _RightBubbleColor;
            b.ForeColor = _RightBubbleTextColor;
            b.Anchor |= AnchorStyles.Right;
            Messages.Add(b);
            Controls.Add(b);
        }

        public void AddReceived(string message)
        {
            Message b = new Message(BubblePositionEnum.Left);
            b = CreateCommonMessage(b, message);
            Console.WriteLine(b.Text);
            b.Left = 10;
            b.BubbleColor = _LeftBubbleColor;
            b.ForeColor = _LeftBubbleTextColor;
            b.Anchor |= AnchorStyles.Left;
            Messages.Add(b);
            Controls.Add(b);
        }

        private void RedrawControls()
        {
            int count = 0;
            Message last = null;
            int new_width = Width;
            SuspendLayout();
            foreach (Message m in Controls)
            {
                if (count > 0)
                {
                    m.Top = last.Top + last.Height + _BubbleSpacing + AutoScrollPosition.Y;
                    if (VerticalScroll.Visible)
                    {
                        m.Width = new_width - (_BubbleIndent + _BubbleSpacing + SystemInformation.VerticalScrollBarWidth);
                    }
                    else
                    {
                        m.Width = new_width - (_BubbleIndent + _BubbleSpacing);
                    }
                }
                last = m;
                count++;
            }
            ResumeLayout();
            Invalidate();
        }

        public class Message : Control
        {
            private Color _TextColor = Color.FromArgb(52, 52, 52);
            private Color _BubbleColor = Color.FromArgb(217, 217, 217);
            private BubblePositionEnum _BubblePosition = BubblePositionEnum.Left;

            public override Color ForeColor { get { return _TextColor; } set { _TextColor = value; Invalidate(); } }
            public BubblePositionEnum BubblePosition
            {
                get { return _BubblePosition; }
                set { _BubblePosition = value; Invalidate(); }
            }
            public Color BubbleColor { get { return _BubbleColor; } set { _BubbleColor = value; Invalidate(); } }

            public Message(BubblePositionEnum Position)
            {
                _BubblePosition = Position;
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint, true);
                DoubleBuffered = true;
                BackColor = Color.Transparent;
                ForeColor = Color.FromArgb(52, 52, 52);
                Font = new Font("Segoe UI", 10);
                Anchor = AnchorStyles.Top;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                Bitmap B = new Bitmap(Width, Height);
                Graphics G = Graphics.FromImage(B);
                var _G = G;

                _G.SmoothingMode = SmoothingMode.HighQuality;
                _G.PixelOffsetMode = PixelOffsetMode.HighQuality;
                _G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                _G.Clear(BackColor);

                // Fill the body of the bubble with the specified color
                GraphicsPath Shape = new GraphicsPath();
                int x = 0;
                int textX = 3;
                int p1 = Width - 10;
                int p2 = Width;
                int p3 = Width - 10;
                if (_BubblePosition == BubblePositionEnum.Left)
                {
                    x = 10;
                    textX = 13;
                    p1 = 10;
                    p2 = 0;
                    p3 = 10;
                }

                Shape.AddRectangle(new Rectangle(x, 0, Width - 10, Height));  // Area to be filled by bubble colour
                Shape.CloseAllFigures();
                _G.FillPath(new SolidBrush(_BubbleColor), Shape);

                _G.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(textX, 4, Width - 10, Height - 5));
                Point[] p = {
                             new Point(p1, Height),
                             new Point(p2, Height),
                             new Point(p3, Height - 15)
                        };
                _G.FillPolygon(new SolidBrush(_BubbleColor), p);
                _G.DrawPolygon(new Pen(new SolidBrush(_BubbleColor)), p);

                _G.Dispose();
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImageUnscaled(B, 0, 0);
                B.Dispose();
            }
        }
    }
}