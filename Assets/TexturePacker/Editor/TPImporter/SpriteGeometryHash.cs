using System;
using System.Collections.Generic;
using UnityEngine;
namespace TPImporter
{
	public class SpriteGeometryHash
	{
		private Dictionary<string, SpriteGeometry> geometries = new Dictionary<string, SpriteGeometry>();
		public void addGeometry(string texturePath, string spriteName, SpriteGeometry geometry)
		{
			this.geometries[texturePath + "/" + spriteName] = geometry;
		}
		public void assignGeometryToSprite(string texturePath, Sprite sprite, float scaleFactor)
		{
			if (this.geometries.ContainsKey(texturePath + "/" + sprite.name))
			{
				SpriteGeometry spriteGeometry = this.geometries[texturePath + "/" + sprite.name];
				spriteGeometry.assignToSprite(sprite, scaleFactor);
			}
		}
	}
}
