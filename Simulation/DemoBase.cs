using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Motion;

namespace Simulation;

public delegate DemoBase PageCreator();
public abstract class DemoBase
{
	public List<PageCreator> Pages { get; } = new List<PageCreator>();
	protected int Count => Pages.Count;
	protected int _testIndex = 0;
	protected MouseAgent _currentMouseAgent;
	//protected List<int> _tests{get;} = new List<int>();

	public DemoBase PreviousTest(MouseAgent mouseAgent)
	{
		int index = _testIndex >= 1 ? _testIndex - 1 : Pages.Count - 1;
		return LoadTest(index, mouseAgent);
	}
	public DemoBase Reload(MouseAgent mouseAgent)
	{
		return LoadTest(_testIndex, mouseAgent);
	}
	public DemoBase NextTest(MouseAgent mouseAgent)
	{
		int index = _testIndex >= Pages.Count - 1 ? 0 : _testIndex + 1;
		return LoadTest(index, mouseAgent);
	}

	public DemoBase LoadTest(int index, MouseAgent mouseAgent)
	{
		_testIndex = index;
		_currentMouseAgent = mouseAgent;
		_currentMouseAgent.IsPaused = true;
		_currentMouseAgent.ClearAll();

		DemoBase wm = Pages[_testIndex]();
		//wm.EnsureRenderers();
		_currentMouseAgent.IsPaused = false;
		return wm;
	}
}
