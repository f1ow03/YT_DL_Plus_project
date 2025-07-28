using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YTDLPWrapper;

//TODO Add version binding in AboutProgram window.
public class Config
{
    private static readonly YTDLPDownloadParams DEFAULT_DOWNLOAD_OPTION = YTDLPDownloadParams.BestVideo | YTDLPDownloadParams.BestAudio;

    protected Dictionary<ConfigParam, string> ActiveContents = new Dictionary<ConfigParam, string>()
    {
        { ConfigParam.SaveFolder,  "" },
        { ConfigParam.DefaultDownloadParam, DEFAULT_DOWNLOAD_OPTION.ToString() }
    };

    private static Config ActiveConfig = new Config();

    private Config(Config other)
    {
        ActiveContents = new Dictionary<ConfigParam, string>(other.ActiveContents);
    }

    private Config() { }

    public static Config GetCopy()
    {
        return new Config(ActiveConfig);
    }

    public static Config GetActive()
    {
        if (ActiveConfig == null)
        {
            ActiveConfig = new Config();
        }
        return ActiveConfig;
    }

    public void SetActive()
    {
        ActiveConfig = this;
    }

    protected enum ConfigParam
    {
        SaveFolder,
        DefaultDownloadParam
    }

    public YTDLPDownloadParams DefaultDownloadValue
    {
        get
        {
            if (ActiveContents.TryGetValue(ConfigParam.DefaultDownloadParam, out string? value))
            {
                if (Enum.TryParse<YTDLPDownloadParams>(value, out var result))
                {
                    return result;
                }
            }
            return DEFAULT_DOWNLOAD_OPTION;
        }
        set
        {
            ActiveContents[ConfigParam.DefaultDownloadParam] = value.ToString();
        }
    }

    public string DefaultSaveFolder
    {
        get
        {
            return ActiveContents.TryGetValue(ConfigParam.SaveFolder, out string? value) 
                ? value 
                : string.Empty;
        }
        set
        {
            ActiveContents[ConfigParam.SaveFolder] = value;
        }
    }

    public static void Load()
    {
        var conf = GetCopy();
        var configContents = AutoIni.GetIniContents();
        foreach (var item in configContents)
        {
            if (Enum.TryParse(typeof(ConfigParam), item.Key, out var enumVal))
            {
                conf.ActiveContents[(ConfigParam)enumVal] = item.Value;
            }
        }
        conf.SetActive();
    }

    public static void Save()
    {
        var conf = GetActive();
        var paramContents = conf.ActiveContents
            .Select(x => new KeyValuePair<string, string>(x.Key.ToString(), x.Value))
            .ToDictionary(x => x.Key, x => x.Value);
        AutoIni.SaveIniContents(paramContents);
    }
}

