using NumbersCore.Utils;
using Simulation.Render;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class SKView
{
	public CoreRenderer Renderer { get; }
	public SKCanvas Canvas => Renderer.Canvas;
}
