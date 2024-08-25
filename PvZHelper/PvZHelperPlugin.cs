using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
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
    public static ConfigEntry<int[,]>? Saved1;

    private bool _uiOpened = false;
    private readonly Dictionary<int, string> _plants = [];

    private void Awake()
    {
        OffGcd = Config.Bind("1.常规", "无冷却模式", false, "植物不受冷却的限制");
        UnlimitedAllPlants = Config.Bind("1.常规", "解开所有配方", false, "解开所有的植物配方");
        Columns = Config.Bind("1.常规", "柱子模式", false, "打开所有柱子");
        Instance = this;
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
