using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using BrandonUtils.Standalone.Exceptions;

using UnityEditor;
using UnityEditor.Build.Reporting;

using UnityEngine;

namespace BrandonUtils.Editor.CommandLine {
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Based on <a href="https://gist.github.com/jagwire/0129d50778c8b4462b68">Unity command line script to build WebGL player</a>
    /// </remarks>
    public static class BuildAutomator {
        /// <summary>
        /// The folder where builds (aka "players"; i.e. the results of <see cref="BuildPipeline.BuildPlayer(BuildPlayerOptions)"/>) are placed.
        /// </summary>
        /// <remarks>
        /// Relative to <c>Assets/</c>.
        /// </remarks>
        public const string BuildOutputDirectory = "Builds";
        /// <summary>
        /// The folder where installations of <a href="https://itch.io/docs/butler/installing.html">butler</a> are extracted to.
        /// </summary>
        /// <remarks>
        /// Relative to <c>Assets/</c>.
        /// </remarks>
        public const string ButlerInstallationDirectory = "butler";
        public const string ButlerBaseUrl = "https://broth.itch.ovh/butler";

        public static readonly Dictionary<OSPlatform, string> ButlerFileNames = new Dictionary<OSPlatform, string>() {
            {OSPlatform.Windows, "windows-amd64"},
            {OSPlatform.OSX, "darin-amd64"},
            {OSPlatform.Linux, "linux-amd64"}
        };

        public static void WebGL() {
            Build(BuildTarget.WebGL);
        }

        public static BuildReport Build(BuildTarget buildTarget) {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes           = GetScenesToBuild();
            buildPlayerOptions.target           = buildTarget;
            buildPlayerOptions.locationPathName = Path.Combine(BuildOutputDirectory, buildTarget.ToString());
            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log(buildReport);
            // Console.WriteLine($"Created the following artifacts: ${buildReport.files.JoinString("\n")}");
            if (buildReport.summary.result == BuildResult.Succeeded) {
                Debug.Log("Build succeeded!");
                return buildReport;
            }

            throw new BrandonException($"Build failed with a result of {buildReport.summary.result}");
        }

        /// <summary>
        /// Returns the <see cref="EditorBuildSettingsScene"/>s that should be included in a <see cref="BuildPipeline.BuildPlayer(BuildPlayerOptions)">build</see>.
        /// </summary>
        /// <returns>the <see cref="EditorBuildSettingsScene.path"/>s of the <see cref="EditorBuildSettings.scenes"/> that are <see cref="EditorBuildSettingsScene.enabled"/>.</returns>
        public static string[] GetScenesToBuild() {
            return EditorBuildSettings.scenes.Where(it => it.enabled).Select(it => it.path).ToArray();
        }

        private static string GetBuildOutputDirectory(BuildTarget buildTarget) {
            return Path.Combine(BuildOutputDirectory, buildTarget.ToString());
        }

        /// <summary>
        /// Returns the current <see cref="OSPlatform"/>, which is one of:
        /// <ul>
        /// <li><see cref="OSPlatform.Windows"/></li>
        /// <li><see cref="OSPlatform.OSX"/></li>
        /// <li><see cref="OSPlatform.Linux"/></li>
        /// </ul>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException">If the current operating system is neither <see cref="OSPlatform.Windows"/>, <see cref="OSPlatform.OSX"/>, or <see cref="OSPlatform.Linux"/>.</exception>
        private static OSPlatform GetOSPlatform() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                return OSPlatform.OSX;
            }
            else {
                throw new PlatformNotSupportedException($"I don't know how, but this isn't running on _any operating system_! (P.S. I only know about {OSPlatform.Windows}, {OSPlatform.OSX}, and {OSPlatform.Linux})");
            }
        }
    }
}
