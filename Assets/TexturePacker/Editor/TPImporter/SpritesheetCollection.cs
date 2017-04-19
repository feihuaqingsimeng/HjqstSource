using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace TPImporter
{
	public class SpritesheetCollection
	{
		private Dictionary<string, SheetInfo> spriteSheetData = new Dictionary<string, SheetInfo>();
		private SpriteGeometryHash spriteGeometries = new SpriteGeometryHash();
		public bool loadSheetData(string dataFile)
		{
			if (this.spriteSheetData.ContainsKey(dataFile))
			{
				this.unloadSheetData(dataFile);
			}
			string[] array = File.ReadAllLines(dataFile);
			int num = 30302;
			string text = null;
			string text2 = null;
			SheetInfo sheetInfo = new SheetInfo();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text3 = array2[i];
				if (text3.StartsWith(":format="))
				{
					num = int.Parse(text3.Remove(0, 8));
				}
				if (text3.StartsWith(":normalmap="))
				{
					text2 = text3.Remove(0, 11);
				}
				if (text3.StartsWith(":texture="))
				{
					text = text3.Remove(0, 9);
				}
				if (text3.StartsWith(":size="))
				{
					string[] array3 = text3.Remove(0, 6).Split(new char[]
					{
						'x'
					});
					sheetInfo.width = int.Parse(array3[0]);
					sheetInfo.height = int.Parse(array3[1]);
				}
				if (text3.StartsWith("# Sprite sheet: "))
				{
					text = text3.Remove(0, 16);
					text = text.Remove(text.LastIndexOf("(") - 1);
				}
			}
			bool flag = array[0].StartsWith("{");
			if (num > 40000 || flag)
			{
				EditorUtility.DisplayDialog("Please update TexturePacker Importer", "Your TexturePacker Importer is too old to import '" + dataFile + "', \nplease load a new version from the asset store!", "Ok");
				return false;
			}
			text = Path.GetDirectoryName(dataFile) + "/" + text;
			if (text2 != null)
			{
				text2 = Path.GetDirectoryName(dataFile) + "/" + text2;
			}
			SettingsDatabase.getInstance().addSheet(dataFile, text, text2);
			Dictionary<string, SpriteMetaData> dictionary = new Dictionary<string, SpriteMetaData>();
			string[] array4 = array;
			for (int j = 0; j < array4.Length; j++)
			{
				string text4 = array4[j];
				if (!string.IsNullOrEmpty(text4) && !text4.StartsWith("#") && !text4.StartsWith(":"))
				{
					string[] array5 = text4.Split(new char[]
					{
						';'
					});
					int num2 = 0;
					if (array5.Length < 7)
					{
						EditorUtility.DisplayDialog("File format error", "Failed to import '" + dataFile + "'", "Ok");
						return false;
					}
					SpriteMetaData value = default(SpriteMetaData);
					value.name = this.unescapeName(array5[num2++]);
					if (dictionary.ContainsKey(value.name))
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dataFile);
						EditorUtility.DisplayDialog("Sprite sheet import failed", string.Concat(new string[]
						{
							"Name conflict: Sprite sheet '",
							fileNameWithoutExtension,
							"' contains multiple sprites with name '",
							value.name,
							"'"
						}), "Abort");
						return false;
					}
					float x = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float y = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float num3 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float num4 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float num5 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float num6 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					value.rect = new Rect(x, y, num3, num4);
					value.pivot = new Vector2(num5, num6);
					if (num5 == 0f && num6 == 0f)
					{
						value.alignment = 6;
					}
					else if ((double)num5 == 0.5 && num6 == 0f)
					{
						value.alignment = 7;
					}
					else if (num5 == 1f && num6 == 0f)
					{
						value.alignment = 8;
					}
					else if (num5 == 0f && (double)num6 == 0.5)
					{
						value.alignment = 4;
					}
					else if ((double)num5 == 0.5 && (double)num6 == 0.5)
					{
						value.alignment = 0;
					}
					else if (num5 == 1f && (double)num6 == 0.5)
					{
						value.alignment = 5;
					}
					else if (num5 == 0f && num6 == 1f)
					{
						value.alignment = 1;
					}
					else if ((double)num5 == 0.5 && num6 == 1f)
					{
						value.alignment = 2;
					}
					else if (num5 == 1f && num6 == 1f)
					{
						value.alignment = 3;
					}
					else
					{
						value.alignment = 9;
					}
					dictionary.Add(value.name, value);
					if (Application.unityVersion[0] == '4')
					{
						if (num2 < array5.Length)
						{
							EditorUtility.DisplayDialog("Sprite sheet import failed.", "Import of polygon sprites requires Unity 5!", "Abort");
							return false;
						}
					}
					else
					{
						SpriteGeometry spriteGeometry = new SpriteGeometry();
						if (num2 < array5.Length)
						{
							num2 = spriteGeometry.parse(array5, num2);
							if (num2 < 0)
							{
								Debug.LogError("format error in file " + dataFile);
								return false;
							}
						}
						else
						{
							spriteGeometry.setQuad((int)num3, (int)num4);
						}
						this.spriteGeometries.addGeometry(text, value.name, spriteGeometry);
					}
				}
			}
			sheetInfo.metadata = new SpriteMetaData[dictionary.Count];
			dictionary.Values.CopyTo(sheetInfo.metadata, 0);
			this.spriteSheetData[dataFile] = sheetInfo;
			return true;
		}
		public void unloadSheetData(string dataFile)
		{
			this.spriteSheetData.Remove(dataFile);
			SettingsDatabase.getInstance().removeSheet(dataFile, true);
		}
		private string unescapeName(string name)
		{
			return name.Replace("%23", "#").Replace("%3A", ":").Replace("%3B", ";").Replace("%25", "%").Replace("/", "-");
		}
		public SheetInfo sheetInfoForSpriteFile(string textureFile)
		{
			string text = SettingsDatabase.getInstance().dataFileForSpriteFile(textureFile);
			if (text == null)
			{
				return null;
			}
			if (!this.spriteSheetData.ContainsKey(text))
			{
				this.loadSheetData(text);
			}
			return this.spriteSheetData[text];
		}
		public void assignGeometries(string texturePath, Sprite[] sprites)
		{
			float num = 1f;
			SheetInfo sheetInfo = this.sheetInfoForSpriteFile(texturePath);
			if (sheetInfo != null && sheetInfo.width > 0 && sprites.Length > 0)
			{
				num = (float)sprites[0].texture.width / (float)sheetInfo.width;
				Dbg.Log("scale factor: " + num);
			}
			for (int i = 0; i < sprites.Length; i++)
			{
				Sprite sprite = sprites[i];
				this.spriteGeometries.assignGeometryToSprite(texturePath, sprite, num);
			}
		}
	}
}
