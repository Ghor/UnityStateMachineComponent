using UnityEngine;
using States;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StateMachine : MonoBehaviour {
	
	//
	// The current state.
	//
	public State state;
	
	//------------------------------------------------//
	// Note about difficulties serializing the state: //
	//------------------------------------------------//
	// - Unfortunately, Unity's serialization system will serialize subclasses as the class they are referenced as, so we can't just serialize the state object itself.
	// - Inheriting from ScriptableObject allows the class of a serialized object to be retained, but ScriptableObject subclass references are lost when making objects into prefabs due to the way garbage is collected.
	// - So, the most reasonable solution at this time is to serialize the type of the state and instantiate it in 'Start'.
	public SerializableSystemType initialStateType;
	
	//
	// Message forwarding.
	//
	void Update ()
	{
		state.Update();
	}
	
	void FixedUpdate()
	{
		state.FixedUpdate();
	}
	
	//
	// State management.
	//
	public State BeginState( State newState )
	{
		if ( newState == null )
		{
			newState = State.NewState<State>();
		}
		State oldState = state;
		newState.outer = this;
		oldState.OnExit( newState );
		state = newState;
		state.OnEnter();
		return state;
	}
	
	public State BeginState< T >() where T: State
	{
		return BeginState( State.NewState< T >() );
	}
	
	public State BeginState( System.Type type )
	{
		return BeginState( State.NewState( type ) );
	}
	
	public bool IsInState<T>()
	{
		return ( state.GetType() == typeof( T ) );
	}
	
	//
	// Initialization.
	//
	void Start()
	{
		// Set default state.
		state = State.NewState<State>();
		
		// If an initial state type is specified, set the state to a new instance of that type.
		if ( initialStateType != null )
		{
			System.Type type = initialStateType.type;
			if ( type != null )
			{
				State newState = (State) System.Activator.CreateInstance( type );
				BeginState( newState );
			}
		}
	}
	
	//
	// Cleanup.
	//
	void OnDestroy()
	{
		// Ensure that OnExit gets called for the current state.
		if ( state != null )
			state.OnExit( null );
	}
}

//
// Custom inspector which allows us to choose a initial state type by dragging a script into the 'Initial State Type' field.
//
#if UNITY_EDITOR
[ CustomEditor( typeof( StateMachine ) ) ]
[ CanEditMultipleObjects() ]
public class StateMachineEditor : Editor
{
	private StateMachine stateMachine { get { return (StateMachine) target; } }
	private MonoScript memoizedScript = null;
	
	// Returns the script which defines the given type.
	private MonoScript TypeToScript( System.Type type )
	{
		if ( type == null )
			return null;
		// If the last script we returned defines this type, return that last script instead of doing an entire search.
		if ( ( memoizedScript != null ) && ( memoizedScript.GetClass() == type ) )
			return memoizedScript;
		
		// Search all scripts for the one that defines the given type.
		MonoScript[] allScripts = Resources.FindObjectsOfTypeAll< MonoScript >();
		foreach( MonoScript script in allScripts )
		{
			if ( script.GetClass() == type )
			{
				// Store a reference to this script to use for skipping lookups on this same type later.
				memoizedScript = script;
				
				return script;
			}
		}
		return null;
	}

	public override void OnInspectorGUI ()
	{
		MonoScript oldScript = TypeToScript( stateMachine.initialStateType );
		MonoScript script = EditorGUILayout.ObjectField( "Initial State Type", oldScript, typeof( MonoScript ), false ) as MonoScript;
		
		if ( script != oldScript )
		{
			if ( script != null )
			{
				// Grab the class defined by the user-provided script.
				System.Type scriptClass = script.GetClass();
				if ( scriptClass != null )
				{
					// Ensure that user-provided script defines a subclass of States.State
					if ( scriptClass.IsSubclassOf( typeof( State ) ) )
					{
						stateMachine.initialStateType = scriptClass;
					}
				}
			}
			else
			{
				stateMachine.initialStateType = null;
			}
			EditorUtility.SetDirty( stateMachine );
		}
	}
}
#endif




