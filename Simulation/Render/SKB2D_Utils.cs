namespace Simulation.Render;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Forces.Primitives.Electricity.Gradients;
using SkiaSharp;

public static class Vector2Extensions
{
    public static SKPoint SKPoint(this Vector2 value)
    {
        return new SKPoint(value.X, value.Y);
    }
}
