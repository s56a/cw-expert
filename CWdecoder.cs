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
        delegate void CrossThreadSetText(string command, int channel_no, string out_txt);

        public const int intg = 3;
        public int eot = 0;
        public int mytimer = 0;
        public double glblthd = 8;
        public int activech = 17;
        public int bot = 0;
        public float[] sigave = new float[2048];
        public int tmin = 2;
        public bool lid = false;
        private int frame_segment = 4;
        public bool rx_only = true;
        public bool once = true;
        public int rcvd = 0;
        public int moni = 12;
        public int ponovi = 32;
        public bool repeat = false;
        public int loopend = 0;
        public int totalsamples = 0;
        public bool medijan = false;
        public bool logmagn = false;
        public int logf2l = 0;
        public int rate = 8000;
        public int FFTlen = 64;
        public int F2L = 128;
        public const int aver = 35;
        public int bwl = 1;
        public int bwh = 62;
        public bool run_thread = false;
        public Thread CWThread1;
        public Thread CWThread2;
        public AutoResetEvent AudioEvent1;
        public AutoResetEvent AudioEvent2;
        public float[] audio_buffer_l;
        public float[] audio_buffer_r;
        public float[] fft_buff_ch5;
        public float[] fft_buff_ch6;
        public bool key = false;
        public bool nr_agn = false;
        public bool call_found = false;
        public bool rprt_found = false;
        public bool transmit = false;
        public int serial = 0;
        public string mystr = new string(' ', 64);
        public string mycall = new string(' ', 14);
        public string rst = new string(' ', 3);
        public string report = new string(' ', 4);
        public string call_sent = new string(' ', 14);
        public string call = new string(' ', 14);
        public string nmbr = new string(' ', 7);
        public string morsealpha = new string(' ', 28);
        public string morsedigit = new string(' ', 30);
        public string[] scp = new string[40350];
        public string[] rprts;
        public double[] snr;
        public string[] calls;
        public double thld = 0.01;
        public bool[] enable;
        public int active = 0;
        public string[] output; // = new string[FFTlen];
        public int[] sum; // = new int[FFTlen];
        public int[] ave; // = new int[FFTlen];
        public double[] Noise; // = new double[FFTlen];
        public float[] RealF; // = new float[F2L * 2];
        public float[] ImagF; // = new float[F2L];
        public float[,] Mag; // = new float[32, 32];    // 32 * 8mS, 32 * 64Hz
        public double[,] medo; // = new double[FFTlen, 5];
        public double[] maxim; // = new double[F2L];
        public double[] temp; // = new double[FFTlen];
        public float[] signal; // = new double[F2L];
        public int[] broj; // = new int[FFTlen];
        public double[] si; // = new double[F2L];
        public double[] co; // = new double[F2L];
        public float[] wd; // = new float[F2L];
        public int[] tim; // = new int[FFTlen];
        public double thd = 0;
        public double max = 0;
        private double Period = 0.0f;
        public int[] ctr = new int[22];
        public int tx_timer = 0;
        public int rx_timer = 50;
        public int dotmin = 0;
        public byte[] bitrev; // = new byte[F2L];
        public float[] old1; // = new float[FFTlen];
        public bool[] keyes; // = new bool[FFTlen];
        public bool[] valid; // = new bool[FFTlen];
        public int nofs = 0;
        private float[] agcvol;  //= new float[64];
        private double[] fagcvol;  //= new double[64];
        private HiPerfTimer display_timer;
        private CWExpert MainForm;
        public bool run_rx1 = false;
        public bool run_rx2 = false;
        private Mutex fft_mutex = new Mutex();

        #endregion

        #region properties

        private bool sql_on = false;
        public bool SqlOn
        {
            get { return sql_on; }
            set { sql_on = value; }
        }

        private bool rx2_enabled = false;
        public bool RX2Enabled
        {
            get { return rx2_enabled; }
            set { rx2_enabled = value; }
        }

        #endregion

        #region constructor and destructor

        public CWDecode(CWExpert mainForm)
        {
            try
            {
                MainForm = mainForm;
                display_timer = new HiPerfTimer();
                audio_buffer_l = new float[2048];
                audio_buffer_r = new float[2048];
                fft_buff_ch5 = new float[2048];
                fft_buff_ch6 = new float[2048];
                AudioEvent1 = new AutoResetEvent(false);
                AudioEvent2 = new AutoResetEvent(false);
                once = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        ~CWDecode()
        {

        }

        public void CWDecodeClose()
        {

        }

        #endregion

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
                rate = Audio.SampleRate;
                frame_segment = 32 * (96000 / Audio.SampleRate);

                if (Audio.SDRmode)
                {
                    FFTlen = 64;
                }
                else
                    FFTlen = 64;

                ctr[5] = 0;
                ctr[6] = 0;
                F2L = 2 * FFTlen;
                output = new string[16384];
                sum = new int[FFTlen];
                ave = new int[FFTlen];
                Noise = new double[FFTlen];
                RealF = new float[F2L * 2];
                ImagF = new float[F2L];
                Mag = new float[16 * frame_segment + 1, 32];    // 32 * 8mS, 32 * 64Hz
                medo = new double[FFTlen, 5];
                maxim = new double[F2L];
                temp = new double[FFTlen];
                signal = new float[64];
                broj = new int[FFTlen];
                si = new double[F2L];
                co = new double[F2L];
                wd = new float[F2L];
                tim = new int[FFTlen];
                bitrev = new byte[F2L];
                old1 = new float[FFTlen];
                keyes = new bool[FFTlen];
                valid = new bool[FFTlen];
                agcvol = new float[FFTlen];
                fagcvol = new double[FFTlen];
                totalsamples = F2L;
                logf2l = (int)(Math.Round(Math.Log(totalsamples) / Math.Log(2.0)));
                global_counterA = 0;
                //                Period = (double)totalsamples / 8000;
                Period = (1.0 / Audio.SampleRate) * 2048.0 / ((double)frame_segment);
                rprts = new string[FFTlen];
                snr = new double[FFTlen];
                calls = new string[FFTlen];
                enable = new bool[FFTlen];

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

                nofs = 16 * frame_segment;                                           // number of decoding segments               
                dotmin = (int)Math.Truncate(0.25 * 1.2 / (40 * Period));            // 40 wpm, 40 msec dot

                if (Audio.SDRmode)
                {
                    morsealpha = "ETIANMSURWDKGOHVF*L*PJBXCYZQ";
                    morsedigit = "54*3***2 *+****16=/***(*7***8*90";
                }
                else
                {
                    morsealpha = "ETIANMSURWDKGOHVF*L*PJBXCYZQ";
                    morsedigit = "54*3***2 ******16*/*****7***8*90";
                }

                for (n = 0; n < FFTlen; n++)
                {
                    sigave[n] = (float)thld;
                    signal[n] = (float)thld;
                    agcvol[n] = 0.0f;
                    fagcvol[n] = 0.0;
                    old1[n] = 0;
                    Noise[n] = 1.0;
                    temp[n] = 0;
                    ave[n] = aver;
                    sum[n] = 0;
                    tim[n] = 1;
                    calls[n] = String.Empty;
                    rprts[n] = String.Empty;
                    output[n] = String.Empty;
                    keyes[n] = false;
                    valid[n] = false;
                    broj[n] = 0;
                    for (z = 0; z < 5; z++)
                        medo[n, z] = 0;
                }

                double v = 2 * Math.PI / totalsamples;
                for (n = 0; n < totalsamples; n++)
                {
                    RealF[n] = 0;
                    ImagF[n] = 0;
                    si[n] = -Math.Sin(n * v);
                    co[n] = Math.Cos(n * v);
                    wd[n] = 1;
                }

                mycall = MainForm.SetupForm.txtStnCALL.Text;
                //                MainForm.Callers.Items.Add(mycall);
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

        public bool reset_after_mox = false;
        private void CW_ThreadRX1()
        {
            try
            {
                if (MainForm.OpModeVFOA == Mode.CW)
                    run_rx1 = true;

                if (MainForm.OpModeVFOB == Mode.CW)
                    run_rx2 = true;

                while (run_thread)
                {
                    AudioEvent1.WaitOne();
                    mytimer++;

                    /*if (reset_after_mox)
                    {
                        reset_after_mox = false;

                        for (int n = 0; n < FFTlen; n++)
                        {
                            ave[n] = aver;
                            sum[n] = 0;
                            broj[n] = 0;
                            tim[n] = 0;
                            ctr = 0;
                        }
                    }

                    /*if (Audio.SDRmode)
                    {
                    }
                    else
                    {
                        //Array.Copy(audio_buffer_l, fft.spec_fin, Audio.BlockSize);
                    }

                    display_timer.Stop();
                    time += display_timer.DurationMsec;

                    if (time - previous_time > 66.6)
                    {
                        previous_time = time;
                        display_timer.Start();

                        if (Audio.SDRmode)
                        {
                            MainForm.data_ready = true;
                            MainForm.display_event.Set();
                        }
                        else
                        {
                            /*if (fft.ComputeSpectrum(0))
                            {
                                AGCSpectrum();

#if(DirectX)
                                if (MainForm.VideoDriver == DisplayDriver.DIRECTX)
                                {
                                    Array.Copy(fft.spec_fout, 0, DX.new_display_data, 0, 4096);
                                    Array.Copy(fft.spec_fout, 0, DX.new_waterfall_data, 0, 4096);
                                }
                                else
#endif
                                {
                                    Array.Copy(fft.spec_fout, 0, Display_GDI.new_display_data, 0, 4096);
                                    Array.Copy(fft.spec_fout, 0, Display_GDI.new_waterfall_data, 0, 4096);
                                }

                                MainForm.data_ready = true;
                                MainForm.display_event.Set();
                            }
                        }
                    }
                    else
                        display_timer.Start();*/

                    //fft_timer.Start();

                    fft_mutex.WaitOne();

                    if (FFT_Spectrum(5))
                    {
                        /*fft_timer.Stop();
                        fft_time += fft_timer.DurationMsec;
                        Debug.Write((fft_time - fft_previous_time).ToString() + "\n");
                        fft_previous_time = fft_time;*/

                        /*if (transmit)
                            TRtiming();
                        else*/
                        {
                            if (run_rx1)
                            {
                                Sig2ASCII(5);
                                //Analyse(5);
                            }

                            //if (rx_only)
                            {
                                //Analyse();
                            }
                            /*else
                            {
                                Analyse();

                                if (mytimer - eot > tmin)
                                    Responder();
                            }*/
                        }
                    }

                    fft_mutex.ReleaseMutex();
                }
            }

            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                run_thread = false;
            }
        }

        private void CW_ThreadRX2()
        {
            try
            {
                if (MainForm.OpModeVFOB == Mode.CW)
                    run_rx2 = true;

                while (run_thread)
                {
                    AudioEvent2.WaitOne();

                    fft_mutex.WaitOne();

                    if (FFT_Spectrum(6))
                    {
                        Sig2ASCII(6);
                        //Analyse(6);
                    }

                    fft_mutex.ReleaseMutex();
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
            bwl = 5;
            bwh = 6;

            if (Init())
            {
                run_thread = true;
                CWThread1 = new Thread(new ThreadStart(CW_ThreadRX1));
                CWThread1.Name = "CW Thread RX1";
                CWThread1.Priority = ThreadPriority.Normal;
                CWThread1.IsBackground = true;
                CWThread1.Start();

                CWThread2 = new Thread(new ThreadStart(CW_ThreadRX2));
                CWThread2.Name = "CW Thread RX2";
                CWThread2.Priority = ThreadPriority.Normal;
                CWThread2.IsBackground = true;
                CWThread2.Start();
            }

            return result;
        }

        public void CWdecodeStop()
        {
            try
            {
                run_thread = false;
                AudioEvent1.Set();
                AudioEvent2.Set();
                Thread.Sleep(100);
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void cw2asc(int z)
        {
            char ch = '*';

            if (sum[z] < 29)
                ch = morsealpha[sum[z] - 1];

            else if ((sum[z] >= 31) && (sum[z] <= 62))
            {
                ch = morsedigit[(sum[z] - 31)];

                if (Char.IsDigit(ch))
                {
                    valid[z] = true;

                    if (!lid)
                        rx_timer = ponovi;
                }
            }
            //else if (sum[z] == 75) { ch = '?'; }
            else if (sum[z] >= 75)
            {
                switch (sum[z])
                {
                    case 75:
                        ch = '?';
                        break;
                    case 76:
                        ch = '_';
                        break;
                    case 81:
                        ch = '"';
                        break;
                    case 84:
                        ch = '.';
                        break;
                    case 89:
                        ch = '@';
                        break;
                    case 93:
                        ch = '\\';
                        break;
                    case 96:
                        ch = '-';
                        break;
                    case 103:
                        ch = ';';
                        break;
                    case 106:
                        ch = '!';
                        break;
                    case 108:
                        ch = ')';
                        break;
                    case 114:
                        ch = ',';
                        break;
                    case 119:
                        ch = ':';
                        break;
                    default:
                        //                       Debug.Write("[" + i.ToString() + "]"); 
                        ch = '*';
                        break;
                }
            } 

            output[z] += ch;
            MainForm.Invoke(new CrossThreadSetText(MainForm.CommandCallback), "Set text", z, ch.ToString());
            sum[z] = 0;
            //            valid[z] = true;
        }

        private void CWdecode(int z)
        {
            int t = 0;
            try
            {
                keyes[z] = key;
                t = ctr[z] - tim[z];
                tim[z] = ctr[z];

                if (!key)
                {
                    if (t > ave[z])
                        sum[z]++;
                    else if (t > dotmin)
                        ave[z] = (2 * t + ave[z]) / 2;

                    sum[z]++;
                }
                else if (t < ave[z])
                {
                    if (t > dotmin)
                        ave[z] = (2 * t + ave[z]) / 2;

                    sum[z] = 2 * sum[z];

                    if (sum[z] > 127)
                        sum[z] = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void Channel(int index)
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
                    ctr[index] = 0;
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
                    //Debug.WriteLine(Math.Round(bwl / Period).ToString() + " - " + Math.Round(bwh / Period).ToString() + " Hz " + Math.Round(thld).ToString());
                    cqcqcq();
                }
            }
        }

        private double[] signal_avg = new double[22];
        private double[] sig_avg = new double[22];
        private double[] signal_maxavg = new double[22];
        private double[] sig_maxavg = new double[22];
        private double[] signal_minavg = new double[22];
        private double[] sig_minavg = new double[22];
        public double sql = 1.0;
        private void Sig2ASCII(int index)
        {
            int t0 = ctr[index];
            int n = 0;
            thd = sql * Math.Sqrt(0.03);
            double[] sig_min = new double[22];
            double[] sig_max = new double[22];
            sig_min[index] = 1.0;
            sig_avg[index] = 0.0;

            try
            {
                double thd_const = sql * Math.Sqrt(0.03);
                ctr[index] = t0;

                if (!sql_on)
                {
                    for (n = 0; n < nofs - 4; n++)
                    {
                        if (Mag[n, index] < sig_min[index])
                        {
                            sig_min[index] = Mag[n, index];
                        }

                        if (Mag[n, index] > sig_max[index])
                        {
                            sig_max[index] = Mag[n, index];
                        }

                        sig_avg[index] += Mag[n, index];
                    }

                    sig_avg[index] /= nofs - 4;
                    signal_avg[index] = 0.8 * signal_avg[index] + 0.2 * sig_avg[index];
                    signal_maxavg[index] = 0.8 * signal_maxavg[index] + 0.2 * sig_max[index];
                    signal_minavg[index] = 0.8 * signal_minavg[index] + 0.2 * sig_min[index];
                    thd = Math.Max((signal_maxavg[index] - signal_minavg[index]) * 0.2, thd_const);
                    thd = (signal_maxavg[index] - signal_minavg[index]) * 0.2;
                    thd = 0.8 * signal_avg[index];
                }
                else
                {
                    thd = sql * Math.Sqrt(0.05);
                }

                for (n = 0; n < nofs - 4; n++)
                {
                    int b = broj[index];

                    if (Mag[n, index] > thd && Mag[n + 1, index] > thd && Mag[n + 2, index] > thd
                        && Mag[n + 3, index] > thd && Mag[n + 4, index] > thd)
                    {
                        if (b < 3)
                            b++;
                    }
                    else /*if (Mag[n, z] < thd && Mag[n + 1, z] < thd && Mag[n + 2, z] < thd
                            && Mag[n + 3, z] < thd && Mag[n+4, z] < thd)*/
                    {
                        if (b > 0)
                            b--;
                        else
                            b = 0;
                    }

                    n += 3;
                    broj[index] = b;
                    key = b > 1;
                    ctr[index]++;

                    if (key != keyes[index])
                        CWdecode(index);
                    else if (!key)
                    {
                        int t = ctr[index] - tim[index];

                        if (t == ave[index])
                        {
                            if (sum[index] > 0)
                            {
                                cw2asc(index);
                            }
                        }
                        else if (t == (2 * ave[index]))
                        {
                            output[index] += "|";
                            enable[index] = false;
                            MainForm.Invoke(new CrossThreadSetText(MainForm.CommandCallback), "Set text", index, " ");
                            if (rx_only)
                            {
                                Debug.Write(index.ToString() + " " + output[index] + "\n");
                                //output[z] = string.Empty;
                            }
                        }
                        else if (t == (3 * ave[index]))
                        {
                            enable[index] = true;
                            if (rx_only) { Debug.WriteLine(" "); }
                            MainForm.Invoke(new CrossThreadSetText(MainForm.CommandCallback), "Set text", index, " ");
                        }
                    }
                }

                Mag[0, index] = Mag[n + 2, index];      // save

                if (Audio.SDRmode && !rx2_enabled)
                    index = 22;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
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
                        RealF[m] = (float)(A1 + B1);
                        ImagF[m] = (float)(A2 + B2);
                        RealF[mx] = (float)(A1 - B1);
                        ImagF[mx] = (float)(A2 - B2);
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

        private int global_counterA = 0;
        private int global_counterB = 0;
        private bool FFT_Spectrum(int index)
        {
            try
            {
                bool return_value = false;
                int z = 0;
                int n = 0;
                int i = 0;
                bool oldy = true;
                int k = index;

                z = 0;

                if (!Audio.SDRmode)
                {
                    while (z < nofs - 1)  // nofs iterations including first unique one
                    {
                        if (oldy)
                        {
                            oldy = false;
                            for (n = 0; n < FFTlen; n++)
                            {
                                ImagF[n] = 0;
                                ImagF[FFTlen + n] = 0;
                                RealF[n] = old1[n];
                                RealF[FFTlen + n] = audio_buffer_l[n];
                            }
                        }
                        else
                        {
                            for (n = 0; n < F2L; n++)
                            {
                                ImagF[n] = 0;
                                RealF[n] = audio_buffer_l[i + n];
                            }

                            z++;
                            i += FFTlen;
                        }

                        MyFFT();

                        for (n = bwl - 1; n <= bwh + 1; n++)
                        {
                            byte y = bitrev[n];
                            double pwr = Math.Sqrt(RealF[y] * RealF[y] + ImagF[y] * ImagF[y]);
                            if (medijan) { pwr = Median(pwr, n); }

                            if (logmagn)
                                if (pwr > 0.001) { pwr = Math.Log10(pwr); }

                            Mag[z, n] = (float)pwr;
                            signal[n] += (float)pwr;
                        }
                    }
                }
                else
                {
                        i = 0;

                        switch (index)
                        {
                            case 5:
                                signal[5] = 0.0f;

                                for (z = 1 + global_counterA; z < global_counterA + frame_segment + 1; z++)
                                {
                                    for (n = i; n < (i + 2048 / frame_segment); n++)
                                    {
                                        signal[5] += Math.Abs(fft_buff_ch5[n]);
                                    }

                                    signal[5] /= 2048 / frame_segment;
                                    Mag[z, 5] = signal[5];
                                    i += 2048 / frame_segment;
                                }

                                global_counterA += frame_segment;

                                if (global_counterA == nofs)
                                {
                                    global_counterA = 0;
                                    return_value = true;
                                }
                                break;

                            case 6:
                                signal[6] = 0.0f;

                                for (z = 1 + global_counterB; z < global_counterB + frame_segment + 1; z++)
                                {
                                    for (n = i; n < (i + 2048 / frame_segment); n++)
                                    {
                                        signal[6] += Math.Abs(fft_buff_ch6[n]);
                                    }

                                    signal[6] /= 2048 / frame_segment;
                                    Mag[z, 6] = signal[6];
                                    i += 2048 / frame_segment;
                                }

                                global_counterB += frame_segment;

                                if (global_counterB == nofs)
                                {
                                    global_counterB = 0;
                                    return_value = true;
                                }
                                break;
                        }
                }

                if (!Audio.SDRmode)
                {
                    for (n = 0; n < FFTlen; n++)
                    {
                        old1[n] = audio_buffer_l[i + n];
                    }

                    for (n = bwl-1; n < bwh+1; n++)
                    {
                        signal[n] /= nofs;
                    }
                }
                else
                {
                    signal[index] = 0;
                }

                return return_value;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        private bool Check_number(string nrs)
        {
            int nr = 0;
            if (Int32.TryParse(nrs, out nr))
            {
                if ((nr < 5000) && (nr > 0))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private bool Report_Filter(string stev)
        {
            string nmbr;

            int i = stev.IndexOf("NN");
            if (i >= 0)
                nmbr = "5" + stev.Substring(i, stev.Length - i);
            else nmbr = stev;

            nmbr = nmbr.Replace("N", "9");
            nmbr = nmbr.Replace("O", "0");
            nmbr = nmbr.Replace("T", "0");
            //            nmbr = nmbr.Replace("A", "1");
            //            nmbr = nmbr.Replace("E", "5");

            rst = "599";
            report = nmbr;

            if (nmbr.Length > 3)
            {
                char er = nmbr[0];
                char es = nmbr[1];
                char te = nmbr[2];

                int ih = es.CompareTo('5');

                if (er.Equals('5') && te.Equals('9') && ih >= 0 && ih <= 4)
                {
                    report = nmbr.Substring(3, nmbr.Length - 3);
                    if (ih != 4)
                        rst = nmbr.Substring(0, 3);
                }
            }

            return (Check_number(report));
        }

        private bool regex_check(String testValue)
        {

            Regex _rgx = new Regex("[ETIANMSH456]");

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

        private bool Call_Filter(string mystr1)
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

            return call_found;
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

        private void Analyse(int index)
        {
            string clstr, nrstr;
            int z = index;

            if (output[z].EndsWith("|"))
            {
                if (output[z].Contains("NR?") || output[z].Contains("AGN")) { nr_agn = true; }
                if (output[z].StartsWith("CQ ")) { lid = true; }

                if (output[z].Length > 4)
                {
                    string mystr = output[z].Substring(0, output[z].Length - 1).Replace("|", " ");
                    //MainForm.Invoke(new CrossThreadSetText(MainForm.CommandCallback), "Set text", z, mystr);
                    int ii = mystr.IndexOf("5NN");

                    if (ii > 3)
                    {
                        clstr = mystr.Substring(0, ii);
                        nrstr = mystr.Substring(ii, mystr.Length - ii);
                    }
                    else
                    {
                        nrstr = mystr;
                        clstr = mystr;
                    }

                    if (Report_Filter(nrstr))
                    {
                        rprts[z] = rst + report;
                        snr[z] = Noise[z];
                    }

                    if (valid[z] && Call_Filter(clstr))
                    {
                        calls[z] = call;
                        snr[z] = Noise[z];
                    }

                    output[z] = " ";
                }

                /*if (output[z].Length > 35)
                {
                    output[z] = "";
                    MainForm.Invoke(new CrossThreadSetText(MainForm.CommandCallback), "Clear text", z, output[z]);
                }*/

                valid[z] = false;
            }
        }

        public int f2len()
        {
            string snr = "000";
            if (serial >= 1000) { snr = serial.ToString(); }
            else { snr = serial.ToString("000"); }

            snr = snr.Replace('0', 'T');
            snr = snr.Replace('9', 'N');

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

            return (tx_len * 5 / nofs - 1);
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
                            Noise[n] = thld;
                            enable[n] = false;
                        }
                        rx_timer = ponovi;
                        //Debug.WriteLine(" RX " + ave[moni].ToString());
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
            MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send CALL", "");
            Debug.Write(" CQ " + tx_timer.ToString());
            repeat = false;
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

                if (rx_timer == 0)
                {
                    if (repeat)
                    {
                        tx_timer = txdots(call_sent) + f2len();
                        MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send F5", "");
                        MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send F2", "");
                        Debug.Write(" RPT " + tx_timer.ToString());
                        repeat = false;
                    }
                    else cqcqcq();
                }


                if (nr_agn)
                {
                    nr_agn = false;
                    tx_timer = f2len();
                    MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send F2", "");
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
                        MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send CALL", call);

                        if (similar(call))
                        {
                            tx_timer += txdots(" ?");
                            MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send F2", "");
                        }

                        repeat = !similar(call);
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
                    MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send RST", rst);
                    MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send NR", report);
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
                    MainForm.Invoke(new CrossThreadCallback(MainForm.CrossThreadCommand), "Send F3", "");
                    Debug.Write(" NR2 " + tx_timer.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
