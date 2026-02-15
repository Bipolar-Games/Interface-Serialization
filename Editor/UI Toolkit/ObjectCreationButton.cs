#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	public class ObjectCreationButton : Button
	{
		public ObjectCreationButton(string text, System.Action onClick) : base(onClick)
		{
			this.text = text;
			style.flexShrink = 0.5f;
			style.marginLeft = 0;
			style.marginRight = 0;
		}
	}
}
#endif
