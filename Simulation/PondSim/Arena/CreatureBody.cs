namespace Simulation.PondSim.Arena;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Forces.Primitives.Steering.Simulation;
using SkiaSharp;
using Simulation.Render;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Collision.Shapes;
using System.Diagnostics;

public class CreatureBody : DrawableBase
{
    public Vector2 Position => Body?.GetPosition() ?? Vector2.Zero;
    public float Rotation => Body?.GetAngle() ?? 0;

    public Body? Body { 
        get;
        private set; }
    private Creature _creature;

    public SKPoint LeftEye { get; }
    public SKPoint RightEye { get; }
	public Simulator _sim { get; }

    public CreatureBody(Simulator sim, Creature creature)
    {
		_sim = sim;
        _creature = creature;

		var offset = _creatureRadius * -0.7f;
		LeftEye = new(-offset, offset);
		RightEye = new(-offset, -offset);
	}

    public void InitPhysics(PondArena arena, Vector2 startPos, float startRot)
    {
        var bodyDef = new BodyDef
        {
            type = BodyType.Dynamic,
            position = new Vector2(startPos.X, startPos.Y),
            angle = startRot,
            linearDamping = 3f,
            angularDamping = 2.3f,
            bullet = false
        };
        var body = arena.PhysicsWorld.CreateBody(bodyDef);
        Body = body;
        var circleShape = new CircleShape { Radius = _creatureRadius };
        Body.CreateFixture(new FixtureDef { 
			shape = circleShape, 
			density = 1f, 
			friction = 0.01f,
			restitution = 0.8f,
			filter = new Filter
			{
				categoryBits = 0x0002,  // Creature category
				maskBits = 0x0001       // Only collide with wall category
			}
		});
    }

	private float _thrustForce = 1000f; // Tune for sim scale
	//private float _turnTorque = 3000f;
	private Random _rnd = new Random();
	private float _turnTotal = 0f;
	public override void Update(float deltaTime)
	{
		SampleEyes();
        _creature.OnTick();

        if(Body != null)
		{
			var speed = Body.GetLinearVelocity().Length();
			var angle = Body.GetAngle();
            var angleVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

			_turnTotal = (float)_creature.BrighterDirection * 6f;
			var thrustScale = _creature.InvBrightness.MinValue + 8f;

			var force = angleVec * _thrustForce * thrustScale;
			var point = Body.GetWorldPoint(new(0, _turnTotal));
			Body.ApplyForce(force, point, true);
			Trace.WriteLine("to: "+ _turnTotal);
			 
		}
	}
	private void SampleEyes()
	{
		if(_leftEyePos != null && _rightEyePos != null)
		{
			var cLeft = _sim.GetLightAt((SKPoint)_leftEyePos);
			var cRight = _sim.GetLightAt((SKPoint)_rightEyePos!);
			_creature.SetSight(cLeft, cRight);

		}
	}

	private SKPoint? _leftEyePos;
	private SKPoint? _rightEyePos;
	private static float _creatureRadius = 8f;
	private SKPaint FillL = CorePens.GetBrush(SKColor.Parse("#00F080"));
	private SKPaint FillR = CorePens.GetBrush(SKColor.Parse("#80F000"));
	private SKPaint White = CorePens.GetBrush(SKColor.Parse("#FFFFFF"));
	private SKPaint Outline = CorePens.GetPen(SKColor.Parse("#F01000"), 2);
	public override void Draw()
    {
        var offset = _creatureRadius * -0.7f;

		if (Body != null)
		{
			var f = _turnTotal > 0 ? FillR : FillL;
			Canvas?.DrawCircle(SKPoint.Empty, _creatureRadius, f);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, White);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, White);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, Outline);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, Outline);

			_leftEyePos = Canvas?.TotalMatrix.MapPoint(LeftEye);
			_rightEyePos = Canvas?.TotalMatrix.MapPoint(RightEye);
		}
    }
}
