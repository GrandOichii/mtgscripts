function _Create()
return Card.Card()
.Activated(ActivatedAbility()
	.Cost(
						Cost.Tap
	)
	.Effect(
			Effect.GainLife(1)
	)
.Build())
.Build()
end