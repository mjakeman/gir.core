﻿using System;
using System.Collections.Generic;

namespace GObject
{
    public partial class Object
    {
        internal sealed class ClosureRegistry : IDisposable
        {
            #region Fields

            private readonly string _name;
            private readonly Object _object;
            private readonly Dictionary<object, (ulong, ClosureHelper)> _connectedHandlers = new();

            #endregion

            #region Constructors

            public ClosureRegistry(Object obj, string name)
            {
                _name = name;
                _object = obj;
            }

            #endregion

            #region Methods

            public void Connect<T>(T action, bool after, Func<T, ActionRefValues> mapping) where T : Delegate
            {
                if (_connectedHandlers.ContainsKey(action))
                    return;

                var closureHelper = new ClosureHelper(mapping(action));

                if (closureHelper.Handle is null)
                    throw new Exception("Closure handle is invalid");

                var handlerId = Native.Functions.SignalConnectClosure(_object.Handle, _name, closureHelper.Handle, after);

                if (handlerId == 0)
                {
                    closureHelper.Dispose();
                    throw new Exception($"Could not connect to event {_name}");
                }

                _connectedHandlers.Add(action, (handlerId, closureHelper));
            }

            public void Disconnect(object callback)
            {
                if (_connectedHandlers.TryGetValue(callback, out (ulong, ClosureHelper) data))
                {
                    Native.Functions.SignalHandlerDisconnect(_object.Handle, data.Item1);
                    data.Item2.Dispose();
                    _connectedHandlers.Remove(data);
                }
            }

            #endregion

            public void Dispose()
            {
                foreach (var (_, closureHelper) in _connectedHandlers.Values)
                    closureHelper.Dispose();
            }
        }
    }
}
