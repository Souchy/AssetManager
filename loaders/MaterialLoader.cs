using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.loaders;
internal static class MaterialLoader
{
    public static Material loadMaterial(FileInfo textureFile) //string texture)
    {
        var path = textureFile.FullName;
        // TODO load materials
        var mat = new StandardMaterial3D();
        var img = Image.LoadFromFile(path);
        var tex = ImageTexture.CreateFromImage(img);
        mat.AlbedoTexture = tex;

        var emi = path[..^5] + "Emissive.png";
        if (File.Exists(emi))
        {
            var imgemi = Image.LoadFromFile(emi);
            var texemi = ImageTexture.CreateFromImage(imgemi);
            mat.EmissionTexture = texemi;
        }
        return mat;
    }

}
