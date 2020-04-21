// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor proxy built by RealProxy class in .NET.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[DebuggerStepThrough]
    public class ActorProxy<T> : DispatchProxy where T : IActor
    {
        private readonly SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1, 1);
        private IActorBase? _instance;
        private IActorManager? _manager;
        private IWorkContext? _workContext;

        public ActorProxy()
        {
        }

        /// <summary>
        /// Create transparent proxy for instance of actor class
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="instance">instance of actor class</param>
        /// <param name="manager">actor manager</param>
        /// <returns>proxy</returns>
        public static T Create(IWorkContext context, IActorBase instance, IActorManager manager)
        {
            context.VerifyNotNull(nameof(context));
            instance.VerifyNotNull(nameof(instance));
            manager.VerifyNotNull(nameof(manager));

            object proxyObject = Create<T, ActorProxy<T>>();

            ActorProxy<T> proxy = (ActorProxy<T>)proxyObject;
            proxy._instance = instance;
            proxy._manager = manager;
            proxy._workContext = context.WithActivity();

            return (T)proxyObject;
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                _lockSemaphore.Wait(_manager!.Configuration.ActorCallTimeout);
                return targetMethod.Invoke(_instance, args);
            }
            catch (Exception ex)
            {
                _workContext!.Telemetry.Error(_workContext, ex.Message, ex);
                throw;
            }
            finally
            {
                _lockSemaphore.Release();
            }
        }
    }
}
