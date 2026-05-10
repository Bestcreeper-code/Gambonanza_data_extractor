using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Logging;
using Blukulele.CHE;
using Blukulele.Core;
using Blukulele.SaveSystem;
using HarmonyLib;
using SaveSystem;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gambo
{
    [BepInPlugin("com.bestcreeper.gambolol", "GamboLol", "1.0.0")]
    public class PluginMain : BaseUnityPlugin
    {
        public static ManualLogSource _logger;
        
        public readonly Harmony _harmony = new Harmony("com.bestcreeper.gambolol");

        public void Awake()
        {
            Logger.LogInfo("Plugin is loaded!");

            GameObject updaterObject = new GameObject("PluginUpdater");
            updaterObject.AddComponent<PluginUpdater>();
            DontDestroyOnLoad(updaterObject);

            _logger = Logger;
            
            _harmony.PatchAll();
        }
        
        private static string Clean(string input)
        {
            string output = Regex.Replace(
                input,
                "#(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{3})\\b",//html colors
                ""
            );

            output = Regex.Replace(output, "<.*?>", "");
            output = Regex.Replace(output, "\\(\\s*\\)", "");// (      )

            output = Regex.Replace(output, "<[^>]+>", " "); //multispces
            output = output.Replace("  "," ");              //multisapces

            return output;
        }
        public static void UpdatePlugin()
        {
            // afk
            //     brave knight
            //     dragon egg
            //     grandma's gift
            // Gambit_GrandMaLetter
                // TokenToBuy
                // PawnPieceBehaviour

        
            if (Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
            {
                string path = Path.Combine(Paths.PluginPath, "Gambo", "gambits.txt");
                JSONNode traduction = SingletonMonoBehaviour<LocalizationManager>.Instance.GetTraduction();

                using (var writer = new StreamWriter(path, false))
                {
                    foreach (var gambit in GambitLibrary.Instance.Gambits)
                    {
                        string formatted =
                            $"Name: {Clean(LocalizationManager.Instance.RewriteDescription(traduction["gambit"][gambit.Info.GambitName]))} " +
                            $"| Description: {Clean(LocalizationManager.Instance.RewriteDescription(traduction["gambit"][gambit.Info.GambitDescription]))} " +
                            $"| Rarity: {gambit.Info.Rarity} " +
                            $"| Unlock Method: {Clean(LocalizationManager.Instance.RewriteDescription(traduction["gambit-unlock-method"][gambit.Info.GambitName.Replace("_name","")]))}";
                            
                            // + $"| Focus: {gambit.Info.Focus[0].ToString()}";
                        
                        writer.WriteLine(formatted);
                        _logger.LogInfo(formatted);
                    } 
                }
            } 
            else if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
            {
                string path = Path.Combine(Paths.PluginPath, "Gambo", "strains.txt");
                JSONNode traduction = SingletonMonoBehaviour<LocalizationManager>.Instance.GetTraduction();
                
                using (var writer = new StreamWriter(path, false))
                {
                    
                    var strain = traduction["strain"].AsObject;
                    _logger.LogInfo(strain.AsArray);

                    var enumerator = strain.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var kv = (KeyValuePair<string, JSONNode>)enumerator.Current;

                        if (!kv.Key.EndsWith("-name"))
                            continue;

                        var prefix = kv.Key.Substring(0, kv.Key.Length - 5);

                        string name = kv.Value;
                        string desc = strain[prefix + "-description"];

                        string frm_message = $"Name: {Clean(LocalizationManager.Instance.RewriteDescription(name))} " +
                                             $"| Description: {Clean(LocalizationManager.Instance.RewriteDescription(desc))} ";
                        
                        writer.WriteLine(frm_message);
                        _logger.LogInfo(frm_message);
                    }
                }
            } 
            else if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl) &&
                     Input.GetKey(KeyCode.LeftShift))
            {
                Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
                string basePath = Path.Combine(Paths.PluginPath, "Gambo", "Textures");
                

                foreach (var sprite in sprites)
                {
                    
                    if (sprite == null || sprite.texture == null) continue;

                    try
                    {
                        Texture2D readable = ExtractSprite(sprite);

                        string path = Path.Combine(basePath, $"{sprite.name}_sprite.png");
                        File.WriteAllBytes(path, ImageConversion.EncodeToPNG(readable));

                        _logger.LogInfo($"sprite: {sprite.name} | {sprite.rect.width}x{sprite.rect.height}");
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning($"sprite failed: {sprite.name} | {e.Message}");
                    }
                }
                
                
            }
            
            Texture2D ExtractSprite(Sprite sprite)
            {
                Texture2D tex = sprite.texture;

                RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height);
                Graphics.Blit(tex, rt);

                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = rt;

                Rect r = sprite.rect;

                Texture2D readable = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGBA32, false);
                readable.ReadPixels(r, 0, 0);
                readable.Apply();

                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rt);

                return readable;
            }
                
                
            // if (Input.GetKeyDown(KeyCode.U))
            // {
            //     SingletonMonoBehaviour<ChessDataManager>.Instance?.IncreaseCoin(404);
            //     Transform tr = new GameObject().transform;
            //     
            //     SingletonMonoBehaviour<MoneyAnimationManager>.Instance?.SpawnMoney(tr, 404);
            //     
            //     
            //     // SingletonMonoBehaviour<GambitManager>.Instance?.
            // }
            // else if (Input.GetKeyDown(KeyCode.I))
            // {
            //     var places = SingletonMonoBehaviour<GambitManager>.Instance?.GambitPlaces;
            //
            //     if (places != null && places.Length > 0)
            //     {
            //         
            //         _logger.LogInfo($"presize = {SingletonMonoBehaviour<GambitManager>.Instance?.GambitPlaces.Length}");
            //         var last = places[places.Length - 1];
            //         var clone = UnityEngine.Object.Instantiate(last, last.transform.parent);
            //
            //         var newArray = new GambitPlaceBehaviour[places.Length + 1];
            //
            //         Array.Copy(places, newArray, places.Length);
            //         newArray[newArray.Length - 1] = clone;
            //
            //         SingletonMonoBehaviour<GambitManager>.Instance.GambitPlaces = newArray;
            //         _logger.LogInfo($"aftersize = {SingletonMonoBehaviour<GambitManager>.Instance?.GambitPlaces.Length}");
            //     }
            // }
                // if(DataManager.Instance)DataManager.Instance.Data.CRTValue = 3f;
            // SingletonMonoBehaviour<ChessDataManager>.Instance;
        }
    }

    public class PluginUpdater : MonoBehaviour
    {
        void Update()
        {
            try
            {
                PluginMain.UpdatePlugin();
            }
            catch (Exception e)
            {
                PluginMain._logger.LogError($"Plugin updater error: {e}");
            }
        }
    }
    
        
    [HarmonyPatch(typeof(CanvasMenu), "Start")]
    class Patch_Gambo_Start_Menu
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                
                if (instr.opcode == OpCodes.Ldc_R4 && (float)instr.operand == 0.99f)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 0f);
                }
                else
                {
                    yield return instr;
                }
            }
        }
    }
    // [HarmonyPatch(typeof(CanvasPachinko), "GetTiles")]
    // class Patch_GetTiles
    // {
    //     public static void Postfix(ref SO_Tiles[] __result)
    //     {
    //         if (__result == null || __result.Length == 0)
    //             return;
    //
    //         // 20% chance to replace a random tile
    //         //if (UnityEngine.Random.value < 0.2f)
    //         //{
    //             int index = UnityEngine.Random.Range(0, __result.Length);
    //             SO_Tiles tile = TileRegistery.GetCustomTile();
    //             tile.Visual_UI = __result[index].Visual_UI;
    //             tile.Visual_World = __result[index].Visual_World;
    //
    //             __result[index] = tile;
    //             //}
    //     }
    // }
    

    [HarmonyPatch(typeof(PawnPieceBehaviour), "GetTilesAvailable")]
    class PatchPawnTiles
    {
        static void Postfix(PawnPieceBehaviour __instance, ref List<TileBehaviour> __result)
        {
            for (int i = 1; i <= 8; i++)
            {
                string fieldName = $"m_HiddenQueen_{i}";
                FieldInfo field = typeof(PawnPieceBehaviour).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

                if (field == null) continue;

                Transform[] hiddenQueenTiles = field.GetValue(__instance) as Transform[];
                if (hiddenQueenTiles == null) continue;

                foreach (var t in hiddenQueenTiles)
                {
                    if (t == null) continue;

                    // Example: log or add to tiles
                    // PluginMain._logger.LogInfo($"{fieldName} - {t.ToString()} @ {t.position.ToString()}");
                    
                }

                if (i == 6)//forward
                {
                    if (__instance != null)
                    {
                        MethodInfo method = typeof(PawnPieceBehaviour)
                            .GetMethod("FillTilesList", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (method != null)
                        {
                            Transform[] diago = hiddenQueenTiles;
                            
                            List<TileBehaviour> result = method.Invoke(__instance, new object[] { diago }) as List<TileBehaviour>;
                            
                            if(result != null&& result.Count>=2)
                            {
                                __result.Add((result[1]));
                                PluginMain._logger.LogInfo("added 2x move");
                            }
                        }
                    }
                    
                }
            }
        }
    }
}