using System;
using System.Collections.Generic;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public class ContainerMetaData : Dictionary<string, string>
	{
		public ContainerMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{}
	}
}
