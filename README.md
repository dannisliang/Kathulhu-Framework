Kathulhu-Framework
==================

A Unity3D framework to invoke the Elder Gods and request their help to accelerate game development during game jams. It consists of a bunch of tools like global event dispatching, scene transitions and object pooling. This framework's goal is to be easy to use and convenient. All the tools in this framework that are using UI elements should and will make use of Unity's new UI system.

## EVENTS

The files in the Event folder contains the event dispatching system. It is a simple system that uses an "Event" abstract class from which all events can inherit from.

#### Subscribing to an event

To subscribe to an event, just use the static Subscribe<T>( Action<Event> callback ) generic method and specify a delegate to handle the event. Delegates have a void return type and receive a 'Kathulhu.Event' argument. Example :

```c#
using UnityEngine;
using System.Collections;
using Kathulhu;

public class SubscribeToMyCustomEventClass : MonoBehaviour {

	void Awake() {
        EventDispatcher.Subscribe<MyConcreteEvent>( EventCallbackMethod );
	}

  void EventCallbackMethod( Kathulhu.Event e ) {
      MyConcreteEvent evt = e as MyConcreteEvent;
      //do something with my event
  }

  void OnDestroy() {
      EventDispatcher.Unsubscribe<MyConcreteEvent>( EventCallbackMethod );
  }
}
```

#### Raising an event

To raise an event, simply use the 'Event(Kathulhu.Event e)' static method. Example :

```c#
  MyConcreteEvent e = new MyConcreteEvent();
  EventDispatcher.Event( e );
```

#### EventCommand.cs

The EventCommand abstract class is simply an event that implements the ICommand interface. The base implementation of the ICommand.Execute() method will simply raise an event of the EventCommand's type. Example usage :

```c#
  MyConcreteEventCommand cmd = new MyConcreteEventCommand();
  cmd.Execute();
```

This code will raise an event of type MyConcreteEventCommand via the EventDispatcher. EventCommands can override the Execute method to add command logic and call base.Execute() when it is time to raise the event.  

## CORE

The files in the 'Core' folder contains classes and interfaces that are related to the GameController. The GameController is a persistent object and a global manager for using this framework and controlling your game. These files make use of the EventDispatcher class to raise some global events (to notify of scene transitions, for example).

#### Registry

The GameController holds a registry that can hold references to any type of object via a specified identifier. This is useful to store references to important objects in the game.

Example usage:
```c#
//Register a GameObject type object with identifier "myobject"
GameController.Registry.Register<GameObject>( this.gameObject, "myobject");

//Find an object of type GameObject with identifier "myobject" in the GameController registry
GameObject go = GameController.Registry.Resolve<GameObject>("myobject");
```

#### Scene Transitions

The GameController contains methods to load another scene or load additive another scene. Using these methods instead of the built-in Application.LoadLevel methods in Unity allows the GameController to use a SceneManager to handle a scene's setup. The GameController keeps a list of all SceneManager objects and can use them to initiate a scene load coroutine or unload a scene when switching to another scene. The GameController will also instantiate a LoadingScreen prefab that can hide the content of a scene while it's being set up and display the loading progress.

To make use of scene transitions and the loading screen, take a look at the "Examples/SceneTransitions" folder in this framerwork. Important things to remember are that you need to specify a prefab for the loading screen in your GameController and that every scene that you want to load need to have a SceneManager script to handle the loading of the scene and the SceneManager need it's Scene Name field populated with the name of the scene this SceneManager is handling.