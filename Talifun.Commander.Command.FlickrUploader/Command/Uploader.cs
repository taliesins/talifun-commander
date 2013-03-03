using System;
using System.IO;
using System.Net;
using FlickrNet;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Events;
using Talifun.Commander.Command.FlickrUploader.Command.Response;

namespace Talifun.Commander.Command.FlickrUploader.Command
{
    public class Uploader
    {
        private readonly AsyncUploadSettings _asyncUploadSettings;
        public Uploader(AsyncUploadSettings asyncUploadSettings)
        {
            _asyncUploadSettings = asyncUploadSettings;
        }

        public void OnUploadProgress(object sender, UploadProgressEventArgs e)
        {
            var message = string.Format("Upload Progress: {0} ({1}/{2} - {3}%)", DateTime.Now, e.BytesSent, e.TotalBytesToSend, e.ProcessPercentage);

            var executeFlickrUploaderWorkflowMessage = _asyncUploadSettings.Message;

            var flickrUploaderProgressMessage = new FlickrUploaderProgressMessage
            {
                CorrelationId = executeFlickrUploaderWorkflowMessage.CorrelationId,
                InputFilePath = executeFlickrUploaderWorkflowMessage.InputFilePath,
                Output = message
            };

            var bus = BusDriver.Instance.GetBus(FlickrUploaderService.BusName);
            bus.Publish(flickrUploaderProgressMessage);
        }

        public void OnUploadCompleted(FlickrResult<string> flickrResult)
        {
            _asyncUploadSettings.InputStream.Close();
            _asyncUploadSettings.InputStream = null;

            var executeFlickrUploaderWorkflowMessage = _asyncUploadSettings.Message;

            if (flickrResult.Error != null)
            {
                var webException = (WebException)flickrResult.Error;
                var response = webException.Response;
                var responseBody = string.Empty;

                if (response != null)
                {
                    if (response.ContentLength > 0)
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                responseBody = reader.ReadToEnd().Trim();
                            }
                        }
                    }
                }
            }

            var executedFlickrUploaderWorkflowMessage = new ExecutedFlickrUploaderWorkflowMessage
            {
                CorrelationId = executeFlickrUploaderWorkflowMessage.CorrelationId,
                Cancelled = flickrResult.HasError && flickrResult.Error is OperationCanceledException,
                Error = flickrResult.Error
            };

            var bus = BusDriver.Instance.GetBus(FlickrUploaderService.BusName);
            bus.Publish(executedFlickrUploaderWorkflowMessage);
        }
    }
}
