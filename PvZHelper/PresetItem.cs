namespace PvZHelper;

internal readonly struct PresetItem(params SeedType[] seeds)
{
    public readonly List<SeedType>? Seeds = [..seeds];

    public void SetPlants(int column, int row)
    {
        if (Seeds == null) return;
        foreach (var item in Seeds)
        {
            CreatePlant.Instance.SetPlant(column, row, (int)item);
        }
    }

    public static implicit operator PresetItem(SeedType seed) => new([SeedType.莲叶, seed, SeedType.激光南瓜]);
}
