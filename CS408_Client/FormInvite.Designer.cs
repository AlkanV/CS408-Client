namespace CS408_Client
{
    partial class FormInvite
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnDecline = new System.Windows.Forms.Button();
            this.lblInvitation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(118, 137);
            this.btnAccept.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(150, 44);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnDecline
            // 
            this.btnDecline.Location = new System.Drawing.Point(348, 137);
            this.btnDecline.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnDecline.Name = "btnDecline";
            this.btnDecline.Size = new System.Drawing.Size(150, 44);
            this.btnDecline.TabIndex = 1;
            this.btnDecline.Text = "Decline";
            this.btnDecline.UseVisualStyleBackColor = true;
            this.btnDecline.Click += new System.EventHandler(this.btnDecline_Click);
            // 
            // lblInvitation
            // 
            this.lblInvitation.AutoSize = true;
            this.lblInvitation.Location = new System.Drawing.Point(276, 48);
            this.lblInvitation.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblInvitation.Name = "lblInvitation";
            this.lblInvitation.Size = new System.Drawing.Size(70, 25);
            this.lblInvitation.TabIndex = 2;
            this.lblInvitation.Text = "label1";
            this.lblInvitation.Click += new System.EventHandler(this.lblInvitation_Click);
            // 
            // FormInvite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 225);
            this.Controls.Add(this.lblInvitation);
            this.Controls.Add(this.btnDecline);
            this.Controls.Add(this.btnAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FormInvite";
            this.Text = "Invitation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnDecline;
        private System.Windows.Forms.Label lblInvitation;
    }
}