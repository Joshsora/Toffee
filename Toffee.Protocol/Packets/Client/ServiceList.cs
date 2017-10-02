/*
This file was generated by the ToffeeCompiler utility.
*/
using Toffee.Core;
using Toffee.Protocol.Objects;
using Toffee.Protocol.Definitions;
using Toffee.Protocol.Definitions.Attributes;
using System.CodeDom.Compiler;

namespace Toffee.Protocol.Packets.Client
{
	[GeneratedCode("ToffeeCompiler", "csharp")]	
	[ToffeeStructure("StdToffee")]
	public partial class ServiceList : IToffeeStructure
	{
		[ToffeeStructureProperty(1)]
		public ServiceOpen[] Services { get; set; }
		
		public void WriteTo(ToffeePacket packet)
		{
			packet.Write(Services);
		}
		
		public void ReadFrom(ToffeePacketIterator iterator)
		{
			Services = (ServiceOpen[])iterator.ReadArray();
		}
	}
}
