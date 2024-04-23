using AssetManager.caches;
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
        var albedoPath = textureFile.FullName;
        var tex = Pearls.Instance.LoadTexture2D(albedoPath);
        Texture2D emission = GetAssociatedEmissionTexture(albedoPath);

        return CreateMaterial(tex, emission);
    }

    public static Texture2D GetAssociatedEmissionTexture(string albedoPath)
    {
        var path = albedoPath[..^5] + "Emissive.png";
        if (File.Exists(path))
        {
            return Pearls.Instance.LoadTexture2D(path);
        }
        return null;
    }

    public static Material CreateMaterial(Texture2D albedo, Texture2D emission)
    {
        var mat = new StandardMaterial3D();
        mat.AlbedoTexture = albedo;
        mat.EmissionTexture = emission;
        Pearls.Instance.DefaultMaterial ??= mat;
        return mat;
    }

}
