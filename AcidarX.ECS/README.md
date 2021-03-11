Entity Component System
=======================

Inspired by [EnTT](https://github.com/skypjack/entt), currently not really good in any way but trying to keep up.

- **Entities** are just simply `uint` variables which gets registered on.. well, `registry`
- **Components** are simple structs which extend `IComponent` interface in order to get associated with `entities`
- **Systems** are any function you would like like, most important part is that they have to take reference of `registry` and operate on that
- All hailed **Registry** is a simple data structure, which holds information about our `entities` with `components`

Code Example
-------

```c#
public class Program
{
    public struct Position : IComponent
    {
        public float X { get; init; }
        public float Y { get; init; }
    }

    public struct Velocity : IComponent
    {
        public float Dx { get; init; }
        public float Dy { get; init; }
    }

    public void Update(ref Registry registry)
    {
        IEnumerable<uint> entities = registry.View(new List<Type>
            {typeof(Position), typeof(Velocity)});

        foreach (uint entity in entities)
        {
            var position = registry.GetComponent<Position>(entity);
            var velocity = registry.GetComponent<Velocity>(entity);

            Console.WriteLine($"{position} | {velocity}");
        }
    }

    public void Main()
    {
        var registry = new Registry();

        for (var i = 0; i < 10; i++)
        {
            uint entity = registry.CreateEntity();

            var position = new Position {X = i, Y = i};
            registry.AddComponent(entity, ref position);
            if (i % 2 == 0)
            {
                var velocity = new Velocity {Dx = 1, Dy = 1};
                registry.AddComponent(entity, ref velocity);
            }
        }

        Update(ref registry);
    }
}
```