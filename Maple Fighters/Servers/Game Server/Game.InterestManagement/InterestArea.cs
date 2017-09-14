using System;
using System.Collections.Generic;
using System.Linq;
using CommonTools.Log;
using MathematicsHelper;
using ServerApplication.Common.ComponentModel;

namespace Game.InterestManagement
{
    public class InterestArea : Component<IGameObject>, IInterestArea
    {
        public Action<IGameObject> GameObjectAdded;
        public Action<int> GameObjectRemoved;
        public Action<IGameObject[]> GameObjectsAdded;
        public Action<int[]> GameObjectsRemoved;

        private Rectangle interestArea;

        public InterestArea(Vector2 position, Vector2 areaSize)
        {
            interestArea = new Rectangle(position, areaSize);
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            DetectOverlapsWithRegions();
            SubscribeToPositionChangesNotifier();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnsubscribeFromPositionChangesNotifier();
        }

        private void SubscribeToPositionChangesNotifier()
        {
            var transform = Entity.Container.GetComponent<Transform>().AssertNotNull();
            transform.PositionChanged += SetPosition;
        }

        private void UnsubscribeFromPositionChangesNotifier()
        {
            var transform = Entity.Container.GetComponent<Transform>().AssertNotNull();
            transform.PositionChanged -= SetPosition;
        }

        private void SetPosition(Vector2 position)
        {
            interestArea.SetPosition(position);

            DetectOverlapsWithRegions();
        }

        public IEnumerable<IRegion> GetPublishers()
        {
            var regions = Entity.Scene.GetAllRegions();
            return regions.Cast<IRegion>().Where(region => region.HasSubscription(Entity.Id)).ToArray();
        }

        private void DetectOverlapsWithRegions()
        {
            var sceneRegions = Entity.Scene.GetAllRegions();

            foreach (var region in sceneRegions)
            {
                if (!Rectangle.Intersect(region.Area, interestArea).Equals(Rectangle.EMPTY))
                {
                    if (region.HasSubscription(Entity.Id))
                    {
                        continue;
                    }

                    region.AddSubscription(Entity);
                }
                else
                {
                    if (region.HasSubscription(Entity.Id))
                    {
                        region.RemoveSubscription(Entity.Id);
                    }
                }
            }
        }
    }
}