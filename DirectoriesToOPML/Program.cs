using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DirectoriesToOpml
{
    internal class Program
    {
        private static StringBuilder _opml = new StringBuilder();
        private static bool _isRoot = true;
        private static List<string> _notPrintExtensions = new List<string>();

        public static void DirectoryToOpml(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            if (_isRoot)
            {
                _opml.Append($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<opml version=\"2.0\">\n<head><title>{dirInfo.Name}</title></head>\n<body>\n");
                _isRoot = false;
            }
            
            FileSystemInfo[] subDirsAndFilesInfo = dirInfo.GetFileSystemInfos();

            foreach (FileSystemInfo FSInfo in subDirsAndFilesInfo)
            {
                if (FSInfo is DirectoryInfo)
                {
                    _opml.Append($"<outline text=\"{FSInfo.Name}\">\n");
                    DirectoryToOpml(FSInfo.FullName);
                    _opml.Append("</outline>\n");
                }
                else
                {
                    foreach (string ex in _notPrintExtensions)
                    {
                        if (ex == "" || !FSInfo.Name.EndsWith(ex))
                        {
                            _opml.Append($"<outline text=\"{FSInfo.Name}\">\n");
                            _opml.Append("</outline>\n");
                        }
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Please input the directory:");
            string targetDir = Console.ReadLine();
            Console.WriteLine("Please input the extensions not printed with spaces as gaps(for example: .sln .meta):");
            _notPrintExtensions = new List<string>(Console.ReadLine().Split(' '));
            DirectoryToOpml(targetDir);
            _opml.Append("</body></opml>\n");
            StreamWriter writer = new StreamWriter("out.opml");
            using (writer)
            {
                writer.Write(_opml.ToString());
            }
        }
    }
}