using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetReferences : EditorWindow
{
    private Object targetAsset;
    private bool searchPrefabs = true;
    private bool searchScenes = true;
    private bool searchMaterials = false;
    private bool searchAssets = false;
    private bool filterBySpriteType = false;
    private string resultLogPath = "Assets/Logs/AssetReferenceSearchResult.txt";
    private List<string> searchResults = new List<string>();
    private Vector2 scroll;

    [MenuItem("Tools/AssetReference Finder")]
    public static void ShowWindow()
    {
        GetWindow<AssetReferences>("AssetReference Finder");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        targetAsset = EditorGUILayout.ObjectField("Target Asset", targetAsset, typeof(Object), false);
        searchPrefabs = EditorGUILayout.Toggle("Search Prefabs (.prefab)", searchPrefabs);
        searchScenes = EditorGUILayout.Toggle("Search Scenes (.unity)", searchScenes);
        searchMaterials = EditorGUILayout.Toggle("Search Materials (.mat)", searchMaterials);
        searchAssets = EditorGUILayout.Toggle("Search Other Assets (.asset)", searchAssets);
        filterBySpriteType = EditorGUILayout.Toggle("Filter by Sprite Type Only", filterBySpriteType);
        resultLogPath = EditorGUILayout.TextField("Result Save Path", resultLogPath);

        if (GUILayout.Button("Find References"))
        {
            if (targetAsset == null)
            {
                Debug.LogError("Select a target asset to search !!");
                return;
            }

            FindReferences();
        }


        if (searchResults.Count > 0)
        {
            GUILayout.Label($"{searchResults.Count} reference(s) found:", EditorStyles.boldLabel);

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(200));
            foreach (string path in searchResults)
            {
                if (GUILayout.Button(path, EditorStyles.linkLabel))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
                }
            }
            GUILayout.EndScrollView();
        }
    }

    private void FindReferences()
    {
        string assetPath = AssetDatabase.GetAssetPath(targetAsset);
        string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

        if (string.IsNullOrEmpty(assetGUID))
        {
            Debug.LogError("Failed to retrieve the asset's GUID.");
            return;
        }

        searchResults.Clear();

        List<string> extensions = new List<string>();
        if (searchPrefabs) extensions.Add("*.prefab");
        if (searchScenes) extensions.Add("*.unity");
        if (searchMaterials) extensions.Add("*.mat");
        if (searchAssets) extensions.Add("*.asset");

        int progress = 0;
        int totalProgress = extensions.Count;

        foreach (var ext in extensions)
        {
            EditorUtility.DisplayProgressBar("Searching...", $"Searching {ext}", (float)progress / totalProgress);
            searchResults.AddRange(SearchInFiles("Assets", ext, assetGUID));
            progress++;
        }

        EditorUtility.ClearProgressBar();

        if (searchResults.Count > 0)
        {
            string header = $"Found {searchResults.Count} file(s) referencing `{assetPath}`:\n";
            string resultText = header + string.Join("\n", searchResults);

            Debug.Log(resultText);
            Directory.CreateDirectory(Path.GetDirectoryName(resultLogPath));
            File.WriteAllText(resultLogPath, resultText);
            AssetDatabase.Refresh();
            EditorUtility.RevealInFinder(resultLogPath);
        }
        else
        {
            Debug.Log($"Could not find any references to: {assetPath}");
        }
    }

    private List<string> SearchInFiles(string rootPath, string extension, string targetGUID)
    {
        List<string> found = new List<string>();
        string[] files = Directory.GetFiles(rootPath, extension, SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string assetFile = file.Replace("\\", "/");

            if (filterBySpriteType && (extension == "*.prefab" || extension == "*.unity"))
            {
                var objs = AssetDatabase.LoadAllAssetsAtPath(assetFile);
                foreach (var obj in objs)
                {
                    if (obj is GameObject go)
                    {
                        foreach (var comp in go.GetComponentsInChildren<SpriteRenderer>(true))
                        {
                            if (comp.sprite != null && AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(comp.sprite)) == targetGUID)
                            {
                                found.Add(assetFile);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                string content = File.ReadAllText(assetFile);
                if (content.Contains(targetGUID))
                {
                    found.Add(assetFile);
                }
            }
        }

        return found;
    }
}

