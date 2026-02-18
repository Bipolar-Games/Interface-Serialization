#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	public class InterfaceField : ObjectField
	{
		public InterfaceField(string label, System.Type interfaceType) : base(label)
		{
			objectType = interfaceType;
			AddToClassList(alignedFieldUssClassName);
			this.Q(className: selectorUssClassName).style.display = DisplayStyle.None;


			var displayElement = this.Q(className: objectUssClassName);
			displayElement.RegisterCallback<KeyDownEvent>(
				ExecuteConfirmKeyboardAction,
				TrickleDown.TrickleDown);

			style.flexGrow = 1;
			style.flexShrink = 1;
			style.minWidth = 0;

			var objectSelectorButton = new ObjectSelectorButton(this, interfaceType);
			this.Q(className: inputUssClassName).Add(objectSelectorButton);
		}

		private void ExecuteConfirmKeyboardAction(KeyDownEvent evt)
		{
			if (IsConfirmKeyboardEvent(evt))
			{
				evt.StopImmediatePropagation();
				evt.PreventDefault();
				InterfaceSelectorWindow.Show(objectType, value, AssignValue);
			}
		}

		private static bool IsConfirmKeyboardEvent(EventBase evt)
		{
			return evt.eventTypeId == EventBase<KeyDownEvent>.TypeId()
				&& evt is KeyDownEvent keyDownEvent
				&& InterfaceEditorUtility.IsConfirmKey(keyDownEvent.keyCode);
		}

		internal void AssignValue(Object selected)
		{
			value = selected;
		}
	}
}
#endif
