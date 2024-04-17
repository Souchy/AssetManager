using MongoDB.Bson;
using System.Collections.Generic;

namespace AssetManager.db;

public class AssetCollection
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public List<ObjectId> Assets { get; set; }
}
