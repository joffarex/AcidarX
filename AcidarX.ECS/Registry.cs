using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AcidarX.ECS
{
    internal class Registry
    {
        public static Dictionary<uint, HashSet<IComponent>> Entities { get; } = new();

        public static uint NumberOfEntities { get; private set; }


        public uint CreateEntity()
        {
            NumberOfEntities++;

            Entities.Add(NumberOfEntities, new HashSet<IComponent>());
            return NumberOfEntities;
        }

        public List<uint> View(List<Type> componentTypes)
        {
            List<uint> entities = new();

            foreach ((uint entity, HashSet<IComponent> components) in Entities)
            {
                List<bool> shouldAddTracker = componentTypes
                    .Select(type => components.Any(component => component.GetType().Name == type.Name)).ToList();

                if (shouldAddTracker.TrueForAll(item => item))
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public void AddComponent<T>(uint entity, ref T component) where T : IComponent
        {
            Entities.TryGetValue(entity, out HashSet<IComponent> components);

            Debug.Assert(components != null, $"Entity {entity} not found");

            components.TryGetValue(component, out IComponent? foundComponent);
            Debug.Assert(foundComponent == null, $"Component {component} already added");

            components.Add(component);
        }

        public T GetComponent<T>(uint entity) where T : IComponent
        {
            Entities.TryGetValue(entity, out HashSet<IComponent> components);

            Debug.Assert(components != null, $"Entity {entity} not found");

            T? component = components
                .Where(c => c.GetType() == typeof(T))
                .Cast<T>()
                .FirstOrDefault();

            Debug.Assert(component != null, $"Component of type {typeof(T).Name} not found");

            return component;
        }

        public void RemoveComponent<T>(uint entity) where T : IComponent
        {
            Entities.TryGetValue(entity, out HashSet<IComponent> components);

            Debug.Assert(components != null, $"Entity {entity} not found");

            var component = GetComponent<T>(entity);
            components.Remove(component);
            // components.RemoveWhere(c => c.GetType() == typeof(T));
        }
    }
}