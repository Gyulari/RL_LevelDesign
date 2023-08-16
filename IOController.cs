using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class IOController : MonoBehaviour
{
    List<List<float>> data = new();

    List<string> columnTitles = new List<string> { "Frames", "Reward(B)", "Reward(R)", "" };

    [SerializeField] private string dataName;

    private string path;
    StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        path = Path.Combine(Application.dataPath + "/Data/", dataName + ".csv");
    }

    public void CSVWrite(string winRate)
    {
        sb.AppendLine(winRate);
        StreamWriter outStream = System.IO.File.CreateText(path);
        outStream.WriteLine(sb);
        outStream.Close();
    }
}
