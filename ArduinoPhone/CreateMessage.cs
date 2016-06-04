/*
MIT License

Copyright (c) 2016 Sam Wilberforce

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ArduinoPhone
{
    public class CreateMessage
    {
        private static int starty = 25;

        public static void Create(Panel c, MessageType mt, string text)
        {
            string wrapped = Wrap(text, 30);
            Graphics g = c.CreateGraphics();
            SizeF textSize = g.MeasureString(wrapped, new Font("Arial", 10));

            TextBox t = new TextBox();
            t.Multiline = true;
            t.WordWrap = true;
            t.Height = (int)textSize.Height + 10;
            t.Text = text;
            t.Width = (int)textSize.Width + 10;
            t.ReadOnly = true;
            t.Font = new Font("Arial", 10);
            int x = 5;
            if (mt == MessageType.Sent)
            {
                x = c.Width - t.Width - c.AutoScrollMargin.Width - 30;
                t.BackColor = Color.DodgerBlue;
                t.ForeColor = Color.White;
            }
            t.Location = new Point(x, starty);
            starty += t.Height + 5;
            c.Controls.Add(t);
        }

        public static void ResetHeight()
        {
            starty = 25;
        }

        private static string Wrap(string text, int length)
        {
            /*
             Wrap a message so that it doesn't take up the full screen width.
             */
            if (text.Length <= length)
                return text;
            string[] words = text.Split(' ');
            if (words.Length == 1)
                return string.Join("\n", SplitStringNoSpaces(text, 30));
            string endText = "";
            string part = string.Empty;
            int partCounter = 0;
            foreach (var word in words)
            {
                if (part.Length + word.Length < length)
                {
                    part += string.IsNullOrEmpty(part) ? word : " " + word;
                }
                else
                {
                    endText += part + "\n";
                    part = word;
                    partCounter++;
                }
            }
            endText += part + "\n";
            return endText;
        }

        static IEnumerable<string> SplitStringNoSpaces(string str, int chunkSize)
        {
            /*
             Ensure that a long string (with no spaces for splitting) is not shown as one long line;
             it shoud still get split across lines.
             */
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public enum MessageType
        {
            Sent,
            Received,
        }
    }
}
