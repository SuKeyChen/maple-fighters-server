﻿using System.Collections.Generic;
using System.Linq;
using CommonTools.Log;
using MathematicsHelper;

namespace Game.InterestManagement
{
    internal class Region : IRegion
    {
        public Rectangle Area { get; }

        private readonly Dictionary<int, IGameObject> gameObjects = new Dictionary<int, IGameObject>();

        public Region(Rectangle rectangle)
        {
            Area = rectangle;
        }

        public void AddSubscription(IGameObject gameObject)
        {
            if (gameObjects.ContainsKey(gameObject.Id))
            {
                LogUtils.Log(MessageBuilder.Trace($"A game object with id #{gameObject.Id} already exists in a region."), LogMessageType.Error);
                return;
            }

            gameObjects.Add(gameObject.Id, gameObject);

            LogUtils.Log(MessageBuilder.Trace($"A new subscriber: {gameObject.Name}"));

            // Show all exists entities for a new game object.
            ShowGameObjectsForGameObject(gameObject);

            // Show a new game object for all exists entities.
            ShowGameObjectForGameObjects(gameObject);
        }

        public void RemoveSubscription(int gameObjectId)
        {
            if (!gameObjects.ContainsKey(gameObjectId))
            {
                LogUtils.Log(MessageBuilder.Trace($"A game object with id #{gameObjectId} does not exists in a region."), LogMessageType.Error);
                return;
            }

            // Hide game objects for the one that left this region.
            HideGameObjectsForGameObject(gameObjectId);

            // Remove him from region's list.
            gameObjects.Remove(gameObjectId);

            // Hide the one who left from this region for other game objects.
            HideGameObjectForGameObjects(gameObjectId);
        }

        public void RemoveSubscriptionForOtherOnly(int gameObjectId)
        {
            if (!gameObjects.ContainsKey(gameObjectId))
            {
                LogUtils.Log(MessageBuilder.Trace($"A game object with id #{gameObjectId} does not exists in a region."), LogMessageType.Error);
                return;
            }

            HideGameObjectsForGameObjectOnly(gameObjectId);

            // Remove him from region's list.
            gameObjects.Remove(gameObjectId);

            // Hide the one who left from this region for other game objects.
            HideGameObjectForOtherOnly(gameObjectId);
        }

        public bool HasSubscription(int gameObjectId)
        {
            return gameObjects.ContainsKey(gameObjectId);
        }

        public IEnumerable<IGameObject> GetAllSubscribers()
        {
            return gameObjects.Select(gameObject => gameObject.Value).ToList();
        }

        private void ShowGameObjectsForGameObject(IGameObject gameObject)
        {
            var gameObjectsTemp = gameObjects.Values.Where(gameObjectValue => gameObjectValue.Id != gameObject.Id).ToArray();
            var interestArea = gameObject.Container.GetComponent<InterestArea>();
            interestArea?.GameObjectsAdded?.Invoke(gameObjectsTemp); 
        }

        private void HideGameObjectsForGameObject(int hideGameObjectId)
        {
            var gameObjectsTemp = gameObjects.Values.Where(gameObject => gameObject.Id != hideGameObjectId).ToArray();
            var interestArea = gameObjects[hideGameObjectId].Container.GetComponent<InterestArea>();
            if (interestArea == null)
            {
                return;
            }

            var removeGameObjects = new List<int>();

            foreach (var gameObject in gameObjectsTemp)
            {
                if (interestArea.GetPublishers().Any(publisher => !publisher.HasSubscription(gameObject.Id)))
                {
                    removeGameObjects.Add(gameObject.Id);
                }
            }

            if (removeGameObjects.Count > 0)
            {
                interestArea.GameObjectsRemoved?.Invoke(removeGameObjects.ToArray());
            }
        }

        private void ShowGameObjectForGameObjects(IGameObject newGameObject)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.Value.Id == newGameObject.Id)
                {
                    continue;
                }

                var interestArea = gameObject.Value.Container.GetComponent<InterestArea>();
                interestArea?.GameObjectAdded?.Invoke(newGameObject);
            }
        }

        private void HideGameObjectForGameObjects(int hideGameObjectId)
        {
            foreach (var gameObject in gameObjects.Values)
            {
                var interestArea = gameObject.Container.GetComponent<InterestArea>();
                if (interestArea == null)
                {
                    continue;
                }

                if (!interestArea.GetPublishers().Any(publisher => publisher.HasSubscription(hideGameObjectId)))
                {
                    interestArea.GameObjectRemoved?.Invoke(hideGameObjectId);
                }
            }
        }

        private void HideGameObjectForOtherOnly(int hideGameObjectId)
        {
            foreach (var gameObject in gameObjects.Values)
            {
                var interestArea = gameObject.Container.GetComponent<InterestArea>();
                interestArea?.GameObjectRemoved?.Invoke(hideGameObjectId);
            }
        }

        private void HideGameObjectsForGameObjectOnly(int hideGameObjectId)
        {
            var gameObjectsTemp = gameObjects.Keys.Where(gameObjectId => gameObjectId != hideGameObjectId).ToArray();

            if (gameObjects[hideGameObjectId]?.Container?.GetComponent<InterestArea>() == null)
            {
                return;
            }

            var interestArea = gameObjects[hideGameObjectId].Container.GetComponent<InterestArea>();
            interestArea?.GameObjectsRemoved?.Invoke(gameObjectsTemp);
        }
    }
}