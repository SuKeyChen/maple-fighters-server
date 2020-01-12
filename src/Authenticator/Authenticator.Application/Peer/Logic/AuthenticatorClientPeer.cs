﻿using Authenticator.Application.Peer.Logic.Operations;
using Authenticator.Common.Enums;
using Authenticator.Domain.Aggregates.User.Services;
using Common.ComponentModel;
using Common.Components;
using CommonTools.Coroutines;
using CommonTools.Log;
using CommunicationHelper;
using ServerCommon.Application.Components;
using ServerCommon.Peer;
using ServerCommunicationInterfaces;

namespace Authenticator.Application.Peer.Logic
{
    public class AuthenticatorClientPeer : ClientPeerBase<AuthenticatorOperations, EmptyEventCode>
    {
        private IComponents Components { get; } = new ComponentsProvider();

        public AuthenticatorClientPeer(IClientPeer peer, bool log = false)
            : base(peer, log)
        {
            AddComponents();

            AddHandlerForLoginOperation();
            AddHandlerForRegisterOperation();
        }

        protected override ICoroutinesExecutor GetCoroutinesExecutor()
        {
            var executor = 
                new FiberCoroutinesExecutor(Peer.Fiber, updateRateMilliseconds: 100);

            return Components.Add(new CoroutinesExecutor(executor));
        }

        private void AddComponents()
        {
            Components.Add(new LoginService());
            Components.Add(new RegistrationService());
        }

        private void AddHandlerForLoginOperation()
        {
            var loginService = Components.Get<ILoginService>().AssertNotNull();
            var handler = new LoginOperationHandler(loginService);

            OperationHandlerRegister.SetHandler(AuthenticatorOperations.Login, handler);
        }

        private void AddHandlerForRegisterOperation()
        {
            var registrationService = 
                Components.Get<IRegistrationService>().AssertNotNull();
            var handler = new RegisterOperationHandler(registrationService);

            OperationHandlerRegister.SetHandler(AuthenticatorOperations.Register, handler);
        }
    }
}