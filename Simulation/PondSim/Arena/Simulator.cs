
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
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;

public class Simulator : DrawableBase
{
    private PondArena _arena;
    private List<CreatureBody> _creatureBodies = new();
    private List<LightSource> _lights = new();
    private float _fixedDt = 1f / 60f;

    private SKImageMapper? _envImage;
    private FastBitmap _bkgMap;
    private int _width => _arena.Control.Width;
    private int _height => _arena.Control.Height;
	private float _wallThickness = 5f;

	public Simulator(PondArena arena)
    {
        _arena = arena;
        _envImage = new SKImageMapper("map1.png", new SKSegment(new(0,0), new(_width, 0)));
        _bkgMap = new FastBitmap(_envImage.WinBmp);

		var light = new LightSource { Position = new Vector2(100, 100), Brightness = 5f };
        _lights.Add(light);

        CreateWalls();

        var margin = 0.9f;
        var offset = (1f - margin) / 2f;
        var rnd = new Random();
        var bodyCount = Debugger.IsAttached ? 1 : 80;

		for (var i = 0; i < bodyCount; i++)
        {
            var creature = new Creature();
            var creatureBody = new CreatureBody(this, creature);
            creatureBody.InitPhysics(_arena, 
                new Vector2(
                    rnd.NextSingle() * _width * margin + _width * offset,
                    rnd.NextSingle() * _height * margin + _height * offset),
                rnd.NextSingle() * MathF.PI * 2f);
            _creatureBodies.Add(creatureBody);
        }

        Renderer.CurrentDrawable = this;
    }
    public long GetLightAt(int x, int y)
	{
		x = int.Clamp(x, 0, _width);
		y = int.Clamp(y, 0, _height);
		return _bkgMap.GetPixel(x, y).R;
    }
	public long GetLightAt(SKPoint pt) => GetLightAt((int)pt.X, (int)pt.Y);

	public override void Update(float deltaTime)
    {
        _arena.PhysicsWorld.Step(_fixedDt, 8, 3); // Vel/pos iterations
        foreach (var creatureBody in _creatureBodies)
        {
            creatureBody.Update(deltaTime);
        }
    }

	private void CreateWalls()
	{
		var world = _arena.PhysicsWorld;
        var w2 = _wallThickness * 2;

		// Left wall
		var leftDef = new BodyDef { type = BodyType.Static, position = new Vector2(-w2, _height / 2f) };
		var leftBody = world.CreateBody(leftDef);
		var leftShape = new PolygonShape();
		leftShape.SetAsBox(_wallThickness / 2f, _height / 2f);
		leftBody.CreateFixture(leftShape, 0f); // Density 0 for static

        // Right wall
        var rightDef = new BodyDef { type = BodyType.Static, position = new Vector2(_width + w2, _height / 2f) };
        var rightBody = world.CreateBody(rightDef);
        var rightShape = new PolygonShape();
        rightShape.SetAsBox(_wallThickness / 2f, _height / 2f);
        rightBody.CreateFixture(rightShape, 0f);

        // Bottom wall
        var bottomDef = new BodyDef { type = BodyType.Static, position = new Vector2(_width / 2f, _height + w2) };
        var bottomBody = world.CreateBody(bottomDef);
        var bottomShape = new PolygonShape();
        bottomShape.SetAsBox(_width / 2f, _wallThickness / 2f);
        bottomBody.CreateFixture(bottomShape, 0f);

        // Top wall
        var topDef = new BodyDef { type = BodyType.Static, position = new Vector2(_width / 2f, -w2) };
        var topBody = world.CreateBody(topDef);
        var topShape = new PolygonShape();
        topShape.SetAsBox(_width / 2f, _wallThickness / 2f);
        topBody.CreateFixture(topShape, 0f);
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

				creature.Draw();
                Canvas.Restore();
			}
        }
    }
}
