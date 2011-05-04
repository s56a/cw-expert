namespace CWExpert
{
    partial class CWExpert
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
            Audio.callback_return = 2;
            Audio.StopAudio();

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
            this.txtCall = new System.Windows.Forms.TextBox();
            this.btnSendCall = new System.Windows.Forms.Button();
            this.btnF1 = new System.Windows.Forms.Button();
            this.btnF2 = new System.Windows.Forms.Button();
            this.btnF3 = new System.Windows.Forms.Button();
            this.btnF4 = new System.Windows.Forms.Button();
            this.btnF5 = new System.Windows.Forms.Button();
            this.btnF6 = new System.Windows.Forms.Button();
            this.btnF8 = new System.Windows.Forms.Button();
            this.btnF7 = new System.Windows.Forms.Button();
            this.btnStartMR = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.setupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSendRST = new System.Windows.Forms.Button();
            this.txtRST = new System.Windows.Forms.TextBox();
            this.btnSendNr = new System.Windows.Forms.Button();
            this.txtNr = new System.Windows.Forms.TextBox();
            this.lblCall = new System.Windows.Forms.Label();
            this.lblRST = new System.Windows.Forms.Label();
            this.lblNr = new System.Windows.Forms.Label();
            this.btnStopMR = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtChannel4 = new System.Windows.Forms.TextBox();
            this.btngrab = new System.Windows.Forms.Button();
            this.btnclr = new System.Windows.Forms.Button();
            this.txtChannel5 = new System.Windows.Forms.TextBox();
            this.txtChannel6 = new System.Windows.Forms.TextBox();
            this.txtChannel7 = new System.Windows.Forms.TextBox();
            this.txtChannel11 = new System.Windows.Forms.TextBox();
            this.txtChannel10 = new System.Windows.Forms.TextBox();
            this.txtChannel9 = new System.Windows.Forms.TextBox();
            this.txtChannel8 = new System.Windows.Forms.TextBox();
            this.txtChannel15 = new System.Windows.Forms.TextBox();
            this.txtChannel14 = new System.Windows.Forms.TextBox();
            this.txtChannel13 = new System.Windows.Forms.TextBox();
            this.txtChannel12 = new System.Windows.Forms.TextBox();
            this.txtChannel19 = new System.Windows.Forms.TextBox();
            this.txtChannel18 = new System.Windows.Forms.TextBox();
            this.txtChannel17 = new System.Windows.Forms.TextBox();
            this.txtChannel16 = new System.Windows.Forms.TextBox();
            this.txtChannel3 = new System.Windows.Forms.TextBox();
            this.txtChannel2 = new System.Windows.Forms.TextBox();
            this.txtChannel20 = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCall
            // 
            this.txtCall.Location = new System.Drawing.Point(70, 36);
            this.txtCall.MaxLength = 16;
            this.txtCall.Name = "txtCall";
            this.txtCall.Size = new System.Drawing.Size(132, 20);
            this.txtCall.TabIndex = 0;
            this.txtCall.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCall_KeyUp);
            // 
            // btnSendCall
            // 
            this.btnSendCall.Location = new System.Drawing.Point(316, 36);
            this.btnSendCall.Name = "btnSendCall";
            this.btnSendCall.Size = new System.Drawing.Size(62, 23);
            this.btnSendCall.TabIndex = 1;
            this.btnSendCall.Text = "Send";
            this.btnSendCall.UseVisualStyleBackColor = true;
            this.btnSendCall.Click += new System.EventHandler(this.btnSendCall_Click);
            // 
            // btnF1
            // 
            this.btnF1.Location = new System.Drawing.Point(76, 486);
            this.btnF1.Name = "btnF1";
            this.btnF1.Size = new System.Drawing.Size(62, 23);
            this.btnF1.TabIndex = 2;
            this.btnF1.Text = "F1";
            this.btnF1.UseVisualStyleBackColor = true;
            this.btnF1.Click += new System.EventHandler(this.btnF1_Click);
            // 
            // btnF2
            // 
            this.btnF2.Location = new System.Drawing.Point(138, 486);
            this.btnF2.Name = "btnF2";
            this.btnF2.Size = new System.Drawing.Size(62, 23);
            this.btnF2.TabIndex = 3;
            this.btnF2.Text = "F2";
            this.btnF2.UseVisualStyleBackColor = true;
            this.btnF2.Click += new System.EventHandler(this.btnF2_Click);
            // 
            // btnF3
            // 
            this.btnF3.Location = new System.Drawing.Point(200, 486);
            this.btnF3.Name = "btnF3";
            this.btnF3.Size = new System.Drawing.Size(62, 23);
            this.btnF3.TabIndex = 4;
            this.btnF3.Text = "F3";
            this.btnF3.UseVisualStyleBackColor = true;
            this.btnF3.Click += new System.EventHandler(this.btnF3_Click);
            // 
            // btnF4
            // 
            this.btnF4.Location = new System.Drawing.Point(262, 486);
            this.btnF4.Name = "btnF4";
            this.btnF4.Size = new System.Drawing.Size(62, 23);
            this.btnF4.TabIndex = 5;
            this.btnF4.Text = "F4";
            this.btnF4.UseVisualStyleBackColor = true;
            this.btnF4.Click += new System.EventHandler(this.btnF4_Click);
            // 
            // btnF5
            // 
            this.btnF5.Location = new System.Drawing.Point(76, 530);
            this.btnF5.Name = "btnF5";
            this.btnF5.Size = new System.Drawing.Size(62, 23);
            this.btnF5.TabIndex = 6;
            this.btnF5.Text = "F5";
            this.btnF5.UseVisualStyleBackColor = true;
            this.btnF5.Click += new System.EventHandler(this.btnF5_Click);
            // 
            // btnF6
            // 
            this.btnF6.Location = new System.Drawing.Point(138, 530);
            this.btnF6.Name = "btnF6";
            this.btnF6.Size = new System.Drawing.Size(62, 23);
            this.btnF6.TabIndex = 7;
            this.btnF6.Text = "F6";
            this.btnF6.UseVisualStyleBackColor = true;
            this.btnF6.Click += new System.EventHandler(this.btnF6_Click);
            // 
            // btnF8
            // 
            this.btnF8.Location = new System.Drawing.Point(262, 530);
            this.btnF8.Name = "btnF8";
            this.btnF8.Size = new System.Drawing.Size(62, 23);
            this.btnF8.TabIndex = 9;
            this.btnF8.Text = "F8";
            this.btnF8.UseVisualStyleBackColor = true;
            this.btnF8.Click += new System.EventHandler(this.btnF8_Click);
            // 
            // btnF7
            // 
            this.btnF7.Location = new System.Drawing.Point(200, 530);
            this.btnF7.Name = "btnF7";
            this.btnF7.Size = new System.Drawing.Size(62, 23);
            this.btnF7.TabIndex = 8;
            this.btnF7.Text = "F7";
            this.btnF7.UseVisualStyleBackColor = true;
            this.btnF7.Click += new System.EventHandler(this.btnF7_Click);
            // 
            // btnStartMR
            // 
            this.btnStartMR.Location = new System.Drawing.Point(76, 448);
            this.btnStartMR.Name = "btnStartMR";
            this.btnStartMR.Size = new System.Drawing.Size(62, 23);
            this.btnStartMR.TabIndex = 10;
            this.btnStartMR.Text = "Start ";
            this.btnStartMR.UseVisualStyleBackColor = true;
            this.btnStartMR.Click += new System.EventHandler(this.btnStartMR_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(401, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // setupMenuItem
            // 
            this.setupMenuItem.Name = "setupMenuItem";
            this.setupMenuItem.Size = new System.Drawing.Size(49, 20);
            this.setupMenuItem.Text = "Setup";
            this.setupMenuItem.Click += new System.EventHandler(this.setupMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // btnSendRST
            // 
            this.btnSendRST.Location = new System.Drawing.Point(316, 176);
            this.btnSendRST.Name = "btnSendRST";
            this.btnSendRST.Size = new System.Drawing.Size(62, 23);
            this.btnSendRST.TabIndex = 14;
            this.btnSendRST.Text = "Send";
            this.btnSendRST.UseVisualStyleBackColor = true;
            this.btnSendRST.Click += new System.EventHandler(this.btnSendRST_Click);
            // 
            // txtRST
            // 
            this.txtRST.Location = new System.Drawing.Point(316, 150);
            this.txtRST.MaxLength = 3;
            this.txtRST.Name = "txtRST";
            this.txtRST.Size = new System.Drawing.Size(62, 20);
            this.txtRST.TabIndex = 13;
            this.txtRST.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRST_KeyUp);
            // 
            // btnSendNr
            // 
            this.btnSendNr.Location = new System.Drawing.Point(316, 237);
            this.btnSendNr.Name = "btnSendNr";
            this.btnSendNr.Size = new System.Drawing.Size(62, 23);
            this.btnSendNr.TabIndex = 16;
            this.btnSendNr.Text = "Send";
            this.btnSendNr.UseVisualStyleBackColor = true;
            this.btnSendNr.Click += new System.EventHandler(this.btnSendNr_Click);
            // 
            // txtNr
            // 
            this.txtNr.Location = new System.Drawing.Point(316, 266);
            this.txtNr.MaxLength = 5;
            this.txtNr.Name = "txtNr";
            this.txtNr.Size = new System.Drawing.Size(62, 20);
            this.txtNr.TabIndex = 15;
            this.txtNr.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtNr_KeyUp);
            // 
            // lblCall
            // 
            this.lblCall.AutoSize = true;
            this.lblCall.Location = new System.Drawing.Point(31, 39);
            this.lblCall.Name = "lblCall";
            this.lblCall.Size = new System.Drawing.Size(33, 13);
            this.lblCall.TabIndex = 17;
            this.lblCall.Text = "CALL";
            // 
            // lblRST
            // 
            this.lblRST.AutoSize = true;
            this.lblRST.Location = new System.Drawing.Point(332, 134);
            this.lblRST.Name = "lblRST";
            this.lblRST.Size = new System.Drawing.Size(29, 13);
            this.lblRST.TabIndex = 18;
            this.lblRST.Text = "RST";
            // 
            // lblNr
            // 
            this.lblNr.AutoSize = true;
            this.lblNr.Location = new System.Drawing.Point(332, 289);
            this.lblNr.Name = "lblNr";
            this.lblNr.Size = new System.Drawing.Size(30, 13);
            this.lblNr.TabIndex = 19;
            this.lblNr.Text = "NBR";
            // 
            // btnStopMR
            // 
            this.btnStopMR.Location = new System.Drawing.Point(262, 448);
            this.btnStopMR.Name = "btnStopMR";
            this.btnStopMR.Size = new System.Drawing.Size(62, 23);
            this.btnStopMR.TabIndex = 20;
            this.btnStopMR.Text = "Stop";
            this.btnStopMR.UseVisualStyleBackColor = true;
            this.btnStopMR.Click += new System.EventHandler(this.btnStopMR_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(159, 453);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "S56A - YT7PWR";
            // 
            // txtChannel4
            // 
            this.txtChannel4.Location = new System.Drawing.Point(31, 97);
            this.txtChannel4.MaxLength = 32768;
            this.txtChannel4.Name = "txtChannel4";
            this.txtChannel4.Size = new System.Drawing.Size(271, 20);
            this.txtChannel4.TabIndex = 22;
            // 
            // btngrab
            // 
            this.btngrab.Location = new System.Drawing.Point(316, 77);
            this.btngrab.Name = "btngrab";
            this.btngrab.Size = new System.Drawing.Size(62, 23);
            this.btngrab.TabIndex = 23;
            this.btngrab.Text = "Grab";
            this.btngrab.UseVisualStyleBackColor = true;
            this.btngrab.Click += new System.EventHandler(this.btngrab_Click);
            // 
            // btnclr
            // 
            this.btnclr.Location = new System.Drawing.Point(316, 106);
            this.btnclr.Name = "btnclr";
            this.btnclr.Size = new System.Drawing.Size(62, 23);
            this.btnclr.TabIndex = 24;
            this.btnclr.Text = "Clear";
            this.btnclr.UseVisualStyleBackColor = true;
            this.btnclr.Click += new System.EventHandler(this.btnclr_Click);
            // 
            // txtChannel5
            // 
            this.txtChannel5.Location = new System.Drawing.Point(31, 117);
            this.txtChannel5.MaxLength = 32768;
            this.txtChannel5.Name = "txtChannel5";
            this.txtChannel5.Size = new System.Drawing.Size(271, 20);
            this.txtChannel5.TabIndex = 25;
            // 
            // txtChannel6
            // 
            this.txtChannel6.Location = new System.Drawing.Point(31, 137);
            this.txtChannel6.MaxLength = 32768;
            this.txtChannel6.Name = "txtChannel6";
            this.txtChannel6.Size = new System.Drawing.Size(271, 20);
            this.txtChannel6.TabIndex = 26;
            // 
            // txtChannel7
            // 
            this.txtChannel7.Location = new System.Drawing.Point(31, 157);
            this.txtChannel7.MaxLength = 32768;
            this.txtChannel7.Name = "txtChannel7";
            this.txtChannel7.Size = new System.Drawing.Size(271, 20);
            this.txtChannel7.TabIndex = 27;
            // 
            // txtChannel11
            // 
            this.txtChannel11.Location = new System.Drawing.Point(31, 237);
            this.txtChannel11.MaxLength = 32768;
            this.txtChannel11.Name = "txtChannel11";
            this.txtChannel11.Size = new System.Drawing.Size(271, 20);
            this.txtChannel11.TabIndex = 31;
            // 
            // txtChannel10
            // 
            this.txtChannel10.Location = new System.Drawing.Point(31, 217);
            this.txtChannel10.MaxLength = 32768;
            this.txtChannel10.Name = "txtChannel10";
            this.txtChannel10.Size = new System.Drawing.Size(271, 20);
            this.txtChannel10.TabIndex = 30;
            // 
            // txtChannel9
            // 
            this.txtChannel9.Location = new System.Drawing.Point(31, 197);
            this.txtChannel9.MaxLength = 32768;
            this.txtChannel9.Name = "txtChannel9";
            this.txtChannel9.Size = new System.Drawing.Size(271, 20);
            this.txtChannel9.TabIndex = 29;
            // 
            // txtChannel8
            // 
            this.txtChannel8.Location = new System.Drawing.Point(31, 177);
            this.txtChannel8.MaxLength = 32768;
            this.txtChannel8.Name = "txtChannel8";
            this.txtChannel8.Size = new System.Drawing.Size(271, 20);
            this.txtChannel8.TabIndex = 28;
            // 
            // txtChannel15
            // 
            this.txtChannel15.Location = new System.Drawing.Point(31, 317);
            this.txtChannel15.MaxLength = 32768;
            this.txtChannel15.Name = "txtChannel15";
            this.txtChannel15.Size = new System.Drawing.Size(271, 20);
            this.txtChannel15.TabIndex = 35;
            // 
            // txtChannel14
            // 
            this.txtChannel14.Location = new System.Drawing.Point(31, 297);
            this.txtChannel14.MaxLength = 32768;
            this.txtChannel14.Name = "txtChannel14";
            this.txtChannel14.Size = new System.Drawing.Size(271, 20);
            this.txtChannel14.TabIndex = 34;
            // 
            // txtChannel13
            // 
            this.txtChannel13.Location = new System.Drawing.Point(31, 277);
            this.txtChannel13.MaxLength = 32768;
            this.txtChannel13.Name = "txtChannel13";
            this.txtChannel13.Size = new System.Drawing.Size(271, 20);
            this.txtChannel13.TabIndex = 33;
            // 
            // txtChannel12
            // 
            this.txtChannel12.Location = new System.Drawing.Point(31, 257);
            this.txtChannel12.MaxLength = 32768;
            this.txtChannel12.Name = "txtChannel12";
            this.txtChannel12.Size = new System.Drawing.Size(271, 20);
            this.txtChannel12.TabIndex = 32;
            // 
            // txtChannel19
            // 
            this.txtChannel19.Location = new System.Drawing.Point(31, 397);
            this.txtChannel19.MaxLength = 32768;
            this.txtChannel19.Name = "txtChannel19";
            this.txtChannel19.Size = new System.Drawing.Size(271, 20);
            this.txtChannel19.TabIndex = 39;
            // 
            // txtChannel18
            // 
            this.txtChannel18.Location = new System.Drawing.Point(31, 377);
            this.txtChannel18.MaxLength = 32768;
            this.txtChannel18.Name = "txtChannel18";
            this.txtChannel18.Size = new System.Drawing.Size(271, 20);
            this.txtChannel18.TabIndex = 38;
            // 
            // txtChannel17
            // 
            this.txtChannel17.Location = new System.Drawing.Point(31, 357);
            this.txtChannel17.MaxLength = 32768;
            this.txtChannel17.Name = "txtChannel17";
            this.txtChannel17.Size = new System.Drawing.Size(271, 20);
            this.txtChannel17.TabIndex = 37;
            // 
            // txtChannel16
            // 
            this.txtChannel16.Location = new System.Drawing.Point(31, 337);
            this.txtChannel16.MaxLength = 32768;
            this.txtChannel16.Name = "txtChannel16";
            this.txtChannel16.Size = new System.Drawing.Size(271, 20);
            this.txtChannel16.TabIndex = 36;
            // 
            // txtChannel3
            // 
            this.txtChannel3.Location = new System.Drawing.Point(31, 77);
            this.txtChannel3.MaxLength = 32768;
            this.txtChannel3.Name = "txtChannel3";
            this.txtChannel3.Size = new System.Drawing.Size(271, 20);
            this.txtChannel3.TabIndex = 40;
            // 
            // txtChannel2
            // 
            this.txtChannel2.Location = new System.Drawing.Point(31, 57);
            this.txtChannel2.MaxLength = 32768;
            this.txtChannel2.Name = "txtChannel2";
            this.txtChannel2.Size = new System.Drawing.Size(271, 20);
            this.txtChannel2.TabIndex = 41;
            // 
            // txtChannel20
            // 
            this.txtChannel20.Location = new System.Drawing.Point(31, 417);
            this.txtChannel20.MaxLength = 32768;
            this.txtChannel20.Name = "txtChannel20";
            this.txtChannel20.Size = new System.Drawing.Size(271, 20);
            this.txtChannel20.TabIndex = 42;
            // 
            // CWExpert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 562);
            this.Controls.Add(this.txtChannel20);
            this.Controls.Add(this.txtChannel2);
            this.Controls.Add(this.txtChannel3);
            this.Controls.Add(this.txtChannel19);
            this.Controls.Add(this.txtChannel18);
            this.Controls.Add(this.txtChannel17);
            this.Controls.Add(this.txtChannel16);
            this.Controls.Add(this.txtChannel15);
            this.Controls.Add(this.txtChannel14);
            this.Controls.Add(this.txtChannel13);
            this.Controls.Add(this.txtChannel12);
            this.Controls.Add(this.txtChannel11);
            this.Controls.Add(this.txtChannel10);
            this.Controls.Add(this.txtChannel9);
            this.Controls.Add(this.txtChannel8);
            this.Controls.Add(this.txtChannel7);
            this.Controls.Add(this.txtChannel6);
            this.Controls.Add(this.txtChannel5);
            this.Controls.Add(this.btnclr);
            this.Controls.Add(this.btngrab);
            this.Controls.Add(this.txtChannel4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStopMR);
            this.Controls.Add(this.lblNr);
            this.Controls.Add(this.lblRST);
            this.Controls.Add(this.lblCall);
            this.Controls.Add(this.btnSendNr);
            this.Controls.Add(this.txtNr);
            this.Controls.Add(this.btnSendRST);
            this.Controls.Add(this.txtRST);
            this.Controls.Add(this.btnStartMR);
            this.Controls.Add(this.btnF8);
            this.Controls.Add(this.btnF7);
            this.Controls.Add(this.btnF6);
            this.Controls.Add(this.btnF5);
            this.Controls.Add(this.btnF4);
            this.Controls.Add(this.btnF3);
            this.Controls.Add(this.btnF2);
            this.Controls.Add(this.btnF1);
            this.Controls.Add(this.btnSendCall);
            this.Controls.Add(this.txtCall);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(420, 600);
            this.MinimumSize = new System.Drawing.Size(320, 465);
            this.Name = "CWExpert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CWExpert  v1.2.7";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CWExpert_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtCall;
        private System.Windows.Forms.Button btnSendCall;
        private System.Windows.Forms.Button btnF1;
        private System.Windows.Forms.Button btnF2;
        private System.Windows.Forms.Button btnF3;
        private System.Windows.Forms.Button btnF4;
        private System.Windows.Forms.Button btnF5;
        private System.Windows.Forms.Button btnF6;
        private System.Windows.Forms.Button btnF8;
        private System.Windows.Forms.Button btnF7;
        private System.Windows.Forms.Button btnStartMR;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button btnSendRST;
        private System.Windows.Forms.TextBox txtRST;
        private System.Windows.Forms.Button btnSendNr;
        private System.Windows.Forms.TextBox txtNr;
        private System.Windows.Forms.Label lblCall;
        private System.Windows.Forms.Label lblRST;
        private System.Windows.Forms.Label lblNr;
        private System.Windows.Forms.Button btnStopMR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btngrab;
        private System.Windows.Forms.Button btnclr;
        public System.Windows.Forms.TextBox txtChannel4;
        public System.Windows.Forms.TextBox txtChannel5;
        public System.Windows.Forms.TextBox txtChannel6;
        public System.Windows.Forms.TextBox txtChannel7;
        public System.Windows.Forms.TextBox txtChannel11;
        public System.Windows.Forms.TextBox txtChannel10;
        public System.Windows.Forms.TextBox txtChannel9;
        public System.Windows.Forms.TextBox txtChannel8;
        public System.Windows.Forms.TextBox txtChannel15;
        public System.Windows.Forms.TextBox txtChannel14;
        public System.Windows.Forms.TextBox txtChannel13;
        public System.Windows.Forms.TextBox txtChannel12;
        public System.Windows.Forms.TextBox txtChannel19;
        public System.Windows.Forms.TextBox txtChannel18;
        public System.Windows.Forms.TextBox txtChannel17;
        public System.Windows.Forms.TextBox txtChannel16;
        public System.Windows.Forms.TextBox txtChannel3;
        public System.Windows.Forms.TextBox txtChannel2;
        public System.Windows.Forms.TextBox txtChannel20;
    }
}

