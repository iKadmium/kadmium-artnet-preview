//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.IO;

public class ArtNetPacket
{
	public string Identifier {get;set;}
	public UInt16 OpCode {get;set;}
	public UInt16 Version {get;set;}

	public byte Sequence {get;set;}
	public byte Physical {get;set;}

	public UInt16 Universe {get;set;}

	public UInt16 Length {get;set;}

	public byte[] Data {get;set;}

	public ArtNetPacket (byte[] data)
	{
		MemoryStream stream = new MemoryStream(data, false);
		BinaryReader reader = new BinaryReader(stream);
		Identifier = new string(reader.ReadChars(8));

		OpCode = reader.ReadUInt16();
		Version = reader.ReadUInt16();

		Sequence = reader.ReadByte();
		Physical = reader.ReadByte();
		Universe = reader.ReadUInt16();

		byte lengthHi = reader.ReadByte();
		byte lengthLo = reader.ReadByte();
		Length = (UInt16)((lengthHi << 8) + lengthLo);

		Data = reader.ReadBytes(Length);
	}
}
