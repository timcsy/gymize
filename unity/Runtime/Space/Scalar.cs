using System;
using Gymize.Protobuf;

namespace Gymize
{
    public enum ScalarType
    {
        UNSPECIFIED,
        DISCRETE,
        FLOAT,
        BOOL,
        NULL
    }

    public class Scalar : IInstance
    {
        private ScalarType m_Type;
        public ScalarType Type { get { return m_Type; } }

        public object Value
        {
            get
            {
                switch (m_Type)
                {
                    case ScalarType.DISCRETE: return m_Integer;
                    case ScalarType.FLOAT: return m_Float;
                    case ScalarType.BOOL: return m_Boolean;
                    case ScalarType.NULL: return null;
                    default: throw new InvalidOperationException("Unknown scalar type");
                }
            }
            set
            {
                Scalar scalar = value as Scalar;
                Enum e = value as Enum;

                if (scalar != null)
                {
                    m_Type = scalar.Type;
                    m_Integer = scalar.Integer;
                    m_Float = scalar.Float;
                    m_Boolean = scalar.Boolean;
                }
                else if (value == null)
                {
                    m_Type = ScalarType.NULL;
                }
                else if (value is bool)
                {
                    m_Type = ScalarType.BOOL;
                    m_Boolean = Convert.ToBoolean(value);
                }
                else if (e != null
                    || value is sbyte
                    || value is short
                    || value is int
                    || value is long
                    || value is byte
                    || value is ushort
                    || value is char
                    || value is uint
                    || value is ulong
                )
                {
                    m_Type = ScalarType.DISCRETE;
                    m_Integer = Convert.ToInt64(value);
                }
                else if (value is float
                    || value is double
                    || value is decimal
                )
                {
                    m_Type = ScalarType.FLOAT;
                    m_Float = Convert.ToDouble(value);
                }
                else throw new InvalidOperationException("Unknown scalar type");
            }
        }

        private long m_Integer;
        public long Integer
        {
            get { return m_Integer; }
            set { m_Integer = value; }
        }

        private double m_Float;
        public double Float
        {
            get { return m_Float; }
            set { m_Float = value; }
        }

        private bool m_Boolean;
        public bool Boolean
        {
            get { return m_Boolean; }
            set { m_Boolean = value; }
        }

        public Scalar()
        {
            m_Type = ScalarType.UNSPECIFIED;
        }
        public Scalar(object obj)
        {
            Value = obj;
        }

        public InstanceProto ToProtobuf()
        {
            InstanceProto instance = new InstanceProto();
            switch (m_Type)
            {
                case ScalarType.DISCRETE:
                    instance.Type = InstanceTypeProto.Discrete;
                    instance.Discrete = m_Integer;
                    break;
                case ScalarType.FLOAT:
                    instance.Type = InstanceTypeProto.Float;
                    instance.Float = m_Float;
                    break;
                case ScalarType.BOOL:
                    instance.Type = InstanceTypeProto.Bool;
                    instance.Boolean = m_Boolean;
                    break;
                case ScalarType.NULL:
                    instance.Type = InstanceTypeProto.Null;
                    break;
                default: throw new InvalidOperationException("Unknown scalar type");
            }
            return instance;
        }

        public static object ParseFrom(object obj)
        {
            Scalar scalar = new Scalar(obj);
            return scalar.Value;
        }

        public override string ToString()
        {
            switch (m_Type)
            {
                case ScalarType.DISCRETE: return m_Integer.ToString();
                case ScalarType.FLOAT: return m_Float.ToString();
                case ScalarType.BOOL: return m_Boolean.ToString();
                case ScalarType.NULL: return "null";
                default: throw new InvalidOperationException("Unknown scalar type");
            }
        }
    }
}