namespace NumbersSkia.Mappers;

using System;
using System.IO;
using Simulation.Motion;
using Simulation.Render;
using SkiaSharp;

public class SKImageMapper : DrawableBase
{
    public SKRect Bounds { get; set; }
    public SKSegment AspectRatio { get; set; } = new SKSegment(SKPoint.Empty, SKPoint.Empty);
    public SKBitmap Bitmap { get; set; }
    public SKPaint BorderPen { get; set; }
    public SKSegment Guideline { get; set; }
    private string _path { get; set; }


	public SKImageMapper(string path, SKSegment guideline) 
    {
        Guideline = guideline;
        SetBitmap(path);
    }

    public void SetBitmap(string imageName)
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string imageFolder = "Images";
        _path = Path.Combine(appDirectory, imageFolder, imageName);
        if (_path != null && _path != "" && File.Exists(_path))
        {
            Bitmap = SKBitmap.Decode(_path);
            AspectRatio.Reset(new SKPoint(0, 0), new SKPoint(Bitmap.Width, Bitmap.Height));
            var w = Bitmap.Width;
            var h = Bitmap.Height;
            Guideline = (Guideline == null) ? new SKSegment(0, 0, w, h) : Guideline;
            var ratio = Guideline.AbsLength / (float)w;
            var x = Guideline.StartPoint.X;
            var y = Guideline.StartPoint.Y;
            Bounds = new SKRect(x, y, w * ratio + x, h * ratio + y);
        }
    }

    public override void Draw()
    {
        if (Bitmap != null)
        {
            Renderer.DrawBitmap(Bitmap, Bounds);
        }
    }

    public void Reset(string path)
    {
        SetBitmap(path);
    }

    public override void Update(float deltaTime)
    {
    }
}
