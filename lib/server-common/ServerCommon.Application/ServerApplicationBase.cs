﻿using Common.ComponentModel;
using Common.Components;
using CommonTools.Coroutines;
using CommonTools.Log;
using ServerCommon.Application.Components;
using ServerCommunicationInterfaces;

namespace ServerCommon.Application
{
    /// <inheritdoc />
    /// <summary>
    /// A base for the server application to be initialized properly.
    /// </summary>
    public abstract class ServerApplicationBase : IApplicationBase
    {
        protected IExposedComponents ExposedComponents => Components.ProvideExposed();

        protected IComponents Components { get; } = new ComponentsContainer();

        protected IServerConnector ServerConnector { get; }

        protected IFiberProvider FiberProvider { get; }

        protected ServerApplicationBase(IServerConnector serverConnector, IFiberProvider fiberProvider)
        {
            ServerConnector = serverConnector;
            FiberProvider = fiberProvider;
        }

        void IApplicationBase.Startup()
        {
            OnStartup();
        }

        void IApplicationBase.Shutdown()
        {
            OnShutdown();
        }

        protected virtual void OnStartup()
        {
            LogUtils.Logger = GetLogger();
            TimeProviders.DefaultTimeProvider = GetTimeProvider();
        }

        protected virtual void OnShutdown()
        {
            Components?.Dispose();
        }

        /// <summary>
        /// Adds common components:
        /// 1. <see cref="IIdGenerator"/>
        /// 2. <see cref="IRandomNumberGenerator"/>
        /// 3. <see cref="IFiberStarterProvider"/>
        /// 4. <see cref="ICoroutinesExecutor"/>
        /// 5. <see cref="IClientPeerContainer"/>
        /// </summary>
        protected void AddCommonComponents()
        {
            ExposedComponents.Add(new IdGenerator());
            Components.Add(new RandomNumberGenerator());

            var fiberStarter = 
                Components.Add(new FiberStarterProvider(FiberProvider));
            var scheduler = fiberStarter.ProvideFiberStarter();
            var executor = 
                new FiberCoroutinesExecutor(scheduler, updateRateMilliseconds: 100);

            Components.Add(new CoroutinesExecutor(executor));
            ExposedComponents.Add(new ClientPeerContainer());
        }

        protected abstract ILogger GetLogger();

        protected abstract ITimeProvider GetTimeProvider();
    }
}