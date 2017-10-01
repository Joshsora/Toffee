using System.IO;
using System.Collections.Generic;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlFile : TdlNamespace
    {
        private string _NetworkName;
        public string NetworkName
        {
            get
            {
                return _NetworkName;
            }

            set
            {
                RemoveStringFromHash(NetworkName);
                _NetworkName = value;
                AddStringToHash(NetworkName);
            }
        }
        public uint Hash { get; internal set; }
        private bool ParserVerbosity { get; set; }

        private List<TdlObject> _AllObjects;
        public TdlObject[] AllObjects
        {
            get
            {
                return _AllObjects.ToArray();
            }
        }

        private List<TdlStruct> _AllStructs;
        public TdlStruct[] AllStructs
        {
            get
            {
                return _AllStructs.ToArray();
            }
        }

        private List<TdlClass> _AllClasses;
        public TdlClass[] AllClasses
        {
            get
            {
                return _AllClasses.ToArray();
            }
        }

        private List<TdlService> _AllServices;
        public TdlService[] AllServices
        {
            get
            {
                return _AllServices.ToArray();
            }
        }

        private Stack<string> DirectoryStack { get; set; }

        public TdlFile(string identifier, bool parserVerbosity) : base (identifier)
        {
            ParserVerbosity = parserVerbosity;
            NetworkName = "";
            _AllObjects = new List<TdlObject>();
            _AllStructs = new List<TdlStruct>();
            _AllClasses = new List<TdlClass>();
            _AllServices = new List<TdlService>();
            DirectoryStack = new Stack<string>();
        }

        public bool ParseFile(string filepath)
        {
            if (DirectoryStack.Count > 0)
                filepath = Path.Combine(DirectoryStack.Peek(), filepath);
            if (!File.Exists(filepath))
                return false;
            string directory = Path.GetDirectoryName(filepath);
            DirectoryStack.Push(directory);

            Scanner scanner = new Scanner(File.Open(filepath, FileMode.Open));
            Parser parser = new Parser(this, ParserVerbosity, scanner);
            bool result = parser.Parse();
            DirectoryStack.Pop();
            return result;
        }

        internal void AddStruct(TdlStruct tdlStruct)
        {
            _AllObjects.Add(tdlStruct);
            _AllStructs.Add(tdlStruct);
        }

        internal void AddClass(TdlClass tdlClass)
        {
            _AllObjects.Add(tdlClass);
            _AllClasses.Add(tdlClass);
        }

        internal void AddService(TdlService tdlService)
        {
            _AllObjects.Add(tdlService);
            _AllServices.Add(tdlService);
        }

        internal void AddStringToHash(string str)
        {
            if (str == null)
                return;
            Hash += (uint)str.Length;
            foreach (char c in str)
                Hash += (byte)c;
        }

        private void RemoveStringFromHash(string str)
        {
            if (str == null)
                return;
            Hash -= (uint)str.Length;
            foreach (char c in str)
                Hash -= (byte)c;
        }
    }
}
