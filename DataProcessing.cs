using BepInEx.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Paulov.UnityBepInExNetworking
{
    public class DataProcessing : MonoBehaviour
    {

        static ManualLogSource Logger { get; set; }

        public readonly BlockingCollection<IPaulovNetworkingPacket> Packets = new(9999);


        void Awake()
        {
            Logger.LogInfo($"{nameof(DataProcessing)}.Awake()");
            DontDestroyOnLoad(this);
            gameObject.name = nameof(DataProcessing);
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        void Update()
        {
            while (Packets.TryTake(out var packet))
            {
                try
                {
                    packet.Process();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        public void ProcessPacketBytes(byte[] data)
        {
            try
            {
                if (data == null)
                {
                    Logger.LogError($"{nameof(ProcessPacketBytes)}. Data is Null");
                    return;
                }

                if (data.Length == 0)
                {
                    Logger.LogError($"{nameof(ProcessPacketBytes)}. Data is Empty");
                    return;
                }

                IPaulovNetworkingPacket packet = null;
                ProcessPaulovPacket(data, out packet);

                if (packet != null)
                    Packets.Add(packet);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }



        private static List<Type> _replicationTypes { get; } = new List<Type>();

        private static Dictionary<int, Type> _replicationTypesToHash { get; } = new();

        static DataProcessing()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource($"{nameof(DataProcessing)}");
        }

        /// <summary>
        /// Setup the replication types. This is used to register the types that will be used for serialization and deserialization.
        /// </summary>
        /// <param name="replicationTypes"></param>
        public void SetupReplicationTypes(Type[] replicationTypes)
        {

            _replicationTypes.AddRange(replicationTypes);

            for (var i = 0; i < replicationTypes.Length; i++)
            {
                var hash = Fnv1a.HashString(_replicationTypes[i].Name);
                if (!_replicationTypesToHash.ContainsKey(hash))
                {
                    _replicationTypesToHash.Add(hash, _replicationTypes[i]);
                }
                else
                {
                    Logger.LogError($"Could not add {_replicationTypes[i]}, hash {hash} already exists...");
                }
            }
        }
        public void ProcessPaulovPacket(byte[] data, out IPaulovNetworkingPacket packet)
        {
            packet = null;

            // If the data is empty. Return;
            if (data == null || data.Length == 0)
                Logger.LogError($"{nameof(ProcessPaulovPacket)}. {nameof(data)} is null");

            var bp = new BasePacket();
            using (var br = new BinaryReader(new MemoryStream(data)))
                bp.ReadHeader(br);

            packet = DeserializeIntoPacket(data, packet, bp);
            bp = null;
        }

        /// <summary>
        /// Deserializes the data into a packet. This is used to deserialize the data into a packet.  
        /// </summary>
        /// <param name="data"></param>
        /// <param name="packet"></param>
        /// <param name="bp"></param>
        /// <returns></returns>
        private IPaulovNetworkingPacket DeserializeIntoPacket(byte[] data, IPaulovNetworkingPacket packet, BasePacket bp)
        {
            var sitPacketType = _replicationTypesToHash[bp.MethodHash];
            if (sitPacketType != null)
            {
                packet = (IPaulovNetworkingPacket)Activator.CreateInstance(sitPacketType);
                packet = packet.Deserialize(data);
            }
            else
            {
#if DEBUG
                Logger.LogDebug($"{nameof(DeserializeIntoPacket)}:{bp.Method} could not find a matching IPaulovPacket type");
#endif
            }

            return packet;
        }
    }
}
