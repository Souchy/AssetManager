using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.db;


public class Pack
{
    public string[] Meshes;
    public string[] AlbedoTextures;
    public string[] EmissiveTextures;
    public string[] Textures;
}

public partial class SyntyPack : Resource
{
    public string NameOrId { get; set; }
    public Array<Node3D> Meshes { get; set; } = new();
    // List of material collections
    public Array<MaterialCollection> MaterialCollections { get; set; } = new();
    public Array<Texture2D> Decals { get; set; } = new();
}

public partial class ModelCollection : Resource
{
    public string NameOrId { get; set; }
    public Array<Node3D> Meshes { get; set; } = new();
    public Array<MaterialCollection> MaterialCollections { get; set; } = new();
}

/// <summary>
/// Ex: can apply a random material from this collection. They're all alternatives that can apply to the same models
/// Ex créé une collection from [texture_01_A, texture_01_B, ...]
/// </summary>
public partial class MaterialCollection : Resource
{
    public MaterialCollection() { }
    public MaterialCollection(string name) => NameOrId = name;
    public string NameOrId {  get; set; }
    /// <summary>
    /// Also known as Alternatives
    /// </summary>
    public Array<Material> Materials { get; set; } = new();
}

public class SyntyPackSchemaMetadata
{
    // define how syntypack should be used
    // 
}