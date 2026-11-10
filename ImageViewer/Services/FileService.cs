using System.Drawing.Imaging;

namespace ImageViewer.Services;

internal class FileService
{
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".ico",
    };

    private List<string> _images = new List<string>();
    private Stack<string> _deletedImages = new Stack<string>();
    private string _directoryPath = string.Empty;
    private const string Empty = "empty.png";

    private string _currentImage;

    public event EventHandler<string> CurrentImageChanged;

    public bool Any
    {
        get
        {
            return _images.Count != 0;
        }
    }

    public bool HasDirectory
    {
        get
        {
            return !string.IsNullOrEmpty(_directoryPath);
        }
    }

    public int Count
    {
        get
        {
            return _images.Count;
        }
    }

    public void SetDirectory(string path)
    {
        _directoryPath = path;
        _images = Directory.GetFiles(_directoryPath)
            .Where(IsImage)
            .ToList();
        NextImage();
    }

    private static bool IsImage(string path)
    {
        return ImageExtensions.Contains(Path.GetExtension(path));
    }

    public void NextImage(Direction direction = Direction.NotSet)
    {
        if (!Any)
        {
            SetCurrentImage(Empty);
            return;
        }

        switch (direction)
        {
            case Direction.NotSet:
                var random = new Random();
                var randomIndex = random.Next(_images.Count());
                SetCurrentImage(_images[randomIndex]);
                break;
            case Direction.Left:
                var indexLeft = _images.IndexOf(_currentImage);
                if (indexLeft != 0)
                {
                    SetCurrentImage(_images[--indexLeft]);
                }
                break;
            case Direction.Right:
                var indexRight = _images.IndexOf(_currentImage);
                if (indexRight != _images.Count() - 1)
                {
                    SetCurrentImage(_images[++indexRight]);
                }
                break;
            default:
                SetCurrentImage(Empty);
                break;
        }
    }

    public void FirstImage()
    {
        if (Any)
        {
            SetCurrentImage(_images[0]);
        }
    }

    public void LastImage()
    {
        if (Any)
        {
            SetCurrentImage(_images[_images.Count - 1]);
        }
    }

    public bool SaveImage(string subDirctoryPath = "saved")
    {
        if (Any)
        {
            var savedDirectory = $"{_directoryPath}\\{subDirctoryPath}";
            var newLocation = $"{_directoryPath}\\{subDirctoryPath}\\{Path.GetFileName(_currentImage)}";

            if (!Directory.Exists(savedDirectory))
            {
                Directory.CreateDirectory(savedDirectory);
            }

            File.Move(_currentImage, newLocation);
            _images.Remove(_currentImage);

            NextImage();

            return true;
        }
        return false;
    }

    public bool RotateCurrentImage(RotateFlipType rotateType)
    {
        if (!Any || string.IsNullOrEmpty(_currentImage) || !File.Exists(_currentImage))
        {
            return false;
        }

        var bytes = File.ReadAllBytes(_currentImage);
        var tempPath = $"{_currentImage}.rotating";

        using (var stream = new MemoryStream(bytes))
        using (var image = Image.FromStream(stream))
        {
            image.RotateFlip(rotateType);

            if (GetImageFormat(_currentImage).Equals(ImageFormat.Jpeg))
            {
                var jpegEncoder = ImageCodecInfo.GetImageEncoders()
                    .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                using var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                image.Save(tempPath, jpegEncoder, encoderParams);
            }
            else
            {
                image.Save(tempPath, GetImageFormat(_currentImage));
            }
        }

        File.Move(tempPath, _currentImage, true);
        SetCurrentImage(_currentImage);

        return true;
    }

    public bool DeleteImage()
    {
        if (Any)
        {
            var deletedDirectory = $"{_directoryPath}\\deleted";
            var newLocation = $"{_directoryPath}\\deleted\\{Path.GetFileName(_currentImage)}";

            if (!Directory.Exists(deletedDirectory))
            {
                Directory.CreateDirectory(deletedDirectory);
            }

            File.Move(_currentImage, newLocation, true);
            _images.Remove(_currentImage);
            _deletedImages.Push(newLocation);

            NextImage();

            return true;
        }
        return false;
    }

    public bool RestoreImage()
    {
        if (Any)
        {
            if (_deletedImages.Count == 0)
            {
                return false;
            }

            var deleted = _deletedImages.Pop();

            var newPath = $"{_directoryPath}\\{Path.GetFileName(deleted)}";

            File.Move(deleted, newPath);

            _images.Add(newPath);
            SetCurrentImage(newPath);

            return true;
        }
        return false;
    }

    private static ImageFormat GetImageFormat(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".png" => ImageFormat.Png,
            ".bmp" => ImageFormat.Bmp,
            ".gif" => ImageFormat.Gif,
            ".tif" or ".tiff" => ImageFormat.Tiff,
            ".ico" => ImageFormat.Icon,
            _ => ImageFormat.Png,
        };
    }

    private void SetCurrentImage(string path)
    {
        _currentImage = path;
        CurrentImageChanged.Invoke(new(), path);
    }
}