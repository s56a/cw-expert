//=================================================================
// SetupDesigner.cs
//=================================================================
// Copyright (C) 2011 S56A YT7PWR
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

namespace CWExpert
{
    partial class Setup
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.comboAudioBuffer = new System.Windows.Forms.ComboBox();
            this.comboAudioOutput = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAudioDriver = new System.Windows.Forms.Label();
            this.lblAudioLatency = new System.Windows.Forms.Label();
            this.comboAudioDriver = new System.Windows.Forms.ComboBox();
            this.udLatency = new System.Windows.Forms.NumericUpDown();
            this.comboAudioInput = new System.Windows.Forms.ComboBox();
            this.lblAudioSampleRate = new System.Windows.Forms.Label();
            this.lblAudioInput = new System.Windows.Forms.Label();
            this.comboAudioSampleRate = new System.Windows.Forms.ComboBox();
            this.tbSetup = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkRXOnly = new System.Windows.Forms.CheckBox();
            this.grpMisc = new System.Windows.Forms.GroupBox();
            this.udDisplayHigh = new System.Windows.Forms.NumericUpDown();
            this.lblDisplayHigh = new System.Windows.Forms.Label();
            this.udDisplayLow = new System.Windows.Forms.NumericUpDown();
            this.lblDisplayLow = new System.Windows.Forms.Label();
            this.udAveraging = new System.Windows.Forms.NumericUpDown();
            this.lblAveraging = new System.Windows.Forms.Label();
            this.grpAudioTests = new System.Windows.Forms.GroupBox();
            this.lblAudioStreamOutputLatencyValuelabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblAudioStreamInputLatencyValue = new System.Windows.Forms.Label();
            this.lblAudioStreamSampleRateValue = new System.Windows.Forms.Label();
            this.chkAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.lblCallSign = new System.Windows.Forms.Label();
            this.txtCALL = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.grpDisplayDriver = new System.Windows.Forms.GroupBox();
            this.radDisplayGDI = new System.Windows.Forms.RadioButton();
            this.radDisplayDirectX = new System.Windows.Forms.RadioButton();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udLatency)).BeginInit();
            this.tbSetup.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grpMisc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDisplayHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDisplayLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udAveraging)).BeginInit();
            this.grpAudioTests.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.grpDisplayDriver.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(101, 340);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(228, 340);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.comboAudioBuffer);
            this.tabPage1.Controls.Add(this.comboAudioOutput);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.lblAudioDriver);
            this.tabPage1.Controls.Add(this.lblAudioLatency);
            this.tabPage1.Controls.Add(this.comboAudioDriver);
            this.tabPage1.Controls.Add(this.udLatency);
            this.tabPage1.Controls.Add(this.comboAudioInput);
            this.tabPage1.Controls.Add(this.lblAudioSampleRate);
            this.tabPage1.Controls.Add(this.lblAudioInput);
            this.tabPage1.Controls.Add(this.comboAudioSampleRate);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(347, 278);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Audio";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Buffer size";
            // 
            // comboAudioBuffer
            // 
            this.comboAudioBuffer.FormattingEnabled = true;
            this.comboAudioBuffer.Items.AddRange(new object[] {
            "64",
            "128",
            "256",
            "512",
            "1024",
            "2048"});
            this.comboAudioBuffer.Location = new System.Drawing.Point(122, 180);
            this.comboAudioBuffer.Name = "comboAudioBuffer";
            this.comboAudioBuffer.Size = new System.Drawing.Size(121, 21);
            this.comboAudioBuffer.TabIndex = 12;
            this.comboAudioBuffer.SelectedIndexChanged += new System.EventHandler(this.comboAudioBuffer_SelectedIndexChanged);
            // 
            // comboAudioOutput
            // 
            this.comboAudioOutput.FormattingEnabled = true;
            this.comboAudioOutput.Location = new System.Drawing.Point(122, 98);
            this.comboAudioOutput.Name = "comboAudioOutput";
            this.comboAudioOutput.Size = new System.Drawing.Size(167, 21);
            this.comboAudioOutput.TabIndex = 10;
            this.comboAudioOutput.SelectedIndexChanged += new System.EventHandler(this.comboAudioOutput_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Output";
            // 
            // lblAudioDriver
            // 
            this.lblAudioDriver.AutoSize = true;
            this.lblAudioDriver.Location = new System.Drawing.Point(41, 19);
            this.lblAudioDriver.Name = "lblAudioDriver";
            this.lblAudioDriver.Size = new System.Drawing.Size(62, 13);
            this.lblAudioDriver.TabIndex = 4;
            this.lblAudioDriver.Text = "AudioDriver";
            // 
            // lblAudioLatency
            // 
            this.lblAudioLatency.AutoSize = true;
            this.lblAudioLatency.Location = new System.Drawing.Point(41, 223);
            this.lblAudioLatency.Name = "lblAudioLatency";
            this.lblAudioLatency.Size = new System.Drawing.Size(45, 13);
            this.lblAudioLatency.TabIndex = 9;
            this.lblAudioLatency.Text = "Latency";
            // 
            // comboAudioDriver
            // 
            this.comboAudioDriver.FormattingEnabled = true;
            this.comboAudioDriver.Location = new System.Drawing.Point(122, 16);
            this.comboAudioDriver.Name = "comboAudioDriver";
            this.comboAudioDriver.Size = new System.Drawing.Size(167, 21);
            this.comboAudioDriver.TabIndex = 2;
            this.comboAudioDriver.SelectedIndexChanged += new System.EventHandler(this.comboAudioDriver_SelectedIndexChanged);
            // 
            // udLatency
            // 
            this.udLatency.Location = new System.Drawing.Point(151, 221);
            this.udLatency.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.udLatency.Name = "udLatency";
            this.udLatency.Size = new System.Drawing.Size(56, 20);
            this.udLatency.TabIndex = 8;
            this.udLatency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udLatency.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udLatency.ValueChanged += new System.EventHandler(this.udLatency_ValueChanged);
            // 
            // comboAudioInput
            // 
            this.comboAudioInput.FormattingEnabled = true;
            this.comboAudioInput.Location = new System.Drawing.Point(122, 57);
            this.comboAudioInput.Name = "comboAudioInput";
            this.comboAudioInput.Size = new System.Drawing.Size(167, 21);
            this.comboAudioInput.TabIndex = 3;
            this.comboAudioInput.SelectedIndexChanged += new System.EventHandler(this.comboAudioInput_SelectedIndexChanged);
            // 
            // lblAudioSampleRate
            // 
            this.lblAudioSampleRate.AutoSize = true;
            this.lblAudioSampleRate.Location = new System.Drawing.Point(41, 142);
            this.lblAudioSampleRate.Name = "lblAudioSampleRate";
            this.lblAudioSampleRate.Size = new System.Drawing.Size(63, 13);
            this.lblAudioSampleRate.TabIndex = 7;
            this.lblAudioSampleRate.Text = "Sample rate";
            // 
            // lblAudioInput
            // 
            this.lblAudioInput.AutoSize = true;
            this.lblAudioInput.Location = new System.Drawing.Point(41, 60);
            this.lblAudioInput.Name = "lblAudioInput";
            this.lblAudioInput.Size = new System.Drawing.Size(31, 13);
            this.lblAudioInput.TabIndex = 5;
            this.lblAudioInput.Text = "Input";
            // 
            // comboAudioSampleRate
            // 
            this.comboAudioSampleRate.FormattingEnabled = true;
            this.comboAudioSampleRate.Items.AddRange(new object[] {
            "6000",
            "8000",
            "11025",
            "12000",
            "22050",
            "24000",
            "44100",
            "48000",
            "96000",
            "192000"});
            this.comboAudioSampleRate.Location = new System.Drawing.Point(122, 139);
            this.comboAudioSampleRate.Name = "comboAudioSampleRate";
            this.comboAudioSampleRate.Size = new System.Drawing.Size(121, 21);
            this.comboAudioSampleRate.TabIndex = 6;
            this.comboAudioSampleRate.SelectedIndexChanged += new System.EventHandler(this.comboAudioSampleRate_SelectedIndexChanged);
            // 
            // tbSetup
            // 
            this.tbSetup.Controls.Add(this.tabPage1);
            this.tbSetup.Controls.Add(this.tabPage2);
            this.tbSetup.Controls.Add(this.tabPage3);
            this.tbSetup.Location = new System.Drawing.Point(25, 12);
            this.tbSetup.Name = "tbSetup";
            this.tbSetup.SelectedIndex = 0;
            this.tbSetup.Size = new System.Drawing.Size(355, 304);
            this.tbSetup.TabIndex = 10;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.chkRXOnly);
            this.tabPage2.Controls.Add(this.grpAudioTests);
            this.tabPage2.Controls.Add(this.chkAlwaysOnTop);
            this.tabPage2.Controls.Add(this.lblCallSign);
            this.tabPage2.Controls.Add(this.txtCALL);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(347, 278);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Misc";
            // 
            // chkRXOnly
            // 
            this.chkRXOnly.AutoSize = true;
            this.chkRXOnly.Location = new System.Drawing.Point(127, 104);
            this.chkRXOnly.Name = "chkRXOnly";
            this.chkRXOnly.Size = new System.Drawing.Size(63, 17);
            this.chkRXOnly.TabIndex = 11;
            this.chkRXOnly.Text = "RX only";
            this.chkRXOnly.UseVisualStyleBackColor = true;
            this.chkRXOnly.CheckedChanged += new System.EventHandler(this.chkRXOnly_CheckedChanged);
            // 
            // grpMisc
            // 
            this.grpMisc.Controls.Add(this.udDisplayHigh);
            this.grpMisc.Controls.Add(this.lblDisplayHigh);
            this.grpMisc.Controls.Add(this.udDisplayLow);
            this.grpMisc.Controls.Add(this.lblDisplayLow);
            this.grpMisc.Controls.Add(this.udAveraging);
            this.grpMisc.Controls.Add(this.lblAveraging);
            this.grpMisc.Location = new System.Drawing.Point(98, 138);
            this.grpMisc.Name = "grpMisc";
            this.grpMisc.Size = new System.Drawing.Size(151, 122);
            this.grpMisc.TabIndex = 10;
            this.grpMisc.TabStop = false;
            this.grpMisc.Text = "Display settings";
            // 
            // udDisplayHigh
            // 
            this.udDisplayHigh.Location = new System.Drawing.Point(84, 27);
            this.udDisplayHigh.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.udDisplayHigh.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.udDisplayHigh.Name = "udDisplayHigh";
            this.udDisplayHigh.Size = new System.Drawing.Size(51, 20);
            this.udDisplayHigh.TabIndex = 11;
            this.udDisplayHigh.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udDisplayHigh.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.udDisplayHigh.ValueChanged += new System.EventHandler(this.udDisplayHigh_ValueChanged);
            // 
            // lblDisplayHigh
            // 
            this.lblDisplayHigh.AutoSize = true;
            this.lblDisplayHigh.Location = new System.Drawing.Point(14, 30);
            this.lblDisplayHigh.Name = "lblDisplayHigh";
            this.lblDisplayHigh.Size = new System.Drawing.Size(54, 13);
            this.lblDisplayHigh.TabIndex = 12;
            this.lblDisplayHigh.Text = "High level";
            // 
            // udDisplayLow
            // 
            this.udDisplayLow.Location = new System.Drawing.Point(84, 59);
            this.udDisplayLow.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.udDisplayLow.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.udDisplayLow.Name = "udDisplayLow";
            this.udDisplayLow.Size = new System.Drawing.Size(51, 20);
            this.udDisplayLow.TabIndex = 9;
            this.udDisplayLow.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udDisplayLow.Value = new decimal(new int[] {
            120,
            0,
            0,
            -2147483648});
            this.udDisplayLow.ValueChanged += new System.EventHandler(this.udDisplayLow_ValueChanged);
            // 
            // lblDisplayLow
            // 
            this.lblDisplayLow.AutoSize = true;
            this.lblDisplayLow.Location = new System.Drawing.Point(14, 62);
            this.lblDisplayLow.Name = "lblDisplayLow";
            this.lblDisplayLow.Size = new System.Drawing.Size(52, 13);
            this.lblDisplayLow.TabIndex = 10;
            this.lblDisplayLow.Text = "Low level";
            // 
            // udAveraging
            // 
            this.udAveraging.Location = new System.Drawing.Point(84, 91);
            this.udAveraging.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udAveraging.Name = "udAveraging";
            this.udAveraging.Size = new System.Drawing.Size(51, 20);
            this.udAveraging.TabIndex = 7;
            this.udAveraging.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udAveraging.ValueChanged += new System.EventHandler(this.udAveraging_ValueChanged);
            // 
            // lblAveraging
            // 
            this.lblAveraging.AutoSize = true;
            this.lblAveraging.Location = new System.Drawing.Point(14, 94);
            this.lblAveraging.Name = "lblAveraging";
            this.lblAveraging.Size = new System.Drawing.Size(55, 13);
            this.lblAveraging.TabIndex = 8;
            this.lblAveraging.Text = "Averaging";
            // 
            // grpAudioTests
            // 
            this.grpAudioTests.Controls.Add(this.lblAudioStreamOutputLatencyValuelabel);
            this.grpAudioTests.Controls.Add(this.button1);
            this.grpAudioTests.Controls.Add(this.lblAudioStreamInputLatencyValue);
            this.grpAudioTests.Controls.Add(this.lblAudioStreamSampleRateValue);
            this.grpAudioTests.Location = new System.Drawing.Point(98, 138);
            this.grpAudioTests.Name = "grpAudioTests";
            this.grpAudioTests.Size = new System.Drawing.Size(151, 122);
            this.grpAudioTests.TabIndex = 9;
            this.grpAudioTests.TabStop = false;
            this.grpAudioTests.Text = "Audio test";
            // 
            // lblAudioStreamOutputLatencyValuelabel
            // 
            this.lblAudioStreamOutputLatencyValuelabel.AutoSize = true;
            this.lblAudioStreamOutputLatencyValuelabel.Location = new System.Drawing.Point(60, 39);
            this.lblAudioStreamOutputLatencyValuelabel.Name = "lblAudioStreamOutputLatencyValuelabel";
            this.lblAudioStreamOutputLatencyValuelabel.Size = new System.Drawing.Size(35, 13);
            this.lblAudioStreamOutputLatencyValuelabel.TabIndex = 4;
            this.lblAudioStreamOutputLatencyValuelabel.Text = "label4";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnAudioStreamInfo_Click);
            // 
            // lblAudioStreamInputLatencyValue
            // 
            this.lblAudioStreamInputLatencyValue.AutoSize = true;
            this.lblAudioStreamInputLatencyValue.Location = new System.Drawing.Point(60, 12);
            this.lblAudioStreamInputLatencyValue.Name = "lblAudioStreamInputLatencyValue";
            this.lblAudioStreamInputLatencyValue.Size = new System.Drawing.Size(35, 13);
            this.lblAudioStreamInputLatencyValue.TabIndex = 3;
            this.lblAudioStreamInputLatencyValue.Text = "label3";
            // 
            // lblAudioStreamSampleRateValue
            // 
            this.lblAudioStreamSampleRateValue.AutoSize = true;
            this.lblAudioStreamSampleRateValue.Location = new System.Drawing.Point(60, 61);
            this.lblAudioStreamSampleRateValue.Name = "lblAudioStreamSampleRateValue";
            this.lblAudioStreamSampleRateValue.Size = new System.Drawing.Size(35, 13);
            this.lblAudioStreamSampleRateValue.TabIndex = 5;
            this.lblAudioStreamSampleRateValue.Text = "label5";
            // 
            // chkAlwaysOnTop
            // 
            this.chkAlwaysOnTop.AutoSize = true;
            this.chkAlwaysOnTop.Location = new System.Drawing.Point(127, 81);
            this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            this.chkAlwaysOnTop.Size = new System.Drawing.Size(92, 17);
            this.chkAlwaysOnTop.TabIndex = 6;
            this.chkAlwaysOnTop.Text = "Always on top";
            this.chkAlwaysOnTop.UseVisualStyleBackColor = true;
            this.chkAlwaysOnTop.CheckedChanged += new System.EventHandler(this.chkAlwaysOnTop_CheckedChanged);
            // 
            // lblCallSign
            // 
            this.lblCallSign.AutoSize = true;
            this.lblCallSign.Location = new System.Drawing.Point(75, 40);
            this.lblCallSign.Name = "lblCallSign";
            this.lblCallSign.Size = new System.Drawing.Size(46, 13);
            this.lblCallSign.TabIndex = 1;
            this.lblCallSign.Text = "Call sign";
            // 
            // txtCALL
            // 
            this.txtCALL.Location = new System.Drawing.Point(171, 37);
            this.txtCALL.MaxLength = 10;
            this.txtCALL.Name = "txtCALL";
            this.txtCALL.Size = new System.Drawing.Size(100, 20);
            this.txtCALL.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.grpDisplayDriver);
            this.tabPage3.Controls.Add(this.grpMisc);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(347, 278);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Display";
            // 
            // grpDisplayDriver
            // 
            this.grpDisplayDriver.Controls.Add(this.radDisplayDirectX);
            this.grpDisplayDriver.Controls.Add(this.radDisplayGDI);
            this.grpDisplayDriver.Location = new System.Drawing.Point(73, 21);
            this.grpDisplayDriver.Name = "grpDisplayDriver";
            this.grpDisplayDriver.Size = new System.Drawing.Size(200, 100);
            this.grpDisplayDriver.TabIndex = 11;
            this.grpDisplayDriver.TabStop = false;
            this.grpDisplayDriver.Text = "Driver";
            // 
            // radDisplayGDI
            // 
            this.radDisplayGDI.AutoSize = true;
            this.radDisplayGDI.Checked = true;
            this.radDisplayGDI.Location = new System.Drawing.Point(58, 23);
            this.radDisplayGDI.Name = "radDisplayGDI";
            this.radDisplayGDI.Size = new System.Drawing.Size(50, 17);
            this.radDisplayGDI.TabIndex = 0;
            this.radDisplayGDI.TabStop = true;
            this.radDisplayGDI.Text = "GDI+";
            this.radDisplayGDI.UseVisualStyleBackColor = true;
            this.radDisplayGDI.CheckedChanged += new System.EventHandler(this.radDisplayGDI_CheckedChanged);
            // 
            // radDisplayDirectX
            // 
            this.radDisplayDirectX.AutoSize = true;
            this.radDisplayDirectX.Location = new System.Drawing.Point(58, 61);
            this.radDisplayDirectX.Name = "radDisplayDirectX";
            this.radDisplayDirectX.Size = new System.Drawing.Size(60, 17);
            this.radDisplayDirectX.TabIndex = 1;
            this.radDisplayDirectX.TabStop = true;
            this.radDisplayDirectX.Text = "DirectX";
            this.radDisplayDirectX.UseVisualStyleBackColor = true;
            this.radDisplayDirectX.CheckedChanged += new System.EventHandler(this.radDisplayDirectX_CheckedChanged);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 388);
            this.Controls.Add(this.tbSetup);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(413, 426);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(413, 426);
            this.Name = "Setup";
            this.Text = "CWExpert Setup";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Setup_Closing);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udLatency)).EndInit();
            this.tbSetup.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.grpMisc.ResumeLayout(false);
            this.grpMisc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDisplayHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDisplayLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udAveraging)).EndInit();
            this.grpAudioTests.ResumeLayout(false);
            this.grpAudioTests.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.grpDisplayDriver.ResumeLayout(false);
            this.grpDisplayDriver.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblAudioDriver;
        private System.Windows.Forms.Label lblAudioLatency;
        private System.Windows.Forms.ComboBox comboAudioDriver;
        private System.Windows.Forms.NumericUpDown udLatency;
        private System.Windows.Forms.ComboBox comboAudioInput;
        private System.Windows.Forms.Label lblAudioSampleRate;
        private System.Windows.Forms.Label lblAudioInput;
        private System.Windows.Forms.ComboBox comboAudioSampleRate;
        private System.Windows.Forms.TabControl tbSetup;
        private System.Windows.Forms.ComboBox comboAudioOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboAudioBuffer;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblCallSign;
        public System.Windows.Forms.TextBox txtCALL;
        private System.Windows.Forms.Label lblAudioStreamSampleRateValue;
        private System.Windows.Forms.Label lblAudioStreamOutputLatencyValuelabel;
        private System.Windows.Forms.Label lblAudioStreamInputLatencyValue;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkAlwaysOnTop;
        private System.Windows.Forms.Label lblAveraging;
        private System.Windows.Forms.NumericUpDown udAveraging;
        private System.Windows.Forms.GroupBox grpMisc;
        private System.Windows.Forms.NumericUpDown udDisplayHigh;
        private System.Windows.Forms.Label lblDisplayHigh;
        private System.Windows.Forms.NumericUpDown udDisplayLow;
        private System.Windows.Forms.Label lblDisplayLow;
        private System.Windows.Forms.GroupBox grpAudioTests;
        public System.Windows.Forms.CheckBox chkRXOnly;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox grpDisplayDriver;
        private System.Windows.Forms.RadioButton radDisplayDirectX;
        private System.Windows.Forms.RadioButton radDisplayGDI;
    }
}