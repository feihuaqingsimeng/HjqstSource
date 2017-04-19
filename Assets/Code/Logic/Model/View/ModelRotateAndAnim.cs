using UnityEngine;
using System.Collections;
namespace Logic.Model.View
{
    public class ModelRotateAndAnim : ModelBehaviour
    {
		public bool canClick = true;
		public bool canDrag = true;

        public override void ClickBehavior()
        {
			if (canClick)
			{
				base.ClickBehavior();
			}
        }

		public override void Rotate (float fac)
		{
			if (canDrag)
			{
				base.Rotate(fac);
			}
		}
    }
}
