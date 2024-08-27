using Godot;
using System;

public partial class MatcherNodeScene : GraphNode, IMatcherNode
{
	private Matcher _matcher;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public GraphNode AsGraphNode() => this;

	public void Load(Matcher matcher)
	{
		_matcher = matcher;
		Title = matcher.Name;

		for (int i = 0; i < matcher.GroupCount; i++) {
			// var child = selector.Children[i];
			var label = new Label();
			AddChild(label);

			label.Text = "";
			SetSlotEnabledRight(i, true);
			SetSlotTypeRight(i, (int)ParserConnectionType.GENERAL);
			SetSlotColorRight(i, new Color("green"));
		}

		if (GetChildCount() == 0)
			AddChild(new Label());
		 
		SetSlotEnabledLeft(0, true);
		SetSlotTypeLeft(0, (int)ParserConnectionType.GENERAL);
		SetSlotColorLeft(0, new Color("red"));
	}

	public int GetAvailablePort(int candidate) => candidate;

	public ParserBase GetParser() => _matcher;
}
