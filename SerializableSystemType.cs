using UnityEngine;

// Exactly as the name implies, this acts as a version of 'System.Type' which can be serialized by Unity.
[System.Serializable]
public class SerializableSystemType
{
	// The type is internally serialized as a string.
	[SerializeField]
	[HideInInspector]
	private string typeAsString;
	
	// Setting or getting the type described by this object is done through this property.
	public System.Type type {
		set { typeAsString = (( value != null ) ? value.AssemblyQualifiedName : null ); }
		get { return ( typeAsString != null ) ? System.Type.GetType( typeAsString ) : null; }
	}
	
	// Implicit conversion for SerializableSystemType -> System.Type.
	public static implicit operator System.Type( SerializableSystemType obj )
	{
		return ( obj != null ) ? obj.type : null;
	}
	
	// Implicit conversion for System.Type -> SerializableSystemType.
	public static implicit operator SerializableSystemType( System.Type type )
	{
		if ( type == null )
			return null;
		SerializableSystemType obj = new SerializableSystemType();
		obj.type = type;
		return obj;
	}
}