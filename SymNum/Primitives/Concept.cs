using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives
{
	public class Concept
	{
		public Dictionary<Aspect, Relation> AspectRelations { get; set; } = new Dictionary<Aspect, Relation>();
	}
}
