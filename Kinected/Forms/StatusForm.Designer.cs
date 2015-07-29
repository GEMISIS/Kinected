namespace Kinected.Forms
{
    partial class StatusForm
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
            this.colorPB = new System.Windows.Forms.PictureBox();
            this.depthPB = new System.Windows.Forms.PictureBox();
            this.bodyCountLabel = new System.Windows.Forms.Label();
            this.bodyCountValLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthPB)).BeginInit();
            this.SuspendLayout();
            // 
            // colorPB
            // 
            this.colorPB.Location = new System.Drawing.Point(12, 12);
            this.colorPB.Name = "colorPB";
            this.colorPB.Size = new System.Drawing.Size(160, 120);
            this.colorPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.colorPB.TabIndex = 0;
            this.colorPB.TabStop = false;
            // 
            // depthPB
            // 
            this.depthPB.Location = new System.Drawing.Point(178, 12);
            this.depthPB.Name = "depthPB";
            this.depthPB.Size = new System.Drawing.Size(160, 120);
            this.depthPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.depthPB.TabIndex = 1;
            this.depthPB.TabStop = false;
            // 
            // bodyCountLabel
            // 
            this.bodyCountLabel.AutoSize = true;
            this.bodyCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bodyCountLabel.Location = new System.Drawing.Point(9, 135);
            this.bodyCountLabel.Name = "bodyCountLabel";
            this.bodyCountLabel.Size = new System.Drawing.Size(96, 20);
            this.bodyCountLabel.TabIndex = 2;
            this.bodyCountLabel.Text = "Body Count:";
            // 
            // bodyCountValLabel
            // 
            this.bodyCountValLabel.AutoSize = true;
            this.bodyCountValLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bodyCountValLabel.Location = new System.Drawing.Point(111, 135);
            this.bodyCountValLabel.Name = "bodyCountValLabel";
            this.bodyCountValLabel.Size = new System.Drawing.Size(18, 20);
            this.bodyCountValLabel.TabIndex = 3;
            this.bodyCountValLabel.Text = "0";
            // 
            // StatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 172);
            this.Controls.Add(this.bodyCountValLabel);
            this.Controls.Add(this.bodyCountLabel);
            this.Controls.Add(this.depthPB);
            this.Controls.Add(this.colorPB);
            this.Name = "StatusForm";
            this.Text = "Status";
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthPB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox colorPB;
        private System.Windows.Forms.PictureBox depthPB;
        private System.Windows.Forms.Label bodyCountLabel;
        private System.Windows.Forms.Label bodyCountValLabel;
    }
}