﻿/*
Copyright (c) 2015-2016 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace LuaInterface
{
    public class LuaFileUtils
    {
        public static LuaFileUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LuaFileUtils();
                }

                return instance;
            }

            protected set
            {
                instance = value;
            }
        }

        //beZip = false 在search path 中查找读取lua文件。否则从外部设置过来bundel文件中读取lua文件
        public bool beZip = false;
        protected List<string> searchPaths = new List<string>();
        protected Dictionary<string, AssetBundle> zipMap = new Dictionary<string, AssetBundle>();

        protected static LuaFileUtils instance = null;

        public LuaFileUtils()
        {
            instance = this;
        }

        public virtual void Dispose()
        {
            if (instance != null)
            {
                instance = null;
                searchPaths.Clear();

                foreach (KeyValuePair<string, AssetBundle> iter in zipMap)
                {
                    iter.Value.Unload(true);
                }

                zipMap.Clear();
            }
        }

        public bool AddSearchPath(string path, bool front = false)
        {
            if (path.Length > 0 && path[path.Length - 1] != '/')
            {
                path += "/";
            }

            int index = searchPaths.IndexOf(path);

            if (index >= 0)
            {
                return false;
            }

            if (front)
            {
                searchPaths.Insert(0, path);
            }
            else
            {
                searchPaths.Add(path);
            }

            return true;
        }

        public void RemoveSearchPath(string path)
        {
            if (path.Length > 0 && path[path.Length - 1] != '/')
            {
                path += "/";
            }

            int index = searchPaths.IndexOf(path);

            if (index >= 0)
            {
                searchPaths.RemoveAt(index);
            }
        }

        public void AddSearchBundle(string name, AssetBundle bundle)
        {
            zipMap[name] = bundle;
            Debugger.Log("Add Lua bundle: " + name);
        }

        public string GetFullPathFileName(string fileName)
        {
            if (fileName == string.Empty)
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }
#if UNITY_EDITOR
            if (Logic.Game.GameConfig.instance.loadLuaRemote)
                fileName = fileName.Replace(".lua", ".txt");
#else
            fileName = fileName.Replace(".lua", ".txt");
#endif
            string fullPath = null;

            for (int i = 0; i < searchPaths.Count; i++)
            {
                fullPath = Path.Combine(searchPaths[i], fileName);

                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

        public virtual byte[] ReadFile(string fileName)
        {
            if (!beZip)
            {
                string path = GetFullPathFileName(fileName);
                byte[] str = null;

                if (File.Exists(path))
                {
#if !UNITY_WEBPLAYER
                    str = File.ReadAllBytes(path);
#if UNITY_EDITOR
                    if (Logic.Game.GameConfig.instance.loadLuaRemote)
                        str = Common.Util.EncryptUtil.MinusExcursionBytes(str, Logic.Game.GameConfig.excursion);
#endif
#if UNITY_IOS && !UNITY_EDITOR
                        str = Common.Util.EncryptUtil.MinusExcursionBytes(str, Logic.Game.GameConfig.excursion);
#endif
#else
                    throw new LuaException("can't run in web platform, please switch to other platform");
#endif
                }

                return str;
            }
            else
            {
                return ReadZipFile(fileName);
            }
        }

        byte[] ReadZipFile(string fileName)
        {
            AssetBundle zipFile = null;
            byte[] buffer = null;
            string zipName = "Lua";
            int pos = fileName.LastIndexOf('/');

            if (pos > 0)
            {
                zipName = fileName.Substring(0, pos);
                zipName.Replace('/', '_');
                zipName = string.Format("Lua_{0}", zipName);
                fileName = fileName.Substring(pos + 1);
            }

            zipMap.TryGetValue(zipName, out zipFile);

            if (zipFile != null)
            {
#if UNITY_5
                TextAsset luaCode = zipFile.LoadAsset<TextAsset>(fileName);
#else
                TextAsset luaCode = zipFile.Load(fileName, typeof(TextAsset)) as TextAsset;
#endif

                if (luaCode != null)
                {
                    buffer = luaCode.bytes;
                    Resources.UnloadAsset(luaCode);
                }
            }

            return buffer;
        }

        public static string GetOSDir()
        {
#if UNITY_STANDALONE
            return "Win";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_IPHONE
        return "iOS";
#else
        return "";
#endif
        }
    }
}
