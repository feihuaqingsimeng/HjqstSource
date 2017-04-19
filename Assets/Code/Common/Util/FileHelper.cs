using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
namespace Common.Util
{
    /// <summary>
    /// ʹ��Application.persistentDataPath��ʽ�������ļ�����дXml�ļ�.
    /// עApplication.persistentDataPathĩβû�С�/������
    /// </summary>
    public class FileHelper : MonoBehaviour
    {
        /// <summary>
        /// ��̬�����ļ���.
        /// </summary>
        /// <returns>The folder.</returns>
        /// <param name="path">�ļ�����Ŀ¼.</param>
        /// <param name="FolderName">�ļ�����(��������).</param>
        public string CreateFolder(string path, string FolderName)
        {
            string FolderPath = path + FolderName;
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            return FolderPath;
        }

        /// <summary>
        /// �����ļ�.
        /// </summary>
        /// <param name="path">�����ļ���·��.</param>
        /// <param name="name">�ļ�������.</param>
        /// <param name="info">д�������.</param>
        public void CreateFile(string path, string name, string info)
        {
            //�ļ�����Ϣ
            StreamWriter sw;
            FileInfo t = new FileInfo(path + name);
            if (!t.Exists)
            {
                //������ļ��������򴴽�
                sw = t.CreateText();
            }
            else
            {
                //������ļ��������
                sw = t.AppendText();
            }
            //���е���ʽд����Ϣ
            sw.WriteLine(info);
            //�ر���
            sw.Close();
            //������
            sw.Dispose();
        }

        /// <summary>
        /// ��ȡ�ļ�.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="path">�����ļ���·��.</param>
        /// <param name="name">��ȡ�ļ�������.</param>
        public ArrayList LoadFile(string path, string name)
        {
            //ʹ��������ʽ��ȡ
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(path + name);
            }
            catch (Exception e)
            {
                Debugger.LogError(e.StackTrace);
                //·��������δ�ҵ��ļ���ֱ�ӷ��ؿ�
                return null;
            }
            string line;
            ArrayList arrlist = new ArrayList();
            while ((line = sr.ReadLine()) != null)
            {
                //һ��һ�еĶ�ȡ
                //��ÿһ�е����ݴ�����������������
                arrlist.Add(line);
            }
            //�ر���
            sr.Close();
            //������
            sr.Dispose();
            //������������������
            return arrlist;
        }
        //д��ģ�͵�����
        private IEnumerator LoadassetbundleCoroutine(string url)
        {
            WWW w = new WWW(url);
            yield return w;
            if (w.isDone)
            {
                byte[] model = w.bytes;
                int length = model.Length;
                //д��ģ�͵�����
                CreateassetbundleFile(Application.persistentDataPath, "Model.assetbundle", model, length);
            }
        }
        /// <summary>
        /// ��ȡ�ļ��������ļ���С
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public int GetAllFileSize(string filePath)
        {
            int sum = 0;
            if (!Directory.Exists(filePath))
            {
                return 0;
            }

            DirectoryInfo dti = new DirectoryInfo(filePath);

            FileInfo[] fi = dti.GetFiles();

            foreach (FileInfo f in fi)
            {

                sum += Convert.ToInt32(f.Length / 1024);
            }

            DirectoryInfo[] di = dti.GetDirectories();

            if (di.Length > 0)
            {
                for (int i = 0; i < di.Length; i++)
                {
                    sum += GetAllFileSize(di[i].FullName);
                }
            }
            return sum;
        }
        /// <summary>
        /// ��ȡָ���ļ���С
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public int GetFileSize(string FilePath, string FileName)
        {
            int sum = 0;
            if (!Directory.Exists(FilePath))
            {
                return 0;
            }
            else
            {
                FileInfo Files = new FileInfo(@FilePath + FileName);
                sum += Convert.ToInt32(Files.Length / 1024);
            }
            return sum;
        }
        void CreateassetbundleFile(string path, string name, byte[] info, int length)
        {
            //�ļ�����Ϣ
            //StreamWriter sw;
            Stream sw;
            FileInfo t = new FileInfo(path + "//" + name);
            if (!t.Exists)
            {
                //������ļ��������򴴽�
                sw = t.Create();
            }
            else
            {
                //������ļ��������
                //sw = t.Append();
                return;
            }
            //���е���ʽд����Ϣ
            sw.Write(info, 0, length);
            //�ر���
            sw.Close();
            //������
            sw.Dispose();
        }
        //��ȡ����AssetBundle�ļ�
        private IEnumerator LoadAssetbundleFromLocalCoroutine(string path, string name)
        {
            print("file:///" + path + "/" + name);

            WWW w = new WWW("file:///" + path + "/" + name);

            yield return w;

            if (w.isDone)
            {
                Instantiate(w.assetBundle.mainAsset);
            }
        }

        /// <summary>
        /// ɾ���ļ�.
        /// </summary>
        /// <param name="path">ɾ�������ļ���·��.</param>
        /// <param name="name">ɾ���ļ�������.</param>
        public void DeleteFile(string path, string name)
        {
            File.Delete(path + name);
        }
        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filesName"></param>
        /// <returns></returns>
        public bool DeleteFiles(string path, string filesName)
        {
            bool isDelete = false;
            try
            {
                if (Directory.Exists(path))
                {
                    if (File.Exists(path + "\\" + filesName))
                    {
                        File.Delete(path + "\\" + filesName);
                        isDelete = true;
                    }
                }
            }
            catch
            {
                return isDelete;
            }
            return isDelete;
        }
    }
}