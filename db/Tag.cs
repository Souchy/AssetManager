using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AssetManager.db;

public record Tag(string Name)
{

    public override string ToString()
    {
        return Name.ToString();
    }

    public static implicit operator Tag(string name) => new Tag(name);
    public static implicit operator string(Tag tag) => tag.ToString();

}
