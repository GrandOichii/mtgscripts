function _Create(children)
    return string.format('.Activated(ActivatedAbility()\n%s\n%s\n.Build())', children[1], children[2])
end