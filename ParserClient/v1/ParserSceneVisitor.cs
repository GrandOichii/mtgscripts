using System;
using Godot;

public class ParserSceneVisitor {
    private readonly PackedScene _matcherScene;
    private readonly PackedScene _selectorScene;
    private readonly PackedScene _splitterScene;

    public ParserSceneVisitor(
        PackedScene matcherScene,
        PackedScene selectorScene,
        PackedScene splitterScene
    ) {
        _matcherScene = matcherScene;
        _selectorScene = selectorScene;
        _splitterScene = splitterScene;
    }

    public IParserBaseNode CreateScene(ParserBase parser) {
        return parser switch {
            Matcher matcher => CreateMatcherNode(matcher),
            Selector selector => CreateSelectorNode(selector),
            Splitter splitter => CreateSplitterNode(splitter),
            
            _ => throw new ArgumentException($"No packed scene available for instantiating graph node {parser.Name}")
        };
    }

    private IMatcherNode CreateMatcherNode(Matcher matcher) {
        var result = _matcherScene.Instantiate() as IMatcherNode;
        result.Load(matcher);
        return result;
    }

    private ISelectorNode CreateSelectorNode(Selector selector) {
        var result = _selectorScene.Instantiate() as ISelectorNode;
        result.Load(selector);
        return result;
    }

    private ISplitterNode CreateSplitterNode(Splitter splitter) {
        var result = _splitterScene.Instantiate() as ISplitterNode;
        result.Load(splitter);
        return result;
    }
}