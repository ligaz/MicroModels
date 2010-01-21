using System;
using System.Collections.Generic;
using System.Linq;
using MicroModels.Description;

namespace MicroModels.Utilities
{
    internal class TypeSignature : IEquatable<TypeSignature>
    {
        private readonly int hashCode;

        public TypeSignature(IEnumerable<PropertyDescriptor> properties)
        {
            this.hashCode = 0;
            foreach (var property in properties.OrderBy(p => p.Name))
            {
                this.hashCode ^= property.Name.GetHashCode() ^ property.PropertyType.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            return ((obj is TypeSignature) && this.Equals((TypeSignature)obj));
        }

        public bool Equals(TypeSignature other)
        {
            return this.hashCode.Equals(other.hashCode);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
