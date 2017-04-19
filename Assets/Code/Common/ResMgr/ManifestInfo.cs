using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Common.ResMgr
{
    public class AssetInfo : System.Object
    {
        public AssetInfo(string subPath, long length, long createDate, string md5,string suffix, List<string> filePathList)
        {
            _subPath = subPath;
            _length = length;
            _createDate = createDate;
            _md5 = md5;
            _suffix = suffix;
            _filePathList = filePathList;
        }

        public AssetInfo(string subPath)
        {
            _subPath = subPath;
        }

        private List<string> _filePathList;

        public List<string> FilePathList
        {
            get { return _filePathList; }
            set { _filePathList = value; }
        }

        private string _subPath;

        public string SubPath
        {
            get { return _subPath; }
        }

        private string _suffix = string.Empty;//文件后缀
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        private long _createDate;

        public long CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        private string _md5;
        public string md5
        {
            get { return _md5; }
            set { _md5 = value; }
        }

        private long _length;

        public long Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();//override　Equals　后必须override GetHashCode
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AssetInfo))
                return false;

            AssetInfo assetInfo = obj as AssetInfo;
            return _subPath == assetInfo.SubPath && _md5 == assetInfo._md5;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[AssetInfo:");
            sb.Append("_subPath:");
            sb.Append(_subPath);
            sb.Append(",_length:");
            sb.Append(_length);
            sb.Append(",_createDate:");
            sb.Append(_createDate);
            sb.Append("]");
            return sb.ToString();
        }
    }

    public class ManifestInfo : System.Object
    {
        public string version; //本地manifest版本不同于服务器的manifest版本的时候，删除本地ab文件夹下所有资源
        public Dictionary<string, AssetInfo> assetDic;
        //public AssetBundleManifest assetBundleManifest;

        public ManifestInfo()
        {
            version = string.Empty;
            assetDic = new Dictionary<string, AssetInfo>();
        }

        public string GetAssetBundlePathByRealPath(string path)
        {
            foreach (var kvp in assetDic)
            {
                if (kvp.Value.FilePathList.Contains(path))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ManifestInfo:");
            sb.Append("version:");
            sb.Append(string.Format("{0}", version));
            sb.Append(",assetDic:{");
            foreach (var kv in assetDic)
            {
                sb.Append(kv.Value.ToString());
                sb.Append(";");
            }

            sb.Append("}]");
            return sb.ToString();
        }
    }
}
