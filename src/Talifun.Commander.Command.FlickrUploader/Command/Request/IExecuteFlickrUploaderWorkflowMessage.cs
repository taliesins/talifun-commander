using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Settings;

namespace Talifun.Commander.Command.FlickrUploader.Command.Request
{
	public interface IExecuteFlickrUploaderWorkflowMessage : ICommandIdentifier
	{
		IFlickrUploaderSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
	}
}
