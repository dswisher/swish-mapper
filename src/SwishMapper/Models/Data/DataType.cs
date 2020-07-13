
using System;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// The primitive types, mostly.
    /// </summary>
    public enum PrimitiveType
    {
        Boolean,
        Date,
        Enum,
        Int,
        String,
        Ref
    }

    /// <summary>
    /// Representation of an attribute's data type.
    /// </summary>
    public class DataType : IEquatable<DataType>
    {
        public DataType(PrimitiveType type)
        {
            Type = type;
        }


        public DataType(string refName)
        {
            if (string.IsNullOrEmpty(refName))
            {
                throw new ArgumentException("refName cannot be null!");
            }

            Type = PrimitiveType.Ref;
            RefName = refName;
        }


        public PrimitiveType Type { get; private set; }

        public string RefName { get; private set; }


        public static bool operator ==(DataType lhs, DataType rhs)
        {
            // Handle nulls on either side.
            if (object.ReferenceEquals(lhs, null))
            {
                if (object.ReferenceEquals(rhs, null))
                {
                    // null == null = true
                    return true;
                }

                // Only the left side is null
                return false;
            }

            return lhs.Equals(rhs);
        }


        public static bool operator !=(DataType lhs, DataType rhs)
        {
            return !(lhs == rhs);
        }


        public override string ToString()
        {
            if (Type == PrimitiveType.Ref)
            {
                return $"ref({RefName})";
            }
            else
            {
                return Type.ToString().ToLower();
            }
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as DataType);
        }


        public bool Equals(DataType other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if ((RefName != null) && (other.RefName != null))
            {
                return RefName.Equals(other.RefName);
            }

            return Type.Equals(other.Type);
        }


        public override int GetHashCode()
        {
            if (RefName != null)
            {
                return RefName.GetHashCode();
            }

            return Type.GetHashCode();
        }
    }
}
