using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YTDLPWrapper
{
    [AttributeUsage(AttributeTargets.Field)]
    public class YTDLPConsoleParamName : Attribute
    {
        public string Value;
        public YTDLPConsoleParamName(string val)
        {
            Value = val;
        }
    }

    public enum CodecType
    {
        Audio,
        Video
    }
   
    [Flags]
    public enum YTDLPDownloadParams
    {
        [YTDLPConsoleParamName("bestaudio")]
        BestAudio = 1,
        [YTDLPConsoleParamName("bestvideo")]
        BestVideo = 2
    }

    public class FileMetadata
    {
        public string ID;
        public string Ext;
        public string Resolution;
        public string Filesize;
        public string Bitrate;
        public string Codec;
        public CodecType CodecType;
        public string DisplayName
        {
            get
            {
                if (ID == YoutubeDLPWrapper.GetConsoleDownloadOption(YTDLPDownloadParams.BestVideo)
                    || ID == YoutubeDLPWrapper.GetConsoleDownloadOption(YTDLPDownloadParams.BestVideo))
                {
                    return ID;
                }
                return CodecType == CodecType.Video
                    ? $"{Codec} | res:{Resolution} bitrate:{Bitrate} | container:{Ext} size:{Filesize}"
                    : $"{Codec} | bitrate:{Bitrate} | container:{Ext} size:{Filesize}";
            }
        }
    }

    public class YoutubeSourceInfo
    {
        public string Url;
        public AvailableFormats AvailableFormats { get; set; }
    }

    public class AvailableFormats
    {
        public List<FileMetadata> AudioCodecs { get; set; } = new List<FileMetadata>();
        public List<FileMetadata> VideoCodecs { get; set; } = new List<FileMetadata>();

    }
    public class ProcessOutput
    {
        public int ExitCode;
        public List<string> DataOutput = new List<string>();
        public List<string> ErrorOutput = new List<string>();
    }

    public class YoutubeDLPWrapper
    {
        private readonly static string FORMATS_REFERENCE_LINE = "[info] Available formats for";
        private readonly static string FORMATS_AUDIO_CODEC_REFERENCE = "audio only";
        private readonly static string FORMATS_VIDEO_CODEC_REFERENCE = "video only";
        private readonly static string FORMATS_PROTOCOL_REFERENCE = "https";
        private readonly static string FORMATS_DRC_AUDIO_REFERENCE = "DRC"; //youtube's volume correction reencode, that decreases audio file bitrate
       
        public static string DefaultFileNameMask => "%(title)s.%(ext)s";

        private IYTDLPWrapperLogger? _logger = null;

        public YoutubeDLPWrapper(IYTDLPWrapperLogger? logger = null)
        {
            _logger = logger;
        }

        public async Task<YoutubeSourceInfo> FetchCodecInfo(string url)
        {
            var res = new YoutubeSourceInfo();
            res.Url = url;
            var processOutput = await FetchCodecFromYTDLP(url);
            var availableFiles = ParseCodecList(processOutput.DataOutput);
            res.AvailableFormats = availableFiles;
            return res;
        }

        public async Task Download(WrapperParams wrapperParams)
        {
            string? videoID = null;
            string? audioID = null;

            //override IDs are specified
            if(wrapperParams.MediaParams.HasValue)
            {
                var par = wrapperParams.MediaParams.Value;
                //bit masks used here
                if ((par & YTDLPDownloadParams.BestVideo) != 0)
                {
                    videoID = GetConsoleDownloadOption(YTDLPDownloadParams.BestVideo);
                }
                //bit masks used here
                if ((par & YTDLPDownloadParams.BestAudio) != 0)
                {
                    audioID = GetConsoleDownloadOption(YTDLPDownloadParams.BestAudio);
                }
            }
            videoID = videoID ?? wrapperParams.VideoID;
            audioID = audioID ?? wrapperParams.AudioID;


            string ytDLPParams = string.Empty;
            bool needsSlash = !(wrapperParams.FilePath.Last() == '/');
            string outputPath = $"{wrapperParams.FilePath}{(needsSlash ? "/" : "")}{wrapperParams.FileName}";
            var videoConsoleParam = wrapperParams.VideoDisabled ? string.Empty : videoID;
            var connector = !wrapperParams.VideoDisabled && !wrapperParams.AudioDisabled ? "+" : string.Empty;
            var audioConsoleParam = wrapperParams.AudioDisabled ? string.Empty : audioID;
            var extractAudioFlag = wrapperParams.VideoDisabled ? " -x" : string.Empty;
            var codecConsoleParam = $"{videoConsoleParam}{connector}{audioConsoleParam}{extractAudioFlag}";
            ytDLPParams = $"-f {codecConsoleParam} {wrapperParams.Url} -o {outputPath}";

            if (wrapperParams.VideoDisabled && wrapperParams.AudioDisabled)
            {
                throw new Exception("Download error! Either video or audio format must be specified!");
            }

            var processOutput = new ProcessOutput();
            var p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data != null)
                {
                    processOutput.DataOutput.Add(e.Data);
                    if (_logger != null)
                    {
                        _logger.WriteLog(e.Data);
                    }
                }
            });
            p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data != null)
                {
                    processOutput.ErrorOutput.Add(e.Data);
                }
            });
            p.StartInfo.FileName = "yt-dlp.exe";
            p.StartInfo.Arguments = ytDLPParams;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();
            processOutput.ExitCode = p.ExitCode;
            return;
        }
        private AvailableFormats ParseCodecList(List<string> dataOutput)
        {
            FileMetadata bestAudioFile = new FileMetadata();
            FileMetadata bestVideoFile = new FileMetadata();
            bestAudioFile.ID = bestAudioFile.Codec = GetConsoleDownloadOption(YTDLPDownloadParams.BestAudio);
            bestVideoFile.ID = bestVideoFile.Codec = GetConsoleDownloadOption(YTDLPDownloadParams.BestVideo);

            int indexOfAvailableFormatsLine = dataOutput.FindIndex(
                    x => x.StartsWith(FORMATS_REFERENCE_LINE)
                );

            if (indexOfAvailableFormatsLine < 0 || indexOfAvailableFormatsLine >= dataOutput.Count - 2)
            {
                //TODO: throw exception here if not found
            }
            indexOfAvailableFormatsLine += 3;

            var availableFiles = new AvailableFormats();
            availableFiles.AudioCodecs.Add(bestAudioFile);
            availableFiles.VideoCodecs.Add(bestVideoFile);

            for (var i = indexOfAvailableFormatsLine; i < dataOutput.Count; i++)
            {
                try
                {
                    if (dataOutput[i].Contains(FORMATS_AUDIO_CODEC_REFERENCE)
                        && dataOutput[i].Contains(FORMATS_PROTOCOL_REFERENCE)
                        && !dataOutput[i].Contains(FORMATS_DRC_AUDIO_REFERENCE))
                    {
                        var audioMetadata = SplitLine(dataOutput[i]);
                        availableFiles.AudioCodecs.Add(audioMetadata);
                    }

                    if (dataOutput[i].Contains(FORMATS_VIDEO_CODEC_REFERENCE) && dataOutput[i].Contains(FORMATS_PROTOCOL_REFERENCE))
                    {
                        var videoMetadata = SplitLine(dataOutput[i]);
                        availableFiles.VideoCodecs.Add(videoMetadata);
                    }
                }
                catch (Exception e)
                {
                    //caught unparsable string, skipping
                }
            }

            return availableFiles;

        }
        private FileMetadata SplitLine(string data)
        {
            var codecMetaData = new FileMetadata();

            var strings = data.Split('|', StringSplitOptions.RemoveEmptyEntries);
            if (strings.Count() < 3)
            {
                //TODO: exception here if line is garbage
            }
            var firstColumnString = strings[0];
            var secondColumnString = strings[1];
            var thirdColumnString = strings[2];

            var firstColumnValues = firstColumnString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var secondColumnsValues = secondColumnString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var thirdColumnValues = thirdColumnString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            codecMetaData.ID = firstColumnValues[0];
            codecMetaData.Ext = firstColumnValues[1];
            codecMetaData.Resolution = firstColumnValues[2];
            codecMetaData.Filesize = secondColumnsValues[0];
            codecMetaData.Bitrate = secondColumnsValues[1];
            if (thirdColumnValues[0] == "audio")
            {
                codecMetaData.CodecType = CodecType.Audio;
                codecMetaData.Codec = thirdColumnValues[2];
            }
            if (thirdColumnValues[2] == "video")
            {
                codecMetaData.CodecType = CodecType.Video;
                codecMetaData.Codec = thirdColumnValues[0];
            }
            return codecMetaData;
        }

        private async Task<ProcessOutput> FetchCodecFromYTDLP(string url)
        {
            var processOutput = new ProcessOutput();
            var p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data != null)
                {
                    processOutput.DataOutput.Add(e.Data);
                    if (_logger != null)
                    {
                        _logger.WriteLog(e.Data);
                    }
                }
            });
            p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data != null)
                {
                    processOutput.ErrorOutput.Add(e.Data);
                }
            });
            p.StartInfo.FileName = "yt-dlp.exe";
            p.StartInfo.Arguments = $"-F {url}";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();
            processOutput.ExitCode = p.ExitCode;
            return processOutput;
        }

        internal static string GetConsoleDownloadOption(YTDLPDownloadParams downloadParam) {
            return typeof(YTDLPDownloadParams)
            .GetField(downloadParam.ToString())?
            .GetCustomAttribute<YTDLPConsoleParamName>(false)?.Value;
        }

    }
}
