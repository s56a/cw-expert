//=================================================================
// CWDecoder.cs
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
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace CWExpert
{
    public class CWDecode
    {
        #region variable

        delegate void CrossThreadCallback(string command, string data);
        delegate void CrossThreadSetText(string command, int channel_no, double thd_txt, string out_txt);

        public bool lid = false;
        public bool rx_only = false;
        public bool once = true;
        public int rcvd = 0;
        public int active = 0;
        public int moni = 12;
        public int ponovi = 32;
        public bool repeat = false;
        public int loopend = 0;
        public int totalsamples = 0;
        public int logf2l = 0;
        public const int rate = 8000;
        public const int FFTlen = 64;
        public const int F2L = 128;
        public const int aver = 10;
        public double thld = 4.0;
        public int bwl = 2;
        public int bwh = 20;
        public bool run_thread = false;
        public Thread CWThread;
        public AutoResetEvent AudioEvent;
        public float[] audio_buffer;
        public float[] fft_buffer;
        public bool key = false;
        public bool nr_agn = false;
        public bool call_found = false;
        public bool rprt_found = false;
        public bool transmit = false;
        public int serial = 0;
        //        public string new_call = new string(' ', 64);
        public string rst = new string(' ', 3);
        public string report = new string(' ', 4);
        public string call_sent = new string(' ', 14);
        public string call = new string(' ', 14);
        public string mycall = new string(' ', 14);
        public string nmbr = new string(' ', 7);
        public string morsealpha = new string(' ', 28);
        public string morsedigit = new string(' ', 30);
        // public string[] scp = new string[40350];
        public string[] output = new string[FFTlen];
        public string[] calls = new string[FFTlen];
        public string[] rprts = new string[FFTlen];
        public int[] sum = new int[FFTlen];
        public int[] ave = new int[FFTlen];
        public double[] Noise = new double[FFTlen];
        public double[] snr = new double[FFTlen];
        public double[] RealF = new double[F2L * 2];
        public double[] ImagF = new double[F2L];
        public double[,] Mag = new double[FFTlen, 32];
        public double[,] medo = new double[FFTlen, 5];
        public double[] temp = new double[FFTlen];
        public double[] signal = new double[FFTlen];
        public int[] broj = new int[FFTlen];
        public double[] si = new double[F2L];
        public double[] co = new double[F2L];
        public double[] wd = new double[F2L];
        public int[] tim = new int[FFTlen];
        public double thd = 0;
        private double[] thd_txt;
        private double Period = 0.0f;
        public int ctr = 0;
        public int tx_timer = 0;
        public int rx_timer = 50;
        public int dotmin = 0;
        public byte[] bitrev = new byte[F2L];
        public double[] old1 = new double[FFTlen];
        public bool[] keyes = new bool[FFTlen];
        public bool[] valid = new bool[FFTlen];
        public bool[] enable = new bool[FFTlen];
        public int nofs = 0;
        private FFTW fftw;

        //        System.IO.StreamReader file = new System.IO.StreamReader("C:\\kc998.wav");

        //        private HiPerfTimer timer;

        private CWExpert MainForm;

        #endregion

        #region constructor and destructor

        public CWDecode(CWExpert mainForm)
        {
            try
            {
                MainForm = mainForm;
                audio_buffer = new float[Audio.BlockSize * 2];
                fft_buffer = new float[Audio.BlockSize * 2];
                thd_txt = new double[FFTlen];
                AudioEvent = new AutoResetEvent(false);
                once = true;
                fftw = new FFTW();
                fftw.InitFFTW(F2L, Audio.BlockSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        ~CWDecode()
        {
            fftw.FreeFFTW();
            fftw = null;
        }

        #endregion


        #region crossthread

        private void SetText(string action, int channel, double thd_txt, string text)
        {
            try
            {
                switch (action)
                {
                    case "Set text":
                        MainForm.WriteOutputText(channel, thd_txt, text);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void CrossThreadCommand(string action, string data)
        {
            try
            {
                if (!rx_only)
                {
                    switch (action)
                    {
                        case "Send CALL":
                            {
                                MainForm.txtCALL = data;
                                MainForm.btnSendCall_Click(null, null);
                            }
                            break;

                        case "Send RST":
                            {
                                MainForm.txtRst = data;
                                MainForm.btnSendRST_Click(null, null);
                            }
                            break;

                        case "Send NR":
                            {
                                MainForm.txtNR = data;
                                MainForm.btnSendNr_Click(null, null);
                            }
                            break;

                        case "Send F1":
                            {
                                MainForm.btnF1_Click(null, null);
                            }
                            break;

                        case "Send F2":
                            {
                                MainForm.btnF2_Click(null, null);
                            }
                            break;

                        case "Send F3":
                            {
                                MainForm.btnF3_Click(null, null);
                            }
                            break;
                        case "Send F5":
                            {
                                MainForm.btnF5_Click(null, null);
                            }
                            break;
                        case "Send F7":
                            {
                                MainForm.btnF7_Click(null, null);
                            }
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex + "\n\n" + ex.StackTrace.ToString());
            }
        }

        #endregion

        public void CW_Thread()
        {
            try
            {
                while (run_thread)
                {
                    AudioEvent.WaitOne();
                    Marshal.Copy(audio_buffer, 0, fftw.spec_in, Audio.BlockSize);

                    if (fftw.ComputeSpectrum())
                    {
                        if (MainForm.VideoDriver == DisplayDriver.DIRECTX)
                        {
                            Array.Copy(fftw.spec_fout, 0, DirectX.new_display_data, 0, Audio.BlockSize);
                            Array.Copy(fftw.spec_fout, 0, DirectX.new_waterfall_data, 0, Audio.BlockSize);
                        }
                        else
                        {
                            Array.Copy(fftw.spec_fout, 0, Display_GDI.new_display_data, 0, Audio.BlockSize);
                            Array.Copy(fftw.spec_fout, 0, Display_GDI.new_waterfall_data, 0, Audio.BlockSize);
                        }

                        MainForm.data_ready = true;
                        MainForm.display_event.Set();
                    }

                    FFT_Spectrum();
                    if (once) { Channel(); }
                    else if (transmit) { TRtiming(); }
                    else
                    {
                        Sig2ASCII();
                        if (!rx_only)
                        {
                            Analyse();
                            Responder();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                run_thread = false;
            }
        }

        public bool CWdecodeStart()
        {
            bool result = false;
            once = true;
            bwl = 2;
            bwh = 20;

            if (Init())
            {
                run_thread = true;
                CWThread = new Thread(new ThreadStart(CW_Thread));
                CWThread.Name = "CW Thread";
                CWThread.Priority = ThreadPriority.Normal;
                CWThread.IsBackground = true;
                CWThread.Start();
            }

            return result;
        }

        public void CWdecodeStop()
        {
            try
            {
                run_thread = false;
                AudioEvent.Set();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public bool Init()
        {

            int n = 0;
            int n1 = 0;
            int w = 0;
            int x = 0;
            int y = 0;
            int z = 0;

            try
            {
                totalsamples = F2L;
                logf2l = (int)(Math.Round(Math.Log(totalsamples) / Math.Log(2.0)));
                Period = (double)totalsamples / (double)rate;
                for (n = 0; n < totalsamples; n++)
                {
                    x = n;
                    y = 0;
                    n1 = totalsamples;
                    for (w = 1; w <= logf2l; w++)
                    {
                        n1 = n1 >> 1;
                        if (x >= n1)
                        {
                            if (w == 1)
                                y++;
                            else
                                y = y + (2 << (w - 2));
                            x = x - n1;
                        }
                    }
                    bitrev[n] = (byte)y;
                }
                nofs = Audio.BlockSize / FFTlen; // number of segments
                dotmin = (int)Math.Round(1.2 / (40 * Period));   // 40 wpm, 30 msec dot
                morsealpha = "ETIANMSURWDKGOHVF*L*PJBXCYZQ";
                morsedigit = "54*3***2 ******16*/*****7***8*90";
                for (n = 0; n < FFTlen; n++)
                {
                    old1[n] = 0;
                    Noise[n] = thld;
                    temp[n] = 0;
                    ave[n] = aver;
                    sum[n] = 0;
                    tim[n] = 1;
                    enable[n] = false;
                    keyes[n] = false;
                    valid[n] = false;
                    output[n] = String.Empty;
                    calls[n] = String.Empty;
                    rprts[n] = String.Empty;
                    broj[n] = 0;
                    for (z = 0; z < 5; z++)
                        medo[n, z] = Noise[n];
                }

                double v = 2 * Math.PI / totalsamples;
                for (n = 0; n < totalsamples; n++)
                {
                    RealF[n] = 0;
                    ImagF[n] = 0;
                    si[n] = -Math.Sin(n * v);
                    co[n] = Math.Cos(n * v);
                    wd[n] = (0.54 + 0.46 * co[n]);
                }

                mycall = MainForm.SetupForm.txtCALL.Text;
                //                MainForm.Callers.Items.Add(mycall);
                mycall = "N1YU";
                call_sent = mycall;
                call_found = false;
                rprt_found = false;
                nr_agn = false;
                tx_timer = 0;
                rx_timer = ponovi;
                transmit = false;
                serial = 1;
                repeat = false;
                //                new_call = " ";
                //                stanje = 1;

                //                SCP_Load();

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }
        /*
                private void SCP_Load()
                {
                    int counter = 0;
                    string line;

                    // Read the file and display it line by line.
                    System.IO.StreamReader file =
                       new System.IO.StreamReader("Master.txt");
                    while ((line = file.ReadLine()) != null)
                    {
                        scp[counter] = line;
                        counter++;
                    }

                    file.Close();

                    Array.Sort(scp);

        //            MainForm.txtChannel4.Items.Add("SCP " + counter.ToString());

                }
        */

        private void cw2asc(int z)
        {
            char ch = '*';
            if (sum[z] < 29) { ch = morsealpha[sum[z] - 1]; }
            else if ((sum[z] >= 31) && (sum[z] <= 62))
            {
                ch = morsedigit[sum[z] - 31];
                if (Char.IsDigit(ch))
                {
                    valid[z] = true;
                    if (!lid) { rx_timer = ponovi; }
                }
            }
            else if (sum[z] == 75) { ch = '?'; }
            output[z] += ch;
            sum[z] = 0;
        }

        private void CWdecode(int z)
        {
            int t = 0;
            try
            {
                keyes[z] = key;
                t = ctr - tim[z];
                tim[z] = ctr;
                if (!key)
                {
                    if (t > ave[z]) { sum[z]++; }
                    else if (t > dotmin) { ave[z] = t + ave[z] / 2; }
                    sum[z]++;
                }
                else if (t < ave[z])
                {
                    if (t > dotmin) { ave[z] = t + ave[z] / 2; }
                    sum[z] = 2 * sum[z];
                    if (sum[z] > 75) { sum[z] = 0; }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void Channel()
        {
            int n = 0;
            for (n = bwl; n <= bwh; n++) { temp[n] += signal[n]; }
            if (transmit)
            {
                tx_timer--;
                if (tx_timer == 1)
                {
                    thd = 0;
                    for (n = bwl; n <= bwh; n++)
                    {
                        if (temp[n] > thd)
                        {
                            thd = temp[n];
                            moni = n;
                        }
                    }
                    Debug.WriteLine("  " + moni.ToString() + "  ");
                    once = false;
                    ctr = 0;
                }
            }
            else
            {
                rx_timer--;
                if (rx_timer == 0)
                {
                    thd = 0;
                    for (n = bwl; n <= bwh; n++)
                    {
                        temp[n] = temp[n] / ponovi;
                        thd += temp[n];
                    }

                    thd = thd / (bwh - bwl + 1);

                    thld = 8 * thd;

                    while ((temp[bwl] < thd) && (bwl < bwh)) { bwl++; }
                    while ((temp[bwh] < thd) && (bwh > bwl)) { bwh--; }

                    for (n = bwl; n <= bwh; n++)
                    {
                        Noise[n] = 8 * temp[n];
                        if (Noise[n] < thld) { Noise[n] = thld; }
                        temp[n] = 0;
                        Debug.WriteLine(n + "  " + Math.Round(Noise[n]).ToString());
                    }
                    Debug.WriteLine(Math.Round(bwl / Period).ToString() + " - " + Math.Round(bwh / Period).ToString() + " Hz " + Math.Round(thld).ToString());
                    cqcqcq();
                }
            }
        }

        private void Sig2ASCII()
        {
            int t0;

            t0 = ctr;
            for (int z = bwl; z <= bwh; z++)
            {
                int b = broj[z];
                thd = Noise[z];
                ctr = t0;
                for (int n = 0; n < nofs; n++)
                {
                    if (Mag[z, n] > thd)
                    {
                        if (b < 3) { b++; }
                    }
                    else if (b > 0) { b--; }
                    key = b > 1;
                    ctr++;
                    if (key != keyes[z]) { CWdecode(z); }
                    else if (!key)
                    {
                        int t = ctr - tim[z];
                        if (t == ave[z]) { if (sum[z] > 0) { cw2asc(z); } }
                        else if (t == (2 * ave[z]))
                        {
                            output[z] += " ";
                            enable[z] = false;
                        }
                        else if (t == (4 * ave[z])) { enable[z] = true; }
                    }
                }
                broj[z] = b;
            }
            ctr = t0 + nofs;
        }


        private void MyFFT()
        {
            int i, k, m, mx, I1, I2, I3, I4, I5, x;
            double A1, A2, B1, B2, Z1, Z2;

            I1 = totalsamples / 2;
            I2 = 1;
            for (i = 1; i <= logf2l; i++)
            {
                I3 = 0;
                I4 = I1;
                for (k = 1; k <= I2; k++)
                {
                    x = I3 / I1;
                    I5 = bitrev[x];
                    Z1 = co[I5];
                    Z2 = si[I5];
                    loopend = I4 - 1;
                    for (m = I3; m <= loopend; m++)
                    {
                        A1 = RealF[m];
                        A2 = ImagF[m];
                        mx = m + I1;
                        B1 = Z1 * RealF[mx] - Z2 * ImagF[mx];
                        B2 = Z2 * RealF[mx] + Z1 * ImagF[mx];
                        RealF[m] = (A1 + B1);
                        ImagF[m] = (A2 + B2);
                        RealF[mx] = (A1 - B1);
                        ImagF[mx] = (A2 - B2);
                    }
                    I3 = I3 + (I1 << 1);
                    I4 = I4 + (I1 << 1);
                }
                I1 = I1 >> 1;
                I2 = I2 << 1;
            }
        }


        private double Median(double mag, int z)
        {
            const int len = 4; // one less that odd number of atoms
            double[] srt = new double[len + 1];

            for (int i = 0; i < len; i++)
            {
                medo[z, i] = medo[z, i + 1];
                srt[i] = medo[z, i];
            }
            medo[z, len] = mag;
            srt[len] = mag;
            Array.Sort(srt);
            return srt[len / 2];
        }

        private void FFT_Spectrum()
        {
            try
            {
                int n;
                int z = 0;
                int i = 0;
                bool oldy = true;

                while (z < nofs - 1)  // nofs iterations including first unique one
                {
                    if (oldy)
                    {
                        oldy = false;
                        for (n = bwl - 1; n <= bwh + 1; n++) { signal[n] = 0; }
                        for (n = 0; n < FFTlen; n++)
                        {
                            ImagF[n] = 0;
                            ImagF[FFTlen + n] = 0;
                            RealF[n] = wd[n] * old1[n];
                            RealF[FFTlen + n] = wd[FFTlen + n] * audio_buffer[n];
                        }
                    }
                    else
                    {
                        for (n = 0; n < F2L; n++)
                        {
                            ImagF[n] = 0;
                            RealF[n] = wd[n] * audio_buffer[i + n];  // (double)audio_buffer[i + n];
                        }
                        z++;
                        i += FFTlen;
                    }

                    MyFFT();

                    for (n = bwl - 1; n <= bwh + 1; n++)
                    {
                        byte y = bitrev[n];
                        double pwr = Math.Sqrt(RealF[y] * RealF[y] + ImagF[y] * ImagF[y]);
                        Mag[n, z] = Median(pwr, n);
                        signal[n] += Mag[n, z];
                    }
                }

                for (n = 0; n < FFTlen; n++)  // Save last 64 samples
                {
                    old1[n] = audio_buffer[i + n];

                    if (n >= bwl && n <= bwh)
                    {
                        signal[n] = signal[n] / nofs;
                        Noise[n] = (31 * Noise[n] + signal[n]) / 32;
                        if (Noise[n] < thld) { Noise[n] = thld; }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }


        private void Check_number(string nrs)
        {
            int nr = 0;

            if (Int32.TryParse(nrs, out nr))
            {
                if ((nr < 499) && (nr > 0))
                {
                    rprt_found = true;  // complete check
                    rcvd = nr;
                }
            }
        }

        public void Report_Filter(string stev)
        {
            int number;
            string nmbr;

            try
            {
                int i = stev.IndexOf("NN");
                if (i == 0) { nmbr = stev.Insert(0, "5"); }
                else if ((i >= 1) && (i <= 3)) { nmbr = "5" + stev.Substring(i, stev.Length - i); }
                else if (stev.StartsWith("R5")) { nmbr = stev.Remove(0, 1); }
                else nmbr = stev;
                nmbr = nmbr.Replace("N", "9");
                nmbr = nmbr.Replace("O", "0");
                nmbr = nmbr.Replace("T", "0");
                //            nmbr = nmbr.Replace("A", "1");
                //            nmbr = nmbr.Replace("E", "5");

                rst = "599";
                rcvd = 0;
                rprt_found = false;
                if (nmbr.Length > 3)
                {
                    rst = nmbr.Substring(0, 3);
                    if (Int32.TryParse(rst, out number))
                    {
                        if ((number == 559) || (number == 569) || (number == 579) || (number == 589) || (number == 599))
                        {
                            report = nmbr.Substring(3, nmbr.Length - 3);
                            Check_number(report);
                        }
                    }
                }
                if (!rprt_found) { Check_number(nmbr); }
                if (!rprt_found && !nmbr.Equals("599"))
                {
                    i = nmbr.Length - 1;
                    int j = i;
                    while (i > 0 && Char.IsDigit(nmbr, i)) { i--; }
                    if (i < j) { Check_number(nmbr.Substring(i, j - i + 1)); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


        private bool regex_check(String testValue)
        {

            Regex _rgx = new Regex("[ETIANSH5]");

            return (_rgx.Matches(testValue).Count == testValue.Length);

        }

        private bool similar(string s)
        {
            int eq = 0;
            for (int i = 0; i < s.Length - 1; i++)
            {
                Char c = s[i];
                for (int j = 0; j < call_sent.Length - 1; j++)
                    if (c == call_sent[j])
                        eq++;
            }
            if (eq >= s.Length / 2) { return true; }
            else return false;
        }


        private bool IsCall(string s)
        {
            int i = s.Length;
            if (i >= 3 && i <= 6 && Char.IsLetter(s, i - 1))
            {
                if (Char.IsDigit(s, 0))
                {
                    if (s.StartsWith("3DA")) { return true; }
                    else if (Char.IsLetter(s, 1) && Char.IsDigit(s, 2)) { return true; }
                    else { return false; }
                }
                else if (!s.StartsWith("Q"))
                {
                    if (Char.IsDigit(s, 1) || Char.IsDigit(s, 2)) { return true; }
                    else { return false; }
                }
                else { return false; }
            }
            else { return false; }
        }

        private void Call_Filter(string mystr1)
        {

            string cl, clsn;

            if (mystr1.StartsWith("DE") || mystr1.StartsWith("TU")) { clsn = mystr1.Substring(2, mystr1.Length - 2); }
            else { clsn = mystr1; }

            if ((((clsn.Length) % 2) == 0) && (clsn.Length > 5) && Doubled(clsn))
                cl = clsn.Substring(clsn.Length / 2, clsn.Length / 2);
            else cl = clsn;

            int i = cl.IndexOf("/");
            if (i > 1 && i < cl.Length - 1)
            {
                string[] ports = cl.Split('/');
                foreach (string part in ports)
                {
                    if (IsCall(part)) { call_found = true; }
                }
            }
            else if (i == -1) { call_found = IsCall(cl); }

            if (call_found)
            {
                if (regex_check(cl) || cl.Contains("*") || cl.Contains("?") || cl.Contains(" ") || cl.Contains(mycall)) { call_found = false; }
            }
            if (call_found) { call = cl; }
        }

        private bool Doubled(string mystr0)
        {

            string s1 = mystr0.Substring(0, mystr0.Length / 2);
            string s2 = mystr0.Substring(mystr0.Length / 2, mystr0.Length / 2);
            if (s1.Equals(s2))
                return true;
            else
                return false;

        }

        /*
                private void Analyse()
                {
                    try
                    {
                        for (int z = bwl; z <= bwh; z++)
                        {
                            if (output[z].Contains("-"))
                            {
                                if (!valid[z] || (output[z].Length < 6))
                                {
                                    if ((output[z].Contains("NR?")) || (output[z].Contains("AGN"))) { nr_agn = true; }
                                    if (output[z].StartsWith("CQ")) { lid = true; }
                                    output[z] = String.Empty;
                                }
                                else
                                {
                                    string text = output[z].Substring(0, output[z].Length - 2);
                                    output[z] = String.Empty;
                                    valid[z] = false;
                                    string[] words = text.Split(' ');
                                    MainForm.Invoke(new CrossThreadSetText(SetText), "Set text", z, thd_txt[z], text);
                                    foreach (string mystr in words)
                                    {
                                        if (mystr.Length >= 3)
                                        {
                                            Debug.WriteLine(ctr.ToString() + "  " + z.ToString() + "  " + mystr);
                                            if (mystr.IndexOf("5NN") > 3)
                                            {
                                                string s = mystr.Substring(0, (mystr.IndexOf("5NN")));
                                                { Call_Filter(s); }
                                                Report_Filter(mystr.Substring(mystr.IndexOf("5NN"), mystr.Length - mystr.IndexOf("5NN")));
                                            }
                                            else
                                            {
                                                Report_Filter(mystr);
                                                if (!rprt_found) { Call_Filter(mystr); }
                                            }
                                            if (call_found) { calls[z] = call; }
                                            if (rprt_found) { rprts[z] = rst + rcvd.ToString(); }
                                        }
                                    }
                                    rprt_found = false;
                                    call_found = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
        */
        private void Analyse()
        {
            for (int z = bwl; z <= bwh; z++)
            {
                if (output[z].EndsWith(" "))
                {
                    if (output[z].Contains("NR?") || output[z].Contains("AGN")) { nr_agn = true; }
                    if (output[z].StartsWith("CQ")) { lid = true; }
                    if (!valid[z] || (output[z].Length < 5)) { output[z] = String.Empty; }
                    else
                    {
                        string mystr = output[z].Substring(0, output[z].Length - 1);
                        MainForm.Invoke(new CrossThreadSetText(SetText), "Set text", z, Math.Round(Noise[z]), mystr);
                        output[z] = String.Empty;
                        Report_Filter(mystr);
                        if (rprt_found)
                        {
                            valid[z] = false;
                            rprts[z] = rst + rcvd.ToString();
                            snr[z] = Noise[z];
                        }
                        else if (valid[z])
                        {
                            valid[z] = false;
                            Call_Filter(mystr);
                            if (call_found)
                            {
                                calls[z] = call;
                                snr[z] = Noise[z];
                            }
                        }
                    }
                }
            }
        }

        public int f2len()
        {
            string snr = "000";
            if (serial >= 1000) { snr = serial.ToString(); }
            else { snr = serial.ToString("000"); }
            /*
            snr = snr.Replace('0', 'T');
            snr = snr.Replace('9', 'N');
             */

            return txdots(" 5NN" + snr);
        }


        public int txdots(String message)
        {
            int[] cwnrs = { 22, 20, 18, 16, 14, 12, 14, 16, 18, 20 };
            int[] cwltr = { 8, 12, 14, 10, 4, 12, 12, 10, 6, 16, 12, 12, 10, 8, 14, 14, 16, 10, 8, 6, 10, 12, 12, 14, 16, 14 };

            int i = 0;
            int tx_len = 0;

            transmit = true;
            rx_timer = ponovi;

            do
            {
                Char c = message[i];
                if (Char.IsLetter(c))
                    tx_len += cwltr[c.CompareTo('A')];
                else if (Char.IsDigit(c))
                    tx_len += cwnrs[c.CompareTo('0')];
                else if (c.Equals(' '))
                    tx_len += 4;
                else if (c.Equals('/'))
                    tx_len += 16;
                else if (c.Equals('?'))
                    tx_len += 18;
                else if (!c.Equals('*'))
                    Debug.Write(".");
                i++;
            }
            while (i < message.Length);

            return (tx_len * 5 / nofs);
        }


        private void TRtiming()
        {
            int n = 0;
            try
            {
                if (tx_timer > 0)
                {
                    tx_timer--;
                    if (tx_timer == 0)
                    {
                        for (n = bwl; n <= bwh; n++)
                        {
                            output[n] = String.Empty;
                            rprts[n] = String.Empty;
                            calls[n] = String.Empty;
                            enable[n] = false;
                        }
                        rx_timer = ponovi;
                        Debug.WriteLine(" RX " + ave[moni].ToString());
                        transmit = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cqcqcq()
        {
            lid = false;
            tx_timer = txdots("CQ ") + txdots(mycall) + txdots(" TEST");
            MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send CALL", "");
            //                      MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send F1", "");
            Debug.Write(" CQ " + tx_timer.ToString());
            repeat = false;
            active = 0;
        }

        private bool CallAvail()
        {
            double max = 0;
            call_found = false;
            for (int z = bwl; z <= bwh; z++)
            {
                if (enable[z] && calls[z].Length > 0 && snr[z] > max)
                {
                    call_found = true;
                    call = calls[z];
                    active = z;
                    max = snr[z];
                }
            }
            return call_found;
        }

        private bool RprtAvail()
        {
            double max = 0;
            rprt_found = false;
            if (active > 0)
            {
                for (int z = active - 1; z <= active + 1; z++)
                {
                    if (enable[z] && rprts[z].Length > 0 && snr[z] > max)
                    {
                        rprt_found = true;
                        rst = rprts[z].Substring(0, 3);
                        report = rprts[z].Substring(3, rprts[z].Length - 3);
                        max = snr[z];
                    }
                }
            }

            return rprt_found;
        }

        private bool LateRprt()
        {
            double max = 0;
            rprt_found = false;
            for (int z = bwl; z <= bwh; z++)
            {
                if (enable[z] && rprts[z].Length > 0 && snr[z] > max)
                {
                    rprt_found = true;
                    rst = rprts[z].Substring(0, 3);
                    report = rprts[z].Substring(3, rprts[z].Length - 3);
                    max = snr[z];
                }
            }
            return rprt_found;
        }

        private bool Silence()
        {
            bool quiet = true;
            if (active == 0)
            {
                for (int n = bwl; n <= bwh; n++)
                {
                    if (output[n].Length > 0) { quiet = false; }
                }
            }
            else
            {
                for (int n = active - 1; n <= active + 1; n++)
                {
                    if (output[n].Length > 0) { quiet = false; }
                }
            }
            return quiet;
        }

        private void Responder()
        {
            try
            {
                if (rx_timer > 0) { rx_timer--; }

                if (rx_timer == ponovi / 2 && Silence()) { rx_timer = 0; }

                if (rx_timer == 0 && repeat)
                {
                    tx_timer = txdots(call_sent) + f2len();
                    MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send F5", "");
                    MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send F2", "");
                    Debug.Write(" RPT " + tx_timer.ToString());
                    repeat = false;
                }

                if (nr_agn)
                {
                    nr_agn = false;
                    tx_timer = f2len();
                    MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send F2", "");
                    Debug.Write(" AGN " + tx_timer.ToString());
                }

                if (CallAvail())
                {
                    calls[active] = String.Empty;
                    enable[active] = false;
                    call_found = false;
                    if (call_sent.Equals(mycall) || similar(call) || !repeat)
                    {
                        tx_timer = txdots(call) + f2len();
                        MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send CALL", call);
                        if (similar(call))
                        {
                            tx_timer += txdots(" ?");
                            MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send F2", "");
                        }
                        repeat = true;
                        call_sent = call;
                        Debug.Write(" CL " + tx_timer.ToString());
                    }
                }

                if (RprtAvail())
                {
                    rprts[active] = String.Empty;
                    rprt_found = false;
                    repeat = false;
                    enable[active] = false;
                    tx_timer += txdots(" TU");
                    //                   if (!rst.Equals("599"))
                    MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send RST", rst);
                    MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send NR", report);
                    call_sent = mycall;
                    serial++;
                    active = 0;
                    Debug.Write(" NR1 " + tx_timer.ToString());
                }
                else if (LateRprt())
                {
                    rprts[active] = String.Empty;
                    rprt_found = false;
                    repeat = false;
                    enable[active] = false;
                    tx_timer = txdots("TU");
                    MainForm.Invoke(new CrossThreadCallback(CrossThreadCommand), "Send F3", "");
                    Debug.Write(" NR2 " + tx_timer.ToString());
                }

                if (rx_timer == 0 && active == 0) { cqcqcq(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}


