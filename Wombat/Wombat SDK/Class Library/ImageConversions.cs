using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace JoinUO.WombatSDK
{
	public class ImageConversions
	{
		#region "Public Functions"
		public static Bitmap LightToGrayScaleBitmap(byte[] b, int width, int height, int strength = 1)
		{
			if (b == null)
				return null;
			if (strength < 1 || strength > 4)
				throw new NotSupportedException();

			// Create a palette based bitmap
			Bitmap returnBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
			BitmapData bmpData = returnBitmap.LockBits(new Rectangle(0, 0, returnBitmap.Width, returnBitmap.Height), ImageLockMode.WriteOnly, returnBitmap.PixelFormat);

			// Currently we do not support bitmaps with a negative stride
			if (bmpData.Stride <= 0)
				throw new NotSupportedException();

			// Copy the RGB values into the array.
			long scan0 = bmpData.Scan0.ToInt64();
			if (bmpData.Stride != bmpData.Width)
				for (int y = 0; y < bmpData.Height; y++)
				{
					Marshal.Copy(b, y * bmpData.Width, new IntPtr(scan0 + y * bmpData.Stride), bmpData.Width);
				}
			else
				Marshal.Copy(b, 0, bmpData.Scan0, b.Length);

			// Release the raw bitmap
			returnBitmap.UnlockBits(bmpData);

			// Get the palette
			System.Drawing.Imaging.ColorPalette pal = returnBitmap.Palette;

			// Modify the palette, converting the lightmap to a grayscale
			for (int j = -31; j <= 31; j++)
			{
				byte i = unchecked((byte)j);
				byte grayscale = grayscale = (byte)(128 + j * strength);
				/*
				if (i >= 0)
				  grayscale = (byte)( 255 - ((31 - i) * 255 / 31));
				else
				  grayscale = (byte)(-i * 255 / 31);
				 */
				pal.Entries[i] = System.Drawing.Color.FromArgb(grayscale, grayscale, grayscale);
			}

			// Fill the remaining colours with RED to see any errors clearly
			for (int i = 32; i <= unchecked((byte)-32); i++)
				pal.Entries[i] = System.Drawing.Color.FromArgb(255, 0, 0);

			// Apply the new palette!
			returnBitmap.Palette = pal;

			return returnBitmap;
		}
		#endregion
	}
}