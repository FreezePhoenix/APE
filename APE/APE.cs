using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using VNyanInterface;

namespace APE {
    public class ActionWrapper<T> {
        public bool Valid;
        public Action<T> Action;
        public ActionWrapper(Action<T> action) {
            Action = action;
            Valid = true;
        }
        public ActionWrapper() => Valid = true;
        public void Add(Action<T> action) => Action += action;
        public void Remove(Action<T> action) => Action -= action;
        public void Invoke(T value) => Action?.Invoke(value);
    }
    public class APE : MonoBehaviour, VNyanInterface.IButtonClickedHandler {
        static Dictionary<string, Dictionary<string, string>> DictionaryParams;
        static Dictionary<string, List<string>> TArrayParams;
        static Dictionary<string, float> NumParams;
        static Dictionary<string, string> StringParams;
        static Dictionary<string, LEKNLMJJGED> Timers;
        static readonly Dictionary<string, ActionWrapper<OCEONADJDLM>> TriggerChannels = new Dictionary<string, ActionWrapper<OCEONADJDLM>>(StringComparer.OrdinalIgnoreCase);
        public static ActionWrapper<OCEONADJDLM> GetActionWrapper(string name) {
            if (!TriggerChannels.TryGetValue(name, out ActionWrapper<OCEONADJDLM> wrapper)) {
                TriggerChannels.Add(name, wrapper = new ActionWrapper<OCEONADJDLM>());
            }
            return wrapper;
        }
        public static void AddActionWrapper(string name, Action<OCEONADJDLM> action) {
            if (!TriggerChannels.TryGetValue(name, out ActionWrapper<OCEONADJDLM> wrapper)) {
                TriggerChannels.Add(name, wrapper = new ActionWrapper<OCEONADJDLM>());
            }
            wrapper.Add(action);
        }
        public static void RemoveActionWrapper(string name, Action<OCEONADJDLM> action) {
            if (TriggerChannels.TryGetValue(name, out ActionWrapper<OCEONADJDLM> wrapper)) {
                wrapper.Remove(action);
            }
        }
        public void Awake() {
            try {
                var ParamTraverse = Traverse.Create(ParamSystem.getInstance());
                DictionaryParams = ParamTraverse.Field("DLCKJNHBNIB").GetValue<Dictionary<string, Dictionary<string, string>>>();
                TArrayParams = ParamTraverse.Field("ODOPIMCAPOK").GetValue<Dictionary<string, List<string>>>();
                NumParams = ParamTraverse.Field("HKKDLJBILOE").GetValue<Dictionary<string, float>>();
                StringParams = ParamTraverse.Field("MOELGFIKMJI").GetValue<Dictionary<string, string>>();
                var TimerField = Traverse.Create(TimerSystem.getInstance()).Field("KPFCLPCENJI");
                TimerField.SetValue(Timers = new Dictionary<string, LEKNLMJJGED>(TimerField.GetValue<Dictionary<string, LEKNLMJJGED>>(), StringComparer.OrdinalIgnoreCase));
                Harmony.CreateAndPatchAll(typeof(TriggerSystemPatches));
                Harmony.CreateAndPatchAll(typeof(TriggerNodePatches));
                Harmony.CreateAndPatchAll(typeof(SplitTParamNodePatches));
                Harmony.CreateAndPatchAll(typeof(TArrayValueNodePatches));
                Harmony.CreateAndPatchAll(typeof(CallTriggerNodePatches));
                Harmony.CreateAndPatchAll(typeof(ParamSystemPatches));
                Harmony.CreateAndPatchAll(typeof(DictionaryValueNodePatches));
                Harmony.CreateAndPatchAll(typeof(UnpackDictionaryNodePatches));
                Harmony.CreateAndPatchAll(typeof(SetTArrayNodePatches));
                Harmony.CreateAndPatchAll(typeof(TextSubstringNodePatches));
            } catch (Exception e) {
                FileLog.Log(e.ToString());
            }
            VNyanInterface.VNyanInterface.VNyanUI.registerPluginButton("Aria's Performance Enhancements (APE)", this);
        }
        public void pluginButtonClicked() { }
        public void Start() { }

        [HarmonyPatch(typeof(CallTriggerNode))]
        public static class CallTriggerNodePatches {
            [HarmonyPatch(nameof(CallTriggerNode.OnDeserialize))]
            [HarmonyPostfix]
            static void DeserializeHook(CallTriggerNode __instance) {
                __instance.GetComponent<CacheActionComponent>().value = GetActionWrapper(__instance.triggerName.text);
            }

            [HarmonyPatch(nameof(CallTriggerNode.Setup))]
            [HarmonyPostfix]
            public static void SetupHook(CallTriggerNode __instance) {
                var customComponent = __instance.gameObject.AddComponent<CacheActionComponent>();
                __instance.triggerName.onValueChanged.AddListener(newValue => customComponent.value = GetActionWrapper(newValue));
            }
            [HarmonyPatch("OBHKFAJCAIJ")]
            [HarmonyPrefix]
            public static bool ActivatePatch(CallTriggerNode __instance,
                                             List<HOODKEPNMHB> ___OLOOJHEPJNL,
                                             ref bool ___LBOGANBLCDD,
                                             ref bool ___EMDALIKOCEF,
                                             HOODKEPNMHB ___BLKJOPOPJAB,
                                             HOODKEPNMHB ___PNAENFDBOKF,
                                             HOODKEPNMHB ___AALJIFAPEFE,
                                             HOODKEPNMHB ___NEGGMGCNGEI,
                                             HOODKEPNMHB ___CAOIDMJMGFH,
                                             HOODKEPNMHB ___JDPONDHMPFH,
                                             HOODKEPNMHB ___LBPLNNAMMJF) {
                foreach (HOODKEPNMHB callback in ___OLOOJHEPJNL) {
                    OCEONADJDLM oCEONADJDLM = callback.GetValue<OCEONADJDLM>();
                    if (oCEONADJDLM != null) {
                        string text = __instance.triggerName.text;
                        if (___LBOGANBLCDD) {
                            ___EMDALIKOCEF = text.Contains("[") || text.Contains("{") || text.Contains("<");
                            ___LBOGANBLCDD = false;
                        }
                        if (___EMDALIKOCEF) {
                            text = ParamSystemPatches.InsertParamValues(text, oCEONADJDLM);
                        }
                        if (___BLKJOPOPJAB != null) {
                            var obj = ___BLKJOPOPJAB.GetValue<string>();
                            if (obj != null) {
                                text = obj;
                            }
                        } else if (__instance.callTime.value == 1) {
                            var cacheComponent = __instance.GetComponent<CacheActionComponent>();
                            OCEONADJDLM obj = new OCEONADJDLM() {
                                Type = 0,
                                Value = text,
                                Parameter1 = ___PNAENFDBOKF?.GetValue<int>() ?? 0,
                                Parameter2 = ___AALJIFAPEFE?.GetValue<int>() ?? 0,
                                Parameter3 = ___NEGGMGCNGEI?.GetValue<int>() ?? 0,
                                Message = ___CAOIDMJMGFH?.GetValue<string>() ?? "",
                                Message2 = ___JDPONDHMPFH?.GetValue<string>() ?? "",
                                Message3 = ___LBPLNNAMMJF?.GetValue<string>() ?? ""
                            };
                            cacheComponent.value.Invoke(obj);
                            cacheComponent.value.Invoke(null);
                            return false;
                        }
                        TriggerSystem.LAEDKDAOFEP lAEDKDAOFEP = new TriggerSystem.LAEDKDAOFEP() {
                            triggerName = text,
                            value1 = ___PNAENFDBOKF?.GetValue<int>() ?? 0,
                            value2 = ___AALJIFAPEFE?.GetValue<int>() ?? 0,
                            value3 = ___NEGGMGCNGEI?.GetValue<int>() ?? 0,
                            text1 = ___CAOIDMJMGFH?.GetValue<string>() ?? "",
                            text2 = ___JDPONDHMPFH?.GetValue<string>() ?? "",
                            text3 = ___LBPLNNAMMJF?.GetValue<string>() ?? ""
                        };
                        switch (__instance.callTime.value) {
                            case 0:
                                TriggerSystem.getInstance().AddTriggerItem(lAEDKDAOFEP);
                                break;
                            case 1:
                                TriggerSystem.getInstance().CallTrigger(lAEDKDAOFEP);
                                break;
                        }
                        return false;
                    }
                }
                return false;
            }

        }
        [HarmonyPatch(typeof(TriggerSystem))]
        public static class TriggerSystemPatches {
            [HarmonyPatch(nameof(TriggerSystem.CallTrigger))]
            [HarmonyPrefix]
            public static bool CallTriggerHook(TriggerSystem __instance, TriggerSystem.LAEDKDAOFEP EDNJHABGGKD) {
                if (TriggerChannels.TryGetValue(EDNJHABGGKD.triggerName, out var OnTrigger)) {
                    OnTrigger.Invoke(new OCEONADJDLM() {
                        Type = 0,
                        Value = EDNJHABGGKD.triggerName,
                        Parameter1 = EDNJHABGGKD.value1,
                        Parameter2 = EDNJHABGGKD.value2,
                        Parameter3 = EDNJHABGGKD.value3,
                        Message = EDNJHABGGKD.text1,
                        Message2 = EDNJHABGGKD.text2,
                        Message3 = EDNJHABGGKD.text3
                    });
                    OnTrigger.Invoke(null);
                }
                foreach (ITriggerHandler interfaceTrigger in __instance.interfaceTriggers) {
                    interfaceTrigger?.triggerCalled(EDNJHABGGKD.triggerName);
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(SplitTParamNode))]
        public static class SplitTParamNodePatches {
            [HarmonyPatch("OBHKFAJCAIJ")]
            [HarmonyPrefix]
            public static bool ActivatePatch(SplitTParamNode __instance, List<HOODKEPNMHB> ___OLOOJHEPJNL, HOODKEPNMHB ___AONBCPCEOLI, HOODKEPNMHB ___NKIMACBBNDJ, HOODKEPNMHB ___EPHLEHONCHF) {
                foreach (HOODKEPNMHB callback in ___OLOOJHEPJNL) {
                    OCEONADJDLM oCEONADJDLM = callback.GetValue<OCEONADJDLM>();
                    if (oCEONADJDLM != null) {
                        string inputText = ___AONBCPCEOLI?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.inputText.text, oCEONADJDLM);
                        string delimiter = ___NKIMACBBNDJ?.GetValue<string>() ?? __instance.delimeter.text;
                        if (___EPHLEHONCHF == null) {
                            var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                            string[] array = inputText.Split(new[] { delimiter }, StringSplitOptions.None);
                            cacheComponent.value.Clear();
                            if (array != null) {
                                cacheComponent.value.AddRange(array);
                            }
                        } else {
                            string arrayName = ___EPHLEHONCHF?.GetValue<string>() ?? __instance.arrayName.text;
                            string[] array = inputText.Split(new[] { delimiter }, StringSplitOptions.None);
                            if (TArrayParams.TryGetValue(arrayName, out List<string> list)) {
                                list.Clear();
                                list.AddRange(array);
                            } else {
                                TArrayParams.Add(arrayName, array.ToList());
                            }
                        }
                        return false;
                    }
                }
                return false;
            }
            [HarmonyPatch(nameof(SplitTParamNode.Setup))]
            [HarmonyPostfix]
            public static void SetupHook(SplitTParamNode __instance) {
                var cacheComponent = __instance.gameObject.AddComponent<CacheArrayComponent>();
                __instance.arrayName.onValueChanged.AddListener(newValue => {
                    if (!TArrayParams.TryGetValue(newValue, out cacheComponent.value)) {
                        TArrayParams.Add(newValue, cacheComponent.value = new List<string>());
                    }
                });
            }
            [HarmonyPatch(nameof(SplitTParamNode.OnDeserialize))]
            [HarmonyPostfix]
            public static void DeserializeHook(SplitTParamNode __instance) {
                var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                if (!TArrayParams.TryGetValue(__instance.arrayName.text, out cacheComponent.value)) {
                    TArrayParams.Add(__instance.arrayName.text, cacheComponent.value = new List<string>());
                }
            }
        }
        [HarmonyPatch(typeof(TArrayValueNode))]
        public static class TArrayValueNodePatches {
            [HarmonyPatch(nameof(TArrayValueNode.UpdateValueSockets))]
            [HarmonyPrefix]
            public static bool UpdateValueSocketsIntercept(TArrayValueNode __instance, HOODKEPNMHB ___EPHLEHONCHF, HOODKEPNMHB ___FEHLMIBCLIP) {
                int index = ___FEHLMIBCLIP?.GetValue<int>() ?? (int.TryParse(ParamSystemPatches.InsertParamValues(__instance.indexField.text), out int result) ? result : 0);
                if (___EPHLEHONCHF == null) {
                    if (index < 0) {
                        __instance.textOutput.SetValue("");
                        return false;
                    }
                    var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                    if (index >= cacheComponent.value.Count) {
                        __instance.textOutput.SetValue("");
                        return false;
                    }
                    __instance.textOutput.SetValue(cacheComponent.value[index]);
                    return false;
                }
                __instance.textOutput.SetValue(ParamSystem.getInstance().GetValueStringArray(___EPHLEHONCHF.GetValue<string>() ?? __instance.arrayField.text, index));
                return false;
            }
            [HarmonyPatch(nameof(TArrayValueNode.Setup))]
            [HarmonyPostfix]
            public static void SetupHook(TArrayValueNode __instance) {
                var cacheComponent = __instance.gameObject.AddComponent<CacheArrayComponent>();
                __instance.arrayField.onValueChanged.AddListener(newValue => {
                    if (!TArrayParams.TryGetValue(newValue, out cacheComponent.value)) {
                        TArrayParams.Add(newValue, cacheComponent.value = new List<string>());
                    }
                });
            }
            [HarmonyPatch(nameof(TArrayValueNode.OnDeserialize))]
            [HarmonyPostfix]
            public static void DeserializeHook(TArrayValueNode __instance) {
                var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                if (!TArrayParams.TryGetValue(__instance.arrayField.text, out cacheComponent.value)) {
                    TArrayParams.Add(__instance.arrayField.text, cacheComponent.value = new List<string>());
                }
            }
        }
        [HarmonyPatch(typeof(SetTArrayNode))]
        public static class SetTArrayNodePatches {
            [HarmonyPatch("OBHKFAJCAIJ")]
            [HarmonyPrefix]
            public static bool ActivatePatch(SetTArrayNode __instance, HOODKEPNMHB ___BLKJOPOPJAB, HOODKEPNMHB ___FEHLMIBCLIP, HOODKEPNMHB ___LAFHOALCHFL, List<HOODKEPNMHB> ___OLOOJHEPJNL) {
                OCEONADJDLM cKHLPKKNCBC = null;
                foreach (HOODKEPNMHB callback in ___OLOOJHEPJNL) {
                    if (callback.GetValue<OCEONADJDLM>() != null) {
                        cKHLPKKNCBC = callback.GetValue<OCEONADJDLM>();
                        break;
                    }
                }
                if (cKHLPKKNCBC != null) {
                    int inputSocketValue = ___FEHLMIBCLIP?.GetValue<int>() ?? ParamSystem.GetIntValueFromString(__instance.indexValue.text, -1, cKHLPKKNCBC);
                    if (___BLKJOPOPJAB == null) {
                        if (__instance.arrayName.text.Length > 0 && inputSocketValue >= 0) {
                            var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                            if (inputSocketValue < cacheComponent.value.Count) {
                                cacheComponent.value[inputSocketValue] = ___LAFHOALCHFL?.GetValue<string>() ?? ParamSystem.getInstance().InsertParamValues(__instance.paramValue.text, cKHLPKKNCBC);
                            }
                        }
                    } else {
                        string inputSocketValue2 = ___BLKJOPOPJAB.GetValue<string>() ?? __instance.arrayName.text;
                        if (inputSocketValue2.Length > 0 && inputSocketValue >= 0) {
                            string inputSocketValue3 = ___LAFHOALCHFL?.GetValue<string>() ?? ParamSystem.getInstance().InsertParamValues(__instance.paramValue.text, cKHLPKKNCBC);
                            ParamSystem.getInstance().SetValueStringArray(inputSocketValue2, inputSocketValue, inputSocketValue3);
                        }
                    }
                }
                return false;
            }
            [HarmonyPatch(nameof(SetTArrayNode.Setup))]
            [HarmonyPostfix]
            public static void SetupHook(SetTArrayNode __instance) {
                var cacheComponent = __instance.gameObject.AddComponent<CacheArrayComponent>();
                __instance.arrayName.onValueChanged.AddListener(newValue => {
                    if (!TArrayParams.TryGetValue(newValue, out cacheComponent.value)) {
                        TArrayParams.Add(newValue, cacheComponent.value = new List<string>());
                    }
                });
            }
            [HarmonyPatch(nameof(SetTArrayNode.OnDeserialize))]
            [HarmonyPostfix]
            public static void DeserializeHook(SetTArrayNode __instance) {
                var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                if (!TArrayParams.TryGetValue(__instance.arrayName.text, out cacheComponent.value)) {
                    TArrayParams.Add(__instance.arrayName.text, cacheComponent.value = new List<string>());
                }
            }
        }
        [HarmonyPatch(typeof(DictionaryValueNode))]
        public static class DictionaryValueNodePatches {
            [HarmonyPatch(nameof(DictionaryValueNode.UpdateValueSockets))]
            [HarmonyPrefix]
            public static bool UpdateValueSocketsIntercept(DictionaryValueNode __instance, HOODKEPNMHB ___NFEDJNPPPMG, HOODKEPNMHB ___EPHLEHONCHF, HOODKEPNMHB ___MDMENHPPGCA) {
                Dictionary<string, string> inputSocketValue = ___NFEDJNPPPMG?.GetValue<Dictionary<string, string>>();
                // If we have no forced dictionary
                if (inputSocketValue == null) {
                    // If there is no incoming name callback
                    if (___EPHLEHONCHF == null) {
                        // Grab the cache component
                        var cacheComponent = __instance.GetComponent<CacheDictionaryComponent>();
                        string inputSocketValue3 = ___MDMENHPPGCA?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.keyField.text);
                        // If there is a param in the string, we are sad, because we cannot cache efficiently.
                        if (cacheComponent.hasparam) {
                            string inputSocketValue2 = ParamSystemPatches.InsertParamValues(__instance.arrayField.text);
                            __instance.textOutput.SetValue(ParamSystem.getInstance().GetDictionaryValue(inputSocketValue2, inputSocketValue3.ToLower()));
                        } else {
                            __instance.textOutput.SetValue(cacheComponent.value[inputSocketValue3.ToLower()]);
                        }
                    } else {
                        string inputSocketValue2 = ___EPHLEHONCHF?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.arrayField.text);
                        string inputSocketValue3 = ___MDMENHPPGCA?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.keyField.text);
                        __instance.textOutput.SetValue(ParamSystem.getInstance().GetDictionaryValue(inputSocketValue2, inputSocketValue3.ToLower()));
                    }
                } else {
                    string inputSocketValue4 = ___MDMENHPPGCA?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.keyField.text);
                    __instance.textOutput.SetValue(inputSocketValue.TryGetValue(inputSocketValue4, out var value) ? value : null);
                }
                return false;
            }
            [HarmonyPatch(nameof(DictionaryValueNode.Setup))]
            [HarmonyPostfix]
            public static void SetupHook(DictionaryValueNode __instance) {
                var cacheComponent = __instance.gameObject.AddComponent<CacheDictionaryComponent>();
                __instance.arrayField.onValueChanged.AddListener(newValue => {
                    cacheComponent.hasparam = ParamSystemPatches.PARAM_TEST_REGEX.IsMatch(newValue);
                    if (!cacheComponent.hasparam && !DictionaryParams.TryGetValue(newValue, out cacheComponent.value)) {
                        DictionaryParams.Add(newValue, cacheComponent.value = new Dictionary<string, string>());
                    }
                });
            }
            [HarmonyPatch(nameof(DictionaryValueNode.OnDeserialize))]
            [HarmonyPostfix]
            public static void DeserializeHook(DictionaryValueNode __instance) {
                var cacheComponent = __instance.GetComponent<CacheDictionaryComponent>();
                cacheComponent.hasparam = ParamSystemPatches.PARAM_TEST_REGEX.IsMatch(__instance.arrayField.text);
                if (!cacheComponent.hasparam && !DictionaryParams.TryGetValue(__instance.arrayField.text, out cacheComponent.value)) {
                    DictionaryParams.Add(__instance.arrayField.text, cacheComponent.value = new Dictionary<string, string>());
                }
            }
        }
        [HarmonyPatch(typeof(TextSubstringNode))]
        public static class TextSubstringNodePatches {
            [HarmonyPatch(nameof(TextSubstringNode.UpdateValueSockets))]
            [HarmonyPrefix]
            public static bool UpdateValueSocketsHook(TextSubstringNode __instance, HOODKEPNMHB ___AONBCPCEOLI, HOODKEPNMHB ___FEHLMIBCLIP, HOODKEPNMHB ___GHOOKJHAGEB) {
                string inputSocketValue = ___AONBCPCEOLI?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.textField.text);
                int inputSocketValue2 = ___FEHLMIBCLIP?.GetValue<int>() ?? ParamSystemPatches.GetIntValueFromString(__instance.indexField.text);
                int inputSocketValue3 = ___GHOOKJHAGEB?.GetValue<int>() ?? ParamSystemPatches.GetIntValueFromString(__instance.lengthField.text);
                try {
                    if (inputSocketValue2 >= 0 && inputSocketValue2 < inputSocketValue.Length) {
                        if (inputSocketValue3 > 0 && inputSocketValue2 + inputSocketValue3 <= inputSocketValue.Length) {
                            __instance.textOutput.SetValue(inputSocketValue.Substring(inputSocketValue2, inputSocketValue3));
                        } else {
                            __instance.textOutput.SetValue(inputSocketValue.Substring(inputSocketValue2));
                        }
                    } else {
                        __instance.textOutput.SetValue("");
                    }
                } catch (Exception) {
                    __instance.textOutput.SetValue("");
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(ParamSystem))]
        public static class ParamSystemPatches {
            public static readonly Regex PARAM_TEST_REGEX = new Regex(@"\{[^][><}]+?\}|\[[^][><]+?\]|<[^][><]+?>", RegexOptions.Compiled);
            public static readonly Regex HEARTRATE_REGEX = new Regex(@"\[heartrate\]", RegexOptions.Compiled);
            public static readonly Regex HEARTPERC_REGEX = new Regex(@"\[heartpercent\]", RegexOptions.Compiled);
            public static readonly Regex USERNAME_REGEX = new Regex(@"<username>", RegexOptions.Compiled);
            public static readonly Regex MESSAGE1_SREGEX = new Regex(@"<message>", RegexOptions.Compiled);
            public static readonly Regex MESSAGE2_SREGEX = new Regex(@"<message2>", RegexOptions.Compiled);
            public static readonly Regex MESSAGE3_SREGEX = new Regex(@"<message3>", RegexOptions.Compiled);
            public static readonly Regex PARAM1_REGEX = new Regex(@"\[message\]", RegexOptions.Compiled);
            public static readonly Regex PARAM2_REGEX = new Regex(@"\[message2\]", RegexOptions.Compiled);
            public static readonly Regex PARAM3_REGEX = new Regex(@"\[message3\]", RegexOptions.Compiled);
            public static readonly Regex NUM_PARAM_REGEX = new Regex(@"\[([^][></]+)\]", RegexOptions.Compiled);
            public static readonly Regex STR_PARAM_REGEX = new Regex(@"<([^][>/]+)>", RegexOptions.Compiled);
            public static readonly Regex TIM_PARAM_REGEX = new Regex(@"\{([^}/]+)\}", RegexOptions.Compiled);
            public static string MyReplaceStr(string KGEIAJAADDD, string PLMOGMPOCCL, string NHHOCGKIPDM) {
                return Regex.Replace(KGEIAJAADDD, Regex.Escape(PLMOGMPOCCL), NHHOCGKIPDM.Replace("$", "$$"), RegexOptions.IgnoreCase);
            }
            [HarmonyPatch(nameof(ParamSystem.GetIntValueFromString))]
            [HarmonyPrefix]
            public static bool GetIntValueHook(ref int __result, string BFDBBGCABBF, int IJLJJCELKMM, OCEONADJDLM COHHGHFNJCM) {
                // var LOWER = IDBCBCHDGJA.ToLower()
                var LABI = BFDBBGCABBF.IndexOf('<');
                if (LABI > -1 && BFDBBGCABBF.IndexOf('>', LABI) > -1) {
                    var LOWER = BFDBBGCABBF.ToLower();
                    if ((LOWER == "<message>" || BFDBBGCABBF.ToLower() == "<message2>" || BFDBBGCABBF.ToLower() == "<message3>") && COHHGHFNJCM != null && int.TryParse(BFDBBGCABBF, out var result)) {
                        __result = result;
                        return false;
                    }
                    string kPBPEDKDCKF = BFDBBGCABBF.Replace("<", "").Replace(">", "");
                    kPBPEDKDCKF = ParamSystem.getInstance().GetParamString(kPBPEDKDCKF, BFDBBGCABBF);
                    BFDBBGCABBF = kPBPEDKDCKF;
                }
                if (BFDBBGCABBF.IndexOf('[') > -1) {
                    if (BFDBBGCABBF.ToLower() == "[message]" && COHHGHFNJCM != null) {
                        __result = COHHGHFNJCM.Parameter1;
                        return false;
                    }
                    if (BFDBBGCABBF.ToLower() == "[message2]" && COHHGHFNJCM != null) {
                        __result = COHHGHFNJCM.Parameter2;
                        return false;
                    }
                    if (BFDBBGCABBF.ToLower() == "[message3]" && COHHGHFNJCM != null) {
                        __result = COHHGHFNJCM.Parameter3;
                        return false;
                    }
                    if (BFDBBGCABBF.ToLower() == "[heartrate]") {
                        __result = PulsoidOAuth.HeartRate;
                        return false;
                    }
                    string kPBPEDKDCKF2 = BFDBBGCABBF.Replace("[", "").Replace("]", "");
                    __result = ParamSystem.getInstance().GetParamInt(kPBPEDKDCKF2, IJLJJCELKMM);
                    return false;
                }
                if (BFDBBGCABBF.IndexOf('{') > -1) {
                    string eDENLIIBBAP = BFDBBGCABBF.Replace("{", "").Replace("}", "");
                    __result = TimerSystem.getInstance().GetTimerLeft(eDENLIIBBAP);
                    return false;
                }
                if (int.TryParse(BFDBBGCABBF, out var result2)) {
                    __result = result2;
                    return false;
                }
                __result = IJLJJCELKMM;
                return false;
            }
            public static int GetIntValueFromString(string IDBCBCHDGJA, int HMKBFKLOIIB = 0, OCEONADJDLM JOJBALNDGJL = null) {
                int result = 0;
                GetIntValueHook(ref result, IDBCBCHDGJA, HMKBFKLOIIB, JOJBALNDGJL);
                return result;
            }
            [HarmonyPatch(nameof(ParamSystem.InsertParamValues))]
            [HarmonyPrefix]
            public static bool InsertParamValuesHook(ref string __result, string JJDKOGKFAAA, OCEONADJDLM COHHGHFNJCM) {
                // Parameters can't be made with an empty string name, so the string we are inserting must be at least 3 long.
                if (JJDKOGKFAAA.Length < 3 || !PARAM_TEST_REGEX.IsMatch(JJDKOGKFAAA)) {
                    __result = JJDKOGKFAAA;
                    return false;
                }
                bool containsSquares = JJDKOGKFAAA.Contains('[') && JJDKOGKFAAA.Contains(']');
                bool containsAngles = JJDKOGKFAAA.Contains('<') && JJDKOGKFAAA.Contains('>');
                bool containsCurlies = JJDKOGKFAAA.Contains('{') && JJDKOGKFAAA.Contains('}');
                JJDKOGKFAAA = HEARTRATE_REGEX.Replace(JJDKOGKFAAA, PulsoidOAuth.HeartRate.ToString() ?? "");
                JJDKOGKFAAA = HEARTPERC_REGEX.Replace(JJDKOGKFAAA, Mathf.Clamp(((float)PulsoidOAuth.HeartRate - 30f) / 270f, 0f, 1f).ToString() ?? "");
                if (COHHGHFNJCM != null) {
                    if (containsAngles) {
                        JJDKOGKFAAA = USERNAME_REGEX.Replace(JJDKOGKFAAA, COHHGHFNJCM.UserName ?? "");
                        JJDKOGKFAAA = MESSAGE1_SREGEX.Replace(JJDKOGKFAAA, COHHGHFNJCM.Message ?? "");
                        JJDKOGKFAAA = MESSAGE2_SREGEX.Replace(JJDKOGKFAAA, COHHGHFNJCM.Message2 ?? "");
                        JJDKOGKFAAA = MESSAGE3_SREGEX.Replace(JJDKOGKFAAA, COHHGHFNJCM.Message3 ?? "");
                    }
                    if (containsSquares) {
                        JJDKOGKFAAA = PARAM1_REGEX.Replace(JJDKOGKFAAA, COHHGHFNJCM.Parameter1.ToString() ?? "");
                        JJDKOGKFAAA = PARAM2_REGEX.Replace(JJDKOGKFAAA, COHHGHFNJCM.Parameter2.ToString() ?? "");
                        JJDKOGKFAAA = PARAM3_REGEX.Replace(JJDKOGKFAAA, COHHGHFNJCM.Parameter3.ToString() ?? "");
                    }
                }
                if (containsSquares) {
                    JJDKOGKFAAA = NUM_PARAM_REGEX.Replace(JJDKOGKFAAA, (match) => {
                        return NumParams.TryGetValue(match.Groups[1].Value, out var replace) ? replace.ToString(CultureInfo.InvariantCulture) : match.Value;
                    });
                }
                if(containsAngles) {
                    JJDKOGKFAAA = STR_PARAM_REGEX.Replace(JJDKOGKFAAA, (match) => {
                        return StringParams.TryGetValue(match.Groups[1].Value, out var replace) ? replace : match.Value;
                    });
                }
                if (containsCurlies) {
                    var now = DateTime.Now;
                    JJDKOGKFAAA = TIM_PARAM_REGEX.Replace(JJDKOGKFAAA, (match) => {
                        if (Timers.TryGetValue(match.Groups[1].Value, out var timer)) {
                            int num = timer.MSeconds - (int)now.Subtract(timer.startTime).TotalMilliseconds;
                            if (num < 0) {
                                return "0";
                            }
                            return num.ToString();
                        }
                        return match.Value;
                    });
                }
                __result = JJDKOGKFAAA;
                return false;
            }
            public static string InsertParamValues(string IAHIDDKBEMH, OCEONADJDLM JOJBALNDGJL = null) {
                string result = IAHIDDKBEMH;
                InsertParamValuesHook(ref result, IAHIDDKBEMH, JOJBALNDGJL);
                return result;
            }

            [HarmonyPatch(nameof(ParamSystem.GetParamFloat))]
            [HarmonyPrefix]
            public static bool GetParamFloatHook(ref float __result, string EOLDCDAMKFP, float IJLJJCELKMM) {
                EOLDCDAMKFP = EOLDCDAMKFP.Replace('[', '\0').Replace(']', '\0');
                if (NumParams.TryGetValue(EOLDCDAMKFP, out var value)) {
                    __result = value;
                    return false;
                }
                __result = IJLJJCELKMM;
                return false;
            }

            [HarmonyPatch(nameof(ParamSystem.JsonToDictionary))]
            [HarmonyPrefix]
            public static bool JsonHook(string LNKGPHJDJJP, string AGBOMCDAKMN) {
                if (DictionaryParams.TryGetValue(LNKGPHJDJJP, out var dict)) {
                    dict.Clear();
                    JsonConvert.PopulateObject(AGBOMCDAKMN, dict);
                } else {
                    DictionaryParams.Add(LNKGPHJDJJP, JsonConvert.DeserializeObject<Dictionary<string, string>>(AGBOMCDAKMN));
                }
                return false;
            }

            [HarmonyPatch(nameof(ParamSystem.UnpackDictionary))]
            [HarmonyPrefix]
            public static bool UnpackHook(string LNKGPHJDJJP, string ENJPFFGDGFN) {
                ENJPFFGDGFN = ParamSystem.Base64Decode(ENJPFFGDGFN);
                if (DictionaryParams.TryGetValue(LNKGPHJDJJP, out Dictionary<string, string> dict)) {
                    dict.Clear();
                    JsonConvert.PopulateObject(ENJPFFGDGFN, dict);
                } else {
                    DictionaryParams.Add(LNKGPHJDJJP, dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(ENJPFFGDGFN));
                }
                foreach (string item in dict.Keys.ToList()) {
                    dict[item] = ParamSystem.Base64Decode(dict[item]);
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(UnpackDictionaryNode))]
        public static class UnpackDictionaryNodePatches {
            [HarmonyPatch("OBHKFAJCAIJ")]
            [HarmonyPrefix]
            public static bool ActivateHook(UnpackDictionaryNode __instance, List<HOODKEPNMHB> ___OLOOJHEPJNL, HOODKEPNMHB ___KMLKDFDKJNB, HOODKEPNMHB ___AKDCJAECNKA) {
                OCEONADJDLM cKHLPKKNCBC = null;
                foreach (HOODKEPNMHB callback in ___OLOOJHEPJNL) {
                    if (callback.GetValue<OCEONADJDLM>() != null) {
                        cKHLPKKNCBC = callback.GetValue<OCEONADJDLM>();
                        break;
                    }
                }
                if (cKHLPKKNCBC != null) {
                    if (___KMLKDFDKJNB == null) {
                        string inputSocketValue2 = ___AKDCJAECNKA?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.paramText.text, cKHLPKKNCBC);
                        if (__instance.dictionaryName.text.Length > 0 && inputSocketValue2.Length > 0) {
                            var cacheComponent = __instance.GetComponent<CacheDictionaryComponent>();
                            inputSocketValue2 = ParamSystem.Base64Decode(inputSocketValue2);
                            cacheComponent.value.Clear();
                            JsonConvert.PopulateObject(inputSocketValue2, cacheComponent.value);
                            foreach (string item in cacheComponent.value.Keys.ToList()) {
                                cacheComponent.value[item] = ParamSystem.Base64Decode(cacheComponent.value[item]);
                            }
                        }
                    } else {
                        string inputSocketValue = ___KMLKDFDKJNB.GetValue<string>() ?? __instance.dictionaryName.text;
                        string inputSocketValue2 = ___AKDCJAECNKA?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.paramText.text, cKHLPKKNCBC);
                        if (inputSocketValue.Length > 0 && inputSocketValue2.Length > 0) {
                            ParamSystem.getInstance().UnpackDictionary(inputSocketValue.ToLower(), inputSocketValue2);
                        }
                    }
                }
                return false;
            }

            [HarmonyPatch(nameof(UnpackDictionaryNode.Setup))]
            [HarmonyPostfix]
            public static void SetupHook(UnpackDictionaryNode __instance) {
                var cacheComponent = __instance.gameObject.AddComponent<CacheDictionaryComponent>();
                __instance.dictionaryName.onValueChanged.AddListener(newValue => {
                    cacheComponent.hasparam = ParamSystemPatches.PARAM_TEST_REGEX.IsMatch(newValue);
                    if (!cacheComponent.hasparam && !DictionaryParams.TryGetValue(newValue, out cacheComponent.value)) {
                        DictionaryParams.Add(newValue, cacheComponent.value = new Dictionary<string, string>());
                    }
                });
            }

            [HarmonyPatch(nameof(UnpackDictionaryNode.OnDeserialize))]
            [HarmonyPostfix]
            public static void DeserializeHook(UnpackDictionaryNode __instance) {
                var cacheComponent = __instance.GetComponent<CacheDictionaryComponent>();
                cacheComponent.hasparam = ParamSystemPatches.PARAM_TEST_REGEX.IsMatch(__instance.dictionaryName.text);
                if (!cacheComponent.hasparam && !DictionaryParams.TryGetValue(__instance.dictionaryName.text, out cacheComponent.value)) {
                    DictionaryParams.Add(__instance.dictionaryName.text, cacheComponent.value = new Dictionary<string, string>());
                }
            }
        }
        [HarmonyPatch(typeof(TriggerNode))]
        public static class TriggerNodePatches {
            [HarmonyPatch("OnDestroy")]
            [HarmonyPrefix]
            public static void DestroyHook(TriggerNode __instance) {
                RemoveActionWrapper(__instance.valueField.text, __instance.OnRedeemCallback);
            }

            [HarmonyPatch(nameof(TriggerNode.Setup))]
            [HarmonyPrefix]
            public static void SetupHook(TriggerNode __instance) {
                __instance.valueField.onValidateInput = (input, charIndex, addedChar) => {
                    RemoveActionWrapper(input, __instance.OnRedeemCallback);
                    return addedChar;
                };
                __instance.valueField.onValueChanged.AddListener(newValue => AddActionWrapper(newValue, __instance.OnRedeemCallback));
            }

            [HarmonyPatch(nameof(TriggerNode.OnDeserialize))]
            [HarmonyPrefix]
            static void DeserializePreHook(TriggerNode __instance) {
                __instance.valueField.onValidateInput = null;
            }

            [HarmonyPatch(nameof(TriggerNode.OnDeserialize))]
            [HarmonyPostfix]
            public static void DeserializePostHook(TriggerNode __instance) {
                __instance.valueField.onValidateInput = (input, charIndex, addedChar) => {
                    RemoveActionWrapper(input, __instance.OnRedeemCallback);
                    return addedChar;
                };
                AddActionWrapper(__instance.valueField.text, __instance.OnRedeemCallback);
            }
        }
    }
}