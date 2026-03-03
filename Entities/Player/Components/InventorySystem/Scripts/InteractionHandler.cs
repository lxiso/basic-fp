using System;
using Godot;

[GlobalClass]
public partial class InteractionHandler : Area3D
{
    [Export] public ItemData[] ItemTypes {get; set;}
}