namespace Simulation.PondSim;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Simulation.PondSim.Arena;
using Forces.Primitives.Steering.Simulation;

public class CreatureBody
{
    public Vector2 Position;
    public float Rotation; // Facing direction
    public float LightSensorValue { get; private set; } // Output: 0 (dark) to 1 (bright)

    public Body PhysicsBody { get; private set; }
    private Creature _creature;
    private float _thrustForce = 50f; // Tune for sim scale
    private float _turnTorque = 10f;

    public CreatureBody(Creature creature)
    {
        _creature = creature;
    }

    public void InitPhysics(PondArena arena, Vector2 startPos, float startRot)
    {
        var bodyDef = new BodyDef
        {
            BodyType = BodyType.DynamicBody,
            Position = new Vector2(startPos.X, startPos.Y),
            Angle = startRot,
            LinearDamping = 0.5f, // Friction-like slowdown
            AngularDamping = 2f
        };
        PhysicsBody = arena.PhysicsWorld.CreateBody(bodyDef);

        var circleShape = new CircleShape { Radius = 5f }; // Tiny creature radius
        PhysicsBody.CreateFixture(new FixtureDef { Shape = circleShape, Density = 1f, Friction = 0.3f, Restitution = 0.1f });
    }

    // Call per sim step with RL action (e.g., float thrust, float turn from agent)
    public void ApplyActions(float thrust, float turn)
    {
        // Forward thrust in facing direction
        var facing = new Vector2((float)Math.Cos(PhysicsBody.GetAngle()), (float)Math.Sin(PhysicsBody.GetAngle()));
        PhysicsBody.ApplyForceToCenter(facing * thrust * _thrustForce, true);

        // Rotation torque
        PhysicsBody.ApplyTorque(turn * _turnTorque, true);
    }

    // Sync after sim step (for rendering/sensor)
    public void SyncFromPhysics()
    {
        var pos = PhysicsBody.GetPosition();
        Position = new Vector2(pos.X, pos.Y);
        Rotation = PhysicsBody.GetAngle();
    }

    public void UpdateLightSensor(PondArena arena, List<LightSource> lights)
    {
        float totalIntensity = 0f;
        foreach (var light in lights)
        {
            Vector2 toLight = new(light.Position.X - Position.X, light.Position.Y - Position.Y);
            float distance = toLight.Length();
            toLight.Normalize();

            // Raycast callback collector
            var rayCastCallback = new RayCastCallback(distance);
            arena.PhysicsWorld.RayCast(rayCastCallback,
                new Vector2(Position.X, Position.Y),
                new Vector2(Position.X, Position.Y) + toLight * distance);

            bool visible = !rayCastCallback.ClosestHitDist.HasValue || rayCastCallback.ClosestHitDist >= distance;
            if (visible)
            {
                float angleDiff = MathF.Acos(Vector2.Dot(new Vector2(toLight.X, toLight.Y),
                    new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation))));
                if (angleDiff < MathF.PI / 2)
                {
                    float intensity = light.Brightness / (distance * distance + 1f);
                    totalIntensity += (float)Math.Clamp(intensity, 0f, 1f);
                }
            }
        }
        LightSensorValue = (float)Math.Clamp(totalIntensity / lights.Count, 0f, 1f);
    }

    private class RayCastCallback : IRayCastCallback
    {
        public float? ClosestHitDist { get; private set; }
        private float _distance;

        public RayCastCallback(float distance)
        {
            _distance = distance;
            ClosestHitDist = null;
        }

        public float ReportFixture(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
        {
            ClosestHitDist ??= fraction * _distance;
            return -1f; // Continue to find closest
        }

        float IRayCastCallback.RayCastCallback(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
        {
            throw new NotImplementedException();
        }
    }
}
