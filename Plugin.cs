using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace EasySnap
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static FirstPersonController FPC { get; set; }
        public static PlayerInteraction Interaction { get; set; }
        public static FurniturePlacer Placer { get; set; }
        public static PlayerController Controller { get; set; }
        public static ConfigEntry<KeyboardShortcut> SnapKey { get; set; }
        public static ConfigEntry<float> GridSize { get; set; }
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded! Applying patch...");
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            SnapKey = Config.Bind("EasySnap", "Snap Key", new KeyboardShortcut(KeyCode.Tab), "What key to press to snap furniture.");
            GridSize = Config.Bind("EasySnap", "Grid Size", 1f, "How far apart the grid lines should be.");
        }
        private void Update()
        {
            if (FPC == null) return;
            if (Interaction == null) Interaction = FPC.GetComponent<PlayerInteraction>();
            if (Placer == null) Placer = FPC.GetComponent<FurniturePlacer>();
            if (Controller == null) Controller = FPC.GetComponent<PlayerController>();

            if(Placer != null && Placer.m_PlacingMode)
            {
                if(Input.GetKey(KeyCode.Tab))
                {
                    if(Placer.m_CurrentPlacingMode != null)
                    {
                        Vector3 current = Placer.m_CurrentPlacingMode.Furniture.position;
                        Vector3 rounded = new Vector3((int)((current.x + (GridSize.Value / 2)) / GridSize.Value) * GridSize.Value - (GridSize.Value / 2), current.y, (int)((current.z + (GridSize.Value / 2)) / GridSize.Value) * GridSize.Value - (GridSize.Value / 2));
                        Logger.LogInfo(rounded);
                        Placer.m_CurrentPlacingMode.Furniture.position = rounded;
                    }
                }
            }
        }
    }
    public static class EasySnapPatches
    {
        [HarmonyPatch(typeof(FirstPersonController), "Start")]
        public static class FirstPersonController_Start_Patch
        {
            public static void Prefix(FirstPersonController __instance)
            {
                Plugin.FPC = __instance;
            }
        }
    }
}
