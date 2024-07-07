using Godot;
using System;

public partial class DashItem : PickupArea2D
{
	protected override void ApplyPickupEffect(Player player)
	{
		//player.DashEnabled = true;
		GD.Print("Dash enabled!");
	}
}
