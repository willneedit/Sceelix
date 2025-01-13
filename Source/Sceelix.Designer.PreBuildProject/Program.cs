using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace Sceelix.Designer.PreBuildProject
{
    internal class Program
    {
        static void PackToZip(string dir)
        {
            string targetZip = $"{dir}.zip";

            if(File.Exists(targetZip)) File.Delete(targetZip);
            ZipFile.CreateFromDirectory(dir, targetZip);

        }

        static int Main(string[] args)
        {
            if (args.Length < 1) return 100;

            PackToZip($"{args[0]}/../Extras/API");
            PackToZip($"{args[0]}/../Extras/Samples");
            PackToZip($"{args[0]}/../Extras/Unity Plugin");

            return 0;
        }
    }
}
