using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Common.Tools.Editor
{
	public class BMFontGenerateMenu
	{
		[ExecuteInEditMode]
		[MenuItem("Tools/GenerateBMFont")]
		public static void GenerateBMFont ()
		{
			Font selectedFont = null;
			TextAsset selectedData = null;
			BMFont bmFont = new BMFont();

			Object[] selectedObjects = Selection.objects;
			Object selectedObject = null;
			for (int selectedObjectIndex = 0; selectedObjectIndex < selectedObjects.Length; selectedObjectIndex++)
			{
				selectedObject = selectedObjects[selectedObjectIndex];
				if (selectedObject is Font)
				{
					selectedFont = selectedObject as Font;
				}
				else if (selectedObject is TextAsset)
				{
					selectedData = selectedObject as TextAsset;
				}
			}

			if (selectedFont == null || selectedData == null)
			{
				string fontNullInfo = selectedFont == null ? "Font is null." : string.Empty;
				string dataNullInfo = selectedData == null ? "Data is null." : string.Empty;
				Debugger.Log(fontNullInfo + dataNullInfo);
				return;
			}
			else
			{
				Debugger.Log(string.Format("Selected Font:{0}, Selected Data:{1}", selectedFont.name, selectedData.name));
				BMFontReader.Load(bmFont, selectedData.name, selectedData.bytes);
				CharacterInfo[] characterInfos = new CharacterInfo[bmFont.glyphs.Count];
				int characterInfoLength = bmFont.glyphs.Count;
				for (int index = 0; index < characterInfoLength; index++)
				{
					BMGlyph bmGlyph = bmFont.glyphs[index];
					CharacterInfo characterInfo = new CharacterInfo();
					characterInfo.index = bmGlyph.index;
					characterInfo.uv.x = (float)bmGlyph.x / (float)bmFont.texWidth;
					characterInfo.uv.y = 1 - (float)bmGlyph.y / (float)bmFont.texHeight;
					characterInfo.uv.width = (float)bmGlyph.width / (float)bmFont.texWidth;
					characterInfo.uv.height = -1f * (float)bmGlyph.height / (float)bmFont.texHeight;

					characterInfo.vert.x = bmGlyph.offsetX;
					characterInfo.vert.y = bmGlyph.offsetY;
					characterInfo.vert.width = bmGlyph.width;
					characterInfo.vert.height = bmGlyph.height;
					characterInfo.width = bmGlyph.advance;

					characterInfos[index] = characterInfo;
				}
				selectedFont.characterInfo = characterInfos;
			}
		}
	}
}