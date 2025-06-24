using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Glizzy.Patches
{
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
}
