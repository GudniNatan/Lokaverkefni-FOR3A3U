namespace Lokaverkefni_Johann_Gudni_Client
{
    partial class ClientForm
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
            this.rtb_output = new System.Windows.Forms.RichTextBox();
            this.bt_guess = new System.Windows.Forms.Button();
            this.lb_score = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rtb_output
            // 
            this.rtb_output.Location = new System.Drawing.Point(12, 225);
            this.rtb_output.Name = "rtb_output";
            this.rtb_output.Size = new System.Drawing.Size(313, 96);
            this.rtb_output.TabIndex = 0;
            this.rtb_output.Text = "";
            // 
            // bt_guess
            // 
            this.bt_guess.Enabled = false;
            this.bt_guess.Location = new System.Drawing.Point(117, 188);
            this.bt_guess.Name = "bt_guess";
            this.bt_guess.Size = new System.Drawing.Size(101, 23);
            this.bt_guess.TabIndex = 1;
            this.bt_guess.Text = "Submit";
            this.bt_guess.UseVisualStyleBackColor = true;
            this.bt_guess.Click += new System.EventHandler(this.bt_guess_Click_1);
            // 
            // lb_score
            // 
            this.lb_score.AutoSize = true;
            this.lb_score.Location = new System.Drawing.Point(258, 13);
            this.lb_score.Name = "lb_score";
            this.lb_score.Size = new System.Drawing.Size(47, 13);
            this.lb_score.TabIndex = 2;
            this.lb_score.Text = "Score: 0";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 333);
            this.Controls.Add(this.lb_score);
            this.Controls.Add(this.bt_guess);
            this.Controls.Add(this.rtb_output);
            this.Name = "ClientForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb_output;
        private System.Windows.Forms.Button bt_guess;
        private System.Windows.Forms.Label lb_score;
    }
}

