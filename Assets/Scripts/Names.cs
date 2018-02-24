using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class Names {

    static string[] forenames;
    static string[] surnames;
    static string[] titles;

    public static void Init() {
        FileStream fileStream;
        string filePath = Application.dataPath + "/Assets/Data/names.txt";
        using (fileStream = File.Open(filePath, FileMode.Open)) {
            byte[] b = new byte[1024];
            UTF8Encoding temp = new UTF8Encoding(true);

            while (fileStream.Read(b, 0, b.Length) > 0) {
            }

        }
    }
}

