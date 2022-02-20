
namespace FencMate
{
    partial class FencingMateField
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LeftPlayer = new System.Windows.Forms.Label();
            this.RightPlayer = new System.Windows.Forms.Label();
            this.DebugText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LeftPlayer
            // 
            this.LeftPlayer.AutoSize = true;
            this.LeftPlayer.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LeftPlayer.Location = new System.Drawing.Point(76, 28);
            this.LeftPlayer.Name = "LeftPlayer";
            this.LeftPlayer.Size = new System.Drawing.Size(146, 65);
            this.LeftPlayer.TabIndex = 0;
            this.LeftPlayer.Text = "Left 0";
            // 
            // RightPlayer
            // 
            this.RightPlayer.AutoSize = true;
            this.RightPlayer.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RightPlayer.Location = new System.Drawing.Point(520, 28);
            this.RightPlayer.Name = "RightPlayer";
            this.RightPlayer.Size = new System.Drawing.Size(179, 65);
            this.RightPlayer.TabIndex = 1;
            this.RightPlayer.Text = "Right 0";
            // 
            // label1
            // 
            this.DebugText.AutoSize = true;
            this.DebugText.Location = new System.Drawing.Point(304, 16);
            this.DebugText.Name = "Log";
            this.DebugText.Size = new System.Drawing.Size(38, 15);
            this.DebugText.TabIndex = 2;
            this.DebugText.Text = "Log";
            // 
            // FencingMateField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DebugText);
            this.Controls.Add(this.RightPlayer);
            this.Controls.Add(this.LeftPlayer);
            this.Name = "FencingMateField";
            this.Text = "FencingMateField";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LeftPlayer;
        private System.Windows.Forms.Label RightPlayer;
        private System.Windows.Forms.Label DebugText;
    }
}

