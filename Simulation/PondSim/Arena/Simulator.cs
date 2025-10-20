
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

public class Simulator : DrawableBase
{
    private PondArena _arena;
    private List<CreatureBody> _creatures = new();
    private List<LightSource> _lights = new();
    private float _fixedDt = 1f / 60f;
    private float _accumulator = 0f;

    public Simulator(PondArena arena)
    {
        _arena = arena;

        var creature1 = new Creature();

        var creature = new CreatureBody(creature1);
        creature.InitPhysics(_arena, new Vector2(400, 300), 0f);
        _creatures.Add(creature);

        var light = new LightSource { Position = new Vector2(100, 100), Brightness = 5f };
        _lights.Add(light);

        Renderer.CurrentDrawable = this;
    }
    public Simulator(PondArena arena, List<CreatureBody> creatures, List<LightSource> lights)
    {
        _arena = arena;
        foreach(var light in lights)
        {
            _lights.Add(light);
        }

        foreach (var creature in _creatures)
        {
            creature.InitPhysics(_arena, creature.Position, creature.Rotation);
        }
    }
    public void Update(float deltaTime)
    {
        _accumulator += deltaTime;
        while (_accumulator >= _fixedDt)
        {
            foreach (var creature in _creatures)
            {
                creature.ApplyActions(1f, 0f); // e.g., constant forward
                creature.UpdateLightSensor(_arena, _lights);
                //creature.SetSight(leftLight, rightLight);
            }

            // Step physics
            _arena.PhysicsWorld.Step(_fixedDt, 8, 3); // Vel/pos iterations
            foreach (var creature in _creatures)
            {
                creature.SyncFromPhysics();
            }

            _accumulator -= _fixedDt;
        }
    }

    public override void Draw()
    {
        if (Canvas != null)
        {
            Canvas.Clear(SKColors.DarkSlateBlue);
            //todo: draw obstacles/lights

            using var creaturePaint = Pens.DrawPen;
            foreach (var creature in _creatures)
            {
                Canvas.Save();
                Canvas.Translate(creature.Position.X, creature.Position.Y);
                Canvas.RotateDegrees(creature.Rotation * 180f / MathF.PI);
                Canvas.DrawCircle(0, 0, 5f, creaturePaint);
                Canvas.Restore();
            }
        }
    }
}
