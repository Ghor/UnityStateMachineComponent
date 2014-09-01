using UnityEngine;

namespace States {
	// The base class for all states. Derive from this.
	public class State
	{
		//
		// Default constructor.
		//
		public State()
		{
		}
		
		//
		// A reference to our state machine.
		//
		public StateMachine outer;
		
		//
		// Our gameobject.
		//
		protected GameObject gameObject
		{
			get { return outer.gameObject; }
		}
		
		//
		// Some component access shortcuts.
		//
		protected Transform transform { get { return gameObject.transform; } }
		protected Rigidbody rigidbody { get { return gameObject.rigidbody; } }
		
		//
		// Passthrough methods.
		//
		protected static void Destroy( Object obj )
		{
			Object.Destroy( obj );
		}
		protected T GetComponent<T>() where T:Component { return gameObject.GetComponent<T>(); }
		protected Component GetComponent( string type ) { return gameObject.GetComponent( type ); }
		protected Component GetComponent( System.Type type ) { return gameObject.GetComponent( type ); }
		
		protected T AddComponent<T>() where T:Component { return gameObject.AddComponent<T>(); }
		protected Component AddComponent( string type ) { return gameObject.AddComponent( type ); }
		protected Component AddComponent( System.Type type ) { return gameObject.AddComponent( type ); }
		
		//
		// Messages.
		//
		public virtual void OnEnter()
		{
		}
		public virtual void OnExit( State nextState )
		{
		}
		public virtual void Update()
		{
		}
		public virtual void FixedUpdate()
		{
		}
		
		//
		// Constructors. Right now, these are a remnant from the attempts to derive this class from ScriptableObject, which could not be constructed directly and needed a more convenient wrapper.
		//
		public static T NewState<T>() where T:State, new()
		{
			return new T();
		}
		
		public static State NewState( System.Type type )
		{
			if ( type.IsSubclassOf( typeof( State ) ) )
			{
				return (State) System.Activator.CreateInstance( type );
			}
			return null;
		}
	}
}