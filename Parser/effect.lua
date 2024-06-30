function _Create(children)
    local result = '.Effect(\n'
    for _, child in ipairs(children) do
        if children ~= '' then
            return result..child..'\n)'
        end
    end
end