namespace OverLayApplicationSearch.Updater
{
    partial class UpdateControlWindow
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelMessage = new System.Windows.Forms.Label();
            this.buttonRestartButterflyFinder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 69);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(451, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(13, 36);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(98, 13);
            this.labelMessage.TabIndex = 1;
            this.labelMessage.Text = "Checking version...";
            // 
            // buttonRestartButterflyFinder
            // 
            this.buttonRestartButterflyFinder.Location = new System.Drawing.Point(314, 36);
            this.buttonRestartButterflyFinder.Name = "buttonRestartButterflyFinder";
            this.buttonRestartButterflyFinder.Size = new System.Drawing.Size(149, 23);
            this.buttonRestartButterflyFinder.TabIndex = 2;
            this.buttonRestartButterflyFinder.Text = "Restart ButterflyFinder";
            this.buttonRestartButterflyFinder.UseVisualStyleBackColor = true;
            this.buttonRestartButterflyFinder.Visible = false;
            this.buttonRestartButterflyFinder.Click += new System.EventHandler(this.buttonRestartButterflyFinder_Click);
            // 
            // UpdateControlWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 109);
            this.Controls.Add(this.buttonRestartButterflyFinder);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateControlWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ButterflyFinder Updater";
            this.Load += new System.EventHandler(this.onFormLoadEvent);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button buttonRestartButterflyFinder;
    }
}

