namespace Simulation.Render;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class DrawableBase
{
    protected CoreRenderer _renderer = CoreRenderer.Instance;
    protected CorePens _pens => _renderer.Pens;
}
