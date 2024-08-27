using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;

public partial class ParserCanvasScene : Control
{
	#region Packed Scenes
	
	[Export]
	public PackedScene MatcherScene { get; set; }
	
	[Export]
	public PackedScene SelectorScene { get; set; }
	
	[Export]
	public PackedScene SplitterScene { get; set; }
	
	#endregion
	
	#region Nodes
	
	public GraphEdit Graph { get; private set; }
	public PopupMenu AddNodePopup { get; private set; }
	public PopupMenu TemplateMatchersPopup { get; private set; }
	public PopupMenu TemplateSelectorsPopup { get; private set; }
	public PopupMenu TemplateSplittersPopup { get; private set; }
	public PopupMenu EditNodePopup { get; private set; }
	public ParserEditWindowScene ParserEditWindow { get; set; }
	
	#endregion

	private ParserSceneVisitor _psv;
	private bool _editNode = false;
	
	public override void _Ready()
	{
		#region Node fetching
		
		Graph = GetNode<GraphEdit>("%Graph");
		AddNodePopup = GetNode<PopupMenu>("%AddNodePopup");
		TemplateMatchersPopup = GetNode<PopupMenu>("%TemplateMatchersPopup");
		TemplateSelectorsPopup = GetNode<PopupMenu>("%TemplateSelectorsPopup");
		TemplateSplittersPopup = GetNode<PopupMenu>("%TemplateSplittersPopup");
		EditNodePopup = GetNode<PopupMenu>("%EditNodePopup");
		ParserEditWindow = GetNode<ParserEditWindowScene>("%ParserEditWindow");
		
		#endregion

		#region Parsers
		
		var todo = new Matcher() {
			Name = "TODO",
			PatternString = ".+",
			Script = "function _Create(children) return '-- TODO' end"
		};

		var manaPip = new Matcher() {
			Name = "mana-pip",
			Script = File.ReadAllText("../scripts/mana-pip.lua"),
			PatternString = @"[W|U|B|R|G|C]",
		};

		var genericPip = new Matcher() {
			Name = "generic-pip",
			PatternString = @"[1-9]?[0-9]",
			Script = File.ReadAllText("../scripts/generic-pip.lua")
		};

		var tap = new Matcher() {
			Name = "tap",
			Script = File.ReadAllText("../scripts/tap.lua"),
			PatternString = @"\{T\}"
		};

		var pipSelector = new Selector() {
			Name = "pip-selector",
			Children = {
				manaPip,
				genericPip
			}
		};

		var pipSplitter = new Splitter() {
			Name = "pip-splitter",
			PatternString = @"\{|\}\{|\}",
			Script = File.ReadAllText("../scripts/pip-splitter.lua"),
			Children = {
				pipSelector
			}
		};

		var payLifeCost = new Matcher() {
			Name = "pay-life-cost",
			PatternString = "pay ([0-9]+) life",
			Script = File.ReadAllText("../scripts/pay-life-cost.lua")
		};

		var pipCost = new Matcher() {
			Name = "pip-cost",
			PatternString = @"\{(.+)\}",
			Script = File.ReadAllText("../scripts/pip-cost.lua"),
			Children = {
				pipSplitter
			}
		};

		var costSelector = new Selector() {
			Name = "cost-selector",
			Children = {
				pipCost,
				tap,
				payLifeCost
			}
		};

		var cost = new Splitter() {
			Name = "cost-splitter",
			PatternString = ", ",
			Script = File.ReadAllText("../scripts/cost-splitter.lua"),
			Children = {
				costSelector
			}
		};

		var addManaSplitter = new Splitter() {
			Name = "add-mana-splitter",
			PatternString = @" or |, or |, ",
			Script = File.ReadAllText("../scripts/add-mana-splitter.lua"),
			Children = {
				// TODO not mana pip
				pipSplitter
			}
		};

		var addMana = new Matcher() {
			Name = "add-mana",
			PatternString = @"add (.+)\.",
			Script = File.ReadAllText("../scripts/add-mana.lua"),
			Children = {
				addManaSplitter,
			}
		};

		var lifeGainEffect = new Matcher() {
			Name = "life-gain-effect",
			PatternString = "you gain ([0-9]+) life.",
			Script = File.ReadAllText("../scripts/life-gain.lua")
		};

		var effect = new Selector() {
			Name = "effect",
			Script = File.ReadAllText("../scripts/effect.lua"),
			Children = {
				addMana,
				lifeGainEffect
				// todo
			}
		};

		var activatedAbility = new Matcher() {
			Name = "activated-ability",
			PatternString = "(.+): (.+)",
			Script = File.ReadAllText("../scripts/activated-ability.lua"),
			Children = {
				cost,
				effect
			}
		};

		var parser = new Matcher(){
			Name = "root",
			PatternString = "(.+)",
			Script = File.ReadAllText("../scripts/root.lua"),
			Children = {
				activatedAbility
			}
		};

		#endregion

		_psv = new (
			MatcherScene,
			SelectorScene,
			SplitterScene
		);

		CreateSubmenus();

		Load(parser);
	}

	private void CreateSubmenus() {
		AddNodePopup.AddSubmenuNodeItem("Matchers", TemplateMatchersPopup);
		AddNodePopup.AddSubmenuNodeItem("Selectors", TemplateSelectorsPopup);
		AddNodePopup.AddSubmenuNodeItem("Splitters", TemplateSplittersPopup);
	}

	public void Load(ParserBase parser) {
		// TODO erase all previous nodes
		CreateParserNode(parser);

		Graph.ArrangeNodes();
		// Graph.CallDeferred("arrange_nodes");
	}

	public IParserBaseNode CreateParserNode(ParserBase parser) {
		var result = _psv.CreateScene(parser);
		var node = result.AsGraphNode();
		node.GuiInput += (e) => OnGraphNodeGuiInput(result, e);
		
		Graph.AddChild(node);

		var children = new List<IParserBaseNode>();
		for (int i = 0; i < parser.Children.Count; i++) {
			var childNode = parser.Children[i];
			
			var c = CreateParserNode(childNode);
			children.Add(c);
		}

		for (int i = 0; i < children.Count; i++) {
			var child = children[i];
			child.ConnectParent(Graph, result, i);
		}

		return result;
	}

	private Vector2 GraphMouseProjection() => (Graph.ScrollOffset + Graph.GetLocalMousePosition()) / Graph.Zoom;
	
	private void AddGraphNode() {
		// var child = new GraphNode();
		// Graph.AddChild(child);

		// var pos = GraphMouseProjection();
		// child.PositionOffset = pos;

		AddNodePopup.Position = (Vector2I)GetGlobalMousePosition();
		AddNodePopup.Show();
	}
	
	#region Signal connections
	
	public void OnGraphGuiInput(InputEvent e) {
		if (e.IsActionPressed("add-graph-node")) {
			if (_editNode) {
				_editNode = false;
				return;
			}
			AddGraphNode();
			return;
		}
	}
	
	public void OnAddNodePopupIdPressed(int id) {
		switch (id) {
			
		case 0:
			ParserEditWindow.Unload();
			ParserEditWindow.Show();
			break;
		default:
			throw new ArgumentException($"Unrecognized AddNodePopup id pressed: {id}");
			
		}
	}
	
	public void OnTemplateMatchersPopupIdPressed(int id) {
		// TODO
		GD.Print(id);
	}
	
	public void OnTemplateSelectorsPopupIdPressed(int id) {
		// TODO
		GD.Print(id);
	}
	
	public void OnTemplateSplittersPopupIdPressed(int id) {
		// TODO
		GD.Print(id);
	}
	
	public void OnEditNodePopupIdPressed(int id) {
		switch (id) {
			
		case 0:
			var node = ParserEditWindow.GetMeta("edited").As<Node>() as IParserBaseNode;
			ParserEditWindow.RemoveMeta("edited");
			ParserEditWindow.Load(node.GetParser());
			ParserEditWindow.Show();
			return;
		case 1:
			GD.Print("saving as template");
			return;
		default:
			throw new ArgumentException($"Unknown EditNodePopup id pressed: {id}");
		}
	}
	
	public void OnGraphNodeGuiInput(IParserBaseNode node, InputEvent e) {
		if (e.IsActionPressed("edit-graph-node")) {
			_editNode = true;
			ParserEditWindow.SetMeta("edited", node.AsGraphNode());
			EditNodePopup.Position = (Vector2I)GetGlobalMousePosition();
			EditNodePopup.Show();
			return;
		}
	}
	
	#endregion
}
