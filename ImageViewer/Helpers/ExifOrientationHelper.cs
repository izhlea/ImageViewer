namespace ImageViewer.Helpers;

internal static class ExifOrientationHelper
{
    private const int OrientationId = 0x0112;

    public static int Get(Image image)
    {
        try
        {
            if (image.PropertyIdList.Contains(OrientationId))
            {
                var item = image.GetPropertyItem(OrientationId);
                if (item?.Value is { Length: >= 2 } value)
                {
                    return BitConverter.ToUInt16(value, 0);
                }
            }
        }
        catch
        {
        }

        return 1;
    }

    public static void Reset(Image image)
    {
        try
        {
            if (image.PropertyIdList.Contains(OrientationId))
            {
                image.RemovePropertyItem(OrientationId);
            }
        }
        catch
        {
        }
    }
}
