using AssetManager.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.caches;

/// <summary>
/// Database cache
/// </summary>
internal class Diamonds
{
    public static List<SyntyPack> Packs { get; set; } = new();

    static Diamonds()
    {

    }

}
