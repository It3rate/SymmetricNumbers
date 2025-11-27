using SymNum.Numbers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Expressions;

public class Expression : RawNumber, IExpression, IDisposable
{
	public INumber InputA { get; }

	public INumber InputB { get; }

	public INumber Output => this;

	public BinaryOp Op { get; }
	public Expression(INumber inputA, INumber inputB, BinaryOp op)
	{
		InputA = inputA;
		InputB = inputB;
		Op = op;
		if (InputA.IsInput)
		{
			InputA.OnValueChanged += OnInputChanged;
		}
		if (InputB.IsInput)
		{
			InputB.OnValueChanged += OnInputChanged;
		}
	}
	public void Calculate()
	{
		SetWith(Op.Invoke(InputA, InputB));
	}

	private void OnInputChanged(INumber obj)
	{
		Calculate();
	}

	public void Dispose()
	{
		InputA.OnValueChanged -= OnInputChanged;
		InputB.OnValueChanged -= OnInputChanged;
	}
}
