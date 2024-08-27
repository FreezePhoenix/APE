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
        static readonly Dictionary<string, Dictionary<string, string>> DictionaryParams = Traverse.Create(ParamSystem.getInstance()).Field("paramDictionary").GetValue<Dictionary<string, Dictionary<string, string>>>();
        static readonly Dictionary<string, List<string>> TArrayParams = Traverse.Create(ParamSystem.getInstance()).Field("paramStringArray").GetValue<Dictionary<string, List<string>>>();
        static Dictionary<string, float> NumParams;
        static Dictionary<string, string> StringParams;
        static readonly Dictionary<string, ActionWrapper<CKHLPKKNCBC>> TriggerChannels = new Dictionary<string, ActionWrapper<CKHLPKKNCBC>>(StringComparer.OrdinalIgnoreCase);
        public static ActionWrapper<CKHLPKKNCBC> GetActionWrapper(string name) {
            if (!TriggerChannels.TryGetValue(name, out ActionWrapper<CKHLPKKNCBC> wrapper)) {
                TriggerChannels.Add(name, wrapper = new ActionWrapper<CKHLPKKNCBC>());
            }
            return wrapper;
        }
        public static void AddActionWrapper(string name, Action<CKHLPKKNCBC> action) {
            if (!TriggerChannels.TryGetValue(name, out ActionWrapper<CKHLPKKNCBC> wrapper)) {
                TriggerChannels.Add(name, wrapper = new ActionWrapper<CKHLPKKNCBC>());
            }
            wrapper.Add(action);
        }
        public static void RemoveActionWrapper(string name, Action<CKHLPKKNCBC> action) {
            if (TriggerChannels.TryGetValue(name, out ActionWrapper<CKHLPKKNCBC> wrapper)) {
                wrapper.Remove(action);
            }
        }
        public void Awake() {
            try {
                var ParamNumberField = Traverse.Create(ParamSystem.getInstance()).Field("paramInt");
                ParamNumberField.SetValue(NumParams = new Dictionary<string, float>(ParamNumberField.GetValue<Dictionary<string, float>>(), StringComparer.OrdinalIgnoreCase));
                var ParamStringField = Traverse.Create(ParamSystem.getInstance()).Field("paramString");
                ParamStringField.SetValue(StringParams = new Dictionary<string, string>(ParamStringField.GetValue<Dictionary<string, string>>(), StringComparer.OrdinalIgnoreCase));
                TimerSystem.getInstance().timers = new Dictionary<string, KJJCCCDGKFK>(TimerSystem.getInstance().timers, StringComparer.OrdinalIgnoreCase);
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
            } catch(Exception e) {
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
            [HarmonyPatch("IHCGBFELBLL")]
            [HarmonyPrefix]
            public static bool ActivatePatch(CallTriggerNode __instance, List<LJOOJBCCAOF> ____callbacks, ref bool ___dirty, ref bool ___hasParam, LJOOJBCCAOF ___nameCallback, LJOOJBCCAOF ___value1Callback, LJOOJBCCAOF ___value2Callback, LJOOJBCCAOF ___value3Callback, LJOOJBCCAOF ___text1Callback, LJOOJBCCAOF ___text2Callback, LJOOJBCCAOF ___text3Callback) {
                CKHLPKKNCBC cKHLPKKNCBC = null;
                foreach (LJOOJBCCAOF callback in ____callbacks) {
                    var newValue = callback.GetValue<CKHLPKKNCBC>();
                    if(newValue != null) {
                        cKHLPKKNCBC = newValue;
                        break;
                    }
                }
                if (cKHLPKKNCBC == null) {
                    return false;
                }
                string text = __instance.triggerName.text;
                if (___dirty) {
                    ___hasParam = text.Contains("[") || text.Contains("{") || text.Contains("<");
                    ___dirty = false;
                }
                if (___hasParam) {
                    text = ParamSystemPatches.InsertParamValues(text, cKHLPKKNCBC);
                }
                if (___nameCallback != null) {
                    var obj = ___nameCallback.GetValue<string>();
                    if (obj != null) {
                        text = obj;
                    }
                } else if (__instance.callTime.value == 1) {
                    var cacheComponent = __instance.GetComponent<CacheActionComponent>();
                    CKHLPKKNCBC obj = new CKHLPKKNCBC() {
                        Type = 0,
                        Value = text,
                        Parameter1 = ___value1Callback?.GetValue<int>() ?? 0,
                        Parameter2 = ___value2Callback?.GetValue<int>() ?? 0,
                        Parameter3 = ___value3Callback?.GetValue<int>() ?? 0,
                        Message =  ___text1Callback?.GetValue<string>() ?? "",
                        Message2 = ___text2Callback?.GetValue<string>() ?? "",
                        Message3 = ___text3Callback?.GetValue<string>() ?? ""
                    };
                    cacheComponent.value.Invoke(obj);
                    cacheComponent.value.Invoke(null);
                    return false;
                }
                TriggerSystem.AIAOHOLGDOE aIAOHOLGDOE = new TriggerSystem.AIAOHOLGDOE() {
                    triggerName = text,
                    value1 = ___value1Callback?.GetValue<int>() ?? 0,
                    value2 = ___value2Callback?.GetValue<int>() ?? 0,
                    value3 = ___value3Callback?.GetValue<int>() ?? 0,
                    text1 = ___text1Callback?.GetValue<string>() ?? "",
                    text2 = ___text2Callback?.GetValue<string>() ?? "",
                    text3 = ___text3Callback?.GetValue<string>() ?? ""
                };
                switch (__instance.callTime.value) {
                    case 0:
                        TriggerSystem.getInstance().AddTriggerItem(aIAOHOLGDOE);
                        break;
                    case 1:
                        TriggerSystem.getInstance().CallTrigger(aIAOHOLGDOE);
                        break;
                }
                return false;
            }

        }
        [HarmonyPatch(typeof(TriggerSystem))]
        public static class TriggerSystemPatches {
            [HarmonyPatch(nameof(TriggerSystem.CallTrigger))]
            [HarmonyPrefix]
            public static bool CallTriggerHook(TriggerSystem __instance, TriggerSystem.AIAOHOLGDOE OFGNEEGAFFM) {
                if (TriggerChannels.TryGetValue(OFGNEEGAFFM.triggerName, out var OnTrigger)) {
                    OnTrigger.Invoke(new CKHLPKKNCBC() {
                        Type = 0,
                        Value = OFGNEEGAFFM.triggerName,
                        Parameter1 = OFGNEEGAFFM.value1,
                        Parameter2 = OFGNEEGAFFM.value2,
                        Parameter3 = OFGNEEGAFFM.value3,
                        Message = OFGNEEGAFFM.text1,
                        Message2 = OFGNEEGAFFM.text2,
                        Message3 = OFGNEEGAFFM.text3
                    });
                    OnTrigger.Invoke(null);
                }
                foreach (ITriggerHandler interfaceTrigger in __instance.interfaceTriggers) {
                    interfaceTrigger?.triggerCalled(OFGNEEGAFFM.triggerName);
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(SplitTParamNode))]
        public static class SplitTParamNodePatches {
            [HarmonyPatch("IHCGBFELBLL")]
            [HarmonyPrefix]
            public static bool ActivatePatch(SplitTParamNode __instance, List<LJOOJBCCAOF> ____callbacks, LJOOJBCCAOF ___textCallback, LJOOJBCCAOF ___delimCallback, LJOOJBCCAOF ___arrayCallback) {
                foreach (LJOOJBCCAOF callback in ____callbacks) {
                    CKHLPKKNCBC cKHLPKKNCBC = callback.GetValue<CKHLPKKNCBC>();
                    if (cKHLPKKNCBC != null) {
                        string inputText = ___textCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.inputText.text, cKHLPKKNCBC);
                        string delimiter = ___delimCallback?.GetValue<string>() ?? __instance.delimeter.text;
                        if (___arrayCallback == null) {
                            var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                            string[] array = inputText.Split(new[] { delimiter }, StringSplitOptions.None);
                            cacheComponent.value.Clear();
                            if (array != null) {
                                cacheComponent.value.AddRange(array);
                            }
                        } else {
                            string arrayName = ___arrayCallback?.GetValue<string>() ?? __instance.arrayName.text;
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
            public static bool UpdateValueSocketsIntercept(TArrayValueNode __instance, LJOOJBCCAOF ___arrayCallback, LJOOJBCCAOF ___indexCallback) {
                int index = ___indexCallback?.GetValue<int>() ?? (int.TryParse(ParamSystemPatches.InsertParamValues(__instance.indexField.text), out int result) ? result : 0);
                if (___arrayCallback == null) {
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
                __instance.textOutput.SetValue(ParamSystem.getInstance().GetValueStringArray(___arrayCallback.GetValue<string>() ?? __instance.arrayField.text, index));
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
            [HarmonyPatch("IHCGBFELBLL")]
            [HarmonyPrefix]
            public static bool ActivatePatch(SetTArrayNode __instance, LJOOJBCCAOF ___nameCallback, LJOOJBCCAOF ___indexCallback, LJOOJBCCAOF ___valueCallback, List<LJOOJBCCAOF> ____callbacks) {
                CKHLPKKNCBC cKHLPKKNCBC = null;
                foreach (LJOOJBCCAOF callback in ____callbacks) {
                    if (callback.GetValue<CKHLPKKNCBC>() != null) {
                        cKHLPKKNCBC = callback.GetValue<CKHLPKKNCBC>();
                        break;
                    }
                }
                if (cKHLPKKNCBC != null) {
                    int inputSocketValue = ___indexCallback?.GetValue<int>() ?? ParamSystem.GetIntValueFromString(__instance.indexValue.text, -1, cKHLPKKNCBC);
                    if(___nameCallback == null) {
                        if(__instance.arrayName.text.Length > 0 && inputSocketValue >= 0) {
                            var cacheComponent = __instance.GetComponent<CacheArrayComponent>();
                            if(inputSocketValue < cacheComponent.value.Count) {
                                cacheComponent.value[inputSocketValue] = ___valueCallback?.GetValue<string>() ?? ParamSystem.getInstance().InsertParamValues(__instance.paramValue.text, cKHLPKKNCBC);
                            }
                        }
                    } else {
                        string inputSocketValue2 = ___nameCallback.GetValue<string>() ?? __instance.arrayName.text;
                        if (inputSocketValue2.Length > 0 && inputSocketValue >= 0) {
                            string inputSocketValue3 = ___valueCallback?.GetValue<string>() ?? ParamSystem.getInstance().InsertParamValues(__instance.paramValue.text, cKHLPKKNCBC);
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
            public static bool UpdateValueSocketsIntercept(DictionaryValueNode __instance, LJOOJBCCAOF ___dictCallback, LJOOJBCCAOF ___arrayCallback, LJOOJBCCAOF ___keyCallback) {
                Dictionary<string, string> inputSocketValue = ___dictCallback?.GetValue<Dictionary<string, string>>();
                // If we have no forced dictionary
                if (inputSocketValue == null) {
                    // If there is no incoming name callback
                    if (___arrayCallback == null) {
                        // Grab the cache component
                        var cacheComponent = __instance.GetComponent<CacheDictionaryComponent>();
                        string inputSocketValue3 = ___keyCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.keyField.text);
                        // If there is a param in the string, we are sad, because we cannot cache efficiently.
                        if (cacheComponent.hasparam) {
                            string inputSocketValue2 = ParamSystemPatches.InsertParamValues(__instance.arrayField.text);
                            __instance.textOutput.SetValue(ParamSystem.getInstance().GetDictionaryValue(inputSocketValue2, inputSocketValue3.ToLower()));
                        } else {
                            __instance.textOutput.SetValue(cacheComponent.value[inputSocketValue3.ToLower()]);
                        }
                    } else {
                        string inputSocketValue2 = ___arrayCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.arrayField.text);
                        string inputSocketValue3 = ___keyCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.keyField.text);
                        __instance.textOutput.SetValue(ParamSystem.getInstance().GetDictionaryValue(inputSocketValue2, inputSocketValue3.ToLower()));
                    }
                } else {
                    string inputSocketValue4 = ___keyCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.keyField.text);
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
            public static bool UpdateValueSocketsHook(TextSubstringNode __instance, LJOOJBCCAOF ___textCallback, LJOOJBCCAOF ___indexCallback, LJOOJBCCAOF ___lengthCallback) {
                string inputSocketValue = ___textCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.textField.text);
                int inputSocketValue2 = ___indexCallback?.GetValue<int>() ?? ParamSystemPatches.GetIntValueFromString(__instance.indexField.text);
                int inputSocketValue3 = ___lengthCallback?.GetValue<int>() ?? ParamSystemPatches.GetIntValueFromString(__instance.lengthField.text);
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
            public static readonly Regex NUM_PARAM_REGEX = new Regex(@"\[([^][><]+)\]", RegexOptions.Compiled);
            public static readonly Regex TIM_PARAM_REGEX = new Regex(@"\{([^}]+)\}", RegexOptions.Compiled);
            public static string MyReplaceStr(string KGEIAJAADDD, string PLMOGMPOCCL, string NHHOCGKIPDM) {
                return Regex.Replace(KGEIAJAADDD, Regex.Escape(PLMOGMPOCCL), NHHOCGKIPDM.Replace("$", "$$"), RegexOptions.IgnoreCase);
            }
            [HarmonyPatch(nameof(ParamSystem.GetIntValueFromString))]
            [HarmonyPrefix]
            public static bool GetIntValueHook(ref int __result, string IDBCBCHDGJA, int HMKBFKLOIIB, CKHLPKKNCBC JOJBALNDGJL) {
                // var LOWER = IDBCBCHDGJA.ToLower()
                var LABI = IDBCBCHDGJA.IndexOf('<');
                if (LABI > -1 && IDBCBCHDGJA.IndexOf('>', LABI) > -1) {
                    var LOWER = IDBCBCHDGJA.ToLower();
                    if ((LOWER == "<message>" || IDBCBCHDGJA.ToLower() == "<message2>" || IDBCBCHDGJA.ToLower() == "<message3>") && JOJBALNDGJL != null && int.TryParse(IDBCBCHDGJA, out var result)) {
                        __result = result;
                        return false;
                    }
                    string kPBPEDKDCKF = IDBCBCHDGJA.Replace("<", "").Replace(">", "");
                    kPBPEDKDCKF = ParamSystem.getInstance().GetParamString(kPBPEDKDCKF, IDBCBCHDGJA);
                    IDBCBCHDGJA = kPBPEDKDCKF;
                }
                if (IDBCBCHDGJA.IndexOf('[') > -1) {
                    if (IDBCBCHDGJA.ToLower() == "[message]" && JOJBALNDGJL != null) {
                        __result = JOJBALNDGJL.Parameter1;
                        return false;
                    }
                    if (IDBCBCHDGJA.ToLower() == "[message2]" && JOJBALNDGJL != null) {
                        __result = JOJBALNDGJL.Parameter2;
                        return false;
                    }
                    if (IDBCBCHDGJA.ToLower() == "[message3]" && JOJBALNDGJL != null) {
                        __result = JOJBALNDGJL.Parameter3;
                        return false;
                    }
                    if (IDBCBCHDGJA.ToLower() == "[heartrate]") {
                        __result = PulsoidOAuth.HeartRate;
                        return false;
                    }
                    string kPBPEDKDCKF2 = IDBCBCHDGJA.Replace("[", "").Replace("]", "");
                    __result = ParamSystem.getInstance().GetParamInt(kPBPEDKDCKF2, HMKBFKLOIIB);
                    return false;
                }
                if (IDBCBCHDGJA.IndexOf('{') > -1) {
                    string eDENLIIBBAP = IDBCBCHDGJA.Replace("{", "").Replace("}", "");
                    __result = TimerSystem.getInstance().GetTimerLeft(eDENLIIBBAP);
                    return false;
                }
                if (int.TryParse(IDBCBCHDGJA, out var result2)) {
                    __result = result2;
                    return false;
                }
                __result = HMKBFKLOIIB;
                return false;
            }
            public static int GetIntValueFromString(string IDBCBCHDGJA, int HMKBFKLOIIB = 0, CKHLPKKNCBC JOJBALNDGJL = null) {
                int result = 0;
                GetIntValueHook(ref result, IDBCBCHDGJA, HMKBFKLOIIB, JOJBALNDGJL);
                return result;
            }
            [HarmonyPatch(nameof(ParamSystem.InsertParamValues))]
            [HarmonyPrefix]
            public static bool InsertParamValuesHook(ref string __result, string IAHIDDKBEMH, CKHLPKKNCBC JOJBALNDGJL) {
                // Parameters can't be made with an empty string name, so the string we are inserting must be at least 3 long.
                if (IAHIDDKBEMH.Length < 3 || !PARAM_TEST_REGEX.IsMatch(IAHIDDKBEMH)) {
                    __result = IAHIDDKBEMH;
                    return false;
                }
                IAHIDDKBEMH = HEARTRATE_REGEX.Replace(IAHIDDKBEMH, PulsoidOAuth.HeartRate.ToString() ?? "");
                IAHIDDKBEMH = HEARTPERC_REGEX.Replace(IAHIDDKBEMH, Mathf.Clamp(((float)PulsoidOAuth.HeartRate - 30f) / 270f, 0f, 1f).ToString() ?? "");
                if (JOJBALNDGJL != null) {
                    IAHIDDKBEMH = USERNAME_REGEX.Replace(IAHIDDKBEMH, JOJBALNDGJL.UserName ?? "");
                    IAHIDDKBEMH = MESSAGE1_SREGEX.Replace(IAHIDDKBEMH, JOJBALNDGJL.Message ?? "");
                    IAHIDDKBEMH = MESSAGE2_SREGEX.Replace(IAHIDDKBEMH, JOJBALNDGJL.Message2 ?? "");
                    IAHIDDKBEMH = MESSAGE3_SREGEX.Replace(IAHIDDKBEMH, JOJBALNDGJL.Message3 ?? "");
                    IAHIDDKBEMH = PARAM1_REGEX.Replace(IAHIDDKBEMH, JOJBALNDGJL.Parameter1.ToString() ?? "");
                    IAHIDDKBEMH = PARAM2_REGEX.Replace(IAHIDDKBEMH, JOJBALNDGJL.Parameter2.ToString() ?? "");
                    IAHIDDKBEMH = PARAM3_REGEX.Replace(IAHIDDKBEMH, JOJBALNDGJL.Parameter3.ToString() ?? "");
                }
                if(IAHIDDKBEMH.Contains('[') && IAHIDDKBEMH.Contains(']')) {
                    IAHIDDKBEMH = NUM_PARAM_REGEX.Replace(IAHIDDKBEMH, (match) => {
                        return NumParams.TryGetValue(match.Groups[1].Value, out var replace) ? replace.ToString(CultureInfo.InvariantCulture) : match.Value;
                    });
                }
                foreach (string key2 in StringParams.Keys) {
                    if (!IAHIDDKBEMH.Contains("<")) {
                        break;
                    }
                    IAHIDDKBEMH = MyReplaceStr(IAHIDDKBEMH, "<" + key2 + ">", (StringParams[key2] ?? "") ?? "");
                }
                if (IAHIDDKBEMH.Contains('{') && IAHIDDKBEMH.Contains('}')) {
                    var timerSystem = TimerSystem.getInstance();
                    var now = DateTime.Now;
                    IAHIDDKBEMH = TIM_PARAM_REGEX.Replace(IAHIDDKBEMH, (match) => {
                        if (timerSystem.timers.TryGetValue(match.Groups[1].Value, out var timer)) {
                            int num = timer.MSeconds - (int)now.Subtract(timer.startTime).TotalMilliseconds;
                            if (num < 0) {
                                return "0";
                            }
                            return num.ToString();
                        }
                        return match.Value;
                    });
                }
                __result = IAHIDDKBEMH;
                return false;
            }
            public static string InsertParamValues(string IAHIDDKBEMH, CKHLPKKNCBC JOJBALNDGJL = null) {
                string result = IAHIDDKBEMH;
                InsertParamValuesHook(ref result, IAHIDDKBEMH, JOJBALNDGJL);
                return result;
            }

            [HarmonyPatch(nameof(ParamSystem.GetParamFloat))]
            [HarmonyPrefix]
            public static bool GetParamFloatHook(ref float __result, string KPBPEDKDCKF, float HMKBFKLOIIB, Dictionary<string, float> ___paramInt) {
                KPBPEDKDCKF = KPBPEDKDCKF.Replace('[', '\0').Replace(']', '\0');
                if (___paramInt.TryGetValue(KPBPEDKDCKF, out var value)) {
                    __result = value;
                    return false;
                }
                __result = HMKBFKLOIIB;
                return false;
            }

            [HarmonyPatch(nameof(ParamSystem.JsonToDictionary))]
            [HarmonyPrefix]
            public static bool JsonHook(string ODMMGJAPFBM, string FHJBCAMKHAP) {
                if (DictionaryParams.TryGetValue(ODMMGJAPFBM, out var dict)) {
                    dict.Clear();
                    JsonConvert.PopulateObject(FHJBCAMKHAP, dict);
                } else {
                    DictionaryParams.Add(ODMMGJAPFBM, JsonConvert.DeserializeObject<Dictionary<string, string>>(FHJBCAMKHAP));
                }
                return false;
            }

            [HarmonyPatch(nameof(ParamSystem.UnpackDictionary))]
            [HarmonyPrefix]
            public static bool UnpackHook(string ODMMGJAPFBM, string DJNHNIPKNGD) {
                DJNHNIPKNGD = ParamSystem.Base64Decode(DJNHNIPKNGD);
                if (DictionaryParams.TryGetValue(ODMMGJAPFBM, out Dictionary<string, string> dict)) {
                    dict.Clear();
                    JsonConvert.PopulateObject(DJNHNIPKNGD, dict);
                } else {
                    DictionaryParams.Add(ODMMGJAPFBM, dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(DJNHNIPKNGD));
                }
                foreach (string item in dict.Keys.ToList()) {
                    dict[item] = ParamSystem.Base64Decode(dict[item]);
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(UnpackDictionaryNode))]
        public static class UnpackDictionaryNodePatches {
            [HarmonyPatch("IHCGBFELBLL")]
            [HarmonyPrefix]
            public static bool ActivateHook(UnpackDictionaryNode __instance, List<LJOOJBCCAOF> ____callbacks, LJOOJBCCAOF ___targetCallback, LJOOJBCCAOF ___dataCallback) {
                CKHLPKKNCBC cKHLPKKNCBC = null;
                foreach (LJOOJBCCAOF callback in ____callbacks) {
                    if (callback.GetValue<CKHLPKKNCBC>() != null) {
                        cKHLPKKNCBC = callback.GetValue<CKHLPKKNCBC>();
                        break;
                    }
                }
                if (cKHLPKKNCBC != null) {
                    if (___targetCallback == null) {
                        string inputSocketValue2 = ___dataCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.paramText.text, cKHLPKKNCBC);
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
                        string inputSocketValue = ___targetCallback.GetValue<string>() ?? __instance.dictionaryName.text;
                        string inputSocketValue2 = ___dataCallback?.GetValue<string>() ?? ParamSystemPatches.InsertParamValues(__instance.paramText.text, cKHLPKKNCBC);
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