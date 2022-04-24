#if (UNITY_EDITOR)


using UnityEngine;
using UnityEditor;
using System;
public class SetupFontEditor : EditorWindow
{
    private static Font selectedFont;
    private static string x;
    private static string y;
    private static string cellWidth = "16";
    private static string cellHeight = "16";
    private static string canvasWidth = "208";
    private static string canvasHeight = "32";
    private static string characterSet = "abcdefghijklmnopqrstuvwxyz1234567890 .,\n";
    [MenuItem("Window/Set up font")]
    public static void WindowPopup()
    {
        GetWindow<SetupFontEditor>("Font Adjustment Editor");
    }
    private void OnGUI()
    {

        Rect rect = new Rect(3, 25, position.width - 6, 20);
        GUILayout.Label("Set up Font automatically");
        selectedFont = (Font)EditorGUI.ObjectField(rect, "Font:", selectedFont, typeof(Font), false);
        GUILayout.Space(32);
        if (selectedFont != null)
        {
            #region Input Fields
            GUILayout.BeginHorizontal();
            GUILayout.Label("Start Position");
            x = GUILayout.TextField(x);
            y = GUILayout.TextField(y);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Cell Size");
            cellWidth = GUILayout.TextField(cellWidth);
            cellHeight = GUILayout.TextField(cellHeight);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Canvas Size");
            canvasWidth = GUILayout.TextField(canvasWidth);
            canvasHeight = GUILayout.TextField(canvasHeight);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Character Set");
            characterSet = GUILayout.TextField(characterSet);
            GUILayout.EndHorizontal();
            #endregion
            if (GUILayout.Button("Apply Settings"))
            {
               int.TryParse(SetupFontEditor.x, out int x);
               int.TryParse(SetupFontEditor.y, out int y);
                int.TryParse(cellWidth, out int cW);
                int.TryParse(cellHeight, out int cH);
                int.TryParse(canvasWidth, out int w);
               int.TryParse(canvasHeight, out int h);
                AdjustSettings(new Vector2(x, y), new Vector2(cW, cH), new Vector2(w, h));
            }
        }
    }
    private void AdjustSettings(Vector2 startPosition, Vector2 gridSize, Vector2 canvasSize)
    {
        if (!characterSet.Contains("\n"))
        {
            characterSet += "\n";
        }
        byte[] chars = CharToByteArray(CharacterSetToCharArray());
        int[] intChars = BytesToIntArray(chars);
        CharacterInfo[] charInfoNew = new CharacterInfo[selectedFont.characterInfo.Length];
        Vector2 fraction = gridSize / canvasSize;
        startPosition /= canvasSize;
        for (int i = 0; i < selectedFont.characterInfo.Length; i++)
        {
            CharacterInfo curr = selectedFont.characterInfo[i];
#pragma warning disable CS0618 // Type or member is obsolete
            curr.uv = new Rect(startPosition.x,1 - fraction.y - startPosition.y, fraction.x, fraction.y);
#pragma warning restore CS0618 // Type or member is obsolete

            curr.index = intChars[i];

#pragma warning disable CS0618 // Type or member is obsolete
            curr.vert = new Rect(0, 0, gridSize.x, -gridSize.y);
#pragma warning restore CS0618 // Type or member is obsolete

            curr.advance = (int)gridSize.x;



            charInfoNew[i] = curr;

            startPosition.x += fraction.x;
            if (startPosition.x >= 1f)
            {
                startPosition.x = 0;
                startPosition.y += fraction.y;
            }
        }
        selectedFont.characterInfo = charInfoNew;
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(selectedFont);
        AssetDatabase.SaveAssets();
        Debug.Log("Completed!");
    }
    private int[] BytesToIntArray(byte[] byteArray)
    {
        int[] result = new int[byteArray.Length];
        for (int i = 0; i < result.Length; i++) 
        {
            result[i] = byteArray[i];
        }
        return result;
    }
    private byte[] CharToByteArray(char[] chars)
    {
        byte[] result = new byte[chars.Length * sizeof(char)];
        for (int i = 0; i < chars.Length; i++)
        {
            short c = (short)chars[i];
            byte[] chunk = BitConverter.GetBytes(c);
            result[i] = chunk[0];
            result[i + 1] = chunk[1];
        }
        return result;
    }
    private char[] CharacterSetToCharArray()
    {
        
        char[] result = new char[characterSet.Length];
        for (int i = 0; i < characterSet.Length; i++)
        {
            result[i] = characterSet[i];
        }

        return result;
    }
}
#endif