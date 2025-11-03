
namespace Simulation.PondSim.Arena;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Forces.Primitives.Steering.Simulation;
using Simulation.Render;
using NumbersSkia.Mappers;
using System.Diagnostics;

public class Simulator : DrawableBase
{
    private PondArena _arena;
    private List<CreatureBody> _creatureBodies = new();
    private List<LightSource> _lights = new();
    private float _fixedDt = 1f / 60f;
    private float _accumulator = 0f;

    private SKImageMapper? _envImage;
    private FastBitmap _bkgMap;
    public Simulator(PondArena arena)
    {
        _arena = arena;
        //var min = MathF.Min(_arena.Control.Width, _arena.Control.Height); //436;// 
        _envImage = new SKImageMapper("map1.png", new SKSegment(new(0,0), new(_arena.Control.Width, 0)));
        _bkgMap = new FastBitmap(_envImage.WinBmp);

		var light = new LightSource { Position = new Vector2(100, 100), Brightness = 5f };
        _lights.Add(light);

        var creature1 = new Creature();

        var creatureBody = new CreatureBody(this, creature1);
        creatureBody.InitPhysics(_arena, new Vector2(500, 500), 10f);
        _creatureBodies.Add(creatureBody);

        Renderer.CurrentDrawable = this;
    }
    public Simulator(PondArena arena, IEnumerable<CreatureBody> creatures, IEnumerable<LightSource> lights)
    {
        _arena = arena;
        _lights.AddRange(lights);
        _creatureBodies.AddRange(creatures);

        foreach (var creatureBody in _creatureBodies)
        {
            creatureBody.InitPhysics(_arena, creatureBody.Position, creatureBody.Rotation);
        }
	}
    public long GetLightAt(int x, int y)
	{
		x = int.Clamp(x, 0, _bkgMap.Width);
		y = int.Clamp(y, 0, _bkgMap.Height);
		return _bkgMap.GetPixel(x, y).R;
    }
	public long GetLightAt(SKPoint pt) => GetLightAt((int)pt.X, (int)pt.Y);

	public override void Update(float deltaTime)
    {
        //_accumulator += deltaTime;
        //while (_accumulator >= _fixedDt)
        //{
            foreach (var creatureBody in _creatureBodies)
            {
                creatureBody.Update(deltaTime);
            }
            _arena.PhysicsWorld.Step(_fixedDt, 8, 3); // Vel/pos iterations

        //    _accumulator -= _fixedDt;
        //}
    }

    public override void Draw()
    {
        if (Canvas != null)
        {
            Canvas.Clear(SKColors.DarkSlateBlue);
            _envImage?.Draw();
            //todo: draw obstacles/lights
            foreach (var creature in _creatureBodies)
            {
                Canvas.Save();
                Canvas.Translate(creature.Position.X, creature.Position.Y);
                Canvas.RotateDegrees(creature.Rotation * 180f / MathF.PI);

				//var matrix = Canvas.TotalMatrix;
				//var left = matrix.MapPoint(creature.LeftEye);
				//var right = matrix.MapPoint(creature.RightEye);
				//var cLeft = GetLightAt(left);
				//var cRight = GetLightAt(right);
				//Trace.WriteLine($"({cLeft}, {cRight})");

				creature.Draw();
                Canvas.Restore();
			}
        }
    }
}
