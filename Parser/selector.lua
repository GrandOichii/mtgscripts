function _Create(children)
    for _, child in ipairs(children) do
        if child ~= '' then
            return child
        end
    end
end