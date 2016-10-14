namespace Lokaverkefni_Johann_Gudni_Client
{
    partial class chooseIP
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
            this.tb_ip = new System.Windows.Forms.TextBox();
            this.bt_ip = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb_ip
            // 
            this.tb_ip.Location = new System.Drawing.Point(210, 145);
            this.tb_ip.Name = "tb_ip";
            this.tb_ip.Size = new System.Drawing.Size(100, 20);
            this.tb_ip.TabIndex = 0;
            // 
            // bt_ip
            // 
            this.bt_ip.Location = new System.Drawing.Point(221, 200);
            this.bt_ip.Name = "bt_ip";
            this.bt_ip.Size = new System.Drawing.Size(75, 25);
            this.bt_ip.TabIndex = 1;
            this.bt_ip.Text = "Submit";
            this.bt_ip.UseVisualStyleBackColor = true;
            this.bt_ip.Click += new System.EventHandler(this.bt_ip_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(231, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose IP";
            // 
            // chooseIP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 401);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_ip);
            this.Controls.Add(this.tb_ip);
            this.Name = "chooseIP";
            this.Text = "chooseIP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_ip;
        private System.Windows.Forms.Button bt_ip;
        private System.Windows.Forms.Label label1;
    }
}