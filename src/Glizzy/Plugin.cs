using BepInEx;
using BepInEx.Logging;
using Glizzy.Modules;
using HarmonyLib;
using Photon.Pun;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

namespace Glizzy;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }
    internal static AssetBundle Bundle { get; set; } = null!;

    private bool patchedAwake = false;

    private void Awake()
    {
        Log = Logger;
        Instance = this;
        string AssetBundlePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "glizzy");
        Bundle = AssetBundle.LoadFromFile(AssetBundlePath);

        // Harmony patches are to fix issues with using a SkinnedMeshRenderer instead of a MeshRenderer for an item (it's p much hardcoded)
        Patch();

        Item glizzy = Bundle.LoadAsset<GameObject>("Glizzy.prefab").GetComponent<Item>();
        RegisterItem(glizzy);

        On.GameHandler.Awake += GameHandler_Awake;
        On.ItemDatabase.OnLoaded += ItemDatabase_OnLoaded;

        // Log our awake here so we can see it in LogOutput.log file
        Log.LogInfo($"Plugin {Name} is loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(Plugin.Instance.Info.Metadata.GUID);

        Log.LogDebug("Patching...");

        Harmony.PatchAll();

        Log.LogDebug("Finished patching!");
    }

    // Item registration should be moved into its own module like in REPOLib, this was just the quick and dirty way of doing it (lazy)
    private List<Item> itemsToRegister = new List<Item>();

    private void RegisterItem(Item item)
    {
        itemsToRegister.Add(item);
        NetworkPrefabs.RegisterNetworkPrefab("0_Items/" + item.gameObject.name, item.gameObject);
    }

    private void GameHandler_Awake(On.GameHandler.orig_Awake orig, GameHandler self)
    {
        orig(self);

        if (!patchedAwake)
        {
            Log.LogInfo("Patching GameHandler.Awake");
            patchedAwake = true;
            // Patch the Awake method to initialize our custom prefab pool
            NetworkPrefabs.Initialize();
        }
    }

    private void ItemDatabase_OnLoaded(On.ItemDatabase.orig_OnLoaded orig, ItemDatabase self)
    {
        Shader peakShader = Shader.Find("W/Peak_Standard");
        foreach (var item in itemsToRegister)
        {
            // Bad way to choose an id -- should probably replace with some kind of automatic hashing?
            ushort id = (ushort)(self.itemLookup.Count);
            item.itemID = id;

            foreach(Renderer renderer in item.GetComponentsInChildren<Renderer>())
            {
                // Replace dummy shader
                renderer.material.shader = peakShader;
            }
            // Fix smoke
            item.gameObject.GetComponentInChildren<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material = Resources.FindObjectsOfTypeAll<Material>().ToList().Find(x => x.name == "Smoke");
            
            // Add item to database
            self.Objects.Add(item);
            self.itemLookup.Add(id, item);
        }        

        orig(self);
    }
}