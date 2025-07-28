using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;


public class AutoIni
{
    static string iniPath = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".autoini").Replace(".exe", ".autoini");

    public static void CheckForConfigFile()
    {
        if (!File.Exists(iniPath))
        {
            var file = File.Create(iniPath);
            file.Close();
        }
    }

    public static Dictionary<string, string> GetIniContents()
    {
        CheckForConfigFile();
        Dictionary<string, string> iniContents = new Dictionary<string, string>();
        string[] arrLine = File.ReadAllLines(iniPath);

        foreach (string line in arrLine)
        {
            var values = line.Split('=');
            iniContents.Add(values[0], values[1]);
        }
        return iniContents;
    }

    public static void SaveIniContents(Dictionary<string, string> iniContents)
    {
        var lines = iniContents.Select(x => $"{x.Key}={x.Value}");
        File.WriteAllLines(iniPath, lines);
    }

    public static void WriteToIni(string key, string value)
    {
        var iniContents = GetIniContents();
        iniContents[key] = value;
        SaveIniContents(iniContents);
    }

    public static string ReadFromIni(string key, string defaulValue)
    {
        var iniContents = GetIniContents();

        if (iniContents.ContainsKey(key))
        {
            return iniContents[key];
        }
        else
        {
            return defaulValue;
        }
    }
}

