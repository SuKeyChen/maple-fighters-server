﻿using System;

namespace Common.ComponentModel
{
    /// <inheritdoc />
    /// <summary>
    /// A component base which every component must inherit from it.
    /// </summary>
    public class ComponentBase : IComponent
    {
        protected IComponents Components { get; private set; }

        void IComponent.Awake(IComponents components)
        {
            Components = components;

            OnAwake();
        }

        void IDisposable.Dispose()
        {
            OnRemoved();
        }

        protected virtual void OnAwake()
        {
            // Left blank intentionally
        }

        protected virtual void OnRemoved()
        {
            // Left blank intentionally
        }
    }
}