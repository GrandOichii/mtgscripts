function _Create(children)
    local result = ''
    for i, child in ipairs(children) do
        if child ~= '' then
            if i ~= 1 then
                result = result..',\n'
            end
            result = result..child
        end
    end
    return 'Mana.Or(\n'..result..'\n)'
end