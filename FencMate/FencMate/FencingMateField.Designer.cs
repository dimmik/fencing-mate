
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
            this.GameStateInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LeftEvents = new System.Windows.Forms.Label();
            this.RightEvents = new System.Windows.Forms.Label();
            this.SoundsLabel = new System.Windows.Forms.Label();
            this.KbdInfo = new System.Windows.Forms.Label();
            this.GameConfigurationLabel = new System.Windows.Forms.Label();
            this.TimerLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LeftPlayer
            // 
            this.LeftPlayer.Anchor = System.Windows.Forms.AnchorStyles.None;
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
            this.RightPlayer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RightPlayer.AutoSize = true;
            this.RightPlayer.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RightPlayer.Location = new System.Drawing.Point(520, 28);
            this.RightPlayer.Name = "RightPlayer";
            this.RightPlayer.Size = new System.Drawing.Size(179, 65);
            this.RightPlayer.TabIndex = 1;
            this.RightPlayer.Text = "Right 0";
            // 
            // GameStateInfo
            // 
            this.GameStateInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameStateInfo.BackColor = System.Drawing.SystemColors.Window;
            this.GameStateInfo.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GameStateInfo.Location = new System.Drawing.Point(320, 28);
            this.GameStateInfo.Name = "GameStateInfo";
            this.GameStateInfo.Size = new System.Drawing.Size(147, 100);
            this.GameStateInfo.TabIndex = 2;
            this.GameStateInfo.Text = "Stopped";
            this.GameStateInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(264, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(273, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Press Middle mouse button to start/stop the game";
            // 
            // LeftEvents
            // 
            this.LeftEvents.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LeftEvents.AutoSize = true;
            this.LeftEvents.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LeftEvents.Location = new System.Drawing.Point(76, 102);
            this.LeftEvents.Name = "LeftEvents";
            this.LeftEvents.Size = new System.Drawing.Size(66, 25);
            this.LeftEvents.TabIndex = 4;
            this.LeftEvents.Text = "Events";
            // 
            // RightEvents
            // 
            this.RightEvents.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RightEvents.AutoSize = true;
            this.RightEvents.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RightEvents.Location = new System.Drawing.Point(520, 102);
            this.RightEvents.Name = "RightEvents";
            this.RightEvents.Size = new System.Drawing.Size(66, 25);
            this.RightEvents.TabIndex = 5;
            this.RightEvents.Text = "Events";
            // 
            // SoundsLabel
            // 
            this.SoundsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SoundsLabel.AutoSize = true;
            this.SoundsLabel.Location = new System.Drawing.Point(562, 9);
            this.SoundsLabel.Name = "SoundsLabel";
            this.SoundsLabel.Size = new System.Drawing.Size(158, 15);
            this.SoundsLabel.TabIndex = 6;
            this.SoundsLabel.Text = "[space to toggle] Sounds: on";
            // 
            // KbdInfo
            // 
            this.KbdInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.KbdInfo.AutoSize = true;
            this.KbdInfo.Location = new System.Drawing.Point(29, 9);
            this.KbdInfo.Name = "KbdInfo";
            this.KbdInfo.Size = new System.Drawing.Size(117, 15);
            this.KbdInfo.TabIndex = 7;
            this.KbdInfo.Text = "P to pause; R to reset";
            // 
            // GameConfigurationLabel
            // 
            this.GameConfigurationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameConfigurationLabel.AutoSize = true;
            this.GameConfigurationLabel.Location = new System.Drawing.Point(291, 138);
            this.GameConfigurationLabel.Name = "GameConfigurationLabel";
            this.GameConfigurationLabel.Size = new System.Drawing.Size(115, 15);
            this.GameConfigurationLabel.TabIndex = 8;
            this.GameConfigurationLabel.Text = "Game Configuration";
            // 
            // TimerLabel
            // 
            this.TimerLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TimerLabel.AutoSize = true;
            this.TimerLabel.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TimerLabel.Location = new System.Drawing.Point(291, 265);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(191, 86);
            this.TimerLabel.TabIndex = 9;
            this.TimerLabel.Text = "00:00";
            // 
            // FencingMateField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TimerLabel);
            this.Controls.Add(this.GameConfigurationLabel);
            this.Controls.Add(this.KbdInfo);
            this.Controls.Add(this.SoundsLabel);
            this.Controls.Add(this.RightEvents);
            this.Controls.Add(this.LeftEvents);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GameStateInfo);
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
        private System.Windows.Forms.Label GameStateInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LeftEvents;
        private System.Windows.Forms.Label RightEvents;
        private System.Windows.Forms.Label SoundsLabel;
        private System.Windows.Forms.Label KbdInfo;
        private System.Windows.Forms.Label GameConfigurationLabel;
        private System.Windows.Forms.Label TimerLabel;
    }
}

