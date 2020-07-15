﻿namespace GhepHinh
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnStart = new System.Windows.Forms.Button();
            this.numCol = new System.Windows.Forms.NumericUpDown();
            this.numRow = new System.Windows.Forms.NumericUpDown();
            this.cbHelp = new System.Windows.Forms.CheckBox();
            this.grpOption = new System.Windows.Forms.GroupBox();
            this.lblRow = new System.Windows.Forms.Label();
            this.lblCol = new System.Windows.Forms.Label();
            this.grpMain = new System.Windows.Forms.GroupBox();
            this.mainPic = new System.Windows.Forms.PictureBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numCol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRow)).BeginInit();
            this.grpOption.SuspendLayout();
            this.grpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPic)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(267, 13);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(112, 28);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Trò chơi mới";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // numCol
            // 
            this.numCol.Location = new System.Drawing.Point(69, 19);
            this.numCol.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCol.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numCol.Name = "numCol";
            this.numCol.Size = new System.Drawing.Size(38, 20);
            this.numCol.TabIndex = 2;
            this.numCol.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // numRow
            // 
            this.numRow.Location = new System.Drawing.Point(190, 19);
            this.numRow.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numRow.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numRow.Name = "numRow";
            this.numRow.Size = new System.Drawing.Size(40, 20);
            this.numRow.TabIndex = 3;
            this.numRow.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // cbHelp
            // 
            this.cbHelp.AutoSize = true;
            this.cbHelp.Location = new System.Drawing.Point(411, 19);
            this.cbHelp.Name = "cbHelp";
            this.cbHelp.Size = new System.Drawing.Size(64, 17);
            this.cbHelp.TabIndex = 5;
            this.cbHelp.Text = "Giúp đỡ";
            this.cbHelp.UseVisualStyleBackColor = true;
            this.cbHelp.CheckedChanged += new System.EventHandler(this.cbHelp_CheckedChanged);
            // 
            // grpOption
            // 
            this.grpOption.Controls.Add(this.lblRow);
            this.grpOption.Controls.Add(this.lblCol);
            this.grpOption.Controls.Add(this.cbHelp);
            this.grpOption.Controls.Add(this.numRow);
            this.grpOption.Controls.Add(this.btnStart);
            this.grpOption.Controls.Add(this.numCol);
            this.grpOption.Location = new System.Drawing.Point(12, 12);
            this.grpOption.Name = "grpOption";
            this.grpOption.Size = new System.Drawing.Size(500, 50);
            this.grpOption.TabIndex = 6;
            this.grpOption.TabStop = false;
            this.grpOption.Text = "Lựa chọn cấu hình";
            // 
            // lblRow
            // 
            this.lblRow.AutoSize = true;
            this.lblRow.Location = new System.Drawing.Point(137, 23);
            this.lblRow.Name = "lblRow";
            this.lblRow.Size = new System.Drawing.Size(47, 13);
            this.lblRow.TabIndex = 7;
            this.lblRow.Text = "Số hàng";
            // 
            // lblCol
            // 
            this.lblCol.AutoSize = true;
            this.lblCol.Location = new System.Drawing.Point(25, 23);
            this.lblCol.Name = "lblCol";
            this.lblCol.Size = new System.Drawing.Size(38, 13);
            this.lblCol.TabIndex = 6;
            this.lblCol.Text = "Số cột";
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.mainPic);
            this.grpMain.Location = new System.Drawing.Point(12, 68);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(500, 412);
            this.grpMain.TabIndex = 7;
            this.grpMain.TabStop = false;
            this.grpMain.Text = "Main";
            // 
            // mainPic
            // 
            this.mainPic.Enabled = false;
            this.mainPic.Location = new System.Drawing.Point(5, 14);
            this.mainPic.Name = "mainPic";
            this.mainPic.Size = new System.Drawing.Size(490, 394);
            this.mainPic.TabIndex = 0;
            this.mainPic.TabStop = false;
            this.mainPic.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPic_Paint);
            this.mainPic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPic_MouseDown);
            this.mainPic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mainPic_MouseMove);
            this.mainPic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mainPic_MouseUp);
            // 
            // lblIP
            // 
            this.lblIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIP.Location = new System.Drawing.Point(29, 483);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(467, 13);
            this.lblIP.TabIndex = 8;
            this.lblIP.Text = "IP:";
            this.lblIP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer
            // 
            this.timer.Interval = 25;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 502);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.grpMain);
            this.Controls.Add(this.grpOption);
            this.DoubleBuffered = true;
            this.Location = new System.Drawing.Point(120, 100);
            this.MaximumSize = new System.Drawing.Size(540, 540);
            this.MinimumSize = new System.Drawing.Size(540, 540);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Ghép hình";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.LocationChanged += new System.EventHandler(this.Main_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.numCol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRow)).EndInit();
            this.grpOption.ResumeLayout(false);
            this.grpOption.PerformLayout();
            this.grpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnStart;
        public System.Windows.Forms.CheckBox cbHelp;
        private System.Windows.Forms.GroupBox grpOption;
        private System.Windows.Forms.Label lblRow;
        private System.Windows.Forms.Label lblCol;
        public System.Windows.Forms.GroupBox grpMain;
        public System.Windows.Forms.PictureBox mainPic;
        private System.Windows.Forms.Label lblIP;
        public System.Windows.Forms.NumericUpDown numCol;
        public System.Windows.Forms.NumericUpDown numRow;
        private System.Windows.Forms.Timer timer;
    }
}

