﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.R.Components.Test.Fakes.Undo {
    /// <summary>
    /// This class is the same as WeakReference, except for its implementation of Object.GetHashCode() and Object.Equals().
    /// In both of those cases, it makes a best attempt at emulating the behavior of the target object, so that when
    /// WeakReferences are placed within a dictionary, as key values, they are properly recognized as references to objects
    /// that are the different (or the same).
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class WeakReferenceForDictionaryKey : WeakReference {
        private readonly int _hashCode;
        private const int XorWeakReference = 0xCCCC;

        public WeakReferenceForDictionaryKey(object target)
            : base(target) {
            if (target != null)
                _hashCode = target.GetHashCode() ^ XorWeakReference;
        }

        protected WeakReferenceForDictionaryKey(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        /// <summary>
        /// This hash code is informed by (but not the same as) the target object, and is constant for the life
        /// of the WeakReference.
        /// </summary>
        /// <returns>A hash code for the current Object.</returns>
        public override int GetHashCode() {
            return _hashCode;
        }

        /// <summary>
        /// Equality is trickier than the Hashcode, because of the possibility that the target of either WeakReference
        /// has been GC'ed or finalized. In those cases, when it is not known that the references must have refered to
        /// the same object, Equals() returns false. Otherwise, it returns the same as the underlying Equals in the
        /// target objects.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise false.</returns>
        public override bool Equals(object obj) {
            bool result = false;
            WeakReferenceForDictionaryKey other = obj as WeakReferenceForDictionaryKey;

            if (ReferenceEquals(other, null)) {
                result = false;
            } else if (ReferenceEquals(this, other)) {
                result = true;
            } else {
                object thisObj = null;
                object otherObj = null;

                try {
                    thisObj = this.Target;
                    otherObj = other.Target;
                } catch (InvalidOperationException) {
                }

                if (thisObj == null || otherObj == null) {
                    // these objects either refered to null (is that even possible?) or refer to
                    // something that has been GC'ed/finalized. the latter case is the important
                    // scenario for us, and we'll return false, in effect causing each GC'ed
                    // weak reference to appear to be equal only to itself.
                    result = false;
                } else {
                    result = object.Equals(thisObj, otherObj);
                }
            }

            return result;
        }
    }
}