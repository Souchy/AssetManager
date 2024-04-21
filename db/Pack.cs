using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.db;


internal partial class SyntyPack : Resource
{
    public Array<Node3D> Meshes { get; set; } = new();
    //public Array<Material> Materials { get; set; } = new();
    public Array<MaterialCollection> Materials { get; set; } = new();
    public Array<Texture2D> Decals { get; set; } = new();
}

internal class Pack
{
    public string[] Meshes;
    public string[] AlbedoTextures;
    public string[] EmissiveTextures;
    public string[] Textures;
}

internal partial class MaterialCollection : Resource
{
    public Array<Material> Alternatives { get; set; } = new();
}