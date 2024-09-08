using System.IO;
using System.Linq;
using UnityEngine;

public static class PackageManagerHelper {

    // Add a specific package to the manifest.json
    public static void InsertPackageLine(string manifestPath, string identifier) {
        // do nothing if package dependency already exists
        if (File.ReadAllText(manifestPath).Contains($@"""{identifier}"": ""file:../../../{identifier}""")) {
            return;
        }

        var lines = File.ReadAllLines(manifestPath).ToList();
        string newLine = $@"    ""{identifier}"": ""file:../../../{identifier}"",";

        // Ensure there are enough lines to insert between the 2nd and 3rd line
        if (lines.Count < 2) {
            Debug.LogError("Manifest.json does not have enough lines to insert the new line.");
            return;
        }

        lines.Insert(2, newLine);
        File.WriteAllLines(manifestPath, lines.ToArray());

        Debug.Log("Inserted package into manifest.json.");
    }
}
