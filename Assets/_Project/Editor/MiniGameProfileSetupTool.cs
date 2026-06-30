#if UNITY_EDITOR
namespace Project.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Project.Data;

    public static class MiniGameProfileSetupTool
    {
        [MenuItem("Tools/Mini Game Profiles/Create Cosmic Hopper Profile")]
        public static void CreateCosmicHopperProfile()
        {
            MiniGameProfile profile = ScriptableObject.CreateInstance<MiniGameProfile>();

            SerializedObject so = new SerializedObject(profile);
            so.FindProperty("_displayNameKey").stringValue = "mini_game_cosmic_hopper";
            so.FindProperty("_educationalWeight").floatValue = 20f;
            so.FindProperty("_pedagogicalWeight").floatValue = 10f;
            so.FindProperty("_entertainmentWeight").floatValue = 70f;
            so.ApplyModifiedProperties();

            string path = "Assets/_Project/Data/MiniGameProfile_CosmicHopper.asset";
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Created MiniGameProfile at: {path}");
            EditorUtility.DisplayDialog("Profile Created", $"Cosmic Hopper profile created at:\n{path}", "OK");
        }

        [MenuItem("Tools/Mini Game Profiles/Create Color Cube Profile")]
        public static void CreateColorCubeProfile()
        {
            MiniGameProfile profile = ScriptableObject.CreateInstance<MiniGameProfile>();

            SerializedObject so = new SerializedObject(profile);
            so.FindProperty("_displayNameKey").stringValue = "mini_game_color_cube";
            so.FindProperty("_educationalWeight").floatValue = 40f;
            so.FindProperty("_pedagogicalWeight").floatValue = 35f;
            so.FindProperty("_entertainmentWeight").floatValue = 25f;
            so.ApplyModifiedProperties();

            string path = "Assets/_Project/Data/MiniGameProfile_ColorCube.asset";
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Created MiniGameProfile at: {path}");
            EditorUtility.DisplayDialog("Profile Created", $"Color Cube profile created at:\n{path}", "OK");
        }
    }
}
#endif
