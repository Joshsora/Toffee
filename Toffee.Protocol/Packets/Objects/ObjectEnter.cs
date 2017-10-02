/*
This file was generated by the ToffeeCompiler utility.
*/
using Toffee.Core;
using Toffee.Protocol.Objects;
using Toffee.Protocol.Definitions;
using Toffee.Protocol.Definitions.Attributes;
using System.CodeDom.Compiler;

namespace Toffee.Protocol.Packets.Objects
{
	[GeneratedCode("ToffeeCompiler", "csharp")]	
	[ToffeeStructure("StdToffee")]
	public partial class ObjectEnter : IToffeeStructure
	{
		[ToffeeStructureProperty(1)]
		public uint DistributedId { get; set; }
		
		[ToffeeStructureProperty(2)]
		public uint ObjectType { get; set; }
		
		[ToffeeStructureProperty(3)]
		public FieldUpdate[] CurrentState { get; set; }
		
		public void WriteTo(ToffeePacket packet)
		{
			packet.Write(DistributedId);
			packet.Write(ObjectType);
			packet.Write(CurrentState);
		}
		
		public void ReadFrom(ToffeePacketIterator iterator)
		{
			DistributedId = iterator.ReadUInt32();
			ObjectType = iterator.ReadUInt32();
			CurrentState = (FieldUpdate[])iterator.ReadArray();
		}
	}
}