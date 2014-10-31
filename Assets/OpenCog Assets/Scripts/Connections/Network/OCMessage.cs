
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using IConvertible = System.IConvertible;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Network
{

/// <summary>
/// The OpenCog Message.  Subclasses that extend fromit can obtain instances
/// via factory method.  @TODO: is this supposed to be a class cluster?  
/// Something fishy about all this...
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCMessage : IConvertible 
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	/// <summary>
	/// The ID of source OCNetworkElement.
	/// </summary>
	private string _sourceID;
		
	/// <summary>
	/// The ID of the target OCNetworkElement.
	/// </summary>
	private string _targetID;
		
	/// <summary>
	/// The type of the message.
	/// </summary>
	private MessageType _type;
		
	private string _content;
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public string SourceID 
	{
		get { return this._sourceID;}
		set { _sourceID = value;}
	}

	public string TargetID 
	{
		get { return this._targetID;}
		set { _targetID = value;}
	}

	public MessageType Type 
	{
		get { return this._type;}
		set { _type = value;}
	}
		
	public string MessageContent
	{
		get{ return _content; }
		set{ _content = value; }
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------
		
	/// <summary>
	/// Creates the message.
	/// </summary>
	/// <returns>
	/// The message.
	/// </returns>
	/// <param name='sourceID'>
	/// Source I.
	/// </param>
	/// <param name='targetID'>
	/// Target I.
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	/// <param name='message'>
	/// Message.
	/// </param>
	public static OCMessage CreateMessage(string sourceID, string targetID, MessageType type, string message = "")
	{
		return new OCMessage(sourceID, targetID, type, message);
	}

	new public string ToString()
	{
		return _content;
	}
	
	public void FromString(string message)
	{
		_content = message;
	}

	#region IConvertible implementation
	public System.TypeCode GetTypeCode ()
	{
		throw new System.NotImplementedException ();
	}

	public bool ToBoolean (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public char ToChar (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public sbyte ToSByte (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public byte ToByte (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public short ToInt16 (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public ushort ToUInt16 (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public int ToInt32 (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public uint ToUInt32 (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public long ToInt64 (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public ulong ToUInt64 (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public float ToSingle (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public double ToDouble (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public decimal ToDecimal (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public System.DateTime ToDateTime (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public string ToString (System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}

	public object ToType (System.Type conversionType, System.IFormatProvider provider)
	{
		throw new System.NotImplementedException ();
	}
		
	#endregion
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		
		
	public OCMessage (string sourceID, string targetID, MessageType type, string messageContents)
	{
		_sourceID = sourceID;
		_targetID = targetID;
		_type = type;
		_content = messageContents;
	}
		
	/// <summary>
	/// Message types definition.
	/// </summary>
	public enum MessageType : int
	{
		NONE = 0
	, STRING = 1
	, LEARN = 2
	, REWARD = 3
	, SCHEMA = 4
	, LS_CMD = 5
	, ROUTER = 6
	, CANDIDATE_SCHEMA = 7
	, TICK = 8
	, FEEDBACK = 9
	, TRY = 10
	, STOP_LEARNING = 11
	/// <summary>
	/// A custom message type for test purposes, to be removed.
	/// </summary>
	, RAW = 12
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCMessage

}// namespace OpenCog.Network




