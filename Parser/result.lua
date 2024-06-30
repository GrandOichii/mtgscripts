function _Create()
return Card.Card()
.Activated(ActivatedAbility()
	.Cost(
						Cost.Tap
	)
	.Effect(
			Effect.AddMana(
						Mana.Or(
										Mana.Green
						,
										Mana.White
						,
										Mana.Blue
						)
			)
	)
.Create())
.Create()
end