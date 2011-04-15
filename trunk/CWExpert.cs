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


namespace CWExpert
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TextEdit
    {
        public int hWnd;
        public int Xpos;
        public int Width;
    }

    public partial class CWExpert : Form
    {
        #region variable definition

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

        #endregion

        #region properites

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
                if (mrIsRunning)
                    btnStopMR_Click(null, null);
                Thread.Sleep(100);
                mrIsRunning = value;
                if (value)
                    btnStartMR_Click(null, null);
            }
        }

        #endregion

        #region constructor

        public CWExpert()
        {
            InitializeComponent();
            msg = new MessageHelper();
            edits = new TextEdit[3];
            DB.AppDataPath = Application.StartupPath;
            DB.Init();
            Audio.MainForm = this;
            PA19.PA_Initialize();
            SetupForm = new Setup(this);
            cwDecoder = new CWDecode(this);
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

        private void btnStartMR_Click(object sender, EventArgs e)
        {
            try
            {
                if (!mrIsRunning)
                {
                    mrIsRunning = true;
                    Audio.callback_return = 0;
                    if (cwDecoder.AudioEvent == null)
                        cwDecoder.AudioEvent = new AutoResetEvent(false);

                    Audio.Start();

                    EnsureMRWindow();
                    if (topWindow == 0)
                        return;

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
                        return;
                    else
                    {
                        msg.sendWindowsMessage(runButton, WM_LBUTTONDOWN, 0, (10 << 16) + 10);
                        msg.sendWindowsMessage(runButton, WM_LBUTTONUP, 0, (10 << 16) + 10);
                        mrIsRunning = true;
 //                       textBox1.BackColor = Color.LightGreen;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Morse Runner not running?\n" + ex.ToString());
            }
        }

        private void btnStopMR_Click(object sender, EventArgs e)
        {
            try
            {
                if (mrIsRunning)
                {
                    mrIsRunning = false;
                    Audio.callback_return = 2;
                    cwDecoder.CWdecodeStop();
                    Audio.StopAudio();
                    Thread.Sleep(100);
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
                Callers.Items.Add(txtCall.Text+" ");
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
            txtCALL = Callers.SelectedItem.ToString();
 
        }

        private void btnclr_Click(object sender, EventArgs e)
        {
            string si = Callers.SelectedItem.ToString();
            Callers.Items.Remove(si);
        }
 
        public void Callers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
