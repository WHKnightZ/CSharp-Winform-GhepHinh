namespace GhepHinh
{
    partial class Remote
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
            this.grpPieces = new System.Windows.Forms.GroupBox();
            this.remotePic = new System.Windows.Forms.PictureBox();
            this.grpSelectPiece = new System.Windows.Forms.GroupBox();
            this.btnPlus = new System.Windows.Forms.PictureBox();
            this.btnMinus = new System.Windows.Forms.PictureBox();
            this.lblSelected = new System.Windows.Forms.Label();
            this.grpControl = new System.Windows.Forms.GroupBox();
            this.btnRotate = new System.Windows.Forms.PictureBox();
            this.btnLeft = new System.Windows.Forms.PictureBox();
            this.btnRight = new System.Windows.Forms.PictureBox();
            this.btnDown = new System.Windows.Forms.PictureBox();
            this.btnUp = new System.Windows.Forms.PictureBox();
            this.grpPieces.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.remotePic)).BeginInit();
            this.grpSelectPiece.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnPlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinus)).BeginInit();
            this.grpControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRotate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUp)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPieces
            // 
            this.grpPieces.Controls.Add(this.remotePic);
            this.grpPieces.Location = new System.Drawing.Point(12, 12);
            this.grpPieces.Name = "grpPieces";
            this.grpPieces.Size = new System.Drawing.Size(344, 347);
            this.grpPieces.TabIndex = 1;
            this.grpPieces.TabStop = false;
            this.grpPieces.Text = "Pieces";
            // 
            // remotePic
            // 
            this.remotePic.Enabled = false;
            this.remotePic.Location = new System.Drawing.Point(2, 20);
            this.remotePic.Name = "remotePic";
            this.remotePic.Size = new System.Drawing.Size(340, 320);
            this.remotePic.TabIndex = 0;
            this.remotePic.TabStop = false;
            this.remotePic.Paint += new System.Windows.Forms.PaintEventHandler(this.remotePic_Paint);
            this.remotePic.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.remotePic_MouseDoubleClick);
            this.remotePic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.remotePic_MouseDown);
            this.remotePic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.remotePic_MouseMove);
            this.remotePic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.remotePic_MouseUp);
            // 
            // grpSelectPiece
            // 
            this.grpSelectPiece.Controls.Add(this.btnPlus);
            this.grpSelectPiece.Controls.Add(this.btnMinus);
            this.grpSelectPiece.Controls.Add(this.lblSelected);
            this.grpSelectPiece.Location = new System.Drawing.Point(370, 12);
            this.grpSelectPiece.Name = "grpSelectPiece";
            this.grpSelectPiece.Size = new System.Drawing.Size(206, 107);
            this.grpSelectPiece.TabIndex = 2;
            this.grpSelectPiece.TabStop = false;
            this.grpSelectPiece.Text = "Select Piece";
            // 
            // btnPlus
            // 
            this.btnPlus.Image = global::GhepHinh.Properties.Resources.plus;
            this.btnPlus.Location = new System.Drawing.Point(142, 36);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Size = new System.Drawing.Size(40, 40);
            this.btnPlus.TabIndex = 1;
            this.btnPlus.TabStop = false;
            this.btnPlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnPlus_MouseDown);
            // 
            // btnMinus
            // 
            this.btnMinus.Image = global::GhepHinh.Properties.Resources.minus;
            this.btnMinus.Location = new System.Drawing.Point(24, 36);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(40, 40);
            this.btnMinus.TabIndex = 0;
            this.btnMinus.TabStop = false;
            this.btnMinus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnMinus_MouseDown);
            // 
            // lblSelected
            // 
            this.lblSelected.Font = new System.Drawing.Font("Arial", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelected.Location = new System.Drawing.Point(45, 28);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(125, 51);
            this.lblSelected.TabIndex = 2;
            this.lblSelected.Text = "0";
            this.lblSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpControl
            // 
            this.grpControl.Controls.Add(this.btnRotate);
            this.grpControl.Controls.Add(this.btnLeft);
            this.grpControl.Controls.Add(this.btnRight);
            this.grpControl.Controls.Add(this.btnDown);
            this.grpControl.Controls.Add(this.btnUp);
            this.grpControl.Location = new System.Drawing.Point(370, 125);
            this.grpControl.Name = "grpControl";
            this.grpControl.Size = new System.Drawing.Size(202, 234);
            this.grpControl.TabIndex = 3;
            this.grpControl.TabStop = false;
            this.grpControl.Text = "Control";
            // 
            // btnRotate
            // 
            this.btnRotate.Image = global::GhepHinh.Properties.Resources.rotate;
            this.btnRotate.Location = new System.Drawing.Point(81, 97);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(40, 40);
            this.btnRotate.TabIndex = 5;
            this.btnRotate.TabStop = false;
            this.btnRotate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRotate_MouseDown);
            // 
            // btnLeft
            // 
            this.btnLeft.Image = global::GhepHinh.Properties.Resources.left;
            this.btnLeft.Location = new System.Drawing.Point(16, 97);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(40, 40);
            this.btnLeft.TabIndex = 4;
            this.btnLeft.TabStop = false;
            this.btnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnLeft_MouseDown);
            // 
            // btnRight
            // 
            this.btnRight.Image = global::GhepHinh.Properties.Resources.right;
            this.btnRight.Location = new System.Drawing.Point(146, 97);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(40, 40);
            this.btnRight.TabIndex = 3;
            this.btnRight.TabStop = false;
            this.btnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRight_MouseDown);
            // 
            // btnDown
            // 
            this.btnDown.Image = global::GhepHinh.Properties.Resources.down;
            this.btnDown.Location = new System.Drawing.Point(81, 162);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(40, 40);
            this.btnDown.TabIndex = 2;
            this.btnDown.TabStop = false;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseDown);
            // 
            // btnUp
            // 
            this.btnUp.Image = global::GhepHinh.Properties.Resources.up;
            this.btnUp.Location = new System.Drawing.Point(81, 32);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(40, 40);
            this.btnUp.TabIndex = 1;
            this.btnUp.TabStop = false;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseDown);
            // 
            // Remote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 368);
            this.ControlBox = false;
            this.Controls.Add(this.grpControl);
            this.Controls.Add(this.grpSelectPiece);
            this.Controls.Add(this.grpPieces);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(600, 406);
            this.MinimumSize = new System.Drawing.Size(600, 406);
            this.Name = "Remote";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Điều khiển";
            this.LocationChanged += new System.EventHandler(this.Remote_LocationChanged);
            this.grpPieces.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.remotePic)).EndInit();
            this.grpSelectPiece.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnPlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinus)).EndInit();
            this.grpControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnRotate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.GroupBox grpPieces;
        private System.Windows.Forms.GroupBox grpSelectPiece;
        private System.Windows.Forms.PictureBox btnMinus;
        private System.Windows.Forms.GroupBox grpControl;
        private System.Windows.Forms.PictureBox btnPlus;
        private System.Windows.Forms.PictureBox btnRotate;
        private System.Windows.Forms.PictureBox btnLeft;
        private System.Windows.Forms.PictureBox btnRight;
        private System.Windows.Forms.PictureBox btnDown;
        private System.Windows.Forms.PictureBox btnUp;
        public System.Windows.Forms.Label lblSelected;
        public System.Windows.Forms.PictureBox remotePic;
    }
}