using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class Names {

    public static string[] forenames;
    public static string[] surnames;
    public static string[] titles;

    public static void Init() {
        string filePath = Application.dataPath + "/Data/names.txt";
        string line;
        int lineNo = 0;

        if (File.Exists(filePath)) {
            StreamReader file = null;
            try {
                file = new StreamReader(filePath);
                while ((line = file.ReadLine()) != null) {
                    // Lines
                    if (lineNo == 0) {
                        forenames = line.Split(new char[] { ',' } );
                    } else if (lineNo == 1) {
                        surnames = line.Split(new char[] { ',' });
                    } else {
                        titles = line.Split(new char[] { ',' });
                    }
                    lineNo++;
                }
            }
            finally {
                if (file != null)
                    file.Close();
            }
        } else {
            Debug.Log("Names file not found!");
        } 
    }
}

