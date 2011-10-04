using System;
using System.Collections;

namespace Talifun.Commander.UI
{
	public interface IBindingGroup
	{
		Type ElementType { get; }
		IEnumerable Items { get; }
		string Parameter { get; }
	}
}
