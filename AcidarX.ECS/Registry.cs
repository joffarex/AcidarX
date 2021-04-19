using System;
using System.Collections.Generic;

namespace AcidarX.ECS
{
    public class Registry
    {
        private const int MaxEntities = 100000;

        private readonly Dictionary<Type, IComponent[]> _componentsMap = new();

        public int EntityCount { get; private set; }

        public void AddComponentType<T>() where T : IComponent =>
            _componentsMap.Add(typeof(T), new IComponent[MaxEntities]);

        public int CreateEntity() => EntityCount++;

        public void AddComponent<T>
            (int entity, T component) where T : IComponent => _componentsMap[typeof(T)][entity] = component;

        public T GetComponent<T>(int entity) where T : IComponent => (T) _componentsMap[typeof(T)][entity];
    }
}