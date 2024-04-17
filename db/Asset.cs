using Godot;
using MongoDB.Bson;

namespace AssetManager.db;

public class Asset
{
    public ObjectId Id {  get; set; }
    public string Path { get; set; }
    public Texture2D Preview { get; set; }


}
