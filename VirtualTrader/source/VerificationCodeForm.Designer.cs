namespace VirtualTrader
{
    partial class VerificationCodeForm
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
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.VerficationCodeValueTextBox = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureBox
            // 
            this.PictureBox.Location = new System.Drawing.Point(0, 2);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(272, 149);
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
            // 
            // VerficationCodeValueTextBox
            // 
            this.VerficationCodeValueTextBox.Location = new System.Drawing.Point(0, 172);
            this.VerficationCodeValueTextBox.Name = "VerficationCodeValueTextBox";
            this.VerficationCodeValueTextBox.Size = new System.Drawing.Size(272, 21);
            this.VerficationCodeValueTextBox.TabIndex = 0;
            this.VerficationCodeValueTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VerficationCodeValueTextBox_KeyUp);
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(0, 196);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(272, 21);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // VerificationCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 241);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.VerficationCodeValueTextBox);
            this.Controls.Add(this.PictureBox);
            this.Name = "VerificationCodeForm";
            this.ShowInTaskbar = false;
            this.Text = "VerificationCodeForm";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox PictureBox;
        public System.Windows.Forms.TextBox VerficationCodeValueTextBox;
        private System.Windows.Forms.Button OKButton;
    }
}