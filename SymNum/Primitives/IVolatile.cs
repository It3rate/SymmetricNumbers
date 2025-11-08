namespace SymNum.Primitives;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IVolatile
{
    void OnUpdate(IMeasure source);
}
