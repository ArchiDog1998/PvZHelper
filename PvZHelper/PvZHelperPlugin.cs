using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using PvZHelper.Behaviors;
using UnityEngine;

namespace PvZHelper;

[BepInPlugin(GUID, "PvZ Helper", "1.0.0")]
public class PvZHelperPlugin : BasePlugin
{
    private readonly Harmony _harmony = new(GUID);
    private const string GUID = "pvz.rh.helper";

    public static ConfigEntry<bool>? OffGcd, UnlimitedAllPlants, Columns;
    public static ConfigEntry<string>? Savings;
    public static List<int[,]> Saves = [
            new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0],
            ];
    public override void Load()
    {
        _harmony.PatchAll();

        SetupConfigs();
        AddBehaviors();
    }

    private void SetupConfigs()
    {
        OffGcd = Config.Bind("1.常规", "无冷却模式", true, "植物不受冷却的限制");
        UnlimitedAllPlants = Config.Bind("1.常规", "解开所有配方", true, "解开所有的植物配方");
        Columns = Config.Bind("1.常规", "柱子模式", true, "打开所有柱子");
    }

    private static void AddBehaviors()
    {
        GameObject obj = new("CustomObject")
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        UnityEngine.Object.DontDestroyOnLoad(obj);

        AddBehavior<AutoSunBehavior>(obj);
    }

    private static void AddBehavior<T>(GameObject obj) where T : Component
    {
        ClassInjector.RegisterTypeInIl2Cpp<T>();
        obj.AddComponent<T>();
    }

    public override bool Unload()
    {
        _harmony.UnpatchSelf();
        return base.Unload();
    }


    //private List<int[,]> LoadSaves(string str)
    //{
    //    var data = JsonConvert.DeserializeObject<List<List<List<int>>>>(str);

    //    var result = data.Select(s =>
    //    {
    //        var items = s.Select(s => s.Count);
    //        var res = new int[s.Count, items.Any() ? items.Max() : 0];

    //        for (int i = 0; i < s.Count; i++)
    //        {
    //            for (int j = 0; j < s[i].Count; j++)
    //            {
    //                res[i,j] = s[i][j];
    //            }
    //        }

    //        return res;
    //    }).ToList();

    //    if (result.Count < 5)
    //    {
    //        return 
    //        [
    //            new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0],
    //        ];
    //    }
    //    return result;
    //}
    //private string SaveSaves(List<int[,]> value)
    //{
    //    var result = value.Select(s =>
    //    {
    //        var res = new List<List<int>>();
    //        for (int i = 0; i < s.GetLength(0); i++)
    //        {
    //            var ls = new List<int>();
    //            for (int j = 0; j < s.GetLength(1); j++)
    //            {
    //                ls.Add(s[i, j]);
    //            }
    //            res.Add(ls);
    //        }

    //        return res;
    //    }).ToList();

    //    return JsonConvert.SerializeObject(result);
    //}

    //public override void Load()
    //{
    //    //Start();
    //}

    //private void Start()
    //{
    //    _harmony = new(GUID);
    //    _harmony.PatchAll();

    //    var json = Resources.Load<TextAsset>("LawnStrings").text;
    //    var plants = JsonUtility.FromJson<AlmanacMgr.PlantData>(json).plants;

    //    Dictionary<int, string> additional = [];
    //    for (int i = 0; i < GameAPP.plantPrefab.Length; i++)
    //    {
    //        var plant = GameAPP.plantPrefab[i];
    //        if (plant == null) continue;

    //        var name = plants.FirstOrDefault(p => p.seedType == i)?.name ?? plant.name;
    //        if (i >= 900 && i < 1000)
    //        {
    //            _plants[i] = name;
    //        }
    //        else
    //        {
    //            additional[i] = name;
    //        }
    //    }

    //    foreach (var item in additional)
    //    {
    //        _plants[item.Key] = item.Value;
    //    }
    //}

    //private Rect windowRect = new (10, 10, 200, 540); // 不显示Popup时窗口的大小
    //private void OnGUI()
    //{
    //    if (!_uiOpened) return;

    //    //windowRect = GUILayout.Window(0, windowRect, DrawWindow, "PvZ Helper");
    //}

    Vector2 scrollPosition = default;
    //public void DrawWindow(int windowID)
    //{
    //    Time.timeScale = GameAPP.gameSpeed = GUILayout.HorizontalSlider(GameAPP.gameSpeed, 0.1f, 10);

    //    if(Saves != null)
    //    {
    //        for (int i = 0; i < Saves.Count; i++)
    //        {
    //            GUILayout.BeginHorizontal();
    //            if (GUILayout.Button("Load " + i, GUILayout.ExpandWidth(false)))
    //            {
    //                for (int x = 0; x < Saves[i].GetLength(0); x++)
    //                {
    //                    for (int y = 0; y < Saves[i].GetLength(1); y++)
    //                    {
    //                        var id = Saves[i][x, y];
    //                        if (id < 0) continue;
    //                        CreatePlant.Instance.SetPlant(x, y, 12); //Lily
    //                        CreatePlant.Instance.SetPlant(x, y, id);
    //                    }
    //                }
    //            }
    //            if (GUILayout.Button("Save " + i, GUILayout.ExpandWidth(false)))
    //            {
    //                Saves[i] = new int[Board.Instance.boxType.GetLength(0), Board.Instance.boxType.GetLength(1)];
    //                for (int x = 0; x < Saves[i].GetLength(0); x++)
    //                {
    //                    for (int y = 0; y < Saves[i].GetLength(1); y++)
    //                    {
    //                        Saves[i][x, y] = -1;
    //                    }
    //                }
    //                foreach (var obj in Board.Instance.plantArray)
    //                {
    //                    if (obj == null) continue;
    //                    var plant = obj.GetComponent<Plant>();
    //                    Saves[i][plant.thePlantColumn, plant.thePlantRow] = plant.thePlantType;
    //                }
    //                if (Savings != null)
    //                {
    //                    Savings.Value = SaveSaves(Saves);
    //                }
    //            }
    //            GUILayout.EndHorizontal();
    //        }
    //    }

    //    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    //    foreach ( var item in _plants)
    //    {
    //        if (GUILayout.Button(item.Value))
    //        {
    //            var id = item.Key;
    //            Mouse.Instance.thePlantTypeOnMouse = id;
    //            Destroy(Mouse.Instance.theItemOnMouse);
    //            typeof(Mouse).GetRuntimeMethods().FirstOrDefault(m => m.Name == "CreatePlantOnMouse")?.Invoke(Mouse.Instance, [id]);
    //        }
    //    }
       
    //    GUILayout.EndScrollView();
    //    GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    //}
}
