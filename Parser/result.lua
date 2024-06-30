function _Create()
return Card.Card()
.Activated(ActivatedAbility()
	.Cost(
						Cost.Mana(
																					Mana.Green
						)
	)
	.Cost(
						Cost.Tap
	)
	.Effect(
			Effect.AddMana(
						Mana.Or(
										Mana.And(
																					Mana.Black
										,
																					Mana.Red
										)
						,
																					Mana.Colorless
						)
			)
	)
.Create())
.Create()
end