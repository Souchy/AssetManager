using Godot;
using MongoDB.Bson;
using System.Collections.Generic;
using Util.json;

namespace AssetManager.db;

public class Asset
{
    //public ObjectId Id {  get; set; }
    public string Path { get; set; }
    //public Texture2D Preview { get; set; }

    public HashSet<string> Tags { get; set; } = new();

}

public class AssetFolder : Config
{
    public static AssetFolder CurrentFolder {  get; set; }

    public List<Asset> Assets { get; set; } = new();
}

