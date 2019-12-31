﻿using Common.ComponentModel;

namespace ServerCommon.PeerLogic.Common
{
    /// <summary>
    /// Exposes a safe access to the inbound peer logic.
    /// </summary>
    public interface IInboundPeerLogic
    {
        IExposedComponents ExposedComponents { get; }
    }
}