using System.Diagnostics;
using System.IO;

public static class GitUtility {

    public static void InitializeRepository(string workingDirectory, string packageName) {
        if (Directory.Exists(Path.Combine(workingDirectory, ".git"))) {
            UnityEngine.Debug.Log($"git repository already exists in '{Path.GetFullPath(workingDirectory)}'. Skipping git initialization step. (This message is harmless)");
            return;
        }
        string remoteRepoPath = $"https://github.com/julienkay/{packageName}.git";
        RunGitCommand("init", workingDirectory);
        RunGitCommand($"remote add origin {remoteRepoPath}", workingDirectory);
        CommitInitialChanges(workingDirectory);
    }

    static void RunGitCommand(string arguments, string workingDirectory, bool suppressStdErr = false) {
        ProcessStartInfo startInfo = new ProcessStartInfo {
            FileName = "git",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        };

        using (Process process = new Process()) {
            process.StartInfo = startInfo;
            process.Start();

            // Capture output and errors (if any)
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(output)) {
                UnityEngine.Debug.Log(output);
            }

            if (!suppressStdErr && !string.IsNullOrEmpty(error)) {
                UnityEngine.Debug.LogError("Error: " + error);
            }
        }
    }

    public static void CommitInitialChanges(string workingDirectory) {
        // Stage all files
        RunGitCommand("add .", workingDirectory, suppressStdErr: true);

        // Commit with the message "initial commit"
        RunGitCommand("commit -m \"initial commit\"", workingDirectory);
    }
}
