using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Sirenix.Utilities;
using UnityEngine;

namespace Glizzy.Patches
{
    [HarmonyPatch(typeof(Item))]
    internal class ItemPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Item.AddPropertyBlock))]
        static bool AddPropertyBlockPrefix(ref Item __instance)
        {
            __instance.mainRenderer = __instance.GetComponentInChildren<SkinnedMeshRenderer>();
            if (__instance.mainRenderer != null)
            {
                __instance.mpb = new MaterialPropertyBlock();
                __instance.mainRenderer.GetPropertyBlock(__instance.mpb);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Item.HoverEnter))]
        static bool HoverEnterPrefix(ref Item __instance)
        {
            __instance.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
            __instance.mainRenderer.SetPropertyBlock(__instance.mpb);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Item.HoverExit))]
        static bool HoverExit(ref Item __instance)
        {
            __instance.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
            __instance.mainRenderer.SetPropertyBlock(__instance.mpb);
            return false;
        }


        [HarmonyPostfix]
        [HarmonyPatch(nameof(Item.HideRenderers))]
        static void HideRenderersPostfix(ref Item __instance)
        {
            __instance.GetComponentsInChildren<SkinnedMeshRenderer>().ForEach(delegate (SkinnedMeshRenderer meshRenderer)
            {
                meshRenderer.enabled = false;
            });
        }
    }
}
