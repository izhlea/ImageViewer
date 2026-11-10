using ImageViewer.Services;

namespace ImageViewer;

public partial class Form1 : Form
{
    private bool _coverMode;
    private bool _random;

    private const string Undecided = "undecided";

    private FileService _fileService;

    public Form1()
    {
        InitializeComponent();
        _fileService = new FileService();
        _fileService.CurrentImageChanged += (sender, e) =>
        {
            pictureBox1.Image = LoadImageUnsafe(e);
        };
    }

    private void openFolderBtn_Click(object sender, EventArgs e)
    {
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            _fileService.SetDirectory(folderBrowserDialog.SelectedPath);

            FullScreen();
        }
    }

    private void KeyDownEvent(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            this.Close();
        }

        if (e.KeyCode == Keys.Enter)
        {
            this.openFolderBtn_Click(sender, e);
        }

        if (e.KeyCode == Keys.F)
        {
            FullScreen();
        }

        if (e.KeyCode == Keys.W)
        {
            CoverMode();
        }

        if (e.KeyCode == Keys.OemOpenBrackets)
        {
            _fileService.RotateCurrentImage(RotateFlipType.Rotate270FlipNone);
        }

        if (e.KeyCode == Keys.OemCloseBrackets)
        {
            _fileService.RotateCurrentImage(RotateFlipType.Rotate90FlipNone);
        }

        if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.D)
        {
            _fileService.DeleteImage();
        }

        if (e.KeyCode == Keys.Back)
        {
            _fileService.RestoreImage();
        }

        if (e.KeyCode == Keys.PageUp || e.KeyCode == Keys.S)
        {
            _fileService.SaveImage();
        }

        if (e.KeyCode == Keys.A)
        {
            _fileService.SaveImage(Undecided);
        }

        if (e.KeyCode == Keys.Up)
        {
            _fileService.FirstImage();
        }

        if (e.KeyCode == Keys.Down)
        {
            _fileService.LastImage();
        }

        if (e.KeyCode == Keys.Right)
        {
            _fileService.NextImage(Direction.Right);
        }

        if (e.KeyCode == Keys.Left)
        {
            _fileService.NextImage(Direction.Left);
        }
    }

    private void FullScreen()
    {
        if (FormBorderStyle == FormBorderStyle.None)
        {
            _coverMode = false;
            pictureBox1.Invalidate();
            TopMost = false;
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Maximized;

            BackColor = SystemColors.Control;
            OpenFolderBtn.Visible = true;
        }
        else
        {
            _coverMode = false;
            pictureBox1.Invalidate();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            Bounds = Screen.FromControl(this).Bounds;
            TopMost = true;

            BackColor = Color.Black;
            OpenFolderBtn.Visible = false;
        }
    }

    private void CoverMode()
    {
        _coverMode = !_coverMode;

        pictureBox1.Invalidate();

        if (FormBorderStyle != FormBorderStyle.None)
        {
            FullScreen();
        }
    }

    private void PreviewKeyDownEvent(object sender, PreviewKeyDownEventArgs e)
    {
        e.IsInputKey = true;
    }

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
        if (_coverMode && pictureBox1.Image != null)
        {
            DrawCover(e.Graphics);
        }

        if (_fileService.Any)
        {
            using Font myFont = new Font("Arial", 14);
            e.Graphics.DrawString($"Files count - {_fileService.Count}", myFont, Brushes.Yellow, new Point(2, 2));
        }
        else if (_fileService.HasDirectory)
        {
            using Font myFont = new Font("Arial", 14);
            e.Graphics.DrawString("Изображения не найдены", myFont, Brushes.Yellow, new Point(2, 2));
        }
    }

    private void DrawCover(Graphics g)
    {
        var image = pictureBox1.Image;
        var box = pictureBox1.ClientRectangle;

        var scale = Math.Max((float)box.Width / image.Width, (float)box.Height / image.Height);
        var drawW = (int)(image.Width * scale);
        var drawH = (int)(image.Height * scale);
        var x = box.Left + (box.Width - drawW) / 2;
        var y = box.Top + (box.Height - drawH) / 2;

        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        g.DrawImage(image, new Rectangle(x, y, drawW, drawH), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        //var path = $"{_directoryPath}\\deleted";

        //if (Directory.Exists(path))
        //{
        //    pictureBox1.Load("empty.png");
        //    var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

        //    foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
        //    {
        //        info.Attributes = FileAttributes.Normal;
        //    }

        //    directory.Delete(true);
        //}
    }

    private Image LoadImageUnsafe(string path)
    {
        // Читаем все байты файла в память
        byte[] bytes = File.ReadAllBytes(path);

        // Создаем поток в памяти и загружаем изображение из него
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            return Image.FromStream(ms);
        }
    }
}