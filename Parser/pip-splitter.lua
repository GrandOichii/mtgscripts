function _Create(children)
    local result = ''
    if #children == 1 then
        return children[1]
    end
    for i, child in ipairs(children) do
        print(child == '')
        if child ~= '' then
            if i ~= 1 then
                result = result..',\n'
            end
            result = result..child
        end
    end
    return 'Mana.And(\n'..result..'\n)'
end