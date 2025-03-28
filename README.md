# Interface Serialization

## Installation 
There are no custom steps to adding the package to your project. Just choose one of following installation methods:

1) Download ZIP of the repository and extract it to your project Assets folder.

2) Add package through Unity Package Manager by choosing "Add package from git URL..." option and then typing "https://github.com/Bipolar-Games/Interface-Serialization.git".

3) Add the Interfaces Serialization repository as submodule to your project git repository.

4) Use any other method you prefer.

## Usage

### Preparation
Assuming you have a following interface:

```cs
public interface IMyInterface
{
    void MyMethod();
}
```

You can implement it in Components:
```cs
using UnityEngine;

public class MyInterfaceComponent : MonoBehaviour, IMyInterface
{
    public void MyMethod()
    {

    }
}
```

And also in ScriptableObjects: 
```cs
using UnityEngine;

[CreateAssetMenu]
public class MyInterfaceScriptableObject : ScriptableObject, IMyInterface
{
    public void MyMethod()
    {

    }
}
```

### Creating the object field

There are a few different ways to create field for the interface. 

#### 1) Using `Serialized<>` type

You can enclose your interface type with `Serialized<>` generic type to create a serialized version of your interface.

```cs
using UnityEngine;
using Bipolar;

public class MyBehaviour : MonoBehaviour
{
    [SerializeField]
    private Serialized<IMyInterface> mySerializedInterface;

    private void CallMyMethod()
    {
        mySerializedInterface.Value.MyMethod();
    }
}
```



#### 2) Inheriting `Serialized<>` class

If you are going to reference your interface in many classes, you might find it more convenient to create your own serialized class of interface by inheriting `Serialized<>` class.

```cs
using UnityEngine;
using Bipolar;

[System.Serializable]
public class MyInterface : Serialized<IMyInterface>, IMyInterface
{
    public void MyMethod() => Value.MyMethod();
}
```

It requires some preparations, but it makes creation and usage of serialized interfaces simpler and more natural.

```cs
using UnityEngine;

public class MyBehaviour : MonoBehaviour
{
    [SerializeField]
    private MyInterface mySerializedInterface;

    private void CallMyMethod()
    {
        mySerializedInterface.MyMethod();
    }
}
```

#### 3) Using `RequireInterface` attribute

If you prefer using an attribute instead of `Serialized<>` class you can add the `RequireInterface` attribute to your `UnityEngine.Object` field. However this method requires casting your object every time you want to use a function of the interface.

```cs
using UnityEngine;
using Bipolar;

public class MyBehaviour : MonoBehaviour
{
    [SerializeField, RequireInterface(typeof(IMyInterface))]
    private Object mySerializedInterface;

    private void CallMyMethod()
    {
        ((IMyInterface)mySerializedInterface).MyMethod();
    }
}
```

### Inspector usage

Regardless of the field declaration method, the correctly configured field of interface will be displayed as shown in the picture.

![image](https://github.com/user-attachments/assets/23727899-8c6d-40d2-ae25-bb860b7ec003)

#### Drag & Drop

You can drag and drop into the field all objects implementing your interface, including:
- ScriptableObjects implementing the interface
- Components implementing the interface
- GameObjects containing such Component
- prefabs whose root GameObject contains such Component

![image](https://github.com/user-attachments/assets/d4ca99ed-1121-4828-9707-280fc88241a6)

#### Object Selector

You can also find the available objects in custom Object Selector window, which can be opened by pressing the default Object Selector Button. The Object Selector displays prefabs and ScriptableObjects under 'Assets' tab and Components in the scene under the 'Scene' tab.

![image](https://github.com/user-attachments/assets/65514655-23e2-4617-bca9-ebdff2465262)
![image](https://github.com/user-attachments/assets/690786ff-8bea-418f-a592-37a87c4c8c42)



