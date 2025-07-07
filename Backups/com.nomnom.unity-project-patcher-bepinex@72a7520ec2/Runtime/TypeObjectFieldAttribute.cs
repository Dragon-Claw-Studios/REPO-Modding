using System;
using UnityEngine;

namespace Nomnom {
    public sealed class TypeObjectFieldAttribute: PropertyAttribute {
        public readonly string FullTypeName;
        
        public TypeObjectFieldAttribute(string fullTypeName) {
            FullTypeName = fullTypeName;
        }
    }
}