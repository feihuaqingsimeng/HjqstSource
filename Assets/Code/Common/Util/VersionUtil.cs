using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System;

public class VersionUtil
{
    public static bool _isDebug=false;

    public static bool IsDebug
    {
        get
        {
            return _isDebug;
        }
        set
        {
            Debugger.Log(string.Format("_isDebug:{0} , value:{1}", _isDebug , value));
            _isDebug = value;
        }
    }

    /// <summary>
    /// 还原代码并删除未加入版本管理的文件
    /// </summary>
    /// <param name="path"></param>
    public static void ResetGit(string path)
    {
        if (IsDebug) return;
        RemoveUntrackedGit(path);
        ResetVerstionGit(path);
    }

    /// <summary>
    /// 删除所有untracked的文件
    /// </summary>
    /// <param name="path"></param>
    public static void RemoveUntrackedGit(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "clean -df");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    /// <summary>
    /// 还原本地代码至上一个修改的版本
    /// </summary>
    /// <param name="path"></param>
    public static void ResetVerstionGit(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "reset --hard");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }


    /// <summary>
    /// 将本地版本还原和远程版本一样
    /// </summary>
    /// <param name="path"></param>
    /// <param name="remoteBranch"></param>
    public static void ResetRemoteBranchGit(string path, string remoteBranch = "origin/master")
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "reset -hard " + remoteBranch);
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }


    public static void PushGit(string path)
    {
        if(IsDebug)return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "push");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    public static void PullGit(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "pull");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    public static void CommitGit(string path, string message = "abs")
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "commit -m \"" + message + "\"");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }


    public static void AddGit(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "add -A ");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }


    public static void BranchGit(string path, string branchName)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("git", "branch " + branchName);
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    public static List<string> GetChangeList(string path)
    {
        List<string> result = new List<string>();
        Process p = new Process();
        p.StartInfo.FileName = "git";
        p.StartInfo.Arguments = "status -s";
        p.StartInfo.WorkingDirectory = path;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        //p.OutputDataReceived += p_OutputDataReceived;
        p.Start();
        //p.BeginOutputReadLine();

        string[] SYMBOL_LINE = new string[] { "\n" };
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        string[] contents = output.Split(SYMBOL_LINE, StringSplitOptions.RemoveEmptyEntries);
        foreach (var c in contents)
        {
            string t = c.Substring(2, c.Length - 3);
            result.Add(t);
        }
        return result;
    }


    public static void CleanupSVN(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("svn", "cleanup");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    public static void RemoveUntrackedSVN(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("svn", "status --no-ignore | grep '^\\?' | sed 's/^\\?      //'  | xargs -Ixx rm -rf xx");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }


    public static void RevertSVN(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("svn", "revert . -R");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    public static void UpdateSVN(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("svn", "update");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    public static void AddSVN(string path, string directory)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("svn", "add " + directory + " --force");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }

    public static void CommitSVN(string path)
    {
        if (IsDebug) return;
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("svn", "commit --message \"\"");
        psi.WorkingDirectory = path;
        System.Diagnostics.Process.Start(psi).WaitForExit();
    }
}

