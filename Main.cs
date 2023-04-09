using HarmonyLib;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityModManagerNet;

namespace RoomTimer
{
#if DEBUG
    [EnableReloading]
#endif
    internal static class Main
    {
        public static Harmony harmony;
        public static UnityModManager.ModEntry.ModLogger logger;

        public static float roomLoadTime = Time.time;

        private static Vector3 timerPosition = new Vector3(-16.36f, 9f, 0f);

        static void Load(UnityModManager.ModEntry modEntry)
        {
            logger = modEntry.Logger;

            modEntry.OnUpdate = OnUpdate;
            modEntry.OnUnload = Unload;
            modEntry.OnToggle = OnToggle;

            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();

            ActivateTimer();

        }
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            AccessTools.Method(typeof(GameBoardLogic), "_TimerDismissComplete").Invoke(PT2.game_board, new object[] { });

            harmony.UnpatchAll(modEntry.Info.Id);

            return true;
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool active)
        {
            if (active) { Load(modEntry); }
            else { return Unload(modEntry); }
            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - Main.roomLoadTime);
            string text = string.Format("{0:D2}:{1:D2}.{2:D2}", timeSpan.Minutes, timeSpan.Seconds, Mathf.FloorToInt((float)timeSpan.Milliseconds / 10f));

            Regex regex = new Regex(@"^[0:]+(?!\.)");
            text = regex.Replace(text, "<color=#999999;\">$&</color>");

            if (!PT2.level_load_in_progress)
                PT2.game_board.timer_text.text = text;
        }

        //public static void DisplayFrames()
        //{
        //    int frames = (PT2.director.global_timer - Main.roomLoadTime + 59) % 60 + 1;
        //    PT2.item_gen.DisplayNumber(PT2.gale_script.GetTransform().position, frames, DamageNumberLogic.DISPLAY_STYLE.GROW_AND_FADE, null);
        //}

        public static void ActivateTimer()
        {
            FieldInfo timerActiveField = AccessTools.Field(typeof(GameBoardLogic), "_timer_is_active");
            timerActiveField.SetValue(PT2.game_board, true);
            PT2.game_board.timer_transform.localPosition = timerPosition;
            //PT2.game_board.timer_transform.localScale = 0.8f*Vector3.one;
        }


    }

    [HarmonyPatch(typeof(PT2), "LoadLevel")]
    public class LoadPatch
    {
        public static void Prefix()
        {
            //Main.DisplayFrames();
        }
    }

    [HarmonyPatch(typeof(PT2), "_LoadLevelCallback")]
    public class LoadRealPatch
    {
        public static void Prefix()
        {
            Main.roomLoadTime = Time.time;
        }
    }

    [HarmonyPatch(typeof(PT2), "GIS_ProcessInstructions")]
    public class GISPatch
    {
        public static void Prefix()
        {
        }
    }

    [HarmonyPatch(typeof(GameBoardLogic), "DismissOnLevelLoad")]
    public class DismissPatch
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PT2), "LoadLevel")]
    public static class LoadLevel_Patch
    {
        public static void Postfix()
        {
            Main.ActivateTimer();
        }
    }
}