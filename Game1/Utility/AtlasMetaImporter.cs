using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    public class AtlasMetaImporter
    {
        public static Dictionary<short, Rectangle> NewImportTileMetadata(string path)
        {
            var meta = new Dictionary<short, Rectangle>();

            using (StreamReader sr = File.OpenText(path))
            {
                String line;
                while (true)
                {
                    // key
                    line = sr.ReadLine();

                    if (line == null)
                        break;
                    string key = line;
                    // rect
                    line = sr.ReadLine();
                    string[] rect = line.Trim(' ', '\t').Split(',');
                    meta.Add(short.Parse(key), new Rectangle(int.Parse(rect[0]), int.Parse(rect[1]), int.Parse(rect[2]), int.Parse(rect[3])));
                    // origin
                    line = sr.ReadLine();
                }
            }

            return meta;
        }
    }
}
