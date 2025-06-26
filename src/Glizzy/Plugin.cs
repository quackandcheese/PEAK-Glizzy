using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PEAKLib.Core;
using PEAKLib.Items;
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
    internal static ModDefinition Definition { get; set; } = null!;

    private void Awake()
    {
        Instance = this;
        Log = Logger;
        Definition = ModDefinition.GetOrCreate(Info.Metadata);

        string AssetBundlePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "glizzy");
        Bundle = AssetBundle.LoadFromFile(AssetBundlePath);

        // Harmony patches are to fix issues with using a SkinnedMeshRenderer instead of a MeshRenderer for an item (it's p much hardcoded)
        Patch();

        Item glizzy = Bundle.LoadAsset<GameObject>("Glizzy.prefab").GetComponent<Item>();
        new ItemContent(glizzy).Register(Definition);

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
}