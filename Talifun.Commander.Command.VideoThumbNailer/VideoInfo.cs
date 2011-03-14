using System;
using System.IO;
using System.Text.RegularExpressions;
using Talifun.Commander.Command.VideoThumbNailer.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command
{
    public class VideoInfo
    {
        public string FileName { get; set; }

        public TimeSpan Duration { get; set; }
        public double Start { get; set; }
        public int VideoBitrate { get; set; }

        public string VideoCodec { get; set; }
        public string ColorEncodingMethod { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double FrameRate { get; set; }
        
        public string AudioCodec { get; set; }
        public int AudioSamplingRate { get; set; }
        public string AudioChannels { get; set; }
        public int AudioBitrate { get; set; }

        public static VideoInfo GetVideoInfo(FileInfo videoFilePath, out string output)
        {
            var helper = new FfMpegCommandLineExecutor();
            var args = string.Format(@"-i ""{0}""", videoFilePath.FullName);
            
            var workingDirectory = videoFilePath.DirectoryName;
            var commandPath = VideoThumbnailerSettingConfiguration.Instance.FFMpegPath;
   
            if (!helper.Execute(workingDirectory, commandPath, args, out output))
            {
                return null;
            }

            var videoInfo = new VideoInfo();
            videoInfo.FileName = videoFilePath.Name;

            //Truncate all the beginning filling
            output = output.Remove(0, output.LastIndexOf("Duration"));

            var regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant |
                               RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;

            //Get duration and video bitrate
            var durationRegex = new Regex(@"^.*Duration:\s([:.\d]*).*start\:\s([\d.]*).*bitrate\:\s([\d]*).*$", regexOptions);
            var matchForDuration = durationRegex.Match(output);
            if (matchForDuration.Success)
            {
                var durationString = matchForDuration.Groups[1].Value;
                var startString = matchForDuration.Groups[2].Value;
                var videoBitrateString = matchForDuration.Groups[3].Value;

                var duration = TimeSpan.Zero;
                TimeSpan.TryParse(durationString, out duration);
                videoInfo.Duration = duration;                

                var start = double.MinValue;
                double.TryParse(startString, out start);
                videoInfo.Start = start;
                
                var videoBitrate = int.MinValue;
                int.TryParse(videoBitrateString, out videoBitrate);
                videoInfo.VideoBitrate = videoBitrate;
            }

            //Get video stream information
            var videoRegex = new Regex(@"^.*Stream\s\#\d\.\d.*\:\sVideo\:\s([^,]*),\s([^,]*),\s([\d]*)x([\d]*).*,\s([\d.]*).*$", regexOptions);
            var matchForVideo = videoRegex.Match(output);
            if (matchForVideo.Success)
            {
                var videoCodecString = matchForVideo.Groups[1].Value;
                var colorEncodingMethodString = matchForVideo.Groups[2].Value;
                var widthString = matchForVideo.Groups[3].Value;
                var heightString = matchForVideo.Groups[4].Value;
                var frameRateString = matchForVideo.Groups[5].Value;

                videoInfo.VideoCodec = videoCodecString;
                videoInfo.ColorEncodingMethod = colorEncodingMethodString;

                var width = int.MinValue;
                int.TryParse(widthString, out width);
                videoInfo.Width = width;

                var height = int.MinValue;
                int.TryParse(heightString, out height);
                videoInfo.Height = height;

                var frameRate = double.MinValue;
                double.TryParse(frameRateString, out frameRate);
                videoInfo.FrameRate = frameRate;
            }

            //Get audio stream information
            var audioRegex = new Regex(@"^.*Stream\s\#\d\.\d.*\:\sAudio\:\s([^,]*),\s([\d]*).*,\s(.*),\s([\d]*).*$", regexOptions);
            var matchForAudio = audioRegex.Match(output);
            if (matchForAudio.Success)
            {
                var audioCodecString = matchForAudio.Groups[1].Value;
                var audioSamplingRateString = matchForAudio.Groups[2].Value;
                var audioChannelsString = matchForAudio.Groups[3].Value;
                var audioBitrateString = matchForAudio.Groups[4].Value;

                videoInfo.AudioCodec = audioCodecString;

                var audioSamplingRate = int.MinValue;
                int.TryParse(audioSamplingRateString, out audioSamplingRate);
                videoInfo.AudioSamplingRate = audioSamplingRate;

                videoInfo.AudioChannels = audioChannelsString;

                var audioBitrate = int.MinValue;
                int.TryParse(audioBitrateString, out audioBitrate);
                videoInfo.AudioBitrate = audioBitrate;
            }
            
            return videoInfo;
        }
    }
}