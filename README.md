EnumCollections
===============

Port of Java's EnumSet collection to C#
-----------------------------------

### EnumSet

#### Overview

- Performance characteristics are similar to the [Java's EnumSet](http://docs.oracle.com/javase/7/docs/api/java/util/EnumSet.html). 
- More efficient with `Enum` types that have less than or equal to 64 values.
- Static factory methods similar to the Java version are provided: `EnumSet.Of(Bird.Stork, ...)`

#### Notes
1. `Enum` constants defined with the same value are treated as aliases:

    ```csharp
    public enum Bird { 
        BlueJay,        // 0
        Stork,          // 1
        Puffin,         // 2
        SeaParrot = 2,  // 2
        Chicken         // 3
    }

    var a = EnumSet.Of(Bird.SeaParrot);
    var b = EnumSet.Of(Bird.Puffin)
    Assert.That(a, Is.EqualTo(b)));
    ```

1. When creating an empty `EnumSet`, you must specify the type:

    ```csharp
    var a = EnumSet.Of<Bird>()
    ```