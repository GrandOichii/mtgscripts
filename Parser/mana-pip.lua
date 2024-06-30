function _Create(children, data)
    assert(#children == 0, 'Expected children count of <mana-pip> to be 0')
    assert(#data == 1, 'Expected data count of <mana-pip> to be 1')
    local colorMap = {
        W = 'White',
        U = 'Blue',
        B = 'Black',
        R = 'Red',
        G = 'Green',
    }

    return 'Mana.'..colorMap[data[1]]
end