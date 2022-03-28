using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UtilsUnknown.Exceptions
{
    [Serializable]
    public class MissingComponentException : Exception
    {
        public MissingComponentException() : base() { }
        public MissingComponentException(string message) : base(message) { }
        public MissingComponentException(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected MissingComponentException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}