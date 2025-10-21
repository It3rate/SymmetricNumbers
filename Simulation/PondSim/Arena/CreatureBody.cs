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
    public Vector2 Position => PhysicsBody?.GetPosition() ?? Vector2.Zero;
    public float Rotation => PhysicsBody?.GetAngle() ?? 0;
    public float LightSensorValue { get; private set; } // Output: 0 (dark) to 1 (bright)

    public Body? PhysicsBody { get; private set; }
    private Creature _creature;

    private float _creatureRadius = 5f;

    public CreatureBody(Creature creature)
    {
        _creature = creature;
    }

    public void InitPhysics(PondArena arena, Vector2 startPos, float startRot)
    {
        var bodyDef = new BodyDef
        {
            type = BodyType.Dynamic,
            position = new Vector2(startPos.X, startPos.Y),
            angle = startRot,
            linearDamping = 0.5f,
            angularDamping = 2f,
            bullet = false
        };
        PhysicsBody = arena.PhysicsWorld.CreateBody(bodyDef);

        var circleShape = new CircleShape { Radius = _creatureRadius };
        PhysicsBody.CreateFixture(new FixtureDef { shape = circleShape, density = 1f, friction = 0.3f, restitution = 0.1f });
    }

	private float _thrustForce = 2f; // Tune for sim scale
	private float _turnTorque = 5f;
	public void ApplyActions()
    {
        _creature.OnTick();
        if(PhysicsBody != null)
        {
            var turn = (float)_creature.SightRatio.Value - 0.8f;
            var thrust = (float)_creature.Thrust.Value;
            // Forward thrust in facing direction
            var facing = new Vector2((float)Math.Cos(PhysicsBody.GetAngle()), (float)Math.Sin(PhysicsBody.GetAngle()));
            PhysicsBody.ApplyForceToCenter(facing * thrust * _thrustForce, true);

            // Rotation torque
            //PhysicsBody.ApplyTorque(turn * _turnTorque, true);
            //_creature.LeftEye.SetPosition(_creature.LeftEye.Position + 200);
            //Trace.WriteLine(thrust + " : " + turn + " : " + turn*_turnTorque);
        }
    }

    // Sync after sim step (for rendering/sensor)
    public override void Update(float deltaTime)
    {
        ApplyActions();
    }

    public void UpdateLightSensor(PondArena arena, List<LightSource> lights)
    {
        float totalIntensity = 0f;
        foreach (var light in lights)
        {
            Vector2 toLight = new Vector2(light.Position.X - Position.X, light.Position.Y - Position.Y);
            float distance = toLight.Length();
            //toLight.Normalize();

            // Raycast callback collector
            //var rayCastCallback = new RayCastCallback(distance);
            //arena.PhysicsWorld.RayCast(rayCastCallback,
            //    new Vector2(Position.X, Position.Y),
            //    new Vector2(Position.X, Position.Y) + toLight * distance);

            //bool visible = !rayCastCallback.ClosestHitDist.HasValue || rayCastCallback.ClosestHitDist >= distance;
            //if (visible)
            //{
            //    float angleDiff = MathF.Acos(Vector2.Dot(new Vector2(toLight.X, toLight.Y),
            //        new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation))));
            //    if (angleDiff < MathF.PI / 2)
            //    {
            //        float intensity = light.Brightness / (distance * distance + 1f);
            //        totalIntensity += (float)Math.Clamp(intensity, 0f, 1f);
            //    }
            //}
        }
        LightSensorValue = (float)Math.Clamp(totalIntensity / lights.Count, 0f, 1f);
    }

    //private class RayCastCallback : IRayCastCallback
    //{
    //    public float? ClosestHitDist { get; private set; }
    //    private float _distance;

    //    public RayCastCallback(float distance)
    //    {
    //        _distance = distance;
    //        ClosestHitDist = null;
    //    }

    //    public float ReportFixture(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
    //    {
    //        ClosestHitDist ??= fraction * _distance;
    //        return -1f; // Continue to find closest
    //    }

    //    float IRayCastCallback.RayCastCallback(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
    //    {
    //        return 0; //throw new NotImplementedException();
    //    }
    //}
    public override void Draw()
    {
        if (PhysicsBody != null)
        {
            Canvas?.DrawCircle(SKPoint.Empty, _creatureRadius, Pens.DrawPen);
        }
    }
}
