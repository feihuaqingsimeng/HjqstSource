using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace TPImporter
{
	public class TexturePackerImporter : AssetPostprocessor
	{
		private const string IMPORTER_VERSION = "4.0.0";
		private static SpritesheetCollection spriteSheets = new SpritesheetCollection();
		[PreferenceItem("TexturePacker")]
		private static void PreferencesGUI()
		{
			EditorGUILayout.HelpBox("Pivot point settings can now be configured per project. Please use Edit -> Project Settings -> TexturePacker", MessageType.Info);
		}
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			List<string> list = new List<string>(deletedAssets);
			list.AddRange(movedFromAssetPaths);
			foreach (string current in list)
			{
				if (Path.GetExtension(current).Equals(".tpsheet"))
				{
					TexturePackerImporter.spriteSheets.unloadSheetData(current);
				}
			}
			List<string> list2 = new List<string>(importedAssets);
			list2.AddRange(movedAssets);
			foreach (string current2 in list2)
			{
				Dbg.Log("OnPostprocessAllAssets: " + current2);
				if (Path.GetExtension(current2).Equals(".tpsheet") && TexturePackerImporter.spriteSheets.loadSheetData(current2))
				{
					SettingsDatabase instance = SettingsDatabase.getInstance();
					string path = instance.spriteFileForDataFile(current2);
					AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
					string text = instance.normalsFileForDataFile(current2);
					if (text != null)
					{
						AssetDatabase.ImportAsset(text, ImportAssetOptions.ForceUpdate);
					}
				}
			}
		}
		private static void GetOrigImageSize(Texture2D texture, TextureImporter importer, out int width, out int height)
		{
			try
			{
				object[] array = new object[]
				{
					0,
					0
				};
				MethodInfo method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
				{
					typeof(int).MakeByRefType(),
					typeof(int).MakeByRefType()
				}, null);
				if (object.Equals(null, method))
				{
					throw new Exception("Method GetWidthAndHeight(int,int) not found");
				}
				method.Invoke(importer, array);
				width = (int)array[0];
				height = (int)array[1];
			}
			catch (Exception ex)
			{
				Debug.LogError("Invoking TextureImporter.GetWidthAndHeight() failed: " + ex.ToString());
				width = texture.width;
				height = texture.height;
			}
		}
		public void OnPostprocessTexture(Texture2D texture)
		{
			TextureImporter textureImporter = base.assetImporter as TextureImporter;
			int num;
			int num2;
			TexturePackerImporter.GetOrigImageSize(texture, textureImporter, out num, out num2);
			Dbg.Log(string.Concat(new object[]
			{
				"OnPostprocessTexture(",
				base.assetPath,
				"), ",
				texture.width,
				"x",
				texture.height,
				", orig:",
				num,
				"x",
				num2
			}));
			SettingsDatabase instance = SettingsDatabase.getInstance();
			if (instance.isSpriteSheet(base.assetPath))
			{
				SheetInfo sheetInfo = TexturePackerImporter.spriteSheets.sheetInfoForSpriteFile(base.assetPath);
				if (sheetInfo.width > 0 && sheetInfo.height > 0 && (sheetInfo.width != num || sheetInfo.height != num2))
				{
					textureImporter.spriteImportMode = SpriteImportMode.Single;
					Dbg.Log("Inconsistent sheet size in png/tpsheet");
				}
				else
				{
					TexturePackerImporter.updateSpriteMetaData(textureImporter, sheetInfo.metadata);
					Dbg.Log("Updated SpriteMetaData for " + base.assetPath);
				}
			}
			if (instance.isNormalmapSheet(base.assetPath))
			{
				if (textureImporter.textureType != TextureImporterType.Advanced)
				{
					textureImporter.textureType = TextureImporterType.Bump;
				}
				TexturePackerImporter.createMaterialForNormalmap(base.assetPath, instance.spriteFileForNormalsFile(base.assetPath));
			}
		}
		public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
		{
			TexturePackerImporter.spriteSheets.assignGeometries(base.assetPath, sprites);
		}
		private static void updateSpriteMetaData(TextureImporter importer, SpriteMetaData[] metaData)
		{
			Dbg.Log("PNG has type " + importer.textureType);
			if (importer.textureType != TextureImporterType.Advanced)
			{
				importer.textureType = TextureImporterType.Sprite;
			}
			importer.spriteImportMode = SpriteImportMode.Multiple;
			SpriteMetaData[] spritesheet = importer.spritesheet;
			for (int i = 0; i < metaData.Length; i++)
			{
				SpriteMetaData[] array = spritesheet;
				for (int j = 0; j < array.Length; j++)
				{
					SpriteMetaData spriteMetaData = array[j];
					if (spriteMetaData.name == metaData[i].name)
					{
						if (!SettingsDatabase.getInstance().importPivotPoints())
						{
							metaData[i].pivot = spriteMetaData.pivot;
							metaData[i].alignment = spriteMetaData.alignment;
						}
						metaData[i].border = spriteMetaData.border;
						break;
					}
				}
			}
			importer.spritesheet = metaData;
		}
		private static void createMaterialForNormalmap(string normalSheet, string spriteSheet)
		{
			string path = Path.ChangeExtension(spriteSheet, ".mat");
			if (!File.Exists(path))
			{
				Texture2D texture = AssetDatabase.LoadAssetAtPath(spriteSheet, typeof(Texture2D)) as Texture2D;
				Texture2D texture2 = AssetDatabase.LoadAssetAtPath(normalSheet, typeof(Texture2D)) as Texture2D;
				bool flag = true;
				Shader shader = Shader.Find("Standard");
				if (shader == null)
				{
					shader = Shader.Find("Transparent/Bumped Diffuse");
					flag = false;
				}
				Material material = new Material(shader);
				material.SetTexture("_MainTex", texture);
				material.SetTexture("_BumpMap", texture2);
				if (flag)
				{
					material.SetFloat("_Mode", 2f);
					material.SetInt("_SrcBlend", 5);
					material.SetInt("_DstBlend", 10);
					material.SetInt("_ZWrite", 0);
					material.DisableKeyword("_ALPHATEST_ON");
					material.EnableKeyword("_ALPHABLEND_ON");
					material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					material.renderQueue = 3000;
				}
				AssetDatabase.CreateAsset(material, path);
				EditorUtility.SetDirty(material);
			}
		}
	}
}
