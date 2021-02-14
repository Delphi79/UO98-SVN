using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;

public class ImageConversions
{
	public static BitmapSource BitmapSourceToBitmap(Bitmap bmp)
	{
		if (bmp == null)
			return null;

		BitmapSource returnSource;
		try
		{
			returnSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		}
		catch
		{
			returnSource = null;
		}
		return returnSource;
	}
}