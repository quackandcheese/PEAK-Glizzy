using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;

namespace Glizzy.Modules
{
    public static class NetworkPrefabs
    {
        internal static CustomPrefabPool CustomPrefabPool
        {
            get
            {
                _customPrefabPool ??= new CustomPrefabPool();
                return _customPrefabPool;
            }
            private set
            {
                _customPrefabPool = value;
            }
        }

        private static CustomPrefabPool? _customPrefabPool;

        internal static void Initialize()
        {
            if (PhotonNetwork.PrefabPool is CustomPrefabPool)
            {
                Plugin.Log.LogWarning("NetworkPrefabs failed to initialize. PhotonNetwork.PrefabPool is already a CustomPrefabPool.");
                return;
            }

            Plugin.Log.LogInfo($"Initializing NetworkPrefabs.");
            Plugin.Log.LogDebug($"PhotonNetwork.PrefabPool = {PhotonNetwork.PrefabPool.GetType()}");

            if (PhotonNetwork.PrefabPool is DefaultPool defaultPool)
            {
                CustomPrefabPool.DefaultPool = defaultPool;
            }
            else if (PhotonNetwork.PrefabPool is not Glizzy.CustomPrefabPool)
            {
                Plugin.Log.LogWarning($"PhotonNetwork has an unknown prefab pool assigned. PhotonNetwork.PrefabPool = {PhotonNetwork.PrefabPool.GetType()}");
            }

            PhotonNetwork.PrefabPool = CustomPrefabPool;

            Plugin.Log.LogInfo("Replaced PhotonNetwork.PrefabPool with CustomPrefabPool.");
            Plugin.Log.LogDebug($"PhotonNetwork.PrefabPool = {PhotonNetwork.PrefabPool.GetType()}");
            Plugin.Log.LogInfo($"Finished initializing NetworkPrefabs.");
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> as a network prefab.
        /// </summary>
        /// <param name="prefab">The <see cref="GameObject"/> to register.</param>
        public static void RegisterNetworkPrefab(GameObject prefab)
        {
            RegisterNetworkPrefab(prefab?.name!, prefab!);
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> as a network prefab.
        /// </summary>
        /// <param name="prefabId">The ID for this <see cref="GameObject"/>.</param>
        /// <param name="prefab">The <see cref="GameObject"/> to register.</param>
        public static void RegisterNetworkPrefab(string prefabId, GameObject prefab)
        {
            CustomPrefabPool.RegisterPrefab(prefabId, prefab);
        }

        /// <summary>
        /// Check if a <see cref="GameObject"/> with the specified ID is a network prefab.
        /// </summary>
        /// <param name="prefabId">The <see cref="GameObject"/> ID to check.</param>
        /// <returns>Whether or not the <see cref="GameObject"/> is a network prefab.</returns>
        public static bool HasNetworkPrefab(string prefabId)
        {
            return CustomPrefabPool.HasPrefab(prefabId);
        }

        /// <summary>
        /// Gets a network prefab.
        /// </summary>
        /// <param name="prefabId">The network prefab ID for the <see cref="GameObject"/>.</param>
        /// <returns>The <see cref="GameObject"/> or null.</returns>
        public static GameObject? GetNetworkPrefab(string prefabId)
        {
            return CustomPrefabPool.GetPrefab(prefabId);
        }

        /// <summary>
        /// Tries to get a network prefab.
        /// </summary>
        /// <param name="prefabId">The network prefab ID for the <see cref="GameObject"/>.</param>
        /// <param name="prefab">The network prefab <see cref="GameObject"/>.</param>
        /// <returns>Whether or not the <see cref="GameObject"/> was found.</returns>
        public static bool TryGetNetworkPrefab(string prefabId, out GameObject? prefab)
        {
            prefab = GetNetworkPrefab(prefabId);
            return prefab != null;
        }

        /// <summary>
        /// Spawns a network prefab.
        /// </summary>
        /// <param name="prefabId">The network prefab ID for the <see cref="GameObject"/> to spawn.</param>
        /// <param name="position">The position where the <see cref="GameObject"/> will be spawned.</param>
        /// <param name="rotation">The rotation of the <see cref="GameObject"/>.</param>
        /// <param name="group">The interest group. See: https://doc.photonengine.com/pun/current/gameplay/interestgroups</param>
        /// <param name="data">Custom instantiation data. See: https://doc.photonengine.com/pun/current/gameplay/instantiation#custom-instantiation-data</param>
        /// <returns>The spawned <see cref="GameObject"/> or null.</returns>
        public static GameObject? SpawnNetworkPrefab(string prefabId, Vector3 position, Quaternion rotation, byte group = 0, object[]? data = null)
        {
            if (string.IsNullOrWhiteSpace(prefabId))
            {
                Plugin.Log.LogError("Failed to spawn network prefab. PrefabId is null.");
                return null;
            }

            if (!HasNetworkPrefab(prefabId))
            {
                Plugin.Log.LogError($"Failed to spawn network prefab \"{prefabId}\". PrefabId is not registered as a network prefab.");
                return null;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                Plugin.Log.LogError($"Failed to spawn network prefab \"{prefabId}\". You are not the host.");
                return null;
            }
            return PhotonNetwork.InstantiateRoomObject(prefabId, position, rotation, group, data);
        }
    }
}
