﻿namespace ArduinoPhone
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.replyBox = new System.Windows.Forms.TextBox();
            this.messageTitle = new System.Windows.Forms.Label();
            this.charCount = new System.Windows.Forms.Label();
            this.newMsg = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.newCallNumber = new System.Windows.Forms.Label();
            this.decline = new System.Windows.Forms.Label();
            this.answer = new System.Windows.Forms.Label();
            this.callNotification = new System.Windows.Forms.GroupBox();
            this.endCall = new System.Windows.Forms.Label();
            this.recipientNumber = new System.Windows.Forms.Label();
            this.btnCall = new System.Windows.Forms.Button();
            this.deleteConvo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newNumber = new System.Windows.Forms.TextBox();
            this.copyMessageText = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyText = new System.Windows.Forms.ToolStripMenuItem();
            this.serial = new System.IO.Ports.SerialPort(this.components);
            this.conversationView = new Conversation();
            this.messageViewer = new ArduinoPhone.MessageControl();
            this.callNotification.SuspendLayout();
            this.deleteConvo.SuspendLayout();
            this.copyMessageText.SuspendLayout();
            this.SuspendLayout();
            // 
            // replyBox
            // 
            this.replyBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replyBox.Enabled = false;
            this.replyBox.Location = new System.Drawing.Point(332, 552);
            this.replyBox.Multiline = true;
            this.replyBox.Name = "replyBox";
            this.replyBox.Size = new System.Drawing.Size(293, 20);
            this.replyBox.TabIndex = 4;
            this.replyBox.TextChanged += new System.EventHandler(this.replyBox_TextChanged);
            // 
            // messageTitle
            // 
            this.messageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageTitle.ForeColor = System.Drawing.Color.Black;
            this.messageTitle.Location = new System.Drawing.Point(82, 9);
            this.messageTitle.Name = "messageTitle";
            this.messageTitle.Size = new System.Drawing.Size(142, 32);
            this.messageTitle.TabIndex = 9;
            this.messageTitle.Text = "Messages";
            this.messageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // charCount
            // 
            this.charCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.charCount.AutoSize = true;
            this.charCount.Enabled = false;
            this.charCount.Location = new System.Drawing.Point(332, 575);
            this.charCount.Name = "charCount";
            this.charCount.Size = new System.Drawing.Size(42, 13);
            this.charCount.TabIndex = 14;
            this.charCount.Text = "0 / 160";
            // 
            // newMsg
            // 
            this.newMsg.AutoSize = true;
            this.newMsg.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.newMsg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.newMsg.FlatAppearance.BorderSize = 0;
            this.newMsg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newMsg.ForeColor = System.Drawing.Color.DodgerBlue;
            this.newMsg.Location = new System.Drawing.Point(278, 12);
            this.newMsg.Name = "newMsg";
            this.newMsg.Size = new System.Drawing.Size(48, 26);
            this.newMsg.TabIndex = 16;
            this.newMsg.Text = "New";
            this.newMsg.UseVisualStyleBackColor = true;
            this.newMsg.Click += new System.EventHandler(this.newMsg_Click);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.AutoSize = true;
            this.btnSend.BackColor = System.Drawing.Color.White;
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Enabled = false;
            this.btnSend.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.ForeColor = System.Drawing.Color.DodgerBlue;
            this.btnSend.Location = new System.Drawing.Point(631, 547);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(46, 28);
            this.btnSend.TabIndex = 18;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // newCallNumber
            // 
            this.newCallNumber.BackColor = System.Drawing.Color.White;
            this.newCallNumber.Location = new System.Drawing.Point(6, 0);
            this.newCallNumber.Name = "newCallNumber";
            this.newCallNumber.Size = new System.Drawing.Size(168, 25);
            this.newCallNumber.TabIndex = 25;
            this.newCallNumber.Text = "Incoming call: None";
            this.newCallNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // decline
            // 
            this.decline.BackColor = System.Drawing.Color.White;
            this.decline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.decline.Location = new System.Drawing.Point(105, 40);
            this.decline.Name = "decline";
            this.decline.Size = new System.Drawing.Size(69, 25);
            this.decline.TabIndex = 26;
            this.decline.Text = "Decline";
            this.decline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.decline.Click += new System.EventHandler(this.decline_Click);
            // 
            // answer
            // 
            this.answer.BackColor = System.Drawing.Color.White;
            this.answer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.answer.Location = new System.Drawing.Point(6, 40);
            this.answer.Name = "answer";
            this.answer.Size = new System.Drawing.Size(69, 25);
            this.answer.TabIndex = 27;
            this.answer.Text = "Answer";
            this.answer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.answer.Click += new System.EventHandler(this.answer_Click);
            // 
            // callNotification
            // 
            this.callNotification.BackColor = System.Drawing.Color.White;
            this.callNotification.Controls.Add(this.endCall);
            this.callNotification.Controls.Add(this.newCallNumber);
            this.callNotification.Controls.Add(this.decline);
            this.callNotification.Controls.Add(this.answer);
            this.callNotification.Location = new System.Drawing.Point(230, 6);
            this.callNotification.Name = "callNotification";
            this.callNotification.Size = new System.Drawing.Size(180, 75);
            this.callNotification.TabIndex = 29;
            this.callNotification.TabStop = false;
            this.callNotification.Visible = false;
            // 
            // endCall
            // 
            this.endCall.BackColor = System.Drawing.Color.White;
            this.endCall.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.endCall.Location = new System.Drawing.Point(6, 40);
            this.endCall.Name = "endCall";
            this.endCall.Size = new System.Drawing.Size(168, 25);
            this.endCall.TabIndex = 32;
            this.endCall.Text = "End call";
            this.endCall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.endCall.Visible = false;
            this.endCall.Click += new System.EventHandler(this.endCall_Click);
            // 
            // recipientNumber
            // 
            this.recipientNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recipientNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recipientNumber.Location = new System.Drawing.Point(435, 9);
            this.recipientNumber.Name = "recipientNumber";
            this.recipientNumber.Size = new System.Drawing.Size(140, 32);
            this.recipientNumber.TabIndex = 30;
            this.recipientNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCall
            // 
            this.btnCall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCall.AutoSize = true;
            this.btnCall.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCall.FlatAppearance.BorderSize = 0;
            this.btnCall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCall.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCall.ForeColor = System.Drawing.Color.DodgerBlue;
            this.btnCall.Location = new System.Drawing.Point(636, 12);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(41, 26);
            this.btnCall.TabIndex = 31;
            this.btnCall.Text = "Call";
            this.btnCall.UseVisualStyleBackColor = true;
            this.btnCall.Visible = false;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // deleteConvo
            // 
            this.deleteConvo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteItem});
            this.deleteConvo.Name = "deleteConvo";
            this.deleteConvo.ShowImageMargin = false;
            this.deleteConvo.Size = new System.Drawing.Size(83, 26);
            // 
            // deleteItem
            // 
            this.deleteItem.Name = "deleteItem";
            this.deleteItem.Size = new System.Drawing.Size(82, 22);
            this.deleteItem.Text = "Delete";
            this.deleteItem.Click += new System.EventHandler(this.deleteItem_Click);
            // 
            // newNumber
            // 
            this.newNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newNumber.BackColor = System.Drawing.Color.White;
            this.newNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.newNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newNumber.Location = new System.Drawing.Point(380, 17);
            this.newNumber.Name = "newNumber";
            this.newNumber.Size = new System.Drawing.Size(250, 15);
            this.newNumber.TabIndex = 23;
            this.newNumber.Visible = false;
            // 
            // copyMessageText
            // 
            this.copyMessageText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyText});
            this.copyMessageText.Name = "copyMessageText";
            this.copyMessageText.ShowImageMargin = false;
            this.copyMessageText.Size = new System.Drawing.Size(78, 26);
            // 
            // copyText
            // 
            this.copyText.Name = "copyText";
            this.copyText.Size = new System.Drawing.Size(77, 22);
            this.copyText.Text = "Copy";
            this.copyText.Click += new System.EventHandler(this.copyText_Click);
            // 
            // serial
            // 
            this.serial.BaudRate = 115200;
            this.serial.PortName = "COM3";
            this.serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.DataReceived);
            // 
            // conversationView
            // 
            this.conversationView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.conversationView.AutoScroll = true;
            this.conversationView.BackColor = System.Drawing.Color.White;
            this.conversationView.Location = new System.Drawing.Point(6, 44);
            this.conversationView.Name = "conversationView";
            this.conversationView.Size = new System.Drawing.Size(320, 555);
            this.conversationView.TabIndex = 0;
            this.conversationView.Text = "conversation1";
            this.conversationView.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.ConversationView_ControlAdded);
            // 
            // messageViewer
            // 
            this.messageViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageViewer.AutoScroll = true;
            this.messageViewer.BackColor = System.Drawing.Color.White;
            this.messageViewer.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.messageViewer.Location = new System.Drawing.Point(332, 44);
            this.messageViewer.Name = "messageViewer";
            this.messageViewer.Size = new System.Drawing.Size(345, 497);
            this.messageViewer.TabIndex = 24;
            this.messageViewer.Text = "messageControl1";
            this.messageViewer.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.MessageViewer_ControlAdded);
            this.messageViewer.Resize += new System.EventHandler(this.MessageViewer_Resize);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(684, 597);
            this.Controls.Add(this.newNumber);
            this.Controls.Add(this.btnCall);
            this.Controls.Add(this.recipientNumber);
            this.Controls.Add(this.callNotification);
            this.Controls.Add(this.conversationView);
            this.Controls.Add(this.messageViewer);
            this.Controls.Add(this.replyBox);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.newMsg);
            this.Controls.Add(this.charCount);
            this.Controls.Add(this.messageTitle);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ArduinoPhone";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.callNotification.ResumeLayout(false);
            this.deleteConvo.ResumeLayout(false);
            this.copyMessageText.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox replyBox;
        private System.Windows.Forms.Label messageTitle;
        private System.Windows.Forms.Label charCount;
        private System.Windows.Forms.Button newMsg;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox newNumber;
        private MessageControl messageViewer;
        private Conversation conversationView;
        private System.Windows.Forms.Label newCallNumber;
        private System.Windows.Forms.Label decline;
        private System.Windows.Forms.Label answer;
        private System.Windows.Forms.GroupBox callNotification;
        private System.Windows.Forms.Label recipientNumber;
        private System.Windows.Forms.Button btnCall;
        private System.Windows.Forms.ContextMenuStrip deleteConvo;
        private System.Windows.Forms.ToolStripMenuItem deleteItem;
        private System.Windows.Forms.Label endCall;
        private System.Windows.Forms.ContextMenuStrip copyMessageText;
        private System.Windows.Forms.ToolStripMenuItem copyText;
        private System.IO.Ports.SerialPort serial;
    }
}

