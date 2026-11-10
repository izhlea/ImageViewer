namespace ImageViewer
{
    partial class Form1
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
            pictureBox1 = new PictureBox();
            OpenFolderBtn = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.None;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1082, 618);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // OpenFolderBtn
            // 
            OpenFolderBtn.Dock = DockStyle.Top;
            OpenFolderBtn.Location = new Point(0, 0);
            OpenFolderBtn.Name = "OpenFolderBtn";
            OpenFolderBtn.Size = new Size(1082, 23);
            OpenFolderBtn.TabIndex = 0;
            OpenFolderBtn.Text = "Open folder";
            OpenFolderBtn.UseVisualStyleBackColor = true;
            OpenFolderBtn.Click += openFolderBtn_Click;
            OpenFolderBtn.KeyDown += KeyDownEvent;
            OpenFolderBtn.PreviewKeyDown += PreviewKeyDownEvent;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1082, 618);
            Controls.Add(OpenFolderBtn);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Picture Viewer";
            WindowState = FormWindowState.Maximized;
            FormClosing += Form1_FormClosing;
            KeyDown += KeyDownEvent;
            PreviewKeyDown += PreviewKeyDownEvent;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button OpenFolderBtn;
        private PictureBox pictureBox1;
        private FolderBrowserDialog folderBrowserDialog;
    }
}