/*
This file was generated by the ToffeeCompiler utility.
*/
using Toffee.Core;
using Toffee.Core.Objects;
using Toffee.Core.Definitions;
using Toffee.Core.Definitions.Attributes;
using System.CodeDom.Compiler;

namespace Toffee.Core.Packets.Client
{
	[GeneratedCode("ToffeeCompiler", "csharp")]	
	[ToffeeStructure("StdToffee")]
	public partial class ClientHello : IToffeeStructure
	{
		[ToffeeStructureProperty(1)]
		public string AppName { get; set; }
		
		[ToffeeStructureProperty(2)]
		public string AppVersion { get; set; }
		
		[ToffeeStructureProperty(3)]
		public uint NetworkHash { get; set; }
		
		[ToffeeStructureProperty(4)]
		public bool HasEncryption { get; set; }
		
		public void WriteTo(ToffeePacket packet)
		{
			packet.Write(AppName);
			packet.Write(AppVersion);
			packet.Write(NetworkHash);
			packet.Write(HasEncryption);
		}
		
		public void ReadFrom(ToffeePacketIterator iterator)
		{
			AppName = iterator.ReadString();
			AppVersion = iterator.ReadString();
			NetworkHash = iterator.ReadUInt32();
			HasEncryption = iterator.ReadBoolean();
		}
	}
}