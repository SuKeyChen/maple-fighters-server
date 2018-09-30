﻿using Authentication.Application;
using Authentication.Application.Peer;
using ServerCommon.PhotonStarter;
using ServerCommunicationInterfaces;

namespace Authentication.PhotonStarter
{
    public class AuthenticationStartup : PhotonStarterBase<AuthenticationApplication, PeerBase>
    {
        protected override AuthenticationApplication CreateApplication(
            IFiberProvider fiberProvider)
        {
            return new AuthenticationApplication(fiberProvider);
        }
    }
}