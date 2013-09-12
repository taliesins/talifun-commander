using System.Threading;
using System.Threading.Tasks;

namespace Talifun.Commander.Command.Esb
{
	public class CancellableTask
	{
		public Task Task { get; set; }
		public CancellationTokenSource CancellationTokenSource { get; set; }
	}
}
