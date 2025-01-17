# Game events
Unity is a bitch, but their UnityEvent system works great for things in unity, but in code the C# event system is way superior, but how about we combine it with a Man in the Middle that manages it and what better way then to use Scriptable Objects?

## Broadcaster
Within Code any MonoBehaviour or NetworkBehaviour can broadcast data, it is in simple syntax and it basically boils down to Get GameEvent and publish.

```cs
public class BroadCaster : MonoBehaviour
{
    [SerializeField] private GameEvent EventToBroadCast;
    // ^This is the Serialized Field we will broad cast to

    public void BroasCastMessage()
    {
        string toSend = "Hello"; //This can be what ever you want it to be

        //Way 1
        if(EventToBroadCast)
        {
            EventToBroadCast.RaiseEvent(this, toSend); // <-- toSend will be broadcasted to any subscriber
        }

        //Way 2
        EventToBroadCast?.RaiseEvent(this, toSend); // Simplifies the line
    }

}
```

## Listener

Listeners can be placed into any game object inside of unity as a component using the [Game Event Listener](xref:BackEndSystems.CustomEventSystem.GameEventListener) component

> [!NOTE]
> If the BroadCaster and the Listeners are 2 seperate Entities set [ActivateOnThisObject](xref:BackEndSystems.CustomEventSystem.GameEventListener.ActivateOnThisObject) To ``false`` 

Inside of unity you can set the respone to anything in code or unity, keep in mind if you point at something inside of script you must set it to ``public``

If you want the data from the ``Component sender``  or ``object data`` you must use a function that take those as parameters

```cs
public void AddKrokketLetterToPlayer(Component sender, object data)
{
    // Let's say we want a GameObject as data
    if (data is not GameObject player) return; 
    // This checks if data IS NOT a gameobject, if it can't form it return because the data is garbage, 
    //note that we can use it later since it isn't destroyed.
    Inventory inv = player.GetComponent<Inventory>();
    inv.AddItem(this.KrokketLetter);
}
```
