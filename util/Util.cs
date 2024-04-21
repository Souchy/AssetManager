using AssetManager.autoload;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;

namespace AssetManager.util;

internal static class Util
{
    public static void FbxToGltf(string folder)
    {
        bool force = false;
        //SelectedPath = CurrentDirectory + selectName;
        //var selectedDir = new DirectoryInfo(SelectedPath);
        var SelectedPath = folder;
        var selectedDir = new DirectoryInfo(SelectedPath);
        var newPath = ConfigGeneral.GeneratedPath.Replace("*", selectedDir.Name);

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
            psi.FileName = ConfigGeneral.fbx2gltf;
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

}
