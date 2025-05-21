using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Paulov.UnityBepInExNetworking
{
    public class BasePacket : IPaulovNetworkingPacket, IDisposable
    {
        [JsonProperty(PropertyName = "m")]
        public virtual string Method
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// Used on Deserialize
        /// </summary>
        public virtual int MethodHash { get; set; }

        public BasePacket()
        {
        }

        public static Dictionary<Type, PropertyInfo[]> TypeProperties = new();
        protected bool disposedValue;

        public static PropertyInfo[] GetPropertyInfos(IPaulovNetworkingPacket packet)
        {
            return GetPropertyInfos(packet.GetType());
        }

        public static PropertyInfo[] GetPropertyInfos(Type t)
        {
            if (!TypeProperties.ContainsKey(t))
            {
                var allProps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanRead && x.CanWrite);
                var allPropsFiltered = allProps
                  .Where(x => x.Name != "Method");
                TypeProperties.Add(t, allPropsFiltered.ToArray());
            }

            return TypeProperties[t];
        }

        public static void WriteHeader(BinaryWriter writer, IPaulovNetworkingPacket packet)
        {

        }

        public virtual void WriteHeader(BinaryWriter writer)
        {
            writer.Write(Fnv1a.HashString(Method));
        }

        public virtual void ReadHeader(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            MethodHash = reader.ReadInt();
        }

        /// <summary>
        /// Serializes the BasePacket header. All inherited Packet classes must override this.
        /// </summary>
        /// <returns></returns>
        public virtual byte[] Serialize()
        {
            var ms = new MemoryStream();
            using BinaryWriter writer = new(ms);
            WriteHeader(writer);
            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes the BasePacket header. All inherited Packet classes must override this.
        /// </summary>
        /// <returns></returns>
        public virtual IPaulovNetworkingPacket Deserialize(byte[] bytes)
        {
            using BinaryReader reader = new(new MemoryStream(bytes));
            ReadHeader(reader);
            return this;
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Serialize());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    //ServerId.Clear();
                    //ServerId = null;

                    //Method.Clear();
                    //Method = null;

                    //TimeSerializedBetter.Clear();
                    //TimeSerializedBetter = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Process this packet. Overridable for other packets to provide their own handlers.
        /// </summary>
        public virtual void Process()
        {
            Dispose();
        }
    }


    public static class SerializerExtensions
    {
        public const char Paulov_SERIALIZATION_PACKET_SEPERATOR = '£';
        private static Dictionary<Type, PropertyInfo[]> TypeToPropertyInfos { get; } = new();

        static SerializerExtensions()
        {
            // Use the static Serializer Extensions to pre populate all Network Packet Property Infos
            var sitPacketTypes = Assembly.GetAssembly(typeof(IPaulovNetworkingPacket))
                .GetTypes()
                .Where(x => x.GetInterface("IPaulovPacket") != null);
            foreach (var packetType in sitPacketTypes)
            {
                TypeToPropertyInfos.Add(packetType, BasePacket.GetPropertyInfos(packetType));
            }

            Plugin.Logger.LogDebug($"{TypeToPropertyInfos.Count} IPaulovPacket types found");
        }

        public unsafe static void Clear(this string s)
        {
            if (s == null)
                return;

            fixed (char* ptr = s)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    ptr[i] = '\0';
                }
            }
        }

        public static void WriteNonPrefixedString(this BinaryWriter binaryWriter, string value)
        {
            binaryWriter.Write(Encoding.UTF8.GetBytes(value));
        }

        public static void WriteLengthPrefixedBytes(this BinaryWriter binaryWriter, byte[] value)
        {
            binaryWriter.Write((int)value.Length);

            //StayInTarkovHelperConstants.Logger.LogDebug($"{nameof(SerializerExtensions)},{nameof(WriteLengthPrefixedBytes)},Write Length {value.Length}");

            binaryWriter.Write(value);
        }

        public static byte[] ReadLengthPrefixedBytes(this BinaryReader binaryReader)
        {
            var length = binaryReader.ReadInt32();

            //StayInTarkovHelperConstants.Logger.LogDebug($"{nameof(SerializerExtensions)},{nameof(ReadLengthPrefixedBytes)},Read Length {length}");

            if (length + binaryReader.BaseStream.Position <= binaryReader.BaseStream.Length)
                return binaryReader.ReadBytes(length);
            else
                return null;
        }

        public static float ReadFloat(this BinaryReader binaryReader)
        {
            return binaryReader.ReadSingle();
        }

        public static int ReadInt(this BinaryReader binaryReader)
        {
            return binaryReader.ReadInt32();
        }

        public static int ReadShort(this BinaryReader binaryReader)
        {
            return binaryReader.ReadInt16();
        }

        public static bool ReadBool(this BinaryReader binaryReader)
        {
            return binaryReader.ReadBoolean();
        }

        // INetSerializable Extensions


        public static void Put(this BinaryWriter binaryWriter, int value)
        {
            binaryWriter.Write(value);
        }

        public static void Put(this BinaryWriter binaryWriter, string value)
        {
            binaryWriter.Write(value);
        }

        public static void Put(this BinaryWriter binaryWriter, bool value)
        {
            binaryWriter.Write(value);
        }

        public static void Put(this BinaryWriter binaryWriter, byte[] value)
        {
            binaryWriter.Write(value);
        }

        public static int GetInt(this BinaryReader binaryReader)
        {
            return binaryReader.ReadInt32();
        }

        public static string GetString(this BinaryReader binaryReader)
        {
            return binaryReader.ReadString();
        }

        public static bool GetBool(this BinaryReader binaryReader)
        {
            return binaryReader.ReadBoolean();
        }

        public static byte[] GetBytes(this BinaryReader binaryReader, byte[] bytes, int length)
        {
            bytes = binaryReader.GetBytes(length);
            return bytes;
        }

        public static byte[] GetBytes(this BinaryReader binaryReader, int length)
        {
            return binaryReader.ReadBytes(length);
        }


        public static Stopwatch swDeserializerDebug = new();


    }


}
