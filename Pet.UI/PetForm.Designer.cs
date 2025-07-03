namespace Pet.UI
{
    partial class PetForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picPet = new System.Windows.Forms.PictureBox();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picPet)).BeginInit();
            this.SuspendLayout();
            // 
            // picPet
            // 
            this.picPet.BackColor = System.Drawing.Color.Transparent;
            this.picPet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPet.Location = new System.Drawing.Point(0, 0);
            this.picPet.Name = "picPet";
            this.picPet.Size = new System.Drawing.Size(284, 261);
            this.picPet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picPet.TabIndex = 0;
            this.picPet.TabStop = false;
            this.picPet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPet_MouseDown);
            this.picPet.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picPet_MouseMove);
            this.picPet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picPet_MouseUp);
            // 
            // animationTimer
            // 
            this.animationTimer.Enabled = true;
            this.animationTimer.Interval = 33;
            this.animationTimer.Tick += new System.EventHandler(this.animationTimer_Tick);
            // 
            // PetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LimeGreen;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.picPet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PetForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.LimeGreen;
            ((System.ComponentModel.ISupportInitialize)(this.picPet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPet;
        private System.Windows.Forms.Timer animationTimer;
    }
}
