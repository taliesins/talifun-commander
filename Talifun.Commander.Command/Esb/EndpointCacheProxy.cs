using System;
using MassTransit;

namespace Talifun.Commander.Command.Esb
{
	public class EndpointCacheProxy : IEndpointCache
	{
		readonly IEndpointCache _endpointCache;

		public EndpointCacheProxy(IEndpointCache endpointCache)
		{
			_endpointCache = endpointCache;
		}

		public void Dispose()
		{
			// we don't dispose, since we're in testing
		}

		public IEndpoint GetEndpoint(Uri uri)
		{
			return _endpointCache.GetEndpoint(uri);
		}
	}
}
