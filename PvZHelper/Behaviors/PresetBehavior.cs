using UnityEngine;

namespace PvZHelper.Behaviors;

internal class PresetBehavior : MonoBehaviour
{
    private readonly Dictionary<KeyCode, PresetItem[][]> Presets = new()
    {
        [KeyCode.F1] = 
        [
            [SeedType.究极大喷菇, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极火爆窝炬, SeedType.矿工南瓜, SeedType.究极忧郁菇, new(SeedType.黑曜石高坚果), new(SeedType.玄钢地刺王)],
            [SeedType.究极大喷菇, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极火爆窝炬, SeedType.矿工南瓜, SeedType.究极忧郁菇, new(SeedType.黑曜石高坚果), new(SeedType.玄钢地刺王)],
            [SeedType.究极大喷菇, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极火爆窝炬, SeedType.矿工南瓜, SeedType.究极忧郁菇, new(SeedType.黑曜石高坚果), new(SeedType.玄钢地刺王)],
            [SeedType.究极大喷菇, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极火爆窝炬, SeedType.矿工南瓜, SeedType.究极忧郁菇, new(SeedType.黑曜石高坚果), new(SeedType.玄钢地刺王)],
            [SeedType.究极大喷菇, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极火爆窝炬, SeedType.矿工南瓜, SeedType.究极忧郁菇, new(SeedType.黑曜石高坚果), new(SeedType.玄钢地刺王)],
            [SeedType.究极大喷菇, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极樱桃射手, SeedType.究极火爆窝炬, SeedType.矿工南瓜, SeedType.究极忧郁菇, new(SeedType.黑曜石高坚果), new(SeedType.玄钢地刺王)],

        ],
    };

    private void Update()
    {
        foreach (var pair in Presets)
        {
            if (!Input.GetKeyDown(pair.Key)) continue;

            for (int i = 0; i < pair.Value.Length; i++)
            {
                var rows = pair.Value[i];
                for (int j = 0; j < rows.Length; j++)
                {
                    var row = rows[j];
                    row.SetPlants(j, i);
                }
            }
        }
    }
}
