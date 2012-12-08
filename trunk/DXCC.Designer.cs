namespace CWExpert
{
    partial class DXCC
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
            this.rtbDXCC = new System.Windows.Forms.RichTextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbDXCC
            // 
            this.rtbDXCC.BackColor = System.Drawing.Color.Black;
            this.rtbDXCC.ForeColor = System.Drawing.Color.LawnGreen;
            this.rtbDXCC.Location = new System.Drawing.Point(32, 21);
            this.rtbDXCC.Name = "rtbDXCC";
            this.rtbDXCC.Size = new System.Drawing.Size(379, 173);
            this.rtbDXCC.TabIndex = 0;
            this.rtbDXCC.Text = "";
            this.rtbDXCC.TextChanged += new System.EventHandler(this.rtbDXCC_TextChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(187, 211);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // DXCC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(448, 250);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.rtbDXCC);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(458, 282);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(458, 282);
            this.Name = "DXCC";
            this.Text = "DXCC";
            this.Load += new System.EventHandler(this.DXCC_Load);
            this.ResumeLayout(false);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DXCC_Closing);

        }

        #endregion

        public System.Windows.Forms.RichTextBox rtbDXCC;
        private System.Windows.Forms.Button btnClear;

    }
}