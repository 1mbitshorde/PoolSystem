# Pool System

A Pool System (also known as Object Pooling) is an optimization technique to relieve the CPU when creating and destroying a lot of commonly used GameObjects, saving memory and CPU cycle time.

The Pool uses internally the [UnityEngine.Pool API](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Pool.ObjectPool_1.html), a stack-based ObjectPool to track objects with the object pool pattern.

At your request, the Pool lazily instantiates GameObjects at runtime using only one prefab. This prefab instance is only created when used until it reaches its maximum size. After that, the Pool will recycle the already created instances. At certain point, those instances must be disabled by any component and sent back to the Pool to be reused again.

GameObjects used by the Pool System must have the [Poolable](/Runtime/Poolable.cs) component attached on it. When disabled, the GameObject will be sent back to its Pool to be reused again, improving memory usage.

If no parent is set when placing the instance, it will be placed at global GameObject **ActivePoolableObjects**. After that will be placed again as a child of the Pool Game Object.

![Pool System Inspector](/Docs~/inspector.gif) 

To disable a Poolable GameObject, you can use any of the [Disablers Scripts](/Runtime/Disablers/) provided by this package or create your own. Your script just need to disable the GameObject or the **Poolable** component attached on it.

## How To Use

### Creating a Poolable Prefab

* Create a prefab 
* Attach the [Poolable](/Runtime/Poolable.cs) component
* Attach any component of the [Disablers Scripts](/Runtime/Disablers/)

### Creating a Pool

* Create a prefab (optional)
* Attach a [Pool](/Runtime/Pool.cs) component and set:
	* **Prefab**: the prefab asset with the ```Poolable``` component you just created.
	* **Size**: The Pool size. Items will be created even above this value.

The pool will always return an item even though reaching its max size. Remaining items will be destroyed after used.

For best performance, always set the Pool size to the max quantity of items you need to use at runtime.

### Call the API methods on Gameplay

```csharp
using UnityEngine;
using OneM.PoolSystem;

namespace YourGameNamespace
{
	public sealed class PoolSystemTester : MonoBehaviour 
	{
		public Pool pool;

		public void PlaceAtOrigin() => pool.Place(Vector3.zero);
		// or any other Place function
	}
}
```

## Installation

### Using the Git URL

You will need a **Git client** installed on your computer with the Path variable already set and the correct git credentials to 1M Bits Horde.

- In this repo, go to Code button, select SSH and copy the URL.
- In Unity, use the **Package Manager** "Add package from git URL..." feature and paste the URL.
- Set the version adding the suffix `#[x.y.z]` at URL

---

**1 Million Bits Horde**

[Website](https://www.1mbitshorde.com) -
[GitHub](https://github.com/1mbitshorde) -
[LinkedIn](https://www.linkedin.com/company/1m-bits-horde)
