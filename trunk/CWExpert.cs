//=================================================================
// CWExpert.cs
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


namespace CWExpert
{
    #region enum

    public enum DisplayDriver
    {
        FIRST = -1,
        GDI,
        DIRECTX,
        LAST,
    }

    public enum ColorSheme
    {
        original = 0,
        enhanced,
        SPECTRAN,
        BLACKWHITE,
        off,
    }

    public enum DisplayMode
    {
        FIRST = -1,
        PANADAPTER,
        WATERFALL,
        PANAFALL,
        OFF,
        LAST,
    }

    #endregion

    #region structures

    [StructLayout(LayoutKind.Sequential)]
    public struct TextEdit
    {
        public int hWnd;
        public int Xpos;
        public int Width;
    }

    #endregion

    unsafe public partial class CWExpert : Form
    {
        #region DLL imports

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy")]
        public static extern void memcpy(void* destptr, void* srcptr, int n);

        #endregion

        #region variable definition
        public bool booting = false;
        private int WM_LBUTTONDOWN = 0x0201;
        private int WM_LBUTTONUP = 0x0202;
        private int WM_KEYDOWN = 0x0100;
        private int WM_KEYUP = 0x0101;
        private int VK_F1 = 0x70;
        private int VK_F2 = 0x71;
        private int VK_F3 = 0x72;
        private int VK_F4 = 0x73;
        private int VK_F5 = 0x74;
        private int VK_F6 = 0x75;
        private int VK_F7 = 0x76;
        private int VK_F8 = 0x77;
        private int VK_RETURN = 0x0D;
        private int WM_APP = 0x8000;
        private int topWindow = 0;
        private int editPanel = 0;
        MessageHelper msg;
        public Setup SetupForm;
        TextEdit[] edits;
        public CWDecode cwDecoder;
        public bool pause_DisplayThread = false;
        public double[,] display_buffer;
//        public float[] display_buffer;
        private Thread display_thread;
        public AutoResetEvent display_event;

        #endregion

        #region properites

        private DisplayDriver video_driver = DisplayDriver.GDI;
        public DisplayDriver VideoDriver
        {
            get { return video_driver; }
            set
            {
                if (!booting)
                {
                    bool mrRuning = btnStartMR.Checked;

                    if (mrRuning)
                        btnStartMR.Checked = false;

                    Thread.Sleep(100);

                    if (value != video_driver)
                    {
                        if (value == DisplayDriver.GDI)
                        {
                            DirectX.DirectXRelease();
                            Thread.Sleep(100);
                            Display_GDI.Target = picPanadapter;
                            Display_GDI.Init();
                            if (File.Exists(".\\picDisplay.png"))
                                picPanadapter.BackgroundImage = Image.FromFile(".\\picDisplay.png");
                        }
                        else
                        {
                            Display_GDI.Close();
                            Thread.Sleep(100);
                            DirectX.MainForm = this;
                            DirectX.PanadapterTarget = picPanadapter;
                            DirectX.WaterfallTarget = picWaterfall;
                            DirectX.DirectXInit();
                        }
                    }

                    btnStartMR.Checked = mrRuning;
                }

                video_driver = value;
            }
        }

        private int refresh_time = 20;
        public int RefreshTime
        {
            set { refresh_time = value; }
        }

        private bool always_on_top = false; // yt7pwr
        public bool AlwaysOnTop
        {
            get { return always_on_top; }
            set
            {
                always_on_top = value;
                if (value)
                {
                    SetWindowPos(this.Handle.ToInt32(),
                        -1, this.Left, this.Top, this.Width, this.Height, 0);
                }
                else
                {
                    SetWindowPos(this.Handle.ToInt32(),
                        -2, this.Left, this.Top, this.Width, this.Height, 0);
                }
            }
        }

        public string txtCALL
        {
            set { this.txtCall.Text = value; }
        }

        public string txtNR
        {
            set { txtNr.Text = value; }
        }

        public string txtRst
        {
            set { txtRST.Text = value; }
        }

  
        private bool mrIsRunning = false;

        public bool MRIsRunning
        {
            get { return mrIsRunning; }
            set
            {
                if (!value)
                    btnStartMR.Checked = false;
                else
                    btnStartMR.Checked = true;

                Thread.Sleep(100);
                mrIsRunning = value;
            }
        }

        #endregion

        #region constructor

        public CWExpert()
        {
            booting = true;
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer, true);
            msg = new MessageHelper();
            edits = new TextEdit[3];
            DB.AppDataPath = Application.StartupPath;
            DB.Init();
            Audio.MainForm = this;
            PA19.PA_Initialize();
            SetupForm = new Setup(this);
            booting = false;
            display_event = new AutoResetEvent(false);
            cwDecoder = new CWDecode(this);
            cwDecoder.rx_only = SetupForm.chkRXOnly.Checked;

            if (VideoDriver == DisplayDriver.DIRECTX)
            {
                DirectX.MainForm = this;
                DirectX.PanadapterTarget = picPanadapter;
                DirectX.WaterfallTarget = picWaterfall;
                DirectX.WaterfallInit();
                DirectX.DirectXInit();
            }
            else
            {
                Display_GDI.Target = picPanadapter;
                Display_GDI.Init();
                if (File.Exists(".\\picDisplay.png"))
                    picPanadapter.BackgroundImage = Image.FromFile(".\\picDisplay.png");
            }

            display_buffer = new double[32,64];
            Application.EnableVisualStyles();
            AlwaysOnTop = always_on_top;
        }

        #endregion

        #region misc function

        private bool EnsureMRWindow()
        {
            try
            {
                topWindow = msg.getWindowId("TMainForm", "Morse Runner");
                if (topWindow != 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Morse Runner not found!\n" + ex.ToString());
                return false;
            }
        }

        private bool EnsureMREditWindows()
        {
            try
            {
                int childAfter = 0;
                int subPanel = 0;
                int tmpEdit = 0;
                WINDOWINFO wInfo = new WINDOWINFO();
                TextEdit[] tmpEdits = new TextEdit[3];

                if (EnsureMRWindow())
                {
                    while (true)
                    {
                        editPanel = msg.getWindowIdEx(topWindow, childAfter, "TPanel", null);
                        if (editPanel == 0)
                            break;
                        childAfter = editPanel;
                        subPanel = msg.getWindowIdEx(editPanel, 0, "TPanel", "0");
                        if (subPanel != 0)
                            break;
                    }
                    int i = 0;
                    while (true)
                    {
                        tmpEdit = msg.getWindowIdEx(editPanel, tmpEdit, "TEdit", null);
                        if (tmpEdit == 0)
                            break;
                        else
                        {
                            wInfo = msg.getWindowInfo(tmpEdit);
                            {
                                tmpEdits[i].hWnd = tmpEdit;
                                tmpEdits[i].Xpos = wInfo.rcClient.Left;
                                i++;
                            }
                        }
                    }

                    if (tmpEdits[0].Xpos < tmpEdits[1].Xpos)
                    {
                        edits[0] = tmpEdits[0];
                        edits[1] = tmpEdits[1];
                        edits[2] = tmpEdits[2];
                    }
                    else
                    {
                        edits[0] = tmpEdits[2];
                        edits[1] = tmpEdits[1];
                        edits[2] = tmpEdits[0];
                    }

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Morse Runner not running?\n" + ex.ToString());
                return false;
            }
        }

        #endregion

        #region MR Functions keys

        public void btnF1_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F1, (1 + (59 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F1, (1 + (59 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F1, (1 + (59 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F1, (1 + (59 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void btnF2_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F2, (1 + (60 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F2, (1 + (60 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F2, (1 + (60 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F2, (1 + (60 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void btnF3_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F3, (1 + (61 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F3, (1 + (61 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F3, (1 + (61 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F3, (1 + (61 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void btnF4_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F4, (1 + (62 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F4, (1 + (62 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F4, (1 + (62 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F4, (1 + (62 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void btnF5_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F5, (1 + (63 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F5, (1 + (63 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F5, (1 + (63 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F5, (1 + (63 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnF6_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F6, (1 + (64 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F6, (1 + (64 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F6, (1 + (64 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F6, (1 + (64 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void btnF7_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F7, (1 + (65 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F7, (1 + (65 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F7, (1 + (65 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F7, (1 + (65 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnF8_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_F8, (1 + (66 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15616, VK_F8, (1 + (66 << 16)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYUP, VK_F8, (1 + (66 << 16) + (3 << 30)));
                    msg.sendWindowsMessage(edits[0].hWnd, WM_APP + 15617, VK_F8, (1 + (66 << 16) + (3 << 30)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region MorseRunner keys

        private void btnStartMR_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnStartMR.Checked)
                {
                    txtChannelClear();

                    if (!mrIsRunning)
                    {
                        mrIsRunning = true;
                        Audio.callback_return = 0;
                        if (cwDecoder.AudioEvent == null)
                            cwDecoder.AudioEvent = new AutoResetEvent(false);

                        Audio.Start();

                        EnsureMRWindow();

                        if (topWindow == 0)
                        {
                            btnStartMR.Checked = false;
                            return;
                        }

                        cwDecoder.CWdecodeStart();

                        int runButton = 0;
                        int panel = 0;
                        int subPanel = 0;

                        while (true)
                        {
                            panel = msg.getWindowIdEx(topWindow, panel, "TPanel", null);
                            if (panel == 0)
                                break;
                            // Test if it has panel with subpanel with empty caption
                            subPanel = msg.getWindowIdEx(panel, 0, "TPanel", "");
                            if (subPanel == 0)
                                break;

                            // Test if that sub-panel has toolbar
                            runButton = msg.getWindowIdEx(subPanel, 0, "TToolBar", null);
                            if (runButton == 0)
                                break;
                        }

                        if (runButton == 0)
                        {
                            btnStartMR.Checked = false;
                            return;
                        }
                        else
                        {
                            msg.sendWindowsMessage(runButton, WM_LBUTTONDOWN, 0, (10 << 16) + 10);
                            msg.sendWindowsMessage(runButton, WM_LBUTTONUP, 0, (10 << 16) + 10);
                            mrIsRunning = true;
                            btnStartMR.Text = "Stop";

                            display_thread = new Thread(new ThreadStart(RunDisplay));
                            display_thread.Name = "Display Thread";
                            display_thread.Priority = ThreadPriority.Normal;
                            display_thread.IsBackground = true;
                            display_thread.Start();
                        }
                    }
                }
                else
                {
                    if (mrIsRunning)
                    {
                        mrIsRunning = false;
                        Audio.callback_return = 2;
                        Thread.Sleep(100);
                        cwDecoder.CWdecodeStop();
                        Audio.StopAudio();
                        Thread.Sleep(100);

                        if (cwDecoder.AudioEvent != null)
                            cwDecoder.AudioEvent.Close();
                        cwDecoder.AudioEvent = null;

                        EnsureMRWindow();
                        if (topWindow == 0)
                            return;

                        int runButton = 0;
                        int panel = 0;
                        int subPanel = 0;

                        while (true)
                        {
                            panel = msg.getWindowIdEx(topWindow, panel, "TPanel", null);
                            if (panel == 0)
                                break;
                            // Test if it has panel with subpanel with empty caption
                            subPanel = msg.getWindowIdEx(panel, 0, "TPanel", "");
                            if (subPanel == 0)
                                break;

                            // Test if that sub-panel has toolbar
                            runButton = msg.getWindowIdEx(subPanel, 0, "TToolBar", null);
                            if (runButton == 0)
                                break;
                        }

                        if (runButton == 0)
                            return;
                        else
                        {
                            msg.sendWindowsMessage(runButton, WM_LBUTTONDOWN, 0, (10 << 16) + 10);
                            msg.sendWindowsMessage(runButton, WM_LBUTTONUP, 0, (10 << 16) + 10);
                            mrIsRunning = false;
                            btnStartMR.Text = "Start";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Morse Runner not running?\n" + ex.ToString());
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void setupMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (SetupForm != null)
                    SetupForm.Show();
                else
                {
                    SetupForm = new Setup(this);
                    SetupForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening Setup!\n" + ex.ToString());
            }
        }

        public void btnSendCall_Click(object sender, EventArgs e)      // button call
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsStringMessage(edits[0].hWnd, 0, txtCall.Text);
                    msg.sendWindowsMessage(edits[0].hWnd, WM_KEYDOWN, VK_RETURN, 1 + (13 << 16));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void btnSendRST_Click(object sender, EventArgs e)       // button RST
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsStringMessage(edits[1].hWnd, 0, txtRST.Text);
                    //  msg.sendWindowsMessage(edits[1].hWnd, WM_KEYDOWN, VK_RETURN, 1 + (13 << 16));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void btnSendNr_Click(object sender, EventArgs e)        // button Nr
        {
            try
            {
                EnsureMREditWindows();

                if (topWindow != 0)
                {
                    msg.sendWindowsStringMessage(edits[2].hWnd, 0, txtNr.Text);
                    msg.sendWindowsMessage(edits[2].hWnd, WM_KEYDOWN, VK_RETURN, 1 + (13 << 16));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtNr_KeyUp(object sender, KeyEventArgs e)     // Nr + enter
        {
            if (e.KeyCode == Keys.Enter)
                btnSendNr_Click(null, null);
        }

        private void txtRST_KeyUp(object sender, KeyEventArgs e)    // RST + enter
        {
            if (e.KeyCode == Keys.Enter)
                btnSendRST_Click(null, null);
        }

        private void txtCall_KeyUp(object sender, KeyEventArgs e)   // Call + enter
        {
            if (e.KeyCode == Keys.Enter)
                btnSendCall_Click(null, null);
        }

        public void CWExpert_KeyUp(object sender, KeyEventArgs e)      // function keys F1...F12
        {
  
            switch (e.KeyCode)
            {
                case Keys.F1:
                    btnF1_Click(null, null);
                    break;
                case Keys.F2:
                    btnF2_Click(null, null);
                    break;
                case Keys.F3:
                    btnF3_Click(null, null);
                    break;
                case Keys.F4:
                    btnF4_Click(null, null);
                    break;
                case Keys.F5:
                    btnF5_Click(null, null);
                    break;
                case Keys.F6:
                    btnF6_Click(null, null);
                    break;
                case Keys.F7:
                    btnF7_Click(null, null);
                    break;
                case Keys.F8:
                    btnF8_Click(null, null);
                    break;
             }
        }

        #endregion

        private void btngrab_Click(object sender, EventArgs e)
        {
//            txtCALL = Callers.SelectedItem.ToString();
 
        }

        private void txtChannelClear()
        {
            try
            {
                txtChannel2.Clear();
                txtChannel3.Clear();
                txtChannel4.Clear();
                txtChannel5.Clear();
                txtChannel6.Clear();
                txtChannel7.Clear();
                txtChannel8.Clear();
                txtChannel9.Clear();
                txtChannel10.Clear();
                txtChannel11.Clear();
                txtChannel12.Clear();
                txtChannel13.Clear();
                txtChannel14.Clear();
                txtChannel15.Clear();
                txtChannel16.Clear();
                txtChannel17.Clear();
                txtChannel18.Clear();
                txtChannel19.Clear();
                txtChannel20.Clear();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnclr_Click(object sender, EventArgs e)
        {
            try
            {
                txtChannelClear();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #region Display functions

        private void picWaterfall_Paint(object sender, PaintEventArgs e)
        {
            if (VideoDriver == DisplayDriver.DIRECTX)
                DirectX.RenderWaterfall(e.Graphics, picWaterfall.Width, picWaterfall.Height);
            else
            {
                if (!Display_GDI.RenderWaterfall(ref e))
                {
                    if (Display_GDI.IsInitialized)
                    {
                        Display_GDI.Close();
                        Thread.Sleep(100);
                        if (!Display_GDI.Init())
                            btnStartMR.Checked = false;
                    }
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (VideoDriver == DisplayDriver.DIRECTX)
                DirectX.RenderWaterfall(e.Graphics, picWaterfall.Width, picWaterfall.Height);
            else
            {
                if (!Display_GDI.RenderPanadapter(ref e))
                {
                    if (Display_GDI.IsInitialized)
                    {
                        Display_GDI.Close();
                        Thread.Sleep(100);
                        if (!Display_GDI.Init())
                            btnStartMR.Checked = false;
                    }
                }
            }
        }

        public bool data_ready = false;
        private void RunDisplay()
        {
            try
            {
                Thread.Sleep(100);

                while (MRIsRunning)
                {
                    display_event.WaitOne();

                    if (VideoDriver == DisplayDriver.DIRECTX)
                    {
                        DirectX.DataReady = true;

                        if (!DirectX.RenderDirectX())
                        {
                            DirectX.DirectXRelease();
                            Thread.Sleep(100);
                            if (!DirectX.DirectXInit())
                                btnStartMR.Checked = false;
                        }

                        DirectX.WaterfallDataReady = true;
                        picWaterfall.Invalidate();
                    }
                    else
                    {
                        if (Display_GDI.IsInitialized)
                        {
                            Display_GDI.DataReady = true;
                            picPanadapter.Invalidate();
                            picWaterfall.Invalidate();
                            Display_GDI.WaterfallDataReady = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void picPanadapter_MouseMove(object sender, MouseEventArgs e)
        {
            Display_GDI.DisplayCursorX = e.X;
            DirectX.DisplayCursorX = e.X;
            float x = PixelToHz(e.X);
            txtFreq.Text = Math.Round(x, 0).ToString() + "Hz";
        }

        private float PixelToHz(float x)
        {
            int low, high;

            low = DirectX.RXDisplayLow;
            high = DirectX.RXDisplayHigh;

            int width = high - low;
            return (float)(low + (double)x / (double)picPanadapter.Width * (double)width);
        }

        private void picPanadapter_MouseDown(object sender, MouseEventArgs e)
        {
            if (VideoDriver == DisplayDriver.DIRECTX)
            {
                if (e.Button == MouseButtons.Right && !DirectX.ShowVertCursor)
                    DirectX.ShowVertCursor = true;
                else if (e.Button == MouseButtons.Right)
                    DirectX.ShowVertCursor = false;
            }
            else
            {
                if (e.Button == MouseButtons.Right && !Display_GDI.ShowVertCursor)
                    Display_GDI.ShowVertCursor = true;
                else if (e.Button == MouseButtons.Right)
                    Display_GDI.ShowVertCursor = false;
            }
        }

        #endregion

        public void WriteOutputText(int chanel_no, double thd_txt, string out_string)
        {
            try
            {
                if (chanel_no <= 20 && chanel_no >= 2)
                {
                    switch (chanel_no)
                    {
                        case 2:
                            txtChannel2.Text = "2  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 3:
                            txtChannel3.Text = "3  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 4:
                            txtChannel4.Text = "4  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 5:
                            txtChannel5.Text = "5  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 6:
                            txtChannel6.Text = "6  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 7:
                            txtChannel7.Text = "7  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 8:
                            txtChannel8.Text = "8  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 9:
                            txtChannel9.Text = "9  " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 10:
                            txtChannel10.Text = "10 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 11:
                            txtChannel11.Text = "11 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 12:
                            txtChannel12.Text = "12 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 13:
                            txtChannel13.Text = "13 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 14:
                            txtChannel14.Text = "14 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 15:
                            txtChannel15.Text = "15 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 16:
                            txtChannel16.Text = "16 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 17:
                            txtChannel17.Text = "17 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 18:
                            txtChannel18.Text = "18 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 19:
                            txtChannel19.Text = "19 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                        case 20:
                            txtChannel20.Text = "20 " + Math.Round(thd_txt, 5).ToString() + "  " + out_string;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void picWaterfall_MouseMove(object sender, MouseEventArgs e)
        {
            picPanadapter_MouseMove(sender, e);
        }

        private void picWaterfall_MouseDown(object sender, MouseEventArgs e)
        {
            picPanadapter_MouseDown(sender, e);
        }
    }
}
