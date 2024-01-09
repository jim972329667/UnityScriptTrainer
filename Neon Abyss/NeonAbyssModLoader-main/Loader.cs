using I2.Loc;
using System;
using System.IO;
using System.Reflection;

namespace ModLoader
{
    public static class Loader
    {
        static TextWriter fs;
        static bool isLoaded = false;

        static string DOORSTOP_INVOKE_DLL_PATH;
        static string DOORSTOP_MANAGED_FOLDER_DIR;
        static string DOORSTOP_PROCESS_PATH;
        static string[] NEEDED_ASSEMBLIES = {
            $"0Harmony20.dll"
            //@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\PresentationFramework.dll",
        };

        internal static void LoadEnv()
        {
            DOORSTOP_INVOKE_DLL_PATH = Environment.GetEnvironmentVariable("DOORSTOP_INVOKE_DLL_PATH");
            DOORSTOP_MANAGED_FOLDER_DIR = Environment.GetEnvironmentVariable("DOORSTOP_MANAGED_FOLDER_DIR");
            DOORSTOP_PROCESS_PATH = Environment.GetEnvironmentVariable("DOORSTOP_PROCESS_PATH");
        }

        internal static void LogHeader()
        {

            fs.WriteLine($"[Moadloder] Loaded at: {DateTime.Now:R}!");
            fs.WriteLine($"Command line: {Environment.CommandLine}");
            fs.WriteLine();
            fs.WriteLine("Doorstop also set the following environment variables:");
            fs.WriteLine($"DOORSTOP_INVOKE_PATH = {DOORSTOP_INVOKE_DLL_PATH}");
            fs.WriteLine($"DOORSTOP_MANAGED_FOLDER_DIR = {DOORSTOP_MANAGED_FOLDER_DIR}");
            fs.WriteLine($"DOORSTOP_PROCESS_PATH = {DOORSTOP_PROCESS_PATH}");
        }

        public static void LoadRequiredAssemblies()
        {
            foreach (var assembly in NEEDED_ASSEMBLIES)
            {
                Assembly.LoadFrom(assembly);
            }

            //var managedDlls = Directory.GetFiles(DOORSTOP_MANAGED_FOLDER_DIR);
            //foreach (var assembly in managedDlls)
            //{
            //    Assembly.LoadFrom(assembly);
            //}
        }

        public static void LoadMods()
        {
            string modsFolderPath = Path.Combine(Path.GetDirectoryName(DOORSTOP_PROCESS_PATH), "mods");
            if(!Directory.Exists(modsFolderPath))
                Directory.CreateDirectory(modsFolderPath);
            var modsFolders = Directory.GetDirectories(modsFolderPath);
            foreach (var modFolderPath in modsFolders)
            {
                var modName = Path.GetFileName(modFolderPath);
                var modDllPath = Path.Combine(modFolderPath, $"{modName}.dll");
                try
                {
                    var mod = Assembly.LoadFrom(modDllPath);

                    Type modClass = mod.GetType($"{modName}.{modName}");
                    modClass.GetMethod("Initialize").Invoke(Activator.CreateInstance(modClass), null);
                    fs.WriteLine($"Mod: {modName} initialized");
                }
                catch (Exception e)
                {
                    fs.WriteLine($"Exception: {e}");
                }
            }
        }

        public static void FixModdedTranslations()
        {
            LocalizationManager.Sources.ForEach(x => x.OnMissingTranslation = LanguageSourceData.MissingTranslationAction.ShowTerm);
            LocalizationManager.UpdateSources();
        }

        private static void SceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            if (!isLoaded)
            {
                FixModdedTranslations();
                LoadMods();
                isLoaded = true;
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                var logFilePath = "ModLoader.log";
                if (File.Exists(logFilePath))
                    File.Delete(logFilePath); 
                fs = File.CreateText(logFilePath);
                LoadEnv();
                LogHeader();
                LoadRequiredAssemblies();
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;

                Console.WriteLine($"[Moadloder] Ready to play");
            }
            catch (Exception e)
            {
                fs.WriteLine($"[Moadloder] EXCEPTION: {e}");
            }
            fs.Flush();
        }
    }
}