using ImageViewer.Helpers;
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

    private bool _isRandom = false;

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

    public string CurrentImage
    {
        get
        {
            return _currentImage;
        }
    }

    public void SetDirectory(string path)
    {
        _directoryPath = path;
        _images = Directory.GetFiles(_directoryPath)
            .Where(IsImage)
            .ToList();

        if (_isRandom)
        {
            NextImage();
        }
        else
        {
            NextImage(0);
        }
    }

    public void SetRandom()
    {
        _isRandom = !_isRandom;
    }

    private void NextImage(int index)
    {
        if (!Any)
        {
            SetCurrentImage(Empty);
            return;
        }

        if (index > _images.Count - 1)
        {
            index--;
        }

        SetCurrentImage(_images[index]);
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

            var index = _images.IndexOf(_currentImage);

            _images.Remove(_currentImage);

            if (_isRandom)
            {
                NextImage();
            }
            else
            {
                NextImage(index);
            }

            return true;
        }
        return false;
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

            var index = _images.IndexOf(_currentImage);

            _images.Remove(_currentImage);
            _deletedImages.Push(newLocation);

            if (_isRandom)
            {
                NextImage();
            }
            else
            {
                NextImage(index);
            }

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

    public bool RotateCurrentImage(RotateFlipType rotateType)
    {
        if (!Any || string.IsNullOrEmpty(_currentImage) || !File.Exists(_currentImage))
        {
            return false;
        }

        var bytes = File.ReadAllBytes(_currentImage);
        var tempPath = $"{_currentImage}.rotating";

        using (var stream = new MemoryStream(bytes))
        using (var image = System.Drawing.Image.FromStream(stream))
        {
            var orientation = ExifOrientationHelper.Get(image);

            if (orientation == 1)
            {
                image.RotateFlip(rotateType);
            }
            else
            {
                ExifOrientationHelper.Reset(image);
                image.RotateFlip(rotateType);
            }

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

    public void ClearDeletedFolder()
    {
        if (!HasDirectory)
        {
            return;
        }

        var path = $"{_directoryPath}\\deleted";

        if (Directory.Exists(path))
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
    }

    private static bool IsImage(string path)
    {
        return ImageExtensions.Contains(Path.GetExtension(path));
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