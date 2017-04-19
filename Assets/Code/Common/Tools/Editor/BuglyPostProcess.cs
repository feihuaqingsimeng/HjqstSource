//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using UnityEditor.Callbacks;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;

//public class BuglyBuilder
//{
//    static string s_PBXBuildFile =
//        "\t\t9F4F2BC91B5D222F00EEE730 /* libz.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = 9F4F2BC81B5D222F00EEE730 /* libz.dylib */; };\n" +
//        "\t\t9F4F2BCB1B5D224000EEE730 /* libstdc++.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = 9F4F2BCA1B5D224000EEE730 /* libstdc++.dylib */; };\n" +
//        "\t\t9F4F2BCD1B5D224900EEE730 /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 9F4F2BCC1B5D224900EEE730 /* Security.framework */; };\n";

//    static string s_PBXFileReference =
//        "\t\t9F4F2BC81B5D222F00EEE730 /* libz.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libz.dylib; path = usr/lib/libz.dylib; sourceTree = SDKROOT; };\n" +
//        "\t\t9F4F2BCA1B5D224000EEE730 /* libstdc++.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = \"libstdc++.dylib\"; path = \"usr/lib/libstdc++.dylib\"; sourceTree = SDKROOT; };\n" +
//        "\t\t9F4F2BCC1B5D224900EEE730 /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n";

//    static string s_PBXFrameworksBuildPhase =
//        "\t\t\t\t56FD43960ED4745200FE3770 /* CFNetwork.framework in Frameworks */,\n" +
//        "\t\t\t\t9F4F2BCD1B5D224900EEE730 /* Security.framework in Frameworks */,\n" +
//        "\t\t\t\t9F4F2BCB1B5D224000EEE730 /* libstdc++.dylib in Frameworks */,\n" +
//        "\t\t\t\t9F4F2BC91B5D222F00EEE730 /* libz.dylib in Frameworks */,\n";

//    static string s_PBXGroup =
//        "\t\t\t\t9F4F2BCC1B5D224900EEE730 /* Security.framework */,\n" +
//        "\t\t\t\t9F4F2BCA1B5D224000EEE730 /* libstdc++.dylib */,\n" +
//        "\t\t\t\t9F4F2BC81B5D222F00EEE730 /* libz.dylib */,\n";

//    // Thanks to https://gist.github.com/tenpn/f8da1b7df7352a1d50ff for inspiration for this code.
//    [PostProcessBuild(1400)]
//    public static void OnPostProcessBuild(BuildTarget target, string path)
//    {
//        if (target != BuildTarget.iOS)
//        {
//            return;
//        }
//        Debugger.Log("BuglyBuilder: " + path);
//        FileInfo pbxPath = FindXcodeProjectPath(new DirectoryInfo(path));
//        if (pbxPath == null)
//        {
//            return;
//        }
//        Debugger.Log("XcodeProjectPath: " + pbxPath.FullName);
//        var xcodeProjectLines = File.ReadAllLines(pbxPath.FullName);
//        var sb = new StringBuilder();
//        var inPBXFrameworksBuildPhase = false;
//        var inPBXGroup = false;

//        foreach (var line in xcodeProjectLines)
//        {
//            if (line.Contains("libz.dylib"))
//            {
//                return;
//            }
//        }

//        foreach (var line in xcodeProjectLines)
//        {
//            if (line.Contains("GCC_ENABLE_OBJC_EXCEPTIONS") ||
//                     line.Contains("GCC_ENABLE_CPP_EXCEPTIONS") ||
//                     line.Contains("GCC_ENABLE_CPP_RTTI"))
//            {
//                var newLine = line.Replace("NO", "YES");
//                sb.AppendLine(newLine);
//            }
//            else if (line.Contains("/* Begin PBXBuildFile section */"))
//            {
//                sb.AppendLine(line);
//                sb.Append(s_PBXBuildFile);
//            }
//            else if (line.Contains("/* Begin PBXFileReference section */"))
//            {
//                sb.AppendLine(line);
//                sb.Append(s_PBXFileReference);
//            }
//            else if (line.Contains("/* Begin PBXFrameworksBuildPhase section */"))
//            {
//                inPBXFrameworksBuildPhase = true;
//                sb.AppendLine(line);
//            }
//            else if (inPBXFrameworksBuildPhase && line.Contains("/* libBugly.a in Frameworks */,"))
//            {
//                inPBXFrameworksBuildPhase = false;
//                sb.AppendLine(line);
//                sb.Append(s_PBXFrameworksBuildPhase);
//            }
//            else if (line.Contains("/* Begin PBXGroup section */"))
//            {
//                inPBXGroup = true;
//                sb.AppendLine(line);
//            }
//            else if (inPBXGroup && line.Contains("/* UIKit.framework */,"))
//            {
//                inPBXGroup = false;
//                sb.AppendLine(line);
//                sb.Append(s_PBXGroup);
//            }
//            else
//            {
//                sb.AppendLine(line);
//            }
//        }

//        File.WriteAllText(pbxPath.FullName, sb.ToString());
//    }
//    public static FileInfo FindXcodeProjectPath(FileSystemInfo info)
//    {
//        if (!info.Exists)
//        {
//            return null;
//        }

//        DirectoryInfo dir = info as DirectoryInfo;
//        if (dir == null)
//        {
//            return null;
//        }

//        FileSystemInfo[] files = dir.GetFileSystemInfos();
//        for (int i = 0; i < files.Length; i++)
//        {
//            FileInfo file = files[i] as FileInfo;
//            if (file != null)
//            {
//                if (file.Name == "project.pbxproj")
//                {
//                    return file;
//                }
//            }
//            else
//            {
//                FileInfo ret = FindXcodeProjectPath(files[i]);
//                if (ret != null)
//                {
//                    return ret;
//                }
//            }
//        }
//        return null;
//    }
//}
