using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using KiteKat.EcsCore.Components;

namespace KiteKat.EcsCore.Systems
{
    public abstract class EcsSystem<TComponentFilter> : IEcsSystem
    {
        protected readonly EcsWorld world;

        private readonly IList<int> entities;

        private readonly IList<TComponentFilter> filters;

        public int EntityCount
        {
            get { return entities.Count; }
        }

        public EcsSystem(EcsWorld world)
        {
            this.world = world;

            // In case this system was created after entities were already created, populate the lists with all matching entities.
            entities = world.GetEntitiesMatchingComponentFilter<TComponentFilter>().ToList();
            filters = world.CreateComponentFilters<TComponentFilter>(entities).ToList();

            world.OnComponentAdded += OnComponentAdded;
            world.OnComponentRemoved += OnComponentRemoved;
            world.OnEntityDestroyed += OnEntityDestroyed;

            Console.WriteLine("Created " + GetType() + " processing " + EntityCount + " entities.");
        }

        public virtual void Process(GameTime gameTime)
        {
            for (var i = 0; i < filters.Count; ++i)
            {
                Process(entities[i], filters[i], gameTime);
            }
        }

        protected abstract void Process(int entity, TComponentFilter components, GameTime gameTime);

        // TODO: Make a FilterCollection or something that does all this so systems can have multiple filters running.
        private void OnComponentAdded(int entity, IEcsComponent component)
        {
            if (!entities.Contains(entity) && world.MatchesComponentFilter<TComponentFilter>(entity))
            {
                TComponentFilter filter = world.CreateComponentFilter<TComponentFilter>(entity);
                filters.Add(filter);
                entities.Add(entity);

                Console.WriteLine(GetType().Name + " will process " + entity);
            }
        }

        private void OnComponentRemoved(int entity, IEcsComponent component)
        {
            int index = entities.IndexOf(entity);
            if (index >= 0 && !world.MatchesComponentFilter<TComponentFilter>(entity))
            {
                filters.RemoveAt(index);
                entities.RemoveAt(index);

                Console.WriteLine(GetType().Name + " will no longer process " + entity + " because it does not have the required components.");
            }
        }

        private void OnEntityDestroyed(int entity)
        {
            int index = entities.IndexOf(entity);
            if (index >= 0)
            {
                filters.RemoveAt(index);
                entities.RemoveAt(index);

                Console.WriteLine(GetType().Name + " will no longer process " + entity + " because it has been destroyed.");
            }
        }
    }
}
