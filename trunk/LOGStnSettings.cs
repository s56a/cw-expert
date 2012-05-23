using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CWExpert
{
    public partial class LOGStnSettings : Form
    {
        LOG_export ParrentForm;

        public LOGStnSettings(LOG_export form)
        {
            this.AutoScaleMode = AutoScaleMode.Inherit;
            InitializeComponent();
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = 8.25f / ratio;
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            ParrentForm = form;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                ParrentForm.ContestName = txtContestName.Text.ToString();
                ParrentForm.MyClub = txtRClub.Text.ToString();
                ParrentForm.Operators1 = txtOps1.Text.ToString();
                ParrentForm.Operators2 = txtOps2.Text.ToString();
                ParrentForm.MyAddr1 = txtAddr1.Text.ToString();
                ParrentForm.MyAddr2 = txtAddr2.Text.ToString();
                ParrentForm.MyPhone = txtPhone.Text.ToString();
                ParrentForm.Remarks = txtRemarks.Text.ToString();
                ParrentForm.MyCity = txtCity.Text.ToString();
                ParrentForm.MyCountry = txtCountry.Text.ToString();
                ParrentForm.Category = txtCategory.Text.ToString();
                ParrentForm.TXequ = txtTXequ.Text.ToString();
                ParrentForm.RXequ = txtRXequ.Text.ToString();
                ParrentForm.Antenna = txtAntenna.Text.ToString();
                ParrentForm.TXPower = txtPower.Text.ToString();
                this.Hide();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void LOGExportSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
