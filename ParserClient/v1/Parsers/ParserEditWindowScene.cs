using Godot;
using System;
using System.Collections.Generic;

public partial class ParserEditWindowScene : Window
{
	#region Nodes
	
	public OptionButton TypeOptions { get; set; }
	public Node MatcherEdit { get; set; }
	public Node SelectorEdit { get; set; }
	public Node SplitterEdit { get; set; }
	public LineEdit NameEdit { get; set; }
	
	#endregion

	private Dictionary<string, int> _reverseTypeIndex = [];
	private Dictionary<string, Node> _editIndex;

	public override void _Ready()
	{
		#region Node fetching
		
		TypeOptions = GetNode<OptionButton>("%Type");
		MatcherEdit = GetNode<Node>("%MatcherEdit");
		SelectorEdit = GetNode<Node>("%SelectorEdit");
		SplitterEdit = GetNode<Node>("%SplitterEdit");
		NameEdit = GetNode<LineEdit>("%Name");
		
		#endregion

		for (int i = 0; i < TypeOptions.ItemCount; i++) {
			_reverseTypeIndex.Add(TypeOptions.GetItemText(i).ToLower(), i);
		}

		_editIndex = new() {
			{ "matcher", MatcherEdit },
			{ "selector", SelectorEdit },
			{ "splitter", SplitterEdit },
		};
	}

	public void Load(ParserBase parser) {
		// TODO
		Title = $"Editing parser {parser.Name}";
		NameEdit.Text = parser.Name;

		LoadType(parser.ParserType());
	}

	public void Unload() {
		Title = "New parser";

		LoadType("matcher");
	}

	private void LoadType(string type) {
		var idx = _reverseTypeIndex[type];
		TypeOptions.Select(idx);
	}
	
	private void Cancel() {
		// TODO
		Hide();
	}
	
	#region Signal connections
	
	public void OnSaveButtonPressed() {
		// TODO
	}
	
	public void OnCancelButtonPressed() {
		Cancel();
	}
	
	public void OnCloseRequested() {
		Cancel();
	}

	public void OnTypeItemSelected(int idx) {
		
	}
	
	#endregion
}
