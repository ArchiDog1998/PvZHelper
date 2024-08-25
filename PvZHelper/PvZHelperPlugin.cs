using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Newtonsoft.Json;
using System.Reflection;
using UnityEngine;

namespace PvZHelper;

[BepInPlugin(GUID, "PvZ Helper", "1.0.0")]
public class PvZHelperPlugin : BaseUnityPlugin
{
    public static PvZHelperPlugin? Instance;

    private Harmony? _harmony = null;
    private const string GUID = "pvz.rh.helper";

    public static ConfigEntry<bool>? OffGcd, UnlimitedAllPlants, Columns;
    public static ConfigEntry<string>? Savings;
    public static List<int[,]> Saves = [
            new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0],
        ];

private bool _uiOpened = false;
    private readonly Dictionary<int, string> _plants = [];

    private void Awake()
    {
        OffGcd = Config.Bind("1.常规", "无冷却模式", false, "植物不受冷却的限制");
        UnlimitedAllPlants = Config.Bind("1.常规", "解开所有配方", false, "解开所有的植物配方");
        Columns = Config.Bind("1.常规", "柱子模式", false, "打开所有柱子");
        Savings = Config.Bind("2.预设", "预设1", SaveSaves(Saves), "预设1");
        Instance = this;
        Saves = LoadSaves(Savings.Value);
    }

    private List<int[,]> LoadSaves(string str)
    {
        var data = JsonConvert.DeserializeObject<List<List<List<int>>>>(str);

        var result = data.Select(s =>
        {
            var items = s.Select(s => s.Count);
            var res = new int[s.Count, items.Any() ? items.Max() : 0];

            for (int i = 0; i < s.Count; i++)
            {
                for (int j = 0; j < s[i].Count; j++)
                {
                    res[i,j] = s[i][j];
                }
            }

            return res;
        }).ToList();

        if (result.Count < 5)
        {
            return 
            [
                new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0], new int[0, 0],
            ];
        }
        return result;
    }
    private string SaveSaves(List<int[,]> value)
    {
        var result = value.Select(s =>
        {
            var res = new List<List<int>>();
            for (int i = 0; i < s.GetLength(0); i++)
            {
                var ls = new List<int>();
                for (int j = 0; j < s.GetLength(1); j++)
                {
                    ls.Add(s[i, j]);
                }
                res.Add(ls);
            }

            return res;
        }).ToList();

        return JsonConvert.SerializeObject(result);
    }

    private void Start()
    {
        _harmony = new(GUID);
        _harmony.PatchAll();

        var json = Resources.Load<TextAsset>("LawnStrings").text;
        var plants = JsonUtility.FromJson<AlmanacMgr.PlantData>(json).plants;

        Dictionary<int, string> additional = [];
        for (int i = 0; i < GameAPP.plantPrefab.Length; i++)
        {
            var plant = GameAPP.plantPrefab[i];
            if (plant == null) continue;

            var name = plants.FirstOrDefault(p => p.seedType == i)?.name ?? plant.name;
            if (i >= 900 && i < 1000)
            {
                _plants[i] = name;
            }
            else
            {
                additional[i] = name;
            }
        }

        foreach (var item in additional)
        {
            _plants[item.Key] = item.Value;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            _uiOpened = !_uiOpened;
        }

        if (Board.Instance == null) return;

        Board.Instance.freeCD = OffGcd?.Value ?? false;
        if ((Mouse.Instance.theItemOnMouse?.TryGetComponent<GloveMgr>(out var gloveMgr) ?? false)
            && gloveMgr != null)
        {
            typeof(GloveMgr).GetRuntimeFields().FirstOrDefault(f => f.Name == "fullCD")?.SetValue(gloveMgr, 0);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Board.Instance.theSun -= 1000;
            InGameText.Instance.EnableText("阳光减少1000", 1f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Board.Instance.theSun += 1000;
            InGameText.Instance.EnableText("阳光增加1000", 1f);
        }
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchAll();
    }

    private Rect windowRect = new (10, 10, 200, 540); // 不显示Popup时窗口的大小
    private void OnGUI()
    {
        if (!_uiOpened) return;

        windowRect = GUILayout.Window(0, windowRect, DrawWindow, "PvZ Helper");
    }

    Vector2 scrollPosition = default;
    public void DrawWindow(int windowID)
    {
        Time.timeScale = GameAPP.gameSpeed = GUILayout.HorizontalSlider(GameAPP.gameSpeed, 0.1f, 10);

        if(Saves != null)
        {
            for (int i = 0; i < Saves.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Load " + i, GUILayout.ExpandWidth(false)))
                {
                    for (int x = 0; x < Saves[i].GetLength(0); x++)
                    {
                        for (int y = 0; y < Saves[i].GetLength(1); y++)
                        {
                            var id = Saves[i][x, y];
                            if (id < 0) continue;
                            CreatePlant.Instance.SetPlant(x, y, 12); //Lily
                            CreatePlant.Instance.SetPlant(x, y, id);
                        }
                    }
                }
                if (GUILayout.Button("Save " + i, GUILayout.ExpandWidth(false)))
                {
                    Saves[i] = new int[Board.Instance.boxType.GetLength(0), Board.Instance.boxType.GetLength(1)];
                    for (int x = 0; x < Saves[i].GetLength(0); x++)
                    {
                        for (int y = 0; y < Saves[i].GetLength(1); y++)
                        {
                            Saves[i][x, y] = -1;
                        }
                    }
                    foreach (var obj in Board.Instance.plantArray)
                    {
                        if (obj == null) continue;
                        var plant = obj.GetComponent<Plant>();
                        Saves[i][plant.thePlantColumn, plant.thePlantRow] = plant.thePlantType;
                    }
                    if (Savings != null)
                    {
                        Savings.Value = SaveSaves(Saves);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach ( var item in _plants)
        {
            if (GUILayout.Button(item.Value))
            {
                var id = item.Key;
                Mouse.Instance.thePlantTypeOnMouse = id;
                Destroy(Mouse.Instance.theItemOnMouse);
                typeof(Mouse).GetRuntimeMethods().FirstOrDefault(m => m.Name == "CreatePlantOnMouse")?.Invoke(Mouse.Instance, [id]);
            }
        }
       
        GUILayout.EndScrollView();
        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
}
