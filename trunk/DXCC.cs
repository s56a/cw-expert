using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;

namespace CWExpert
{
    public partial class DXCC : Form
    {
        #region DLL import

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion

        #region variable

        const int WM_VSCROLL = 0x115;
        const int SB_BOTTOM = 7;

        public string call = new string(' ', 14);
        public string pfx = new string(' ', 6);
        public string sfx = new string(' ', 6);
        public string cty = new string(' ', 4);
        public string beam = new string(' ', 3);
        public string car = new string(' ', 3);
        public string cont = new string(' ', 2);
        public string zon = new string(' ', 2);
        public string itu = new string(' ', 2);
        public string state = new string(' ', 2);
        public int dxpos = 0;
        public int dxcnt = 0;
        public int sttcnt = 0;
        public int execnt = 0;
        public int usacnt = 0;
        public int itucnt = 0;
        public bool cqww = false;
        public string stnr = "1";
        string[] dxcl = new string[350];
        string[] itul = new string[150];
        string[] exebfr = new string[20];
        string[] usabfr = new string[20];
        string[] wvest = new string[64];

        #endregion

        public DXCC()
        {
            this.AutoScaleMode = AutoScaleMode.Inherit;
            InitializeComponent();
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = 8.25f / ratio;
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            GetOptions();
            SetWindowPos(this.Handle.ToInt32(), -1, this.Left, this.Top,
                    this.Width, this.Height, 0);  // on top others
        }

        private void DXCC_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void DXCC_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #region Save/Restore settings

        public void SaveOptions()
        {
            try
            {
                ArrayList a = new ArrayList();

                a.Add("DXCC_top/" + this.Top.ToString());		// save form positions
                a.Add("DXCC_left/" + this.Left.ToString());

                DB.SaveVars("DXCCOptions", ref a);		        // save the values to the DB
                DB.Update();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in DXCC SaveOptions function!\n" + ex.ToString());
            }
        }

        public void GetOptions()
        {
            try
            {
                ArrayList a = DB.GetVars("DXCCOptions");
                a.Sort();

                foreach (string s in a)
                {
                    string[] vals = s.Split('/');
                    string name = vals[0];

                    if (vals.Length > 2)
                    {
                        for (int i = 2; i < vals.Length; i++)
                            vals[1] += "/" + vals[i];
                    }

                    string val = vals[1];

                    if (s.StartsWith("DXCC_top"))
                    {
                        int top = Int32.Parse(vals[1]);
                        this.Top = top;
                    }
                    else if (s.StartsWith("DXCC_left"))
                    {
                        int left = Int32.Parse(vals[1]);
                        this.Left = left;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        public string Pr4(string cty, string pfx, string sfx)
        {
            cty = cty + pfx[pfx.Length - 1] + sfx[0];
            return cty;
        }

        public string GetState(string pfx, string sfx)
        {

            int i = usacnt;
            bool found = false;

            string cl = pfx + sfx;

            while ((i > 0) & !found)
            {
                i--;
                string s = usabfr[i];
                if (s != null && s.Length > 2 && s.StartsWith(cl))
                    found = true;
            }

            if (found)
            {
                state = usabfr[i].Substring(12, 2);
                FindStateNr(state);
            }
            else
            {
                stnr = pfx.Substring(pfx.Length - 1);
                switch (stnr)
                {
                    case "0": state = "MN"; break;
                    case "1": state = "MA"; break;
                    case "2": state = "NY"; break;
                    case "3": state = "PA"; break;
                    case "4": state = "FL"; break;
                    case "5": state = "TX"; break;
                    case "6": state = "CA"; break;
                    case "7": state = "AZ"; break;
                    case "8": state = "OH"; break;
                    case "9": state = "IL"; break;
                }
            }
            return state;
        }


        public void FindStateNr(string ulaz)
        {
            string s = "";
            string s2 = "  ";
            string s4 = "    ";

            bool found = false;
            int i = sttcnt;

            while (!found & (i > 0))
            {
                i--;
                s = wvest[i];
                if (s != null && s != "")
                {
                    s2 = s.Substring(0, 2);
                    s4 = s.Substring(2, 4);
                    if (s4.IndexOf(' ') > 1)
                        s4 = s4.Substring(0, s4.IndexOf(' '));
                    if ((ulaz.Equals(s2)) || (ulaz.StartsWith(s4)) || (s4.StartsWith(ulaz)))
                    {
                        state = s2;
                        stnr = s.Substring(7, 1);
                        found = true;
                    }
                }
            }
        }


        public void USA(string pfx, string sfx)
        {
            cty = "K" + pfx.Substring(1);

            if (pfx[1] == 'P')
            {
                if (pfx[2] == '3')
                    cty = "KP4";
            }
            else if (pfx == "KG4" & sfx.Length == 2)
                cty = "KG4";
            else if (pfx[1] == 'H')
            {
                if (pfx[2] == '7' & sfx[0] != 'K')
                    cty = "KH6";
            }
            else if (pfx[1] != 'L')
            {
                cty = "K";
                state = GetState(pfx, sfx);
                car = cty + stnr;

                string z5 = "1234";
                string z3 = "67";

                if (z5.Contains(stnr))
                {
                    zon = "05";
                    itu = "08";
                }
                else if (z3.Contains(stnr))
                {
                    zon = "03";
                    itu = "06";
                }
                else
                {
                    zon = "04";
                    itu = "07";
                }
            }
        }


        public void Russia(string pfx, string sfx)
        {
            int m = 2;
            if (char.IsDigit(pfx[1]))
                m = 1;

            if ((pfx[m] == '0') || (pfx[m] == '8') || (pfx[m] == '9'))
                cty = "UA9";
            else if (pfx[m] == '2' & (sfx[0] == 'F' || sfx[0] == 'K'))
                cty = "UA2";
            else if (pfx == "RI1")
            {
                if (sfx.StartsWith("AN"))
                    cty = "ANT";
                else if (sfx.StartsWith("FJ"))
                    cty = "R1FJ";
            }
            else
                cty = "UA";

            string s = sfx.Substring(0, 1);

            if (pfx[m] == '0')
            {
                string z32 = "AOPRSTWY";
                string z33 = "CDJUV";
                string z34 = "EFGLMN";
                string z19 = "CDEFGIJKLMNQRXZ";

                if (z19.Contains(s))
                    zon = "19";
                else if (s == "Y")
                    zon = "23";
                else
                    zon = "18";

                if (z32.Contains(s))
                    itu = "32";
                else if (z33.Contains(s))
                    itu = "33";
                else if (z34.Contains(s))
                    itu = "34";
                else
                    switch (sfx[0])
                    {
                        case 'H': itu = "22"; break;
                        case 'B': itu = "22"; break;
                        case 'Q': itu = "23"; break;
                        case 'I': itu = "24"; break;
                        case 'X': itu = "25"; break;
                        case 'K': itu = "26"; break;
                        case 'Z': itu = "35"; break;
                    }
            }
            else if (pfx[m] == '1')
            {
                string z19 = "NOZ";
                if (z19.Contains(s))
                    itu = "19";
                else if (sfx[0] == 'P')
                    itu = "20";
            }
            else if (pfx[m] == '4')
            {
                string z30 = "HIW";
                if (z30.Contains(s))
                    itu = "30";
            }
            else if (pfx[m] == '8' || pfx[m] == '9')
            {
                string z20 = "JKX";
                string z21 = "LMN";
                string z16 = "STWX";
                string z18 = "HIOPUVYZ";

                if (z16.Contains(s))
                    zon = "16";
                else if (z18.Contains(s))
                {
                    zon = "18";
                    if (sfx[0] == 'V')
                        itu = "33";
                    else
                        itu = "31";
                }
                else
                    zon = "17";

                if (z20.Contains(s))
                    itu = "20";
                else if (z21.Contains(s))
                    itu = "21";
                else if (zon != "18")
                    itu = "30";
            }
        }


        public void VEC(string pfx, string sfx)
        {
            cty = "VE";
            car = cty + pfx[2];

            switch (pfx[2])
            {
                case '0': if (pfx == "CY0")
                    {
                        itu = "09";
                        zon = "05";
                        state = "SI";
                        cty = "CY0";
                    }
                    else if (pfx == "VY0")
                    {
                        itu = "03";
                        state = "NU";
                        zon = "02";
                    }
                    break;

                case '1': if (pfx[1] == 'Y')
                    {
                        itu = "02";
                        state = "YK";
                        zon = "01";
                    }
                    else
                    {
                        itu = "09";
                        zon = "05";
                        if (pfx[2] == 'O')
                            state = "NF";
                        else
                            state = "NS";
                    }
                    break;

                case '2': if (pfx[1] == 'O')
                    {
                        state = "LB";
                        zon = "02";
                        itu = "09";
                    }
                    else
                    {
                        itu = "09";
                        zon = "05";
                        if (pfx[2] == 'Y')
                            state = "PE";
                        else
                            state = "QU";
                    }
                    break;

                case '3':
                    {
                        itu = "04";
                        zon = "04";
                        state = "ON";
                    }
                    break;

                case '4':
                    {
                        itu = "03";
                        zon = "04";
                        state = "MB";
                    }
                    break;

                case '5':
                    {
                        itu = "03";
                        zon = "04";
                        state = "SK";
                    }
                    break;

                case '6':
                    {
                        itu = "02";
                        zon = "04";
                        state = "AB";
                    }
                    break;

                case '7':
                    {
                        itu = "02";
                        zon = "03";
                        state = "BC";
                    }
                    break;

                case '8':
                    {
                        itu = "02";
                        zon = "01";
                        state = "NW";
                    }
                    break;

                case '9':
                    {
                        itu = "09";
                        zon = "05";
                        if (pfx == "CY9")
                        {
                            state = "SP";
                            cty = "CY9";
                        }
                        else
                        {
                            state = "NB";
                        }
                    }
                    break;
            }
        }



        public void VKC(string pfx, string sfx)
        {
            cty = "VK";
            zon = "30";
            itu = "59";

            if (pfx[2] == '0' || pfx[2] == '9')
            {
                cty += pfx[2] + sfx[0];
                if (cty[3] == 'W')
                    cty = "VK9Z";
            }
            else if (pfx[2] == '4')
                itu = "55";
            else if (pfx[2] == '6' || pfx[2] == '8')
            {
                zon = "29";
                itu = "55";
            }
        }


        public void BYC(string pfx, string sfx)
        {
            int m = 2;
            if (char.IsDigit(pfx[1]))
                m = 1;

            cty = "BY";
            zon = "24";
            itu = "44";

            char c = sfx[0];

            switch (pfx[m])
            {
                case '0':
                    {
                        zon = "23";
                        itu = "42";
                    }
                    break;
                case '2':
                    if (c >= 'A' && c <= 'P')
                        itu = "33";
                    break;
                case '3':
                    if (c >= 'G' && c <= 'L')
                    {
                        zon = "23";
                        itu = "33";
                    }
                    break;
                case '6':
                    if (c >= 'Q' && c <= 'Z')
                        itu = "43";
                    break;
                case '7':
                    if (c >= 'A' && c <= 'H')
                        itu = "43";
                    break;
                case '8':
                    itu = "43";
                    break;
                case '9':
                    if (c >= 'A' && c <= 'F')
                        itu = "43";
                    else if (c >= 'G' && c <= 'R')
                    {
                        zon = "23";
                        itu = "43";
                    }
                    else if (c >= 'S' && c <= 'Z')
                    {
                        zon = "23";
                        itu = "42";
                    }
                    break;
            }
        }


        public void CEC(string pfx, string sfx)
        {
            cty = "CE";
            if (pfx[2] >= '6' && pfx[2] <= '8')
                itu = "16";
            else if (pfx[2] == '0')
                cty += pfx[2] + sfx[0];
            else if (pfx[2] == '9')
                cty = "ANT";
        }

        public void EAC(string pfx, string sfx)
        {
            cty = "EA";
            string s = pfx.Substring(2, 1);
            string ea = "689";
            if (ea.Contains(s))
                cty += s;
        }


        public void Itus(string pfx, string sfx)
        {
            string pc = "";
            bool hit = false;
            int j = 1;
            int i = 0;
            int k = itucnt - 1;
            int l = 0;

            cty = pfx.Substring(0, 2);

            do
            {
                i = (j + k) / 2;
                pc = itul[i];
                l = pfx.CompareTo(pc);
                if (l > 0)
                    j = i + 1;
                else
                    k = i - 1;
                hit = ((pfx[0] == pc[0]) & (pfx[1].CompareTo(pc[1]) >= 0) & (pfx[1].CompareTo(pc[2]) <= 0));
            }
            while (!hit & j <= k);

            if (hit)
            {
                if (pc.Length > 4)
                    cty = pc.Substring(3, 2);
                else
                    cty = pc.Substring(3, 1);

                if (pc.Length > 6)
                {
                    if (pfx[2] == pc[6] || pfx[2] == pc[5])
                        cty = Pr4(cty, pfx, sfx);
                }
                else if (pc.Length > 5)
                {
                    if (pfx[2] == pc[5] || (pc[5] == 'N'))
                        cty = Pr4(cty, pfx, sfx);
                    else if (pc[5] == 'C')
                    {
                        switch (pc[3])
                        {
                            case '4': if (pc[4] == 'U')
                                {
                                    if (sfx.Equals("WB"))
                                        cty = "K";
                                    else if (sfx.Equals("GSC"))
                                        cty = "I";
                                    else
                                        cty = cty + "/" + sfx[0];
                                }
                                break;
                            case 'B':
                                {
                                    cty = Pr4(cty, pfx, sfx);
                                    if (cty.EndsWith("9P"))
                                        cty = "BV9P";
                                    else if (cty.Equals("BV7H"))
                                        cty = "BV7H";
                                    else if (pfx[1] == 'S' || pfx[1] == 'V')
                                        cty = "BV";
                                    else
                                        BYC(pfx, sfx);
                                }
                                break;
                            case 'C': if (pc[4] == 'E')
                                    CEC(pfx, sfx);
                                else
                                {
                                    if (pfx.Equals("CR2") || pfx[2] == 'U')
                                        cty = "CU";
                                    else if (pfx.EndsWith("3") || pfx.EndsWith("9"))
                                        cty = "CT3";
                                    else
                                        cty = "CT";
                                }
                                break;
                            case 'E': EAC(pfx, sfx); break;
                            case 'L':
                                if (sfx[0] == 'Z')
                                    cty = "ANT";
                                else
                                {
                                    string s = sfx.Substring(0, 1);
                                    string z16 = "VWX";
                                    if (z16.Contains(s))
                                        itu = "16";
                                }
                                break;
                            case 'O':
                                if (pfx[2] == '0')
                                {
                                    if (pfx[1] == 'J')
                                        cty = "OJ0";
                                    else
                                        cty = "OH0";
                                }
                                break;
                            case 'P':
                                if (pfx[1] == 'J')
                                    cty += pfx[2];
                                else if (pfx[2] == '0')
                                    cty += pfx[2] + sfx[0];
                                else
                                {
                                    if (pfx[2] == '8')
                                    {
                                        string z12 = "PTVW";
                                        if (z12.Contains(pfx[1]))
                                            itu = "12";
                                        else
                                            itu = "13";
                                    }
                                    else if (pfx[2]=='6' || pfx[2]=='7' || pfx.Contains("Q2"))
                                        itu = "13";
                                }
                                break;
                            case 'V':
                                if (pc[4] == 'E')
                                    VEC(pfx, sfx);
                                else if (pc[4] == 'K')
                                    VKC(pfx, sfx);
                                else
                                    cty = Pr4(cty, pfx, sfx);
                                break;
                        }
                    }
                }
            }
        }


        public void DXposition(string wntd)
        {
            int i = 1;
            int j = dxcnt - 1;
            int k = 0;
            int l = 0;
            string s = "    ";

            do
            {
                k = (i + j) / 2;
                s = dxcl[k].Substring(0, 4);
                l = s.IndexOf(' ');
                if (l > 0)
                    s = s.Substring(0, l);
                l = wntd.CompareTo(s);
                if (l > 0)
                    i = k + 1;
                else
                    j = k - 1;
            }
            while ((s != wntd) & (i <= j));

            if (wntd != s)
                dxpos = j;
            else
                dxpos = k;

            s = dxcl[dxpos];
            cty = s.Substring(0, 4);
            if (zon == "  ")
                zon = s.Substring(8, 2);
            if (itu == "  ")
                itu = s.Substring(6, 2);

            cont = s.Substring(4, 2);
            beam = s.Substring(10, 3);
        }


        public string DXstatus(string pfx, string sfx)
        {
            string uk = "DIJMUW";
            string altg = "TNHSPC";

            cty = pfx.Substring(0, 2);
            string s = cty.Substring(1, 1);

            switch (cty[0])
            {
                case '2':
                    {
                        if (altg.Contains(s))
                            cty = "G" + s;
                        else if (uk.Contains(s))
                            cty = "G" + s;
                        else
                            cty = "G";
                    }
                    break;
                case 'A':
                    if (pfx[2] >= 'A' && pfx[2] <= 'L')
                        USA(pfx, sfx);
                    else
                        Itus(pfx, sfx);
                    break;
                case 'B':
                    {
                        cty = Pr4(cty, pfx, sfx);
                        if (cty.EndsWith("9P"))
                            cty = "BV9P";
                        else if (cty.Equals("BV7H"))
                            cty = "BV7H";
                        else if (pfx[1] == 'S' || pfx[1] == 'V')
                            cty = "BV";
                        else
                            BYC(pfx, sfx);
                    }
                    break;
                case 'D':
                    if (pfx[2] >= 'A' && pfx[2] <= 'R')
                        cty = "DL";
                    else
                        Itus(pfx, sfx);
                    break;
                case 'E':
                    if (pfx[2] >= 'A' && pfx[2] <= 'H')
                        EAC(pfx, sfx);
                    else
                        Itus(pfx, sfx);
                    break;
                case 'F':
                    {
                        string tro = "TRO";
                        string franki = "GHJKMPSWY";

                        if (tro.Contains(s))
                        {
                            cty = cty + "/" + sfx[0];
                            if (cty == "FT/Y")
                                cty = "ANT";
                        }
                        else if (!franki.Contains(s))
                            cty = "F";
                    }
                    break;
                case 'G':
                    {
                        if (uk.Contains(s))
                            cty = "G" + s;
                        else
                            cty = "G";
                    }
                    break;
                case 'M':
                    {
                        if (altg.Contains(s))
                            cty = "G" + s;
                        else if (uk.Contains(s))
                            cty = "G" + s;
                        else
                            cty = "G";
                    }
                    break;
                case 'I':
                    {
                        cty = "I";
                        if (pfx[2] == '9')
                        {
                            if (pfx[1] == 'G' || pfx[1] == 'H')
                                cty = "IG";
                            else
                                cty = "IT";
                        }
                        else if (pfx[2] == '0' && (pfx[1] == 'M' || pfx[1] == 'S'))
                            cty = "IS";
                    }
                    break;
                case 'J':
                    {
                        if (pfx[1] >= 'A' && pfx[1] <= 'S')
                        {
                            if (pfx != "JD1")
                                cty = "JA";
                        }
                        else
                            Itus(pfx, sfx);
                    }
                    break;
                case 'K': USA(pfx, sfx); break;
                case 'N': USA(pfx, sfx); break;
                case 'R': Russia(pfx, sfx); break;
                case 'U':
                    {
                        if (char.IsDigit(pfx[1]))
                        {
                            if (pfx[1] == '5')
                                cty = "UR";
                            else
                                Russia(pfx, sfx);
                        }
                        else if (pfx[1] >= 'A' && pfx[1] <= 'I')
                            Russia(pfx, sfx);
                        else if (pfx[1] >= 'J' && pfx[1] <= 'M')
                            cty = "UK";
                        else if (pfx[1] >= 'N' && pfx[1] <= 'Q')
                        {
                            cty = "UN";
                            string un31 = "DFGJQV";
                            if (un31.Contains(sfx[0]))
                                itu = "31";
                        }
                        else
                            Itus(pfx, sfx);
                    }
                    break;
                case 'W': USA(pfx, sfx); break;

                default: Itus(pfx, sfx); break;
            }
            return cty;
        }


        public void DXCCload()
        {
            dxcnt = 0;
            string line = new string(' ', 14);

            System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + "\\DXCC\\DXCC.txt");

            while ((line = file.ReadLine()) != null)
            {
                dxcl[dxcnt] = line;
                dxcnt++;
            }

            file.Close();
        }

        public void ITUload()
        {
            itucnt = 0;
            string line = "";

            System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + "\\DXCC\\ITU.txt");

            while ((line = file.ReadLine()) != null)
            {
                itul[itucnt] = line;
                itucnt++;
            }

            file.Close();
        }

        public void EXCload()
        {
            execnt = 0;
            string line = "";

            System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + "\\DXCC\\Except.txt");

            while ((line = file.ReadLine()) != null)
            {
                exebfr[execnt] = line;
                execnt++;
            }

            file.Close();
        }

        public void USAload()
        {
            usacnt = 0;
            string line = "";

            System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + "\\DXCC\\USAexe.txt");

            while ((line = file.ReadLine()) != null)
            {
                usabfr[usacnt] = line;
                usacnt++;
            }

            file.Close();
        }


        public void ARRLst()
        {
            sttcnt = 0;
            string line = "";

            System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + "\\DXCC\\ARRLST.txt");

            while ((line = file.ReadLine()) != null)
            {
                wvest[sttcnt] = line;
                sttcnt++;
            }

            file.Close();
        }

        public string Prefix(string call)
        {
            int m = call.Length - 1;
            while (Char.IsLetter(call, m)) { m--; }
            pfx = call.Substring(0, m + 1);
            return pfx;
        }

        public string Suffix(string call)
        {
            int m = call.Length - 1;
            while (Char.IsLetter(call, m)) { m--; }
            sfx = call.Substring(m + 1);
            return sfx;
        }

        public bool IsCall(string call)
        {
            bool f = false;
            if (call[0] != 'Q' && call.Length > 2)
            {
                int i = call.Length - 1;
                while (i > 0 && char.IsLetter(call, i))
                    i--;
                while (i > 0 && char.IsDigit(call, i))
                    i--;
                if (char.IsLetter(call, i))
                    f = true;
            }
            return f;
        }

        public string Portable(string call)
        {
            string[] parts;
            parts = call.Split('/');
            if (parts[0].Length > parts[1].Length)
            {
                if (IsCall(parts[0]))
                {
                    pfx = Prefix(parts[0]);
                    sfx = Suffix(parts[0]);
                    if (parts[1].Length == 1 && char.IsDigit(parts[1], 0))
                        pfx = pfx.Remove(pfx.Length - 1) + parts[1];
                    else if (parts[1].Length > 1 && char.IsDigit(parts[1], parts[1].Length - 1))
                        pfx = parts[1];
                }
            }
            else if (IsCall(parts[1]))
            {
                string zero = "A,D,E,J,P,S,T,V,Z"; 
                pfx = parts[0];
                sfx = Suffix(parts[1]);
                if (char.IsLetter(pfx, pfx.Length - 1) && !IsCall(pfx))
                    pfx += "0";
                else if (pfx.Length == 2 && char.IsDigit(pfx, 1) && zero.Contains(pfx[0]))
                    pfx += "0";

            }
            return (pfx + sfx);
        }

        public void Init()
        {
            try
            {
                DXCCload();
                ITUload();
                EXCload();
                USAload();
                ARRLst();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public string Except(string call)
        {
            int i = execnt;
            bool found = false;
            cty = "    ";
            zon = "  ";
            itu = "  ";
            string s = "";

            while (!found && i > 0)
            {
                i--;
                s = exebfr[i];
                if (s != null && s.Length > 2 && s.StartsWith(call))
                {
                    cty = s.Substring(12, s.Length - 12);
                    found = true;
                }
            }
            return cty;
        }

        public bool Analyze(string call1, out string result)
        {
            try
            {
                result = "";
                if (call1.Contains("/"))
                    call = Portable(call1);  // standard pfx + sfx syntax
                else call = call1;
                if (IsCall(call))
                {
                    pfx = Prefix(call);
                    sfx = Suffix(call);
                    cty = Except(call);

                    if (cty == "    ")
                        cty = DXstatus(pfx, sfx);

                    DXposition(cty);

                    if (cty.Length < 3)
                        car = cty + pfx[pfx.Length - 1];
                    else
                        car = cty;

                    string text = (call1 + "   " + cty + "  " + zon + "  " + itu);

                    result = text + "\n";
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                result = "";
                return false;
            }
        }

        private void rtbDXCC_TextChanged(object sender, EventArgs e)
        {
            SendMessage(rtbDXCC.Handle, WM_VSCROLL, SB_BOTTOM, 1);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                rtbDXCC.Clear();
                rtbDXCC.Refresh();
                SendMessage(rtbDXCC.Handle, WM_VSCROLL, SB_BOTTOM, 1);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }
}
