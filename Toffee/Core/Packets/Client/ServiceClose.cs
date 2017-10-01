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
	public partial class ServiceClose : IToffeeStructure
	{
		[ToffeeStructureProperty(1)]
		public uint DistributedId { get; set; }
		
		[ToffeeStructureProperty(2)]
		public byte ErrorCode { get; set; }
		
		[ToffeeStructureProperty(3)]
		public string Message { get; set; }
		
		public void WriteTo(ToffeePacket packet)
		{
			packet.Write(DistributedId);
			packet.Write(ErrorCode);
			packet.Write(Message);
		}
		
		public void ReadFrom(ToffeePacketIterator iterator)
		{
			DistributedId = iterator.ReadUInt32();
			ErrorCode = iterator.ReadUInt8();
			Message = iterator.ReadString();
		}
	}
}
