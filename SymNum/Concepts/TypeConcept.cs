using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Concepts
{
	public class TypeConcept
	{
		public Dictionary<DenominatorAspect, RelationEquation> AspectRelations { get; set; } = new Dictionary<DenominatorAspect, RelationEquation>();
	}
}
