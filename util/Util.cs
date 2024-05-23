using AssetManager.autoload;
using AssetManager.caches;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;

namespace AssetManager.util;

internal static class Util
{
    public static void FbxToGltf(ConfigGeneral config) //string folder)
    {
        string folder = config.SelectedPath;
        bool force = false;
        //SelectedPath = CurrentDirectory + selectName;
        //var selectedDir = new DirectoryInfo(SelectedPath);
        var SelectedPath = folder;
        var selectedDir = new DirectoryInfo(SelectedPath);
        var newPath = config.GeneratedPath.Replace("*", selectedDir.Name);

        GD.Print("ON GLTF");

        var fbxDir = folder;
        var gltfDir = "";

        var fbx_files = Directory.GetFiles(SelectedPath, "*.fbx", SearchOption.AllDirectories);

        for (int i = 0; i < fbx_files.Length; i++)
        {
            var input = fbx_files[i];
            string output = input.Replace($"\\{selectedDir.Name}\\", $"\\{newPath}\\").Replace(".fbx", ".glb");
            if (File.Exists(output) && !force)
            {
                //LblStatusValue.Text = (i + 1) + " / " + fbx_files.Length;
                EventBus.centralBus.publish(UiStatusBar.EventStatusValue, i + 1 + " / " + fbx_files.Length);
                continue;
            }
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = config.fbx2gltf;
            psi.ArgumentList.Add("-b");
            psi.ArgumentList.Add("-i");
            psi.ArgumentList.Add(input);
            psi.ArgumentList.Add("-o");
            psi.ArgumentList.Add(output);
            var process = Process.Start(psi);
            //process.WaitForExit();

            //LblStatusValue.Text = (i + 1) + " / " + fbx_files.Length;
            EventBus.centralBus.publish(UiStatusBar.EventStatusValue, i + 1 + " / " + fbx_files.Length);
            //string stdout = process.StandardOutput.ReadToEnd(); 
            //Process.Start(fbx2gltf, $"-b -i {input} -o {output}");
        }
    }

    public static void ExportMeshes()
    {
        int meshCount = 0;
        int itemCount = 0;
        foreach (var i in Pearls.Instance.CurrentItems)
        {
            if(i is not item_3d)
                continue;
            item_3d item = (item_3d) i;
            var mis = item.Model.GetChildrenOfType<MeshInstance3D>();
            foreach (var mi in mis)
            {
                var mesh = mi.Mesh;
                var resname = mesh.ResourceName;
                mesh.ResourceName = mi.Name;
                string mipath = ConfigGeneral.Instance.CurrentDirectory + "/" + mi.Name + ".glb";
                string meshpath = ConfigGeneral.Instance.CurrentDirectory + "/" + mi.Name + ".tres";
                //Directory.CreateDirectory(ConfigGeneral.Instance.CurrentDirectory.Replace(ConfigGeneral.Instance.GeneratedPath, "meshes"));
                //Directory.CreateDirectory(ConfigGeneral.Instance.CurrentDirectory += "/mesh");
                var arrayMesh = mesh as ArrayMesh;
                if(arrayMesh != null)
                {
                    var shadowmesh = arrayMesh.ShadowMesh;
                    //string path = ConfigGeneral.Instance.CurrentDirectory + "/" + mi.Name + ".mesh";
                    ResourceSaver.Save(arrayMesh, meshpath, ResourceSaver.SaverFlags.None);

                    //var gltf_document_save = new GltfDocument();
                    //var gltf_state_save = new GltfState();
                    //gltf_document_save.AppendFromScene(mi, gltf_state_save);
                    ////# The file extension in the output `path` (`.gltf` or `.glb`) determines
                    ////# whether the output uses text or binary format.
                    ////# `GLTFDocument.generate_buffer()` is also available for saving to memory.
                    //gltf_document_save.WriteToFilesystem(gltf_state_save, mipath);
                }
                //var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Write);
                //file.StoreVar(mesh);
                //ResourceSaver.Save(mesh, path, ResourceSaver.SaverFlags.None);

                meshCount++;
            }
            itemCount++;
        }
        EventBus.centralBus.publish(UiStatusBar.EventStatusValue, $"{itemCount} nodes, {meshCount}");
    }

}
