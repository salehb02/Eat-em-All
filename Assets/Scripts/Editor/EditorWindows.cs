using UnityEditor;

public class EditorWindows : Editor
{
    [MenuItem("Eat 'em All/Control Panel", false, 0)]
    public static void OpenControlPanel()
    {
        Selection.activeObject = ControlPanel.Instance;
    }
}