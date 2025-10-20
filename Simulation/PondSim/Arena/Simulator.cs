
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

        var light = new LightSource { Position = new Vector2(100, 100), Brightness = 5f };
        _lights.Add(light);

        var creature1 = new Creature();

        var creature = new CreatureBody(creature1);
        creature.InitPhysics(_arena, new Vector2(400, 600), 10f);
        _creatures.Add(creature);

        Renderer.CurrentDrawable = this;
    }
    public Simulator(PondArena arena, IEnumerable<CreatureBody> creatures, IEnumerable<LightSource> lights)
    {
        _arena = arena;
        _lights.AddRange(lights);
        _creatures.AddRange(creatures);

        foreach (var creature in _creatures)
        {
            creature.InitPhysics(_arena, creature.Position, creature.Rotation);
        }
    }
    public override void Update(float deltaTime)
    {
        _accumulator += deltaTime;
        while (_accumulator >= _fixedDt)
        {
            foreach (var creature in _creatures)
            {
                creature.UpdateLightSensor(_arena, _lights);
                creature.Update(deltaTime);
            }
            _arena.PhysicsWorld.Step(_fixedDt, 8, 3); // Vel/pos iterations

            _accumulator -= _fixedDt;
        }
    }

    public override void Draw()
    {
        if (Canvas != null)
        {
            Canvas.Clear(SKColors.DarkSlateBlue);
            //todo: draw obstacles/lights
            foreach (var creature in _creatures)
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
