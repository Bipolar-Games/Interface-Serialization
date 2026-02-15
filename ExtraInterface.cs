using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Bipolar
{
	public class ExtraInterface<TInterface> 
		where TInterface : class
	{
		[SerializeField]
		private Object serializedObject;
		private TInterface objectAsInterface;

		public static System.Type InterfaceType { get; } = typeof(TInterface);

		public const BindingFlags MembersBindingFlags = BindingFlags.Public | BindingFlags.Instance;
		public static IReadOnlyList<MemberInfo> RequiredMembers { get; } = InterfaceType.GetMembers(MembersBindingFlags);

		public Object Value
		{
			get => serializedObject;
			set => SetValue(value);
		}

		public void SetValue(Object value)
		{
			serializedObject = null;
			objectAsInterface = null;
			if (value is TInterface typedValue)
			{
				serializedObject = value;
				objectAsInterface = typedValue;
				return;
			}

			var objectMembers = value.GetType().GetMembers(MembersBindingFlags);
			if (CompareLists(objectMembers, RequiredMembers))
			{
				serializedObject = value;
			}
		}

		public bool CompareLists<T>(IReadOnlyList<T> lhs, IReadOnlyList<T> rhs)
		{
			if (lhs.Count != rhs.Count)
				return false;
			
			for (int i = 0; i < lhs.Count; i++)
				if (!rhs.Contains(lhs[i]) || !lhs.Contains(rhs[i]))
					return false;
		
			return true;
		}
	}

	public static class ReadOnlyCollectionsExtensions
	{
		public static bool Contains<T>(this IReadOnlyList<T> collection, T element)
		{
			for (int i = 0; i < collection.Count; i++)
				if (collection[i].Equals(element))
					return true;

			return false;
		}

	}

	public class Be : MonoBehaviour
	{
		public ExtraInterface<IAnimationClipSource> extra;


		private void Start()
		{
			extra.Value = this;

			var valueGet = extra.Value;
		}

	}

}
