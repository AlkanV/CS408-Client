namespace CS408_Client
{
    partial class FormGame
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
            this.button1 = new System.Windows.Forms.Button();
            this.lblGuessedNumber = new System.Windows.Forms.Label();
            this.btnGuess = new System.Windows.Forms.Button();
            this.txtGuessedNumber = new System.Windows.Forms.TextBox();
            this.lblScore = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(272, 138);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 60);
            this.button1.TabIndex = 0;
            this.button1.Text = "Surrender!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblGuessedNumber
            // 
            this.lblGuessedNumber.AutoSize = true;
            this.lblGuessedNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGuessedNumber.Location = new System.Drawing.Point(13, 33);
            this.lblGuessedNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGuessedNumber.Name = "lblGuessedNumber";
            this.lblGuessedNumber.Size = new System.Drawing.Size(209, 29);
            this.lblGuessedNumber.TabIndex = 1;
            this.lblGuessedNumber.Text = "Guessed Number:";
            // 
            // btnGuess
            // 
            this.btnGuess.Location = new System.Drawing.Point(12, 138);
            this.btnGuess.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGuess.Name = "btnGuess";
            this.btnGuess.Size = new System.Drawing.Size(140, 60);
            this.btnGuess.TabIndex = 2;
            this.btnGuess.Text = "Guess";
            this.btnGuess.UseVisualStyleBackColor = true;
            this.btnGuess.Click += new System.EventHandler(this.btnGuess_Click);
            // 
            // txtGuessedNumber
            // 
            this.txtGuessedNumber.Location = new System.Drawing.Point(272, 31);
            this.txtGuessedNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtGuessedNumber.Name = "txtGuessedNumber";
            this.txtGuessedNumber.Size = new System.Drawing.Size(144, 31);
            this.txtGuessedNumber.TabIndex = 3;
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.Location = new System.Drawing.Point(13, 78);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(102, 29);
            this.lblScore.TabIndex = 4;
            this.lblScore.Text = "Score: 0";
            // 
            // FormGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 217);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.txtGuessedNumber);
            this.Controls.Add(this.btnGuess);
            this.Controls.Add(this.lblGuessedNumber);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormGame";
            this.Text = "The Game";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblGuessedNumber;
        private System.Windows.Forms.Button btnGuess;
        private System.Windows.Forms.TextBox txtGuessedNumber;
        private System.Windows.Forms.Label lblScore;
    }
}