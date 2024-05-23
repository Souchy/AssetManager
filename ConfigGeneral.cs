using AssetManager.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.json;

namespace AssetManager;

public class ConfigGeneral : Config
{
    public static ConfigGeneral Instance { get; private set; }

    public ConfigGeneral()
    {
        Instance = this;
    }

    #region Configuration
    public string fbx2gltf { get; set; } = "G:/Downloads/FBX2glTF-windows-x86_64.exe";
    public string GeneratedPath { get; set; } = "*__AS_GEN_GLTF";
    #endregion

    #region Paths
    public string CurrentDirectory { get; set; }
    public string[] SelectedPaths { get; set; } = new string[1];
    public string SelectedPath { get => SelectedPaths[0]; set => SelectedPaths = [value]; }
    #endregion

    public List<Workspace> Workspaces { get; set; } = new();
    public Workspace CurrentWorkspace { get; set; } = new()
    {
        Name = "Synty",
        Path = "G:\\Assets\\pack\\HumbleBundle\\Synty\\"
    };

    public List<Tag> Tags { get; set; } = [
        "Wall",
        "Floor",
        "Door",
        "Window",
        "Decoration",
        "OnTheWall", // between 0.4 and 0.8 of the wall height by default
        "OnTheFloor", // can 
        "Collision",
        "Light"
    ];

}

public class Workspace
{
    public string Name { get; set; }
    public string Path { get; set; }
}