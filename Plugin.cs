using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace NOCounterMeasureKeybinds
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static private ConfigEntry<bool> deployInstant { get; set; }
        static private ConfigEntry<KeyboardShortcut> flareHotkey { get; set; }
        static private ConfigEntry<KeyboardShortcut> jamHotkey { get; set; }
        private int counterMeasureIndex;

        private void Awake()
        {
            deployInstant = Config.Bind("Counter measures", "Use countermeasure on hold", true, "Wether the countermeasure should be triggered when holding down the key.");
            flareHotkey = Config.Bind("Counter measures", "Flares", new KeyboardShortcut(KeyCode.None));
            jamHotkey = Config.Bind("Counter measures", "Jamming", new KeyboardShortcut(KeyCode.None));

            new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();
        }

        private void Update()
        {
            GameManager.GetLocalAircraft(out Aircraft aircraft);

            if (aircraft == null)
            {
                return;
            }

            if (flareHotkey.Value.IsPressed())
            {
                counterMeasureIndex = 0;
            }
            else if (jamHotkey.Value.IsPressed())
            {
                counterMeasureIndex = 1;
            }
            else
            {
                return;
            }

            if (aircraft.countermeasureManager.activeIndex != counterMeasureIndex)
            {
                aircraft.countermeasureManager.NextCountermeasure();
            }

            if (deployInstant.Value)
            {
                aircraft.countermeasureManager.DeployCountermeasure(aircraft);
            }
        }
    }
}
