﻿using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace FreeRedis.Client.Protocol.Serialization
{

    public delegate T DeserializeResponseDelegate<T>(string result);
    public delegate Span<byte> SerializeResponseDelegate<T>(T value);

    public static class ProtocolSerializer<T>
    {
        private static readonly byte[] _trueBytes = new byte[1] { 48 };
        private static readonly byte[] _falseBytes = new byte[1] { 49 };
        private static readonly byte[][] _byteArray;
        internal static readonly DeserializeResponseDelegate<T> ReadFunc = default!;
        internal static readonly SerializeResponseDelegate<T> WriteFunc = default!;
        static ProtocolSerializer()
        {
            _byteArray = new byte[byte.MaxValue - byte.MinValue][];
            for (byte i = byte.MinValue; i < byte.MaxValue; i++)
            {
                _byteArray[i - byte.MinValue] = new byte[1] { i };
            }
            var type = typeof(T);
            Action makeAction = () => { };

           if (type == typeof(byte))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<byte>.ReadFunc) = result => Convert.ToByte(result);
                    Unsafe.AsRef(ProtocolSerializer<byte>.WriteFunc) = value => _byteArray[value];
                };
            }
            else if (type == typeof(sbyte))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<sbyte>.ReadFunc) = result => Convert.ToSByte(result);
                    Unsafe.AsRef(ProtocolSerializer<sbyte>.WriteFunc) = value => _byteArray[Convert.ToByte(value)];
                };
            }
            else if (type == typeof(string))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<string>.ReadFunc) = (bytes) => Encoding.UTF8.GetString(bytes);
                    Unsafe.AsRef(ProtocolSerializer<string>.WriteFunc) = value => Encoding.UTF8.GetBytes(value);
                };

            }
            else if (type == typeof(int))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<int>.ReadFunc) = (bytes) => Convert.ToInt32(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<int>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteInt32LittleEndian(span, value);
                        return span;
                    };
                };
            }
            else if (type == typeof(short))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<short>.ReadFunc) = (bytes) => Convert.ToInt16(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<short>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteInt16LittleEndian(span, value);
                        return span;
                    };
                };
            }
            else if (type == typeof(long))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<long>.ReadFunc) = (bytes) => Convert.ToInt64(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<long>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteInt64LittleEndian(span, value);
                        return span;
                    };
                };
            }
            else if (type == typeof(uint))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<uint>.ReadFunc) = (bytes) => Convert.ToUInt32(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<uint>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteUInt32LittleEndian(span, value);
                        return span;
                    };
                };
            }
            else if (type == typeof(ushort))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<ushort>.ReadFunc) = (bytes) => Convert.ToUInt16(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<ushort>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteUInt16LittleEndian(span, value);
                        return span;
                    };
                };
            }
            else if (type == typeof(ulong))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<ulong>.ReadFunc) = (bytes) => Convert.ToUInt64(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<ulong>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteUInt64LittleEndian(span, value);
                        return span;
                    };
                };
            }
            else if (type == typeof(float))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<float>.ReadFunc) = (bytes) => Convert.ToSingle(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<float>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteSingleLittleEndian(span, value);
                        return span;
                    };
                };

            }
            else if (type == typeof(double))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<double>.ReadFunc) = (bytes) => Convert.ToDouble(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<double>.WriteFunc) = value =>
                    {
                        var span = Span<byte>.Empty;
                        BinaryPrimitives.WriteDoubleLittleEndian(span, value);
                        return span;
                    };
                };
            }
            else if (type == typeof(decimal))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<decimal>.ReadFunc) = (bytes) => Convert.ToDecimal(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<decimal>.WriteFunc) = value => Encoding.UTF8.GetBytes(value.ToString());
                };
            }
            else if (type == typeof(DateTime))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<DateTime>.ReadFunc) = (bytes) => Convert.ToDateTime(Encoding.UTF8.GetString(bytes));
                    Unsafe.AsRef(ProtocolSerializer<DateTime>.WriteFunc) = value => Encoding.UTF8.GetBytes(value.ToString());
                };
            }
            else if (type == typeof(bool))
            {
                makeAction = () =>
                {
                    Unsafe.AsRef(ProtocolSerializer<bool>.ReadFunc) = bytes => bytes[0] == 48;
                    Unsafe.AsRef(ProtocolSerializer<bool>.WriteFunc) = value => value ? _trueBytes : _falseBytes;
                };
            }
            else
            {
                ProtocolSerializer<T>.ReadFunc = bytes => JsonSerializer.Deserialize<T>(bytes)!;
                ProtocolSerializer<T>.WriteFunc = value => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)!);
            }
            makeAction();
        }

    }
}