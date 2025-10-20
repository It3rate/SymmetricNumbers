namespace Simulation.Render;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

public interface IDrawable
{
    CoreRenderer Renderer { get; } 
    CorePens Pens { get; }
    SKCanvas? Canvas { get; }
    void Draw();
    void Update(float deltaTime);
}

public abstract class DrawableBase : IDrawable
{
    public CoreRenderer Renderer => CoreRenderer.Instance;
    public CorePens Pens => Renderer.Pens;
    public SKCanvas? Canvas => Renderer.Canvas;
    public abstract void Draw();
    public abstract void Update(float deltaTime);
}
