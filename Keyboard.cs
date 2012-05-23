﻿//=================================================================
// Keyboard.cs
//=================================================================
// Copyright (C) 2012 S56A YT7PWR
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//=================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;


namespace CWExpert
{
    public partial class Keyboard : Form
    {
        #region DLL imports

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion

        #region variable

        CWExpert MainForm;
        public RichTextBox rtbKeyboardText;
        private Button btnSave;
        private Button btnClear;
        private Button btn1;
        private Button btn2;
        private Button btn4;
        private Button btn3;
        private Button btn6;
        private Button btn5;
        private Button btn8;
        private Button btn7;
        private Button btnSettings;
        private Button btnSend;
        KeyboardSetup SetupForm;
        string CMD1 = "";
        string CMD2 = "";
        string CMD3 = "";
        string CMD4 = "";
        string CMD5 = "";
        string CMD6 = "";
        string CMD7 = "";
        string CMD8 = "";

        #endregion

        #region windows generate code

        private void InitializeComponent()
        {
            this.rtbKeyboardText = new System.Windows.Forms.RichTextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn6 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.btn8 = new System.Windows.Forms.Button();
            this.btn7 = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbKeyboardText
            // 
            this.rtbKeyboardText.BackColor = System.Drawing.Color.Black;
            this.rtbKeyboardText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbKeyboardText.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbKeyboardText.ForeColor = System.Drawing.Color.LawnGreen;
            this.rtbKeyboardText.Location = new System.Drawing.Point(96, 16);
            this.rtbKeyboardText.MaxLength = 32767;
            this.rtbKeyboardText.Name = "rtbKeyboardText";
            this.rtbKeyboardText.Size = new System.Drawing.Size(345, 183);
            this.rtbKeyboardText.TabIndex = 1;
            this.rtbKeyboardText.Text = "";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Location = new System.Drawing.Point(273, 219);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save(F11)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClear.ForeColor = System.Drawing.Color.Black;
            this.btnClear.Location = new System.Drawing.Point(93, 219);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear(F9)";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSend.ForeColor = System.Drawing.Color.Black;
            this.btnSend.Location = new System.Drawing.Point(183, 219);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send(F10)";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btn1
            // 
            this.btn1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn1.ForeColor = System.Drawing.Color.Black;
            this.btn1.Location = new System.Drawing.Point(10, 16);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(70, 23);
            this.btn1.TabIndex = 5;
            this.btn1.Text = "1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn2
            // 
            this.btn2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn2.ForeColor = System.Drawing.Color.Black;
            this.btn2.Location = new System.Drawing.Point(10, 45);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(70, 23);
            this.btn2.TabIndex = 6;
            this.btn2.Text = "2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btn4
            // 
            this.btn4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn4.ForeColor = System.Drawing.Color.Black;
            this.btn4.Location = new System.Drawing.Point(10, 103);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(70, 23);
            this.btn4.TabIndex = 8;
            this.btn4.Text = "4";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btn3
            // 
            this.btn3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn3.ForeColor = System.Drawing.Color.Black;
            this.btn3.Location = new System.Drawing.Point(10, 74);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(70, 23);
            this.btn3.TabIndex = 7;
            this.btn3.Text = "3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btn6
            // 
            this.btn6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn6.ForeColor = System.Drawing.Color.Black;
            this.btn6.Location = new System.Drawing.Point(10, 161);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(70, 23);
            this.btn6.TabIndex = 10;
            this.btn6.Text = "6";
            this.btn6.UseVisualStyleBackColor = true;
            this.btn6.Click += new System.EventHandler(this.button6_Click);
            // 
            // btn5
            // 
            this.btn5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn5.ForeColor = System.Drawing.Color.Black;
            this.btn5.Location = new System.Drawing.Point(10, 132);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(70, 23);
            this.btn5.TabIndex = 9;
            this.btn5.Text = "5";
            this.btn5.UseVisualStyleBackColor = true;
            this.btn5.Click += new System.EventHandler(this.button5_Click);
            // 
            // btn8
            // 
            this.btn8.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn8.ForeColor = System.Drawing.Color.Black;
            this.btn8.Location = new System.Drawing.Point(10, 219);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(70, 23);
            this.btn8.TabIndex = 12;
            this.btn8.Text = "8";
            this.btn8.UseVisualStyleBackColor = true;
            this.btn8.Click += new System.EventHandler(this.button8_Click);
            // 
            // btn7
            // 
            this.btn7.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn7.ForeColor = System.Drawing.Color.Black;
            this.btn7.Location = new System.Drawing.Point(10, 190);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(70, 23);
            this.btn7.TabIndex = 11;
            this.btn7.Text = "7";
            this.btn7.UseVisualStyleBackColor = true;
            this.btn7.Click += new System.EventHandler(this.button7_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSettings.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSettings.ForeColor = System.Drawing.Color.Black;
            this.btnSettings.Location = new System.Drawing.Point(363, 219);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(80, 23);
            this.btnSettings.TabIndex = 13;
            this.btnSettings.Text = "Settings(F12)";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // Keyboard
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.CancelButton = this.btnSave;
            this.ClientSize = new System.Drawing.Size(452, 258);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btn8);
            this.Controls.Add(this.btn7);
            this.Controls.Add(this.btn6);
            this.Controls.Add(this.btn5);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.rtbKeyboardText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(458, 282);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(458, 282);
            this.Name = "Keyboard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "   Keyboard";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Keyboard_Closing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Keyboard_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        #region constructor/destructor

        public Keyboard(CWExpert form)
        {
            try
            {
                this.AutoScaleMode = AutoScaleMode.Inherit;
                InitializeComponent();
                float dpi = this.CreateGraphics().DpiX;
                float ratio = dpi / 96.0f;
                string font_name = this.Font.Name;
                float size = 8.25f / ratio;
                System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
                this.Font = new_font;

                MainForm = form;
                GetOptions();
                SetWindowPos(this.Handle.ToInt32(), -1, this.Left, this.Top,
                        this.Width, this.Height, 0);  // on top others
                UpdateButtons();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        ~Keyboard()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void Keyboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.Hide();
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region Start/Stop

        public void Start(string msg)
        {
            try
            {
                rtbKeyboardText.AppendText(msg);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public void SendMessage(string msg)
        {
            try
            {
                btnSend_Click(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region cross thread

        public void CommandCallback(string action, int param_1, string param_2)
        {
            try
            {
                switch (action)
                {
                    case "Set Keyboard text":
                            if (rtbKeyboardText.Text != "")
                                rtbKeyboardText.Text = rtbKeyboardText.Text.Remove(0, 1);
                        break;

                    case "Reload Keyboard text":
                        Thread.Sleep(100);
                        rtbKeyboardText.Text = param_2;
                        break;

                    case "Send Keyboard text":
                        btnSend_Click(this, EventArgs.Empty);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region Save/Restore settings

        public void SaveOptions()
        {
            try
            {
                ArrayList a = new ArrayList();

                a.Add("Keyboard_top/" + this.Top.ToString());		// save form positions
                a.Add("Keyboard_left/" + this.Left.ToString());
                a.Add("btn1text/" + btn1.Text.ToString());
                a.Add("btn2text/" + btn2.Text.ToString());
                a.Add("btn3text/" + btn3.Text.ToString());
                a.Add("btn4text/" + btn4.Text.ToString());
                a.Add("btn5text/" + btn5.Text.ToString());
                a.Add("btn6text/" + btn6.Text.ToString());
                a.Add("btn7text/" + btn7.Text.ToString());
                a.Add("btn8text/" + btn8.Text.ToString());
                a.Add("btn1cmd/" + CMD1);
                a.Add("btn2cmd/" + CMD2);
                a.Add("btn3cmd/" + CMD3);
                a.Add("btn4cmd/" + CMD4);
                a.Add("btn5cmd/" + CMD5);
                a.Add("btn6cmd/" + CMD6);
                a.Add("btn7cmd/" + CMD7);
                a.Add("btn8cmd/" + CMD8);

                DB.SaveVars("KeyboardOptions", ref a);		        // save the values to the DB
                DB.Update();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Keyboard SaveOptions function!\n" + ex.ToString());
            }
        }

        public void GetOptions()
        {
            try
            {
                ArrayList a = DB.GetVars("KeyboardOptions");
                a.Sort();

                foreach (string s in a)
                {
                    string[] vals = s.Split('/');
                    string name = vals[0];
                    string val = vals[1];

                    if (s.StartsWith("Keyboard_top"))
                    {
                        int top = Int32.Parse(vals[1]);
                        this.Top = top;
                    }
                    else if (s.StartsWith("Keyboard_left"))
                    {
                        int left = Int32.Parse(vals[1]);
                        this.Left = left;
                    }
                    else if (s.StartsWith("btn1text"))
                    {
                        btn1.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton1.Text = vals[1];
                    }
                    else if (s.StartsWith("btn2text"))
                    {
                        btn2.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton2.Text = vals[1];
                    }
                    else if (s.StartsWith("btn3text"))
                    {
                        btn3.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton3.Text = vals[1];
                    }
                    else if (s.StartsWith("btn4text"))
                    {
                        btn4.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton4.Text = vals[1];
                    }
                    else if (s.StartsWith("btn5text"))
                    {
                        btn5.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton5.Text = vals[1];
                    }
                    else if (s.StartsWith("btn6text"))
                    {
                        btn6.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton6.Text = vals[1];
                    }
                    else if (s.StartsWith("btn7text"))
                    {
                        btn7.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton7.Text = vals[1];
                    }
                    else if (s.StartsWith("btn8text"))
                    {
                        btn8.Text = vals[1];

                        if (SetupForm != null)
                            SetupForm.txtButton8.Text = vals[1];
                    }
                    else if (s.StartsWith("btn1cmd"))
                    {
                        CMD1 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn1cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn2cmd"))
                    {
                        CMD2 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn2cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn3cmd"))
                    {
                        CMD3 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn3cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn4cmd"))
                    {
                        CMD4 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn4cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn5cmd"))
                    {
                        CMD5 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn5cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn6cmd"))
                    {
                        CMD6 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn6cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn7cmd"))
                    {
                        CMD7 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn7cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn8cmd"))
                    {
                        CMD8 = vals[1];

                        if (SetupForm != null)
                            SetupForm.btn8cmd.Text = vals[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public void UpdateButtonsText()
        {
            try
            {
                btn1.Text = SetupForm.txtButton1.Text;
                btn2.Text = SetupForm.txtButton2.Text;
                btn3.Text = SetupForm.txtButton3.Text;
                btn4.Text = SetupForm.txtButton4.Text;
                btn5.Text = SetupForm.txtButton5.Text;
                btn6.Text = SetupForm.txtButton6.Text;
                btn7.Text = SetupForm.txtButton7.Text;
                btn8.Text = SetupForm.txtButton8.Text;
                CMD1 = SetupForm.btn1cmd.Text;
                CMD2 = SetupForm.btn2cmd.Text;
                CMD3 = SetupForm.btn3cmd.Text;
                CMD4 = SetupForm.btn4cmd.Text;
                CMD5 = SetupForm.btn1cmd.Text;
                CMD6 = SetupForm.btn2cmd.Text;
                CMD7 = SetupForm.btn3cmd.Text;
                CMD8 = SetupForm.btn4cmd.Text;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region buttons event

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm.AddLOGEntry();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                rtbKeyboardText.Clear();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (rtbKeyboardText.Text == "")
                    return;

                Mode new_mode;

                if (!MainForm.TXSplit)
                    new_mode = MainForm.OpModeVFOA;
                else
                    new_mode = MainForm.OpModeVFOB;

                switch (new_mode)
                {
                    case Mode.CW:
                        MainForm.cwEncoder.Start(rtbKeyboardText.Text.ToString());
                        break;

                    case Mode.RTTY:
                        MainForm.rtty.Start(rtbKeyboardText.Text.ToString());
                        break;

                    case Mode.BPSK31:
                    case Mode.BPSK63:
                    case Mode.BPSK125:
                    case Mode.BPSK250:
                    case Mode.QPSK31:
                    case Mode.QPSK63:
                    case Mode.QPSK125:
                    case Mode.QPSK250:
                        MainForm.psk.Start(rtbKeyboardText.Text.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (SetupForm == null || SetupForm.IsDisposed)
                SetupForm = new KeyboardSetup(this);

            SetupForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msg = CMD1;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string msg = CMD4;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string msg = CMD5;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string msg = CMD6;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string msg = CMD7;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string msg = CMD8;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string msg = CMD3;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string msg = CMD2;
            msg = MacroHandler(msg);
            rtbKeyboardText.AppendText(msg);
        }

        private string MacroHandler(string msg)
        {
            try
            {
                System.DateTime date = DateTime.UtcNow;
                string freq = MainForm.VFOA.ToString();
                string mode = MainForm.OpModeVFOA.ToString();
                string name = MainForm.txtLogName.Text.ToString();

                if (name == "")
                    name = "OM";

                if (MainForm.TXSplit)
                {
                    freq = MainForm.VFOB.ToString();
                    mode = MainForm.OpModeVFOB.ToString();
                }

                msg = msg.Replace("<CALL>", MainForm.txtLogCall.Text.ToUpper());
                msg = msg.Replace("<My CALL>", MainForm.SetupForm.txtStnCALL.Text.ToString());
                msg = msg.Replace("<My RST>", MainForm.txtLogMyRST.Text.ToString());
                msg = msg.Replace("<My NR>", MainForm.udLOGMyNR.Text.ToString());
                msg = msg.Replace("<My QTH>", MainForm.SetupForm.txtStnQTH.Text.ToString());
                msg = msg.Replace("<My LOC>", MainForm.SetupForm.txtStnLOC.Text.ToString());
                msg = msg.Replace("<My Name>", MainForm.SetupForm.txtStnName.Text.ToString());
                msg = msg.Replace("<Name>", name);
                msg = msg.Replace("<My Info>", MainForm.SetupForm.txtStnInfoTxt.Text.ToString());
                msg = msg.Replace("<My Zone>", MainForm.SetupForm.txtStnZone.Text.ToString());
                msg = msg.Replace("<Freq>", freq);
                msg = msg.Replace("<Mode>", mode);
                msg = msg.Replace("<Date>", date.Date.ToShortDateString());
                msg = msg.Replace("<Time>", date.ToUniversalTime().ToShortTimeString());
                msg = msg.Replace("<Band>", MainForm.CurrentBand.ToString().Replace("B", ""));

                if (msg.Contains("<No>"))
                {
                    Mode opmode = MainForm.OpModeVFOA;
                    int cnt = 1;

                    if (MainForm.TXSplit)
                        opmode = MainForm.OpModeVFOB;

                    switch (opmode)
                    {
                        case Mode.CW:
                            cnt = int.Parse(MainForm.log_book.txtCW.Text.ToString());
                            cnt++;
                            break;

                        case Mode.RTTY:
                            cnt = int.Parse(MainForm.log_book.txtRTTY.Text.ToString());
                            cnt++;
                            break;

                        case Mode.BPSK31:
                            cnt = int.Parse(MainForm.log_book.txtPSK31.Text.ToString());
                            cnt++;
                            break;

                        case Mode.BPSK63:
                            cnt = int.Parse(MainForm.log_book.txtPSK63.Text.ToString());
                            cnt++;
                            break;

                        case Mode.BPSK125:
                            cnt = int.Parse(MainForm.log_book.txtPSK125.Text.ToString());
                            cnt++;
                            break;

                        case Mode.BPSK250:
                            cnt = int.Parse(MainForm.log_book.txtPSK250.Text.ToString());
                            cnt++;
                            break;

                        case Mode.QPSK31:
                            cnt = int.Parse(MainForm.log_book.txtQPSK31.Text.ToString());
                            cnt++;
                            break;

                        case Mode.QPSK63:
                            cnt = int.Parse(MainForm.log_book.txtQPSK63.Text.ToString());
                            cnt++;
                            break;

                        case Mode.QPSK125:
                            cnt = int.Parse(MainForm.log_book.txtQPSK125.Text.ToString());
                            cnt++;
                            break;

                        case Mode.QPSK250:
                            cnt = int.Parse(MainForm.log_book.txtQPSK250.Text.ToString());
                            cnt++;
                            break;
                    }

                    msg = msg.Replace("<No>", cnt.ToString());
                }

                return msg;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return "";
            }
        }

        public void UpdateButtons()
        {
            try
            {
                btn1.Text = SetupForm.txtButton1.Text;
                btn2.Text = SetupForm.txtButton2.Text;
                btn3.Text = SetupForm.txtButton3.Text;
                btn4.Text = SetupForm.txtButton4.Text;
                btn5.Text = SetupForm.txtButton5.Text;
                btn6.Text = SetupForm.txtButton6.Text;
                btn7.Text = SetupForm.txtButton7.Text;
                btn8.Text = SetupForm.txtButton8.Text;
                CMD1 = SetupForm.btn1cmd.Text;
                CMD2 = SetupForm.btn2cmd.Text;
                CMD3 = SetupForm.btn3cmd.Text;
                CMD4 = SetupForm.btn4cmd.Text;
                CMD5 = SetupForm.btn5cmd.Text;
                CMD6 = SetupForm.btn6cmd.Text;
                CMD7 = SetupForm.btn7cmd.Text;
                CMD8 = SetupForm.btn8cmd.Text;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void Keyboard_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.F1:
                        string msg = CMD1;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F2:
                        msg = CMD2;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F3:
                        msg = CMD3;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F4:
                        msg = CMD4;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F5:
                        msg = CMD5;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F6:
                        msg = CMD6;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F7:
                        msg = CMD7;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F8:
                        msg = CMD8;
                        msg = MacroHandler(msg);
                        rtbKeyboardText.AppendText(msg);
                        break;

                    case Keys.F9:
                        rtbKeyboardText.Clear();
                        break;

                    case Keys.F10:
                        btnSend_Click(this, EventArgs.Empty);
                        break;

                    case Keys.F11:
                        this.Hide();
                        break;

                    case Keys.F12:
                        if (SetupForm == null || SetupForm.IsDisposed)
                            SetupForm = new KeyboardSetup(this);

                        SetupForm.Show();
                        break;
                }

            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion
    }
}