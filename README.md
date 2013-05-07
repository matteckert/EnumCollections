EnumCollections
===============

Specialized Enum collections for C#
-----------------------------------

Uses [ExtraConstraints](https://github.com/Fody/ExtraConstraints, "ExtraConstraints") internally to assure generics are constrained to `Enum` types.

### EnumSet
- Implements `IFiniteSet<T>`, which extends `ISet<T>` with a `Complement` method.
- `T` is restricted to `Enum` types.
- Performance characteristics are similar to the [Java version] (http://docs.oracle.com/javase/7/docs/api/java/util/EnumSet.html "Java EnumSet"). 
- Faster with `Enum` types that have less than or equal to 64 values.
- Static factory methods similar to the Java version are provided.
    - Requires generic qualifiers: `EnumSet<Bird>.Of(Bird.Stork)` instead of `EnumSet.Of(Bird.Stork)`.
    - `NoneOf` is `None` (no parameters).
    - `AllOf` is `All` (no parameters).
    - `Of` is provded for up to 7 arguments; more calls the variable `params` version.
- `Enum` constants defined with the same value are aliases:

    ```csharp
    public enum Bird { BlueJay, Stork, Puffin, SeaParrot = 2, Chicken }
    ...
    Assert.IsTrue(EnumSet<Bird>.Of(Bird.Stork, Bird.SeaParrot) == EnumSet<Bird>.Of(Bird.Stork, Bird.Puffin));
    ```

### EnumMap
- Implements `IDictionary<TKey, TValue>`.
- `TKey` is restricted to `Enum` types.
- Performance characteristics are similar to the [Java version] (http://docs.oracle.com/javase/7/docs/api/java/util/EnumMap.html "Java EnumMap"). 
- Worst case access, insert, and delete is constant.
- `Enum` constants defined with the same value are aliases:

    ```csharp
    public enum Bird { BlueJay, Stork, Puffin, SeaParrot = 2, Chicken }
    ...
    var birdMap = new EnumMap<Bird, int>();
    birdMap[Bird.Puffin] = 4;
    birdMap[Bird.SeaParrot] = 5;
    Assert.IsTrue(birdMap[Bird.Puffin] == 5);
    ```