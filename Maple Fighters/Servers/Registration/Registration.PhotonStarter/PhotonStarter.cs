﻿using PhotonStarter.Common;
using Registration.Application;
using ServerCommunicationInterfaces;

namespace Registration.PhotonStarter
{
    public class PhotonStarter : PhotonStarterBase<RegistrationApplication>
    {
        protected override RegistrationApplication CreateApplication(IFiberProvider fiberProvider, IServerConnector serverConnector)
        {
            return new RegistrationApplication(fiberProvider, serverConnector);
        }
    }
}