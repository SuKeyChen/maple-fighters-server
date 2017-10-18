﻿using System;
using CommonCommunicationInterfaces;
using CommonTools.Log;
using PeerLogic.Common.Components;
using ServerCommunicationHelper;
using ServerCommunicationInterfaces;
using JsonConfig;

namespace PeerLogic.Common
{
    public abstract class PeerLogicBase<TOperationCode, TEventCode> : IPeerLogicBase
        where TOperationCode : IComparable, IFormattable, IConvertible
        where TEventCode : IComparable, IFormattable, IConvertible
    {
        public IPeerEntity Entity { get; private set; }

        protected IClientPeerWrapper<IClientPeer> PeerWrapper { get; private set; }
        protected IOperationRequestHandlerRegister<TOperationCode> OperationRequestHandlerRegister { get; private set; }
        protected IEventSender<TEventCode> EventSender { get; private set; }

        public virtual void Initialize(IClientPeerWrapper<IClientPeer> peer)
        {
            PeerWrapper = peer;

            Entity = new PeerEntity(PeerWrapper.PeerId);

            var logEvents = Config.Global.Logs.Events;
            EventSender = new EventSender<TEventCode>(PeerWrapper.Peer.EventSender, logEvents);

            AddCommonComponents();

            var coroutinesExecutor = Entity.Container.GetComponent<ICoroutinesExecutor>().AssertNotNull();

            var logOperationsRequest = Config.Global.Logs.OperationsRequest;
            var logOperationsResponse = Config.Global.Logs.OperationsResponse;
            OperationRequestHandlerRegister = new OperationRequestsHandler<TOperationCode>(PeerWrapper.Peer.OperationRequestNotifier, 
                PeerWrapper.Peer.OperationResponseSender, logOperationsRequest, logOperationsResponse, coroutinesExecutor);

            PeerWrapper.Peer.NetworkTrafficState = NetworkTrafficState.Flowing;
        }

        public virtual void Dispose()
        {
            Entity?.Dispose();
            OperationRequestHandlerRegister?.Dispose();
        }

        private void AddCommonComponents()
        {
            Entity.Container.AddComponent(new EventSenderWrapper(EventSender.AssertNotNull()));
            Entity.Container.AddComponent(new CoroutinesExecutor(new FiberCoroutinesExecutor(PeerWrapper.Peer.Fiber, 100)));
        }
    }
}