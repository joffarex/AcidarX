Entity Component System
=======================

Inspired by [EnTT](https://github.com/skypjack/entt), currently not really good in any way but trying to keep up.

- **Entities** are just simply `uint` variables which gets registered on.. well, `registry`
- **Components** are simple structs which extend `IComponent` interface in order to get associated with `entities`
- **Systems** are any function you would like like, most important part is that they have to take reference of `registry` and operate on that
- All hailed **Registry** is a simple data structure, which holds information about our `entities` with `components`

Code Example
-------

TBD