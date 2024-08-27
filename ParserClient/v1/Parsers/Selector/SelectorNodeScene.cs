using Godot;
using System;

public partial class SelectorNodeScene : GraphNode, ISelectorNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public GraphNode AsGraphNode() => this;

	public void Load(Selector selector)
	{
		Title = selector.Name;
		
		for (int i = 0; i < selector.Children.Count; i++) {
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
}
