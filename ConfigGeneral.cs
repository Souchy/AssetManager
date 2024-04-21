using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.json;

namespace AssetManager;

public class ConfigGeneral : Config
{
    #region Configuration
    public const string fbx2gltf = "G:/Downloads/FBX2glTF-windows-x86_64.exe";
    public const string GeneratedPath = "*__AS_GEN_GLTF";
    public const string Mansion = "G://Assets/pack/HumbleBundle/Synty/BUNDLE_1/POLYGON_Horror_CurrentDirectory_SourceFiles/SourceFiles";
    public string selectName = "FBX";
    #endregion

    #region Paths
    public string CurrentDirectory { get; set; }
    public string SelectedPath { get => SelectedPaths[0]; set => SelectedPaths = [value]; }
    public string[] SelectedPaths { get; set; } = new string[1];
    #endregion

}

