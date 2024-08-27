using Godot;
using System;

public partial class SplitterNodeScene : GraphNode, ISplitterNode
{
	public override void _Ready()
	{
	}

	public GraphNode AsGraphNode() => this;

	public void Load(Splitter splitter)
	{
		Title = splitter.Name;
		AddChild(new Label());

		SetSlotEnabledLeft(0, true);
		SetSlotTypeLeft(0, (int)ParserConnectionType.GENERAL);
		SetSlotColorLeft(0, new Color("red"));

		SetSlotEnabledRight(0, true);
		SetSlotTypeRight(0, (int)ParserConnectionType.GENERAL);
		SetSlotColorRight(0, new Color("blue"));
	}

	public int GetAvailablePort(int candidate) => 0;
}
