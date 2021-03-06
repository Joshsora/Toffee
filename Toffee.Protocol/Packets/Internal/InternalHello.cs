/*
This file was generated by the ToffeeCompiler utility.
*/
using Toffee.Core;
using Toffee.Protocol.Objects;
using Toffee.Protocol.Definitions;
using Toffee.Protocol.Definitions.Attributes;
using System.CodeDom.Compiler;

namespace Toffee.Protocol.Packets.Internal
{
	[GeneratedCode("ToffeeCompiler", "csharp")]	
	[ToffeeStructure("StdToffee")]
	public partial class InternalHello : IToffeeStructure
	{
		[ToffeeStructureProperty(1)]
		public string Name { get; set; }
		
		[ToffeeStructureProperty(2)]
		public string Role { get; set; }
		
		[ToffeeStructureProperty(3)]
		public uint Channel { get; set; }
		
		public void WriteTo(ToffeePacket packet)
		{
			packet.Write(Name);
			packet.Write(Role);
			packet.Write(Channel);
		}
		
		public void ReadFrom(ToffeePacketIterator iterator)
		{
			Name = iterator.ReadString();
			Role = iterator.ReadString();
			Channel = iterator.ReadUInt32();
		}
	}
}
