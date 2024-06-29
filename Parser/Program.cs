using System.Text.Json;
using Parser;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var card = new Card {
    Name = "Eastern Paladin",
    Text = "{B}{B}, {T}: Destroy target green creature.",
};

var manaPip = new Matcher() {
    Name = "mana-pip",
    Script = File.ReadAllText("mana-pip.lua"),
    PatternString = @"[W|U|B|R|G]",
};

var tap = new Matcher() {
    Name = "tap",
    Script = File.ReadAllText("tap.lua"),
    PatternString = @"\{T\}"
};

var pipSplitter = new Splitter() {
    Name = "pip-splitter",
    PatternString = @"{|}{|}",
    Children = {
        manaPip
    }
};

var costSelector = new Selector() {
    Name = "cost-selector",
    Children = {
        pipSplitter,
        tap
    }
};

var cost = new Splitter() {
    Name = "cost-splitter",
    PatternString = ", ",
    Script = File.ReadAllText("cost-splitter.lua"),
    Children = {
        costSelector
    }
};

var effect = new Matcher() {
    Name = "effect",
    PatternString = @".+",
    Script = "function _Create(children) return '.Effect(\\n-- TODO\\n)' end"
};

var activatedAbility = new Matcher() {
    Name = "activated-ability",
    PatternString = "(.+): (.+)",
    Script = File.ReadAllText("activated-ability.lua"),
    Children = {
        cost,
        effect
    }
};

var parser = new Matcher(){
    Name = "root",
    PatternString = "(.+)",
    Script = File.ReadAllText("root.lua"),
    Children = {
        activatedAbility
    }
};

var result = parser.Parse(card.Text);

var serializer = new SerializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();
var serialized = serializer.Serialize(result);

var script = result.CreateScript();

File.WriteAllText("result.lua", script);

File.WriteAllText("result.yaml", serialized);