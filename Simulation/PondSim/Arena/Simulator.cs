
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
using SymNum.Primitives;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Collision;

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
        _envImage = new SKImageMapper("map0.png", new SKSegment(new(0,0), new(_width, 0)));
        _bkgMap = new FastBitmap(_envImage.WinBmp);

		var light = new LightSource { Position = new Vector2(100, 100), Brightness = 5f };
        _lights.Add(light);

        CreateWalls();

        var margin = 0.7f;
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
		_arena.PhysicsWorld.SetContactListener(new WallHitContactListener(this));

		Pens.GenTheme(ColorTheme.Dark);
        Renderer.CurrentDrawable = this;
    }

	private void CreateWalls()
	{
		var world = _arena.PhysicsWorld;
        var w2 = _wallThickness * 2;

		// Left wall
		var leftDef = new BodyDef { type = BodyType.Static, position = new Vector2(-w2, _height / 2f) };
		var leftBody = world.CreateBody(leftDef);
		leftBody.SetUserData("left");
		var leftShape = new PolygonShape();
		leftShape.SetAsBox(_wallThickness / 2f, _height / 2f);
		leftBody.CreateFixture(leftShape, 0f);

		// Right wall
		var rightDef = new BodyDef { type = BodyType.Static, position = new Vector2(_width + w2, _height / 2f) };
		var rightBody = world.CreateBody(rightDef);
		rightBody.SetUserData( "right");
		var rightShape = new PolygonShape();
		rightShape.SetAsBox(_wallThickness / 2f, _height / 2f);
		rightBody.CreateFixture(rightShape, 0f);

		// Bottom wall
		var bottomDef = new BodyDef { type = BodyType.Static, position = new Vector2(_width / 2f, _height + w2) };
		var bottomBody = world.CreateBody(bottomDef);
		bottomBody.SetUserData("bottom");
		var bottomShape = new PolygonShape();
		bottomShape.SetAsBox(_width / 2f, _wallThickness / 2f);
		bottomBody.CreateFixture(bottomShape, 0f);

		// Top wall
		var topDef = new BodyDef { type = BodyType.Static, position = new Vector2(_width / 2f, -w2) };
		var topBody = world.CreateBody(topDef);
		topBody.SetUserData("top");
		var topShape = new PolygonShape();
		topShape.SetAsBox(_width / 2f, _wallThickness / 2f);
		topBody.CreateFixture(topShape, 0f);
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
	public override void Draw()
    {
        if (Canvas != null)
        {
            Canvas.Clear(SKColors.DarkSlateBlue);
            _envImage?.Draw();
            Canvas.DrawRect(0, 0, _width, _height, new SKPaint
			{
				Color = SKColors.Black.WithAlpha(150),
				BlendMode = SKBlendMode.Multiply
			});
            //todo: draw obstacles/lights
            foreach (var creature in _creatureBodies)
            {
                Canvas.Save();
                Canvas.Translate(creature.Position.X, creature.Position.Y);
                Canvas.RotateDegrees(creature.Rotation * 180f / MathF.PI);

				creature.Draw();
                Canvas.Restore();
			}

            if (_creatureBodies.Count > 0 && _creatureBodies[0]._creature != null)
            {
                var c = _creatureBodies[0]._creature;
                var dml = (DecayMeasure)c.LeftEyeDecay;
                var dmr = (DecayMeasure)c.RightEyeDecay;
                var lineWidth = 255 * 2;
                var offset = 50;
                var y = 20;
				Canvas.DrawLine(
					new(offset + dml.Minimum.Position * 2, y-5),
					new(offset + dml.Maximum.Position * 2, y-5),
					Pens.SegPen2);
				Canvas.DrawLine(
					new(offset + dml.Position * 2, y - 5),
					new(offset + dml.Position * 2, y - 5),
					Pens.SegPen1);

				Canvas.DrawLine(
					new(offset + dmr.Minimum.Position * 2, y),
					new(offset + dmr.Maximum.Position * 2, y),
					Pens.SegPen2);
				Canvas.DrawLine(
					new(offset + dmr.Position * 2, y - 0),
					new(offset + dmr.Position * 2, y - 0),
					Pens.SegPen3);

				Canvas.DrawLine(new(offset, 0), new(offset, y), Pens.TickBoldPen);
				Canvas.DrawLine(new(offset + lineWidth / 2f, 0), new(offset + lineWidth/2f, y), Pens.TickBoldPen);
				Canvas.DrawLine(new(offset + lineWidth, 0), new(offset + lineWidth, y), Pens.TickBoldPen);
			}
		}
    }
}





public class WallHitContactListener : ContactListener
{
	private readonly Simulator _sim;

	public WallHitContactListener(Simulator sim)
	{
		_sim = sim;
	}

	public override void BeginContact(in Contact contact)
	{
		//base.BeginContact(contact);

		// Quick filter: Check if one fixture is a creature (category 0x0002) and the other is a wall
		var fixtureA = contact.FixtureA;
		var fixtureB = contact.FixtureB;
		var creatureFixture = (fixtureA.FilterData.categoryBits == 0x0002) ? fixtureA :
							  (fixtureB.FilterData.categoryBits == 0x0002) ? fixtureB : null;
		var wallFixture = (creatureFixture == fixtureA) ? fixtureB : fixtureA;

		if (creatureFixture == null || wallFixture.Body.UserData is not string wallTag)
		{
			return;  // Not a valid creature-wall hit
		}

		// Get the creature body
		if (creatureFixture.Body.UserData is not CreatureBody creatureBody)
		{
			return;
		}

		creatureBody.OnWallHit();
	}

	// Optional: Override EndContact if you need to track ongoing collisions
	public override void EndContact(in Contact contact)
	{
		//base.EndContact(contact);
		// Could add logic here if needed (e.g., reset hit state)
	}

	public override void PostSolve(in Contact contact, in ContactImpulse impulse)
	{
	}

	public override void PreSolve(in Contact contact, in Manifold oldManifold)
	{
	}
}