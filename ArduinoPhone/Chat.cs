using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class MessageControl : ScrollableControl
{

    public List<Message> Messages { get; private set; }

    private Color _LeftBubbleColor = Color.LightGray;
    private Color _RightBubbleColor = Color.LimeGreen;
    private Color _LeftBubbleTextColor = Color.Black;
    private Color _RightBubbleTextColor = Color.White;
    private bool _DrawArrow = true;
    private int _BubbleIndent = 40;
    private int _BubbleSpacing = 10;
    public enum BubblePositionEnum { Left, Right }

    public Color LeftBubbleColor { get { return _LeftBubbleColor; } set { _LeftBubbleColor = value; } }
    public Color RightBubbleColor { get { return _RightBubbleColor; } set { _RightBubbleColor = value; } }
    public Color LeftBubbleTextColor { get { return _LeftBubbleTextColor; } set { _LeftBubbleTextColor = value; } }
    public Color RightBubbleTextColor { get { return _RightBubbleTextColor; } set { _RightBubbleTextColor = value; } }
    public int BubbleIndent { get { return _BubbleIndent; } set { _BubbleIndent = value; } }
    public int BubbleSpacing { get { return _BubbleSpacing; } set { _BubbleSpacing = value; } }
    public bool DrawArrow { get { return _DrawArrow; } set { _DrawArrow = value; } }

    public MessageControl()
    {
        Messages = new List<Message>();
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
        DoubleBuffered = true;
        BackColor = Color.Orange;
        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        AutoScroll = true;
    }

    public void Remove(Message message)
    {
        Invalidate();
        Messages.Remove(message);
        RedrawControls();
    }

    public void RemoveAll()
    {
        Messages.Clear();
        Controls.Clear();
    }

    public void Add(string Message, BubblePositionEnum Position, ContextMenuStrip cms)
    {
        Message b = new Message(Position);

        if (Messages.Count > 0)
        {
            b.Top = Messages[Messages.Count - 1].Top + Messages[Messages.Count - 1].Height + _BubbleSpacing;
        }
        else
        {
            b.Top = _BubbleSpacing + AutoScrollPosition.Y;
        }

        b.Text = Message;
        b.DrawBubbleArrow = _DrawArrow;

        if (VerticalScroll.Visible)
        {
            b.Width = Width - (_BubbleIndent + _BubbleSpacing + SystemInformation.VerticalScrollBarWidth);
        }
        else
        {
            b.Width = Width - (_BubbleIndent + _BubbleSpacing);
        }
        if (Position == BubblePositionEnum.Right)
        {
            b.Left = _BubbleIndent;
            b.BubbleColor = _RightBubbleColor;
            b.ForeColor = _RightBubbleTextColor;
        }
        else
        {
            b.Left = _BubbleSpacing;
            b.BubbleColor = _LeftBubbleColor;
            b.ForeColor = _LeftBubbleTextColor;
        }
        b.ContextMenuStrip = cms;
        Messages.Add(b);
        this.Controls.Add(b);
    }

    private void RedrawControls()
    {
        int count = 0;
        Message last = null;
        int new_width = Width;
        SuspendLayout();
        foreach (Message m in this.Controls)
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
        private GraphicsPath Shape;
        private Color _TextColor = Color.FromArgb(52, 52, 52);
        private Color _BubbleColor = Color.FromArgb(217, 217, 217);
        private bool _DrawBubbleArrow = true;
        private BubblePositionEnum _BubblePosition = BubblePositionEnum.Left;

        public override Color ForeColor { get { return this._TextColor; } set { this._TextColor = value; this.Invalidate(); } }
        public BubblePositionEnum BubblePosition { get { return this._BubblePosition; } set { this._BubblePosition = value; this.Invalidate(); } }
        public Color BubbleColor { get { return this._BubbleColor; } set { this._BubbleColor = value; this.Invalidate(); } }
        public bool DrawBubbleArrow { get { return _DrawBubbleArrow; } set { _DrawBubbleArrow = value; Invalidate(); } }
        public Message(BubblePositionEnum Position)
        {
            _BubblePosition = Position;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            Size = new Size(152, 38);
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(52, 52, 52);
            Font = new Font("Segoe UI", 10);
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        protected override void OnResize(EventArgs e)
        {
            Bitmap B = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(B);

            SizeF s = G.MeasureString(Text, Font, Width - 25);
            this.Height = (int)(Math.Floor(s.Height) + 10);
            Shape = new GraphicsPath();

            var _Shape = Shape;
            if (BubblePosition == BubblePositionEnum.Left)
            {
                _Shape.AddArc(9, 0, 10, 10, 180, 90);
                _Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
                _Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
                _Shape.AddArc(9, Height - 11, 10, 10, 90, 90);
            }
            else
            {
                _Shape.AddArc(0, 0, 10, 10, 180, 90);
                _Shape.AddArc(Width - 18, 0, 10, 10, -90, 90);
                _Shape.AddArc(Width - 18, Height - 11, 10, 10, 0, 90);
                _Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            }
            _Shape.CloseAllFigures();
            

            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
           
            Bitmap B = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(B);
            var _G = G;

            _G.SmoothingMode = SmoothingMode.HighQuality;
            _G.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _G.Clear(BackColor);

            // Fill the body of the bubble with the specified color
            _G.FillPath(new SolidBrush(_BubbleColor), Shape);
            // Draw the string specified in 'Text' property
            if (_BubblePosition == BubblePositionEnum.Left)
            {
                _G.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(13, 4, Width - 25, Height - 5));
            }
            else
            {
                _G.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(5, 4, Width - 25, Height - 5));
            }

            // Draw a polygon on the right side of the bubble
            if (_DrawBubbleArrow == true)
            {
                if (_BubblePosition == BubblePositionEnum.Left)
                {
                    Point[] p = {
                        new Point(9, 9),
                        new Point(0, 15),
                        new Point(9, 20)
                   };
                    _G.FillPolygon(new SolidBrush(_BubbleColor), p);
                    _G.DrawPolygon(new Pen(new SolidBrush(_BubbleColor)), p);
                }
                else
                {
                    Point[] p = {
                        new Point(Width - 8, 9),
                        new Point(Width, 15),
                        new Point(Width - 8, 20)
                    };
                    _G.FillPolygon(new SolidBrush(_BubbleColor), p);
                    _G.DrawPolygon(new Pen(new SolidBrush(_BubbleColor)), p);
                }
            }
            G.Dispose();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(B, 0, 0);
            B.Dispose();
        }
    }
}