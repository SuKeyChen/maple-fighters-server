﻿using System.Collections.Generic;
using CommonCommunicationInterfaces;
using CommonTools.Log;
using ComponentModel.Common;
using Game.InterestManagement;
using PeerLogic.Common.Components;
using Shared.Game.Common;

namespace Game.Application.PeerLogic.Components
{
    internal class InterestManagementNotifier : Component
    {
        private IEventSenderWrapper eventSender;

        protected override void OnAwake()
        {
            base.OnAwake();

            eventSender = Components.GetComponent<IEventSenderWrapper>().AssertNotNull();

            var sceneObjectGetter = Components.GetComponent<ISceneObjectGetter>().AssertNotNull();
            var interestArea = sceneObjectGetter.GetSceneObject().Components.GetComponent<IInterestArea>().AssertNotNull();
            interestArea.SubscriberAdded += OnSubscriberAdded;
            interestArea.SubscriberRemoved += OnSubscriberRemoved;
            interestArea.SubscribersAdded += OnSubscribersAdded;
            interestArea.SubscribersRemoved += OnSubscribersRemoved;
        }

        private void OnSubscriberAdded(ISceneObject sceneObject)
        {
            var transform = sceneObject.Components.GetComponent<ITransform>().AssertNotNull();
            var sharedSceneObject = new SceneObjectParameters(sceneObject.Id, sceneObject.Name, transform.Position.X, transform.Position.Y);

            var parameters = new SceneObjectAddedEventParameters(sharedSceneObject);
            eventSender.Send((byte)GameEvents.SceneObjectAdded, parameters, MessageSendOptions.DefaultReliable());
        }

        private void OnSubscriberRemoved(int subscriberId)
        {
            var parameters = new SceneObjectRemovedEventParameters(subscriberId);
            eventSender.Send((byte)GameEvents.SceneObjectRemoved, parameters, MessageSendOptions.DefaultReliable());
        }

        private void OnSubscribersAdded(IReadOnlyList<ISceneObject> sceneObjects)
        {
            var sharedSceneObjects = new SceneObjectParameters[sceneObjects.Count];
            for (var i = 0; i < sceneObjects.Count; i++)
            {
                sharedSceneObjects[i].Id = sceneObjects[i].Id;
                sharedSceneObjects[i].Name = sceneObjects[i].Name;

                var transform = sceneObjects[i].Components.GetComponent<ITransform>().AssertNotNull();
                sharedSceneObjects[i].X = transform.Position.X;
                sharedSceneObjects[i].Y = transform.Position.Y;
            }

            var parameters = new SceneObjectsAddedEventParameters(sharedSceneObjects);
            eventSender.Send((byte)GameEvents.SceneObjectsAdded, parameters, MessageSendOptions.DefaultReliable());
        }

        private void OnSubscribersRemoved(int[] sceneObjectsId)
        {
            var parameters = new SceneObjectsRemovedEventParameters(sceneObjectsId);
            eventSender.Send((byte)GameEvents.SceneObjectsRemoved, parameters, MessageSendOptions.DefaultReliable());
        }
    }
}