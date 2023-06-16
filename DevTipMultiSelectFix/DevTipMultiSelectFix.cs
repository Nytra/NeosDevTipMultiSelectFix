using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace DevTipMultiSelectFix
{
    public class DevTipMultiSelectFix : NeosMod
    {
        public override string Name => "DevTipMultiSelectFix";
        public override string Author => "Nytra";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/Nytra/NeosDevTipMultiSelectFix";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("owo.Nytra.DevTipMultiSelectFix");
            harmony.PatchAll();
        }
		
        [HarmonyPatch(typeof(DevToolTip), "TryOpenGizmo")]
        class DevTipMultiSelectFixPatch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                bool found = false;
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldloc_S && codes[i+1].opcode == OpCodes.Callvirt && codes[i+2].opcode == OpCodes.Callvirt)
                    {
                        Msg("Found pattern! Inserting instructions!");
                        found = true;
                        var instructionsToInsert = new List<CodeInstruction>
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, typeof(DevToolTip).GetField("_currentGizmo", BindingFlags.NonPublic | BindingFlags.Instance)),
                            new CodeInstruction(OpCodes.Ldnull),
                            new CodeInstruction(OpCodes.Callvirt, typeof(SyncRef).GetMethod("set_Target", BindingFlags.Public | BindingFlags.Instance))
                        };
                        codes.InsertRange(i, instructionsToInsert);
                        break;
                    }
                }
                if (!found)
                {
                    Msg("Did not find pattern! Something went wrong!");
                }
                return codes.AsEnumerable();
            }
        }
    }
}