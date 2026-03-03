using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string itemName;
    [Export] public PackedScene itemScene;
}