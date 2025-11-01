namespace Simulation.Render;

//using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using InterpolationMode = System.Drawing.Drawing2D.InterpolationMode;


public class FastBitmap : IDisposable
{
	public Image SourceBitmap { get; private set; }
	public int[] Bits { get; private set; }
	public bool Disposed { get; private set; }
	public int Height { get; private set; }
	public int Width { get; private set; }
	public int LatentScaleFactor { get; private set; } = 8;

	protected GCHandle BitsHandle { get; private set; }

	public FastBitmap(int width, int height)
	{
		Width = width;
		Height = height;
		Bits = new int[Width * Height];
		BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
		SourceBitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
	}

	public FastBitmap(Image sourceBitmap, int targetWidth = -1, int targetHeight = -1, int modSize = -1)
	{
		Width = targetWidth > 0 ? targetWidth : sourceBitmap.Width;
		Height = targetHeight > 0 ? targetHeight : sourceBitmap.Height;
		if (modSize > 0)
		{
			Width = Width - Width % modSize;
			Height = Height - Height % modSize;
		}
		Bits = new int[Width * Height];
		BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
		SourceBitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());

		var g = Graphics.FromImage(SourceBitmap);
		g.InterpolationMode = InterpolationMode.High;
		g.CompositingQuality = CompositingQuality.HighQuality;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.DrawImage(sourceBitmap, 0, 0, Width, Height);
	}

	public Image ToBitmap()
	{
		var result = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);
		var g = Graphics.FromImage(result);
		g.InterpolationMode = InterpolationMode.High;
		g.CompositingQuality = CompositingQuality.HighQuality;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.DrawImage(SourceBitmap, 0, 0, Width, Height);
		return result;
	}

	public static Image ScaledPortion(Image img, Rectangle from, Size to)
	{
		// todo: size should be auto scale that is about 512x512 pixels
		var imgRect = new Rectangle(Point.Empty, img.Size);
		from.Intersect(imgRect);
		Image result = new Bitmap(to.Width, to.Height);
		using (Graphics g = Graphics.FromImage(result))
		{
			g.DrawImage(img, new Rectangle(Point.Empty, to), from, GraphicsUnit.Pixel);
		}
		return result;
	}
	public static Image ScaleImageInto(Image image, Image overlay, Rectangle target, Image? mask = null)
	{
		overlay = Resize(overlay, target.Width, target.Height);
		Image result = new Bitmap(image.Width, image.Height);
		using (Graphics g = Graphics.FromImage(result))
		{
			//g.DrawImage(image, Point.Empty);
			g.DrawImage(overlay, target);
		}
		if (mask != null)
		{
			result = MergeMaskedOverlay(mask, image, result);
		}
		return result;
	}
	public static Image Resize(Image image, int targetWidth, int targetHeight)
	{
		var result = image;
		if (image.Width != targetWidth || image.Height != targetHeight)
		{
			result = new FastBitmap(image, targetWidth, targetHeight).ToBitmap();
		}
		return result;
	}
	public static Image MergeMaskedOverlay(Image? maskBmp, Image? orgBmp, Image overlay)
	{
		var result = overlay;
		if (maskBmp != null && orgBmp != null)
		{
			var msk = new FastBitmap(maskBmp, orgBmp.Width, orgBmp.Height);
			var org = new FastBitmap(orgBmp, orgBmp.Width, orgBmp.Height);
			var ovr = new FastBitmap(overlay, orgBmp.Width, orgBmp.Height);
			for (int y = 0; y < orgBmp.Height; y++)
			{
				for (int x = 0; x < orgBmp.Width; x++)
				{
					float scale = msk.GetPixel(x, y).R / 255f;
					Color colorO = org.GetPixel(x, y);
					Color colorN = ovr.GetPixel(x, y);
					var c = Color.FromArgb(
						(int)(colorO.R * (1f - scale) + colorN.R * scale),
						(int)(colorO.G * (1f - scale) + colorN.G * scale),
						(int)(colorO.B * (1f - scale) + colorN.B * scale));

					org.SetPixel(x, y, c);
				}
			}
			result = org.ToBitmap();
		}
		return result;
	}
	public static FastBitmap ConvertToMaskFB(Image image, float threshold = 0.5f, bool invert = false, int targetWidth = -1, int targetHeight = -1)
	{
		var w = targetWidth > 0 ? targetWidth : image.Width;
		var h = targetHeight > 0 ? targetHeight : image.Height;
		var result = new FastBitmap(image, w, h);
		for (int y = 0; y < h; y++)
		{
			for (int x = 0; x < w; x++)
			{
				Color color = result.GetPixel(x, y);
				var masked = invert ? GetBrightness(color) <= threshold : GetBrightness(color) > threshold;
				Color maskColor = masked ? Color.White : Color.Black;
				result.SetPixel(x, y, maskColor);
			}
			// optionally expand or blur here
		}
		return result;
	}
	public static Image ConvertToMask(Image image, float threshold, bool invert = false, int targetWidth = -1, int targetHeight = -1)
	{
		return ConvertToMaskFB(image, threshold, invert, targetWidth, targetHeight).ToBitmap();
	}
	public static int[] GetMaskRect(Image image, int threshold = 0, int padding = 0)
	{
		var w = image.Width;
		var h = image.Height;
		var fbmp = ConvertToMaskFB(image, 0.5f);// new FastBitmapX(image);//
		int minX = w;
		int minY = h;
		int maxX = 0;
		int maxY = 0;
		// minY
		for (int y = 0; y < h; y++)
		{
			for (int x = 0; x < w; x++)
			{
				var col = fbmp.GetPixel(x, y).R;
				if (col > threshold)
				{
					minY = y;
					goto maxY;
				}
			}
		}
	maxY:
		for (int y = h - 1; y > minY; y--)
		{
			for (int x = 0; x < w; x++)
			{
				var col = fbmp.GetPixel(x, y).R;
				if (col > threshold)
				{
					maxY = y;
					goto minX;
				}
			}
		}
	minX:
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				var col = fbmp.GetPixel(x, y).R;
				if (col > threshold)
				{
					minX = x;
					goto maxX;
				}
			}
		}
	maxX:
		for (int x = w - 1; x > minX; x--)
		{
			for (int y = 0; y < h; y++)
			{
				var col = fbmp.GetPixel(x, y).R;
				if (col > threshold)
				{
					maxX = x;
					goto end;
				}
			}
		}
	end:
		if (padding > 0)
		{
			minX = Math.Max(0, minX - padding);
			minY = Math.Max(0, minY - padding);
			maxX = Math.Min(w, maxX + padding);
			maxY = Math.Min(h, maxY + padding);
		}
		return new int[] { minX, minY, maxX - minX, maxY - minY };
	}
	public static Rectangle IntsToRectangle(int[] rectInts)
	{
		return rectInts.Length == 4 && rectInts[2] > 0 && rectInts[3] > 0 ?
			new Rectangle(rectInts[0], rectInts[1], rectInts[2], rectInts[3]) :
			Rectangle.Empty;

	}
	public static int[] RectangleToInts(Rectangle rect)
	{
		return rect.IsEmpty ? new int[0] : new int[] { rect.X, rect.Y, rect.Width, rect.Height };
	}
	public static Image ConvertToVisualMask(Image image, Image mask)
	{
		var tImage = new FastBitmap(image);
		var tMask = new FastBitmap(mask);
		for (int y = 0; y < image.Height; y++)
		{
			for (int x = 0; x < image.Width; x++)
			{
				Color color = tImage.GetPixel(x, y);
				bool masked = tMask.GetPixel(x, y).GetBrightness() < 0.5;
				if (masked)
				{
					color = Color.FromArgb(255,
						//(int)Math.Clamp(color.R + 140, 0, 255),
						//(int)Math.Clamp(color.G + 80, 0, 255),
						//(int)Math.Clamp(color.B + 80, 0, 255)
						Math.Clamp((int)(color.R / 2.5f), 0, 120),
						Math.Clamp((int)(color.G / 3f), 0, 120),
						Math.Clamp((int)(color.B / 3f), 0, 120)
						);
				}
				tImage.SetPixel(x, y, color);
			}
		}
		return tImage.ToBitmap();
	}
	public static Image? InvertImage(Image? image)
	{
		var result = image;
		if (image != null)
		{
			var w = image.Width;
			var h = image.Height;
			var bmp = new FastBitmap(image, w, h);
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					Color color = bmp.GetPixel(x, y);
					var invColor = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
					bmp.SetPixel(x, y, invColor);
				}
			}
			result = bmp.ToBitmap();
		}
		return result;
	}

	public void SetPixel(int x, int y, uint color)
	{
		int index = x + y * Width;
		Bits[index] = (int)color;
	}
	public void SetPixel(int x, int y, Color color)
	{
		int index = x + y * Width;
		int col = color.ToArgb();

		Bits[index] = col;
	}
	public int GetPixelValues(int x, int y)
	{
		int index = x + y * Width;
		return Bits[index];
	}
	public Color GetPixel(int x, int y)
	{
		int index = x + y * Width;
		int col = Bits[index];
		Color result = Color.FromArgb(col);

		return result;
	}
	public static float GetBrightness(Color c)
	{
		return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f;
	}


	public static FastBitmap? Load(string path)
	{
		FastBitmap? result = null;
		try
		{
			if (File.Exists(path))
			{
				using (Bitmap? bmp = (Bitmap?)Image.FromFile(path))
				{
					if (bmp != null)
					{
						result = new FastBitmap(bmp);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine(ex.Message);
		}
		return result;
	}
	public string Save(string path)
	{
		var success = false;
		try
		{
			var format = Path.GetExtension(path).ToUpper() == ".PNG" ? ImageFormat.Png : ImageFormat.Jpeg;
			using (var file = new FileStream(path, FileMode.Create))
			{
				SourceBitmap.Save(file, format);
			}
			success = true;
		}
		catch (Exception ex)
		{
			Trace.WriteLine(ex.Message);
		}
		return success ? path : "";
	}

	public void Dispose()
	{
		if (Disposed) return;
		Disposed = true;
		SourceBitmap.Dispose();
		BitsHandle.Free();
	}
}