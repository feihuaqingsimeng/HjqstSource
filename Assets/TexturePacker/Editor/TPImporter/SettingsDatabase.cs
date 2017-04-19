using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace TPImporter
{
    public class SettingsDatabase : ScriptableObject
    {
        [CustomEditor(typeof(SettingsDatabase))]
        public class SettingsDatabaseEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                SettingsDatabase instance = SettingsDatabase.getInstance();
                bool flag = EditorGUILayout.Toggle("Always import pivot points", instance.importPivots, new GUILayoutOption[0]);
                if (flag != instance.importPivots)
                {
                    instance.importPivots = flag;
                    instance.saveDatabase();
                }
            }
        }
        [SerializeField]
        private int version = 2;
        private bool importPivots = EditorPrefs.GetBool("TPImporter.ImportPivotPoints", true);
        private List<string> tpsheetFileNames = new List<string>();
        private List<string> textureFileNames = new List<string>();
        private List<string> normalmapFileNames = new List<string>();
        private const int DATABASE_VERSION = 2;
        private const string PIVOTPOINT_KEY = "TPImporter.ImportPivotPoints";
        private const string IMPORTER_DLL = "TexturePackerImporter.dll";
        private const string DATABASE_FILE = "SettingsTexturePackerImporter.txt";
        private const string ASSET_FILE = "SettingsTexturePackerImporter.asset";
        private static string DATA_PATH = Application.dataPath;
        private static string DEFAULT_PATH = SettingsDatabase.DATA_PATH + "/TexturePacker/Editor";
        private static string databaseFilePath;
        private static string assetFilePath;
        private static SettingsDatabase instance = null;
        private SettingsDatabase()
        {
            this.loadDatabase();
        }
        public void addSheet(string dataFile, string textureFile, string normalmapFile)
        {
            this.removeSheet(dataFile, false);
            this.tpsheetFileNames.Add(dataFile);
            this.textureFileNames.Add(textureFile);
            this.normalmapFileNames.Add(normalmapFile);
            this.saveDatabase();
        }
        public void removeSheet(string dataFile, bool save = true)
        {
            Dbg.Log("dataFile:" + dataFile);
            int num = this.tpsheetFileNames.IndexOf(dataFile);
            String str = string.Empty;
            int i = 0;
            foreach (var item in tpsheetFileNames)
            {
                i++;
                str += i + item + "\n";
            }
            Dbg.Log(str);
            Dbg.Log("num:" + num);
            if (num >= 0)
            {
                this.tpsheetFileNames.RemoveAt(num);
                this.textureFileNames.RemoveAt(num);
                this.normalmapFileNames.RemoveAt(num);
                if (save)
                {
                    this.saveDatabase();
                }
            }
        }
        public string spriteFileForNormalsFile(string normalsFile)
        {
            int num = this.normalmapFileNames.IndexOf(normalsFile);
            return (num < 0) ? null : this.textureFileNames[num];
        }
        public string spriteFileForDataFile(string dataFile)
        {
            Dbg.Log("dataFile:" + dataFile);
            int num = this.tpsheetFileNames.IndexOf(dataFile);
			String str = string.Empty;
            int i = 0;
            foreach (var item in tpsheetFileNames)
            {
                i++;
                str += i + item + "\n";
            }
            Dbg.Log(str);
            Dbg.Log("num:" + num);
            return (num < 0) ? null : this.textureFileNames[num];
        }
        public string normalsFileForDataFile(string dataFile)
        {
            int num = this.tpsheetFileNames.IndexOf(dataFile);
            return (num < 0) ? null : this.normalmapFileNames[num];
        }
        public string dataFileForSpriteFile(string spriteFile)
        {
            int num = this.textureFileNames.IndexOf(spriteFile);
            return (num < 0) ? null : this.tpsheetFileNames[num];
        }
        public bool isSpriteSheet(string textureFile)
        {
            return this.textureFileNames.Contains(textureFile);
        }
        public bool isNormalmapSheet(string textureFile)
        {
            return this.normalmapFileNames.Contains(textureFile);
        }
        public List<string> allDataFiles()
        {
            return new List<string>(this.tpsheetFileNames);
        }
        public bool importPivotPoints()
        {
            return this.importPivots;
        }
        private static void updateFileLocations()
        {
            string text = SettingsDatabase.DEFAULT_PATH;
            /*if (!File.Exists(text + "/TexturePackerImporter.dll"))
            {
                string[] files = Directory.GetFiles(SettingsDatabase.DATA_PATH, "TexturePackerImporter.dll", SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    text = files[0].Remove(files[0].Length - "TexturePackerImporter.dll".Length);
                }
                else
                {
                    Debug.LogError("TexturePackerImporter.dll not found in " + Application.dataPath);
                }
            }*/
            string str = "Assets/" + text.Remove(0, SettingsDatabase.DATA_PATH.Length);
            SettingsDatabase.databaseFilePath = (text + "/SettingsTexturePackerImporter.txt").Replace("//", "/");
            SettingsDatabase.assetFilePath = (str + "/SettingsTexturePackerImporter.asset").Replace("//", "/");
            Dbg.Log("database location: " + SettingsDatabase.databaseFilePath + "\nasset location: " + SettingsDatabase.assetFilePath);
        }
        public static SettingsDatabase getInstance()
        {
            if (SettingsDatabase.instance == null)
            {
                SettingsDatabase.updateFileLocations();
                SettingsDatabase.instance = ScriptableObject.CreateInstance<SettingsDatabase>();
                AssetDatabase.CreateAsset(SettingsDatabase.instance, SettingsDatabase.assetFilePath);
            }
            return SettingsDatabase.instance;
        }
        ~SettingsDatabase()
        {
            this.saveDatabase();
        }
        private static List<string> readStringList(StreamReader file)
        {
            List<string> list = new List<string>();
            while (file.Peek() == 45)
            {
                string item = file.ReadLine().Remove(0, 1).Trim();
                list.Add(item);
            }
            return list;
        }
        private void loadDatabase()
        {
            if (SettingsDatabase.databaseFilePath == null)
            {
                return;
            }
            try
            {
                Dbg.Log("Loading database " + SettingsDatabase.databaseFilePath);
                StreamReader streamReader = new StreamReader(SettingsDatabase.databaseFilePath);
                string text;
                while ((text = streamReader.ReadLine()) != null)
                {
                    string[] array = text.Split(new char[]
					{
						':'
					});
                    string a = array[0].Trim();
                    string s = array[1].Trim();
                    if (a == "version")
                    {
                        this.version = int.Parse(s);
                    }
                    else if (a == "importPivots")
                    {
                        this.importPivots = (int.Parse(s) != 0);
                    }
                    else if (a == "tpsheetFileNames")
                    {
                        this.tpsheetFileNames = SettingsDatabase.readStringList(streamReader);
                    }
                    else if (a == "textureFileNames")
                    {
                        this.textureFileNames = SettingsDatabase.readStringList(streamReader);
                    }
                    else if (a == "normalmapFileNames")
                    {
                        this.normalmapFileNames = SettingsDatabase.readStringList(streamReader);
                    }
                }
                string str = string.Empty;
                Dbg.Log(this.tpsheetFileNames.ToCustomString(new char[] { '-' }));
                Dbg.Log(this.textureFileNames.ToCustomString(new char[] { '-' }));
                Dbg.Log(this.normalmapFileNames.ToCustomString(new char[] { '-' }));
                streamReader.Close();
            }
            catch (IOException)
            {
            }
        }
        private static void writeStringList(StreamWriter file, List<string> list)
        {
            foreach (string current in list)
            {
                file.WriteLine("- " + current);
            }
        }
        private void saveDatabase()
        {
            if (SettingsDatabase.databaseFilePath == null)
            {
                return;
            }
            Dbg.Log("Saving database " + SettingsDatabase.databaseFilePath);
            StreamWriter streamWriter = new StreamWriter(SettingsDatabase.databaseFilePath);
            streamWriter.WriteLine(string.Format("version: {0}", this.version));
            streamWriter.WriteLine(string.Format("importPivots: {0}", (!this.importPivots) ? 0 : 1));
            streamWriter.WriteLine("tpsheetFileNames:");
            SettingsDatabase.writeStringList(streamWriter, this.tpsheetFileNames);
            streamWriter.WriteLine("textureFileNames:");
            SettingsDatabase.writeStringList(streamWriter, this.textureFileNames);
            streamWriter.WriteLine("normalmapFileNames:");
            SettingsDatabase.writeStringList(streamWriter, this.normalmapFileNames);
            streamWriter.Close();
        }
        [MenuItem("Edit/Project Settings/TexturePacker")]
        private static void NewMenuOption()
        {
            Selection.activeObject = SettingsDatabase.getInstance();
        }
    }
}
