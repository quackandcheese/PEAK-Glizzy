using HarmonyLib;
using UnityEngine;
//using MonoDetour;
//using PEAKLib.Core;
//using MonoDetour.HookGen;
//using On.ItemCooking;

namespace Glizzy.Patches;

[HarmonyPatch(typeof(ItemCooking))]
internal class ItemCookingPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ItemCooking.UpdateCookedBehavior))]
    static bool UpdateCookedBehaviorPrefix(ref ItemCooking __instance)
    {
        if (!__instance.setup)
        {
            __instance.setup = true;
            __instance.renderers = __instance.GetComponentsInChildren<MeshRenderer>();
            Renderer[] combinedRenderers = GetCombinedRenderers(__instance);
            __instance.defaultTints = new Color[combinedRenderers.Length];
            for (int i = 0; i < combinedRenderers.Length; i++)
            {
                __instance.defaultTints[i] = combinedRenderers[i].material.GetColor("_Tint");
            }
        }
        return true;
    }
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ItemCooking.CookVisually))]
    static bool CookVisuallyPrefix(ref ItemCooking __instance, int cookedAmount)
    {
        Renderer[] combined = GetCombinedRenderers(__instance);
        for (int i = 0; i < combined.Length; i++)
        {
            if (cookedAmount > 0)
            {
                Debug.Log(string.Format("Cooked amount is {0}", cookedAmount));
                combined[i].material.SetColor("_Tint", __instance.defaultTints[i] * ItemCooking.GetCookColor(cookedAmount));
            }
        }
        return false;
    }

    private static Renderer[] GetCombinedRenderers(ItemCooking __instance)
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = __instance.GetComponentsInChildren<SkinnedMeshRenderer>();
        Renderer[] combinedRenderers = [];
        combinedRenderers = combinedRenderers.AddRangeToArray(__instance.renderers);
        combinedRenderers = combinedRenderers.AddRangeToArray(skinnedMeshRenderers);
        return combinedRenderers;
    }
}

//[MonoDetourTargets(typeof(ItemCooking))]
//static class ItemCookingHooks
//{
//    [MonoDetourHookInitialize]
//    static void Init()
//    {
//        UpdateCookedBehavior.Prefix(Prefix_UpdateCookedBehavior);
//        CookVisually.Prefix(Prefix_CookVisually);
//    }

//    static void Prefix_UpdateCookedBehavior(ItemCooking self)
//    {

//    }
//    static void Prefix_CookVisually(ItemCooking self, ref int cookedAmount)
//    {

//    }


//    static Renderer[] GetCombinedRenderers(ItemCooking self)
//    {
//        SkinnedMeshRenderer[] skinnedMeshRenderers = self.GetComponentsInChildren<SkinnedMeshRenderer>();
//        Renderer[] combinedRenderers = [];
//        combinedRenderers = combinedRenderers.AddRangeToArray(self.renderers);
//        combinedRenderers = combinedRenderers.AddRangeToArray(skinnedMeshRenderers);
//        return combinedRenderers;
//    }
//}