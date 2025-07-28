using Avalonia.Controls.Templates;
using Avalonia.Controls;
using System;
using youtube_downloader_plus.ViewModels;
using youtube_downloader_plus.Views;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace youtube_downloader_plus.ViewLocators;


public class Locator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        Regex regex = new Regex("(?<path>.*)\\.ViewModels.*\\.(?<name>.*)ViewModel$");
        var modelName = data.GetType().FullName;
        var name = regex.Replace(modelName, "${path}.Views.${name}View");
        var type = Type.GetType(name);
        var realType = typeof(DownloadOptionsView);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        return new TextBlock { Text = name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}

