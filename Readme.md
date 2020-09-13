## Fonderie source generators

This repository is the home of Fonderie set of Roslyn generators. 

**These generators need at least Visual Studio 16.8 Preview 2.1 or .NET 5 Preview 8 to work properly.**

## Fonderie.INPC

This is repository for the `INotifyPropertyChanged` generator based on field discovery, using C# 9.0 source generators.

Find out more about it [in this article](https://jaylee.org/archive/2019/12/08/roslyn-sourcegeneration-reborn-replace-inotifypropertychanged.html).

### How to use it

Add the following attribute in your source:
```csharp
namespace Fonderie
{
    public class GeneratedPropertyAttribute : Attribute { }
}
```

Add a reference to [`Fonderie.INPC.Generator`](https://www.nuget.org/packages/Fonderie.INPC.Generator), the add the following class :

```csharp
    public partial class MyClass
    {
        [GeneratedProperty]
        private string _stringProperty;

        [GeneratedProperty]
        private int _intProperty;

        private bool _otherField;

        partial void OnIntPropertyChanged(int previous, int value)
            => Console.WriteLine($"OnIntPropertyChanged({previous},{value})");

        partial void OnStringPropertyChanged(string previous, string value) 
            => Console.WriteLine($"OnIntPropertyChanged({previous},{value})");
    }
```

The `INotifyPropertyChanged` interface is automatically implemented, and `XXPropertyChanged` methods are generated to be notified inside the class if a property changed.

## Fonderie.Resw.Generator

This generator is a sample to demonstrate the ability for Roslyn generators to use MSBuild properties and items.