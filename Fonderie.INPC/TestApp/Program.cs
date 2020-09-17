using Fonderie;
using System;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new MyClass();
            c.IntProperty = 42;
            c.StringProperty = "My 42";

            var c2 = new MyClass3();
            c2.IntProperty = 42;
            c2.StringProperty = "My 42";
        }
    }

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
    public partial class MyClass3
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

}

namespace Fonderie
{
    public class GeneratedPropertyAttribute : Attribute { }
}