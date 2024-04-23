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
    // List of material collections
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

/// <summary>
/// Ex: can apply a random material from this collection. They're all alternatives that can apply to the same models
/// Ex créé une collection from [texture_01_A, texture_01_B, ...]
/// </summary>
internal partial class MaterialCollection : Resource
{
    /// <summary>
    /// Also known as Alternatives
    /// </summary>
    public Array<Material> Materials { get; set; } = new();
}