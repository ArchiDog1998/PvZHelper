using System.Reflection;
using UnityEngine;

namespace PvZHelper.Behaviors;

internal class AutoSunBehavior : MonoBehaviour
{
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F2))
        //{
        //    _uiOpened = !_uiOpened;
        //}

        if (Board.Instance == null) return;

        Board.Instance.freeCD = true; // OffGcd?.Value ?? false;
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
}
