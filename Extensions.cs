using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace AssetManager;

internal static class Extensions
{
    public static Array<T> GetChildrenOfType<[MustBeVariant] T>(this Node node, Array<T> container = null) where T : Node
    {
        container ??= [];
        var children = node.GetChildren();
        container.AddRange(children.Where(c => c is T).Select(c => c as T));
        foreach (var child in children)
            child.GetChildrenOfType<T>(container);
        return container;
    }
}
