using System;
using MassTransit;
using MassTransit.Diagnostics.Introspection;

namespace Talifun.Commander.Command.Esb
{
	public class EndpointCacheProxy : IEndpointCache
	{
		private readonly IEndpointCache _endpointCache;

		public EndpointCacheProxy(IEndpointCache endpointCache)
		{
			_endpointCache = endpointCache;
		}

		public IEndpoint GetEndpoint(Uri uri)
		{
			return _endpointCache.GetEndpoint(uri);
		}

	    public void Inspect(DiagnosticsProbe probe)
	    {
	    }

        public void Dispose()
        {
        }
	}
}
