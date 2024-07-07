using Godot;
using System;

public partial class DoubleJumpItem : PickupArea2D
{
	protected override void ApplyPickupEffect(Player player)
	{
		player.DoubleJumpEnabled = true;
		GD.Print("Double Jump enabled!");
	}
}
