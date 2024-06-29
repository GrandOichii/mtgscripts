using NLua;
using YamlDotNet.Serialization;

namespace Parser;

public enum ParseResultStatus {
    SUCCESS = 0,
    DIDNT_MATCH,
    CHILD_FAILED,
    ALL_CHILDREN_FAILED
}

public class ParseResult {
    [YamlIgnore]
    public Parser Parent { get; }
    // public readonly Parser Parent;
    public string ParentName { get; }
    public ParseResultStatus Status { get; }
    public List<ParseResult> Children { get; }
    public string Text { get; }

    public ParseResult(Parser parent, ParseResultStatus status, string text) {
        Text = text;
        Parent = parent;
        ParentName = parent.Name;
        Status = status;
        Children = new();
    }

    public ParseResult(Parser parent, ParseResultStatus status, string text, List<ParseResult> children) : this(parent, status, text) {
        Children = children;
    }

    public override string ToString()
    {
        var result = $"{Parent.Name}\tStatus: {Status}";
        foreach (var child in Children) {
            result += $"\n[\n{child}\n]";
        }
        return result;
    }

    public string CreateScript() {
        var state = new Lua();
        return CreateScript(state);
    }

    public string CreateScript(Lua state, int indent = 0) {
        if (Status != ParseResultStatus.SUCCESS) return string.Empty;
        
        var children = new List<string>();
        foreach (var child in Children) {
            var s = child.CreateScript(state, indent + 1);
            var split = s.Split("\n");
            var indented = "";
            var indentString = new string('\t', indent + 1);
            foreach (var line in split)
                if (!string.IsNullOrEmpty(line))
                    indented += indentString + line + "\n";
            children.Add(indented);
        }
        var childrenTable = LuaUtility.CreateTable(state, children);

        state.DoString(Parent.Script);
        var creationF = LuaUtility.GetGlobalF(state, "_Create");
        var returned = CallCreationFunc(state, creationF, childrenTable);

        return LuaUtility.GetReturnAs<string>(returned);
    }

    public virtual object[] CallCreationFunc(Lua state, LuaFunction func, LuaTable childrenTable) => func.Call(childrenTable);
}