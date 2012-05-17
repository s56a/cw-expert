﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace CWExpert
{
    public partial class LOG_export : Form
    {
        #region Enum

        enum LOGMode
        {
            BARTG_RTTY_Contest = 0,
            RSGB_IOTA_Contest,
            NA_Sprint,
            ARRL_DX_SSB,
            CQ_VHF_Contest,
            ARRL_SS_CW
        }

        #endregion

        #region variable

        LOGStnSettings SettingsForm;
        public string MyCALL = "";
        public string MyName = "";
        public string MyQTH = "";
        public string MyLOC = "";
        public string MyInfo = "";
        public string ContestName = "";
        public string MyClub = "";
        public string Operators1 = "";
        public string Operators2 = "";
        public string MyAddr1 = "";
        public string MyAddr2 = "";
        public string MyCity = "";
        public string MyCountry = "";
        public string MyPhone = "";
        public string Remarks = "";
        public string Antenna = "";
        public string TXequ = "";
        public string RXequ = "";
        public string Category = "";
        public string TXPower = "";
        const double PI = 3.141592653589;

        #endregion

        #region Structures

        struct location
        {
            public double longit;
            public double latit;
        };

        struct polar
        {
            public double distance;
            public double direction;
        };

        #endregion

        public LOG_export()
        {
            InitializeComponent();
            comboLOGformat.SelectedIndex = 0;
        }

        private void btnLOGExport_Click(object sender, EventArgs e)
        {
            try
            {
                int first = (int)udFirst.Value;
                int last = (int)udLast.Value;
                string date = "";
                string time = "";
                string text = "";
                string call = "";
                string nr = "";
                string my_nr = "";
                string freq = "";
                string rst = "";
                string snt = "";
                string day = "";
                string month = "";
                string name = "";
                string qth = "";
                string loc = "";
                string distance = "";
                string zone = "";
                string band = "";
                string info = "";
                string mode = "";
                string my_call = MyCALL.PadRight(10, ' ');
                string my_name = MyName.PadRight(10, ' ');
                string my_qth = MyQTH.PadRight(10, ' ');
                DataRow[] rows = DB.log_ds.Tables["LOG"].Select("", "No");
                DateTime date_time;
                CultureInfo provider = CultureInfo.CurrentCulture;

                if (last <= first)
                {
                    MessageBox.Show("Wrong settings! \n Last must be greater than first.", "Error!");
                    return;
                }

                switch (comboLOGformat.Text)
                {
                    case "BARTG RTTY Contest":
                        text = "BARTG RTTY Contest\n";
                        rtbLOGPreview.AppendText(text);

                        for (int i = first - 1; i < last; i++)
                        {
                            date = rows[i]["Date"].ToString();
                            DateTime.TryParse(date, provider, DateTimeStyles.AssumeLocal, out date_time);
                            day = date_time.Day.ToString();
                            day = day.PadLeft(2, '0');
                            month = date_time.Month.ToString();
                            month = month.PadLeft(2, '0');
                            date = date_time.Year + "-" + month + "-" + day;
                            time = rows[i]["Time"].ToString();
                            string[] t = time.Split(':');
                            time = t[0] + t[1];
                            time = time.PadLeft(4, '0');
                            call = rows[i]["CALL"].ToString();
                            call = call.ToUpper();
                            call = call.PadRight(10);
                            nr = rows[i]["NR"].ToString();
                            nr = nr.PadLeft(4, '0');
                            my_nr = rows[i]["MyNR"].ToString();
                            my_nr = my_nr.PadLeft(4, '0');
                            freq = rows[i]["Freq"].ToString();
                            freq = freq.PadLeft(6, ' ');
                            freq = freq.Remove(5);
                            rst = rows[i]["RST"].ToString();
                            rst = rst.PadRight(3, ' ');
                            snt = rows[i]["SNT"].ToString();
                            snt = snt.PadRight(3, ' ');

                            text = freq + " RY " +
                                date + " " + time + " " +
                                my_call + " " + snt + " " + 
                                my_nr + " " + call + " " +
                                rst + " " + nr + " " + "\n";
                            rtbLOGPreview.AppendText(text);
                        }
                        break;

                    case "NA Sprint":
                        my_qth = my_qth.Remove(3);
                        my_name = my_name.Remove(9);
                        text = "NA Sprint\n";
                        rtbLOGPreview.AppendText(text);

                        for (int i = first - 1; i < last; i++)
                        {
                            date = rows[i]["Date"].ToString();
                            DateTime.TryParse(date, provider, DateTimeStyles.AssumeLocal, out date_time);
                            day = date_time.Day.ToString();
                            day = day.PadLeft(2, '0');
                            month = date_time.Month.ToString();
                            month = month.PadLeft(2, '0');
                            date = date_time.Year + "-" + month + "-" + day;
                            time = rows[i]["Time"].ToString();
                            string[] t = time.Split(':');
                            time = t[0] + t[1];
                            time = time.PadLeft(4, '0');
                            call = rows[i]["CALL"].ToString();
                            call = call.ToUpper();
                            call = call.PadRight(10);
                            nr = rows[i]["NR"].ToString();
                            nr = nr.PadLeft(4, '0');
                            my_nr = rows[i]["MyNR"].ToString();
                            my_nr = my_nr.PadLeft(4, '0');
                            freq = rows[i]["Freq"].ToString();
                            freq = freq.PadLeft(6, ' ');
                            freq = freq.Remove(5);
                            name = rows[i]["Name"].ToString();
                            name = name.Trim();
                            name = name.PadRight(10, ' ');
                            name = name.Remove(9);
                            qth = rows[i]["QTH"].ToString();
                            qth = qth.Trim();
                            qth = qth.PadRight(4, ' ');
                            qth = qth.Remove(3);

                            text = freq + " CW " + date + " " + time + " " +
                                my_call + " " + my_nr + " " + my_name + " " + my_qth + " " +
                                call + " " + nr + " " + name + " " + qth + "\r\n";
                            rtbLOGPreview.AppendText(text);
                        }
                        break;

                    case "TXT":
                        int no = 1;

                        for (int i = first - 1; i < last; i++)
                        {
                            date = rows[i]["Date"].ToString();
                            time = rows[i]["Time"].ToString();
                            call = rows[i]["CALL"].ToString();
                            nr = rows[i]["NR"].ToString();
                            my_nr = rows[i]["MyNR"].ToString();
                            freq = rows[i]["Freq"].ToString();
                            name = rows[i]["Name"].ToString();
                            name = name.Replace("\n", " ");
                            name = name.Replace("\r", " ");
                            qth = rows[i]["QTH"].ToString();
                            qth = qth.Replace("\n", " ");
                            qth = qth.Replace("\r", " ");
                            rst = rows[i]["RST"].ToString();
                            snt = rows[i]["SNT"].ToString();
                            zone = rows[i]["Zone"].ToString();
                            band = rows[i]["Band"].ToString();
                            info = rows[i]["Info"].ToString();
                            info = info.Replace("\n", " ");
                            info = info.Replace("\r", " ");
                            mode = rows[i]["Mode"].ToString();

                            text = no.ToString() + " " + date + " " + time + " " +
                                call + " " + rst + " " + snt + " " + my_nr + " " +
                                nr + " " + nr + " " + zone + " " + freq + " " + band +
                                " " + mode + " " + loc + " " + qth + " " + name + " " + info + "\r\n";
                            no++;
                            rtbLOGPreview.AppendText(text);
                        }
                        break;

                    case "EDI":
                        string last_date = "";
                        int qrb = 0;
                        int qrb_final = 0;
                        string loc_list = "";
                        string tmp_loc = "";
                        int loc_count = 0;
                        int odx_qrb = 0;
                        string tmp_output = "";
                        string odx = "";
                        string mode_code = "";

                        for (int i = first - 1; i < last; i++)
                        {
                            date = rows[i]["Date"].ToString();
                            DateTime.TryParse(date, provider, DateTimeStyles.AssumeLocal, out date_time);
                            day = date_time.Day.ToString();
                            day = day.PadLeft(2, '0');
                            month = date_time.Month.ToString();
                            month = month.PadLeft(2, '0');
                            date = date_time.Year + month + day;
                            time = rows[i]["Time"].ToString();
                            string[] t = time.Split(':');
                            time = t[0] + t[1];
                            time = time.PadLeft(4, '0');
                            call = rows[i]["CALL"].ToString();
                            call = call.ToUpper();
                            nr = rows[i]["NR"].ToString();
                            nr = nr.PadLeft(4, '0');
                            my_nr = rows[i]["MyNR"].ToString();
                            my_nr = my_nr.PadLeft(4, '0');
                            snt = rows[i]["SNT"].ToString();
                            rst = rows[i]["RST"].ToString();
                            loc = rows[i]["LOC"].ToString();
                            loc = loc.ToUpper();
                            loc = loc.PadRight(6);
                            tmp_loc = loc.Remove(3, 2);
                            mode = rows[i]["Mode"].ToString();

                            switch (mode)
                            {
                                case "SSB":
                                    mode_code = "1";
                                    break;

                                case "CW":
                                    mode_code = "2";
                                    break;

                                case "AM":
                                    mode_code = "5";
                                    break;

                                case "RTTY":
                                    mode_code = "7";
                                    break;

                                case "SSTV":
                                    mode_code = "8";
                                    break;

                                case "ATV":
                                    mode_code = "9";
                                    break;

                                default:
                                    mode_code = "0";
                                    break;
                            }

                            if (!loc_list.Contains(tmp_loc))
                            {
                                loc_list += tmp_loc + " ";
                                loc_count++;
                            }

                            qrb = CalculateQRB(MyLOC, loc);

                            if (qrb >= 0)
                            {
                                qrb_final += qrb;
                                distance = qrb.ToString();

                                if (qrb > odx_qrb)
                                {
                                    odx_qrb = qrb;
                                    odx = call + ";" + loc + ";" + odx_qrb.ToString();
                                }
                            }
                            else
                                distance = "";

                            tmp_output += date + ";" + time + ";" +
                                call + ";" + mode_code + ";" + snt + ";" + my_nr +
                                ";" + rst + ";" + nr + ";;" + loc + ";" + distance + ";;;" +
                                "\r\n";
                        }

                        date = rows[first - 1]["Date"].ToString();
                        DateTime.TryParse(date, provider, DateTimeStyles.AssumeLocal, out date_time);
                        day = date_time.Day.ToString();
                        day = day.PadLeft(2, '0');
                        month = date_time.Month.ToString();
                        month = month.PadLeft(2, '0');
                        date = date_time.Year + month + day;
                        last_date = rows[last - 1]["Date"].ToString();
                        DateTime.TryParse(last_date, provider, DateTimeStyles.AssumeLocal, out date_time);
                        day = date_time.Day.ToString();
                        day = day.PadLeft(2, '0');
                        month = date_time.Month.ToString();
                        month = month.PadLeft(2, '0');
                        last_date = date_time.Year + month + day;

                        text += ContestName + "\n";
                        text += "[REG1TEST;1] \n";
                        text += "Tname=" + ContestName + "\n"; ;
                        text += "Tdate:" + date + ";" + last_date + "\n";
                        text += "PCall=" + MyCALL + "\n";
                        text += "PWWLo=" + MyLOC + "\n";
                        text += "PExch=\n";
                        text += "PAdr1=" + MyAddr1 + "\n";
                        text += "PAdr2=" + MyAddr2 + "\n";
                        text += "PSect=\n";
                        text += "PBand=\n";
                        text += "PClub=" + MyClub + "\n";
                        text += "RName=" + MyName + "\n";
                        text += "RCall=\n";
                        text += "RAdr1=" + MyAddr1 + "\n";
                        text += "RAdr2=" + MyAddr2 + "\n";
                        text += "RPoco\n";
                        text += "RCity=" + MyCity + "\n";
                        text += "RCoun=" + MyCountry + "\n";
                        text += "RPhon\n";
                        text += "RHBBS=\n";
                        text += "MOpe1=" + Operators1 + "\n";
                        text += "MOpe2=" + Operators2 + "\n";
                        text += "STXEq=" + TXequ + "\n";
                        text += "SPowe=" + TXPower + "\n";
                        text += "SRXEq=" + RXequ + "\n";
                        text += "SAnte=\n";
                        text += "SAntH=" + Antenna + "\n";
                        text += "CQSOs=" + (last - first + 1).ToString() + "\n";
                        text += "CQSOP=" + qrb_final.ToString() + "\n";
                        text += "CWWLs=" + loc_count.ToString() + "\n";
                        text += "CWWLB=\n";
                        text += "CExcs=\n";
                        text += "CExcB=\n";
                        text += "CDXCs=\n";
                        text += "CDXCB=\n";
                        text += "CToSc=\n";
                        text += "CODXC=" + odx + "\n";
                        text += "[Remarks]" + "\n" + Remarks + "\n";
                        text += "[QSOrecords;" + (last - first + 1).ToString() + "]\n";
                        text += tmp_output;

                        rtbLOGPreview.AppendText(text);
                        break;

                    case "Custom":
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnLOGSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                string path = Application.StartupPath;
                saveFileDialog1.InitialDirectory = path;
                saveFileDialog1.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                MessageBox.Show("Failed to export LogBook! \n" + ex.ToString(), "Error!");
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                FileStream file;
                ASCIIEncoding buff = new ASCIIEncoding();
                byte[] buffer = new byte[rtbLOGPreview.Text.Length];
                buff.GetBytes(rtbLOGPreview.Text.ToString(), 0, rtbLOGPreview.Text.ToCharArray().Length, buffer, 0);
                file = File.Create(saveFileDialog1.FileName);
                file.Write(buffer, 0, buffer.Length);
                file.Flush();
                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnPreviewClear_Click(object sender, EventArgs e)
        {
            try
            {
                rtbLOGPreview.Clear();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private int CalculateQRB(string my_loc, string loc)
        {
            try
            {
                char[] from = new char[6];
                char[] to = new char[6];
                location from_spheric = new location();
                location to_spheric = new location();
                polar bearing = new polar();
                int from_status, to_status;
                int distance = 0;

                my_loc = my_loc.ToLower();
                loc = loc.ToLower();

                from = my_loc.ToCharArray();
                to = loc.ToCharArray();

                from_status = convert(from, ref from_spheric);
                to_status = convert(to, ref to_spheric);

                if ((from_status + to_status) > 0)
                    return (0);

                transform(ref from_spheric, ref to_spheric, ref bearing);

                distance = (int)bearing.distance;

                return distance;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return 0;
            }
        }

        int convert(char[] loc, ref location place)
        {
            try
            {
                int i;

                for (i = 0; i <= 1; i++)
                {
                    if ((loc[i] < 'a') || (loc[i] > 'r')) return 1;
                }

                for (i = 2; i <= 3; i++)
                {
                    if ((loc[i] < '0') || (loc[i] > '9')) return 1;
                }

                for (i = 4; i <= 5; i++)
                {
                    if ((loc[i] < 'a') || (loc[i] > 'x')) return 1;
                }

                (place.latit) = 10 * (loc[1] - 'a') - 90;
                (place.latit) += loc[3] - '0';
                (place.latit) += (loc[5] - 'a') / 24.0 + 1 / 48.0;
                (place.latit) *= PI / 180.0;

                (place.longit) = 20 * (loc[0] - 'a') - 180;
                (place.longit) += 2 * (loc[2] - '0');
                (place.longit) += (loc[4] - 'a') / 12.0 + 1 / 24.0;
                (place.longit) *= PI / 180.0;

                return 0;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return 1;
            }
        }

        int transform(ref location from, ref location to, ref polar dd)
        {
            try
            {
                double temp, nominator, radius;

                radius = 180.0 / PI * (111.1451 + 0.56 * Math.Sin(to.latit + from.latit - PI / 2.0));

                (dd.distance) = radius * Math.Acos(temp = (Math.Sin(to.latit) * Math.Sin(from.latit)
                     + Math.Cos(to.latit) * Math.Cos(from.latit) * Math.Cos(to.longit - from.longit)));

                if (dd.distance < 0.1)
                {
                    dd.direction = 0;
                }
                else
                {
                    (dd.direction) = Math.Atan((Math.Cos(from.latit)
                                              * Math.Sin(to.longit - from.longit)
                                              * Math.Cos(to.latit))
                      / (nominator = (Math.Sin(to.latit) - temp * Math.Sin(from.latit))));
                    if (nominator < 0) dd.direction -= PI;
                    if (dd.direction < 0) dd.direction += 2 * PI;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return 0;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (SettingsForm == null || SettingsForm.IsDisposed)
                    SettingsForm = new LOGStnSettings(this);

                SettingsForm.Show();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }
}
