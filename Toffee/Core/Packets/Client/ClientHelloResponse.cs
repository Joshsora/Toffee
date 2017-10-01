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
	public partial class ClientHelloResponse : IToffeeStructure
	{
		[ToffeeStructureProperty(1)]
		public bool Success { get; set; }
		
		[ToffeeStructureProperty(2)]
		public byte ErrorCode { get; set; }
		
		[ToffeeStructureProperty(3)]
		public string Message { get; set; }
		
		[ToffeeStructureProperty(4)]
		public uint HeartbeatTime { get; set; }
		
		[ToffeeStructureProperty(5)]
		public bool ForcedEncryption { get; set; }
		
		public void WriteTo(ToffeePacket packet)
		{
			packet.Write(Success);
			packet.Write(ErrorCode);
			packet.Write(Message);
			packet.Write(HeartbeatTime);
			packet.Write(ForcedEncryption);
		}
		
		public void ReadFrom(ToffeePacketIterator iterator)
		{
			Success = iterator.ReadBoolean();
			ErrorCode = iterator.ReadUInt8();
			Message = iterator.ReadString();
			HeartbeatTime = iterator.ReadUInt32();
			ForcedEncryption = iterator.ReadBoolean();
		}
	}
}
