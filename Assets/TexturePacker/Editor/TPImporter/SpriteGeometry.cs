using System;
using System.Globalization;
using UnityEngine;
namespace TPImporter
{
	public class SpriteGeometry
	{
		private Vector2[] vertices;
		private ushort[] triangles;
		public int parse(string[] values, int idx)
		{
			if (idx >= values.Length)
			{
				return idx;
			}
			int num = int.Parse(values[idx++]);
			if (2 * num + idx > values.Length)
			{
				Debug.LogError("vertex values missing: " + num + values.Length);
				return -1;
			}
			this.vertices = new Vector2[num];
			for (int i = 0; i < num; i++)
			{
				this.vertices[i].Set(float.Parse(values[idx++], CultureInfo.InvariantCulture), float.Parse(values[idx++], CultureInfo.InvariantCulture));
			}
			int num2 = int.Parse(values[idx++]);
			if (3 * num2 + idx > values.Length)
			{
				Debug.LogError("triangle values missing: " + num2 + values.Length);
				return -1;
			}
			this.triangles = new ushort[3 * num2];
			for (int j = 0; j < 3 * num2; j++)
			{
				this.triangles[j] = ushort.Parse(values[idx++]);
			}
			return idx;
		}
		public void setQuad(int w, int h)
		{
			this.vertices = new Vector2[4];
			this.triangles = new ushort[]
			{
				0,
				2,
				1,
				1,
				2,
				3
			};
			this.vertices[0].Set(0f, 0f);
			this.vertices[1].Set((float)w, 0f);
			this.vertices[2].Set(0f, (float)h);
			this.vertices[3].Set((float)w, (float)h);
		}
		public void assignToSprite(Sprite sprite, float scaleFactor)
		{
			if ((double)scaleFactor != 1.0)
			{
				Vector2[] array = new Vector2[this.vertices.Length];
				for (int i = 0; i < this.vertices.Length; i++)
				{
					array[i].Set(scaleFactor * this.vertices[i].x, scaleFactor * this.vertices[i].y);
				}
				sprite.OverrideGeometry(array, this.triangles);
			}
			else
			{
				sprite.OverrideGeometry(this.vertices, this.triangles);
			}
		}
	}
}
