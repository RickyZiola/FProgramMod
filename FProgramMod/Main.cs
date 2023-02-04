using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using System.Collections.Generic;
using UnityEngine;
using SFS.UI;
using SFS.UI.ModGUI;
using Type = SFS.UI.ModGUI.Type;
using System;
using System.IO;
using Python.Runtime;
namespace SFSMod
{

    /**
     * You only need to implement the Mod class once in your mod. The Mod class is how 
     * you tell the mod loader what it needs to load and execute.
     */
    public class SFSFP : Mod
    {
        public static void WriteTextToFile(string text, string filename)
        {
            File.AppendAllText(filename, text);
        }
        public static SFSFP Main;
        static GameObject windowHolder;

        static readonly int MainWindowID = Builder.GetRandomID();
        string scriptFilename;
        static Window window;
        static RectInt windowRect = new RectInt(0, 0, 300, 500);
        public static void ShowScriptSelectionGUI()
        {
            windowHolder = Builder.CreateHolder(Builder.SceneToAttach.CurrentScene, "WindowHolder");
            window = Builder.CreateWindow(windowHolder.transform, MainWindowID, windowRect.width, windowRect.height, windowRect.x, windowRect.y, true, true, .95f, "Script selection");

            window.CreateLayoutGroup(Type.Vertical);
            window.EnableScrolling(Type.Vertical);
            int i = 1;
            foreach (string fname in Directory.GetFiles("Scripts/")) {
                Debug.Log(fname);
                Main.scriptFilename = fname;
                Builder.CreateButtonWithLabel(window, 290, 30, 0, 70 * i, fname, "Load Script", StartScript(fname));
                Builder.CreateSeparator(window, 300, 0, 70 * (i + 1));
                i++;
            }

            

        }
        // this ModNameID can be whatever you want
        public override string ModNameID => "amgongus";

        public override string DisplayName => "SFS Flight Programs";

        public override string Author => "FrederickAmpsUp";

        public override string MinimumGameVersionNecessary => "0.3.7";

        // I recommend use MAJOR.MINOR.PATCH Semantic versioning. 
        // Reference link: https://semver.org/ 
        public override string ModVersion => "0.0.1i57";

        public override string Description => "Adds flight programming support in Python.";

        // With this variable you can define if your mods needs the others mods to work
        public override Dictionary<string, string> Dependencies
        {
            get
            {
                return this._dependencies;
            }
        }

        // Here you can specify which mods and version you need
        private Dictionary<string, string> _dependencies = new Dictionary<string, string>() { };

        public override void Early_Load()
        {
            // This is for a singleton pattern. you can see more about singleton here https://www.c-sharpcorner.com/UploadFile/8911c4/singleton-design-pattern-in-C-Sharp/
            Main = this;

            // Make use to use Debug.log from UnityEngine
            Debug.Log("Initialising SFS Flight program");

            // Use early load to use Harmony and patch function
            Harmony harmony = new Harmony(ModNameID);
            // I use ModNameID in Harmony, because you need to pass string ID to create an instance of Harmony.

            // This function finds all the patches you have created and runs them
            harmony.PatchAll();

            // you can subscribe to scene changes
            SceneHelper.OnWorldSceneLoaded += this.OnWorld;
            SceneHelper.OnBuildSceneLoaded += this.OnBuild;

        }

        public override void Load()
        {
            Debug.Log("Running Load code");

            // init your keybindings
            Settings.Setup();
            SceneHelper.OnWorldSceneLoaded += () =>
            {
                SFSFP.ShowScriptSelectionGUI();
            };

        }
        static void RunScript (string scriptName)
        {
            Debug.Log("starting script " + scriptName);
            //MsgDrawer.main.Log("Loading script: " + scriptName);
            Runtime.PythonDLL = "./python310.dll";
            PythonEngine.Initialize();
            string program_name = "script_loader.py";
            using (Py.GIL())
            {
                dynamic os = Py.Import("os");
                dynamic sys = Py.Import("sys");
                sys.path.append(os.path.dirname(os.path.expanduser(program_name)));
                dynamic fromFile = Py.Import(Path.GetFileNameWithoutExtension(program_name));
                fromFile.RunScript(scriptName);
            }
        }
        
        static Action StartScript(string scriptName)
        {
            return new Action(() => { RunScript(scriptName); });
        }
        // When the world scene is loaded
        private void OnWorld()
        {
            Debug.Log("On World");

        }

        // When the Build scene is loaded
        private void OnBuild()
        {
            Debug.Log("On Build");
        }

    }
}