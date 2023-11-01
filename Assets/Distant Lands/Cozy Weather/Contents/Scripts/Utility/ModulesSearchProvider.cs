#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace DistantLands.Cozy
{
    public class ModulesSearchProvider : ScriptableObject, ISearchWindowProvider
    {

        public List<Type> modules;
        public CozyWeather weather;



        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select a Module"), 0));
            foreach (Type i in modules)
            {

                entries.Add(GetSearchTreeEntry(i.Name, i));

            }
            return entries;
        }

        public SearchTreeEntry GetSearchTreeEntry(string name, Type type)
        {

            GUIContent content = new GUIContent();

            switch (name)
            {
                case ("CozyAmbienceManager"):
                    content = new GUIContent(" Ambience Manager", (Texture)Resources.Load("Ambience Profile"));
                    break;
                case ("CozyEventManager"):
                    content = new GUIContent(" Event Manager", (Texture)Resources.Load("Events"));
                    break;
                case ("CozyMaterialManager"):
                    content = new GUIContent(" Material Manager", (Texture)Resources.Load("MaterialManager"));
                    break;
                case ("CozyMicrosplatModule"):
                    content = new GUIContent(" Microsplat Integration", (Texture)Resources.Load("Integration"));
                    break;
                case ("CozyReflect"):
                    content = new GUIContent(" Reflection Module", (Texture)Resources.Load("Reflections"));
                    break;
                case ("CozyReports"):
                    content = new GUIContent(" Reports Module", (Texture)Resources.Load("Reports"));
                    break;
                case ("CozySatelliteManager"):
                    content = new GUIContent(" Satellite Manager", (Texture)Resources.Load("CozyMoon"));
                    break;
                case ("CozySaveLoad"):
                    content = new GUIContent(" Save/Load Module", (Texture)Resources.Load("Save"));
                    break;
                case ("CozyTVEModule"):
                    content = new GUIContent(" The Vegetation Engine Integration", (Texture)Resources.Load("Boxophobic"));
                    break;
                case ("CozyButoModule"):
                    content = new GUIContent(" Buto Integration", (Texture)Resources.Load("Occa"));
                    break;
                case ("VFXModule"):
                    content = new GUIContent(" Visual FX Module", (Texture)Resources.Load("FX Module"));
                    break;
                case ("BlocksModule"):
                    content = new GUIContent(" BLOCKS Module", (Texture)Resources.Load("Blocks"));
                    break;
                case ("PlumeModule"):
                    content = new GUIContent(" PLUME Module", (Texture)Resources.Load("Cloud"));
                    break;
                case ("CataclysmModule"):
                    content = new GUIContent(" CATACLYSM Module", (Texture)Resources.Load("Tornado"));
                    break;
                case ("LinkFishnetModule"):
                    content = new GUIContent(" LINK Module", (Texture)Resources.Load("Link"));
                    break;
                case ("LinkNetcodeModule"):
                    content = new GUIContent(" LINK Module", (Texture)Resources.Load("Link"));
                    break;
                case ("LinkPhotonModule"):
                    content = new GUIContent(" LINK Module", (Texture)Resources.Load("Link"));
                    break;
                case ("LinkMirrorModule"):
                    content = new GUIContent(" LINK Module", (Texture)Resources.Load("Link"));
                    break;
                case ("CultivateModule"):
                    content = new GUIContent(" CULTIVATE Module", (Texture)Resources.Load("Ecosystem"));
                    break;
                case ("CozyHabits"):
                    content = new GUIContent(" HABITS Module", (Texture)Resources.Load("Habits"));
                    break;
                default:
                    content = new GUIContent(name);
                    break;
            }

            SearchTreeEntry entry = new SearchTreeEntry(content);
            entry.level = 1;
            entry.userData = type;
            return entry;

        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            weather?.InitializeModule((Type)SearchTreeEntry.userData);
            return true;
        }
    }

    public class BiomeModulesSearchProvider : ScriptableObject, ISearchWindowProvider
    {

        public List<Type> modules;
        public CozyBiome biome;



        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select a Module"), 0));
            foreach (Type i in modules)
            {

                entries.Add(GetSearchTreeEntry(i.Name, i));

            }
            return entries;
        }

        public SearchTreeEntry GetSearchTreeEntry(string name, Type type)
        {

            GUIContent content = new GUIContent();

            switch (name)
            {
                case ("BlocksBiome"):
                    content = new GUIContent(" BLOCKS Extension", (Texture)Resources.Load("Blocks"));
                    break;
                case ("AmbienceBiomeModule"):
                    content = new GUIContent(" Ambience Extension", (Texture)Resources.Load("Ambience Profile"));
                    break;
                default:
                    content = new GUIContent(name);
                    break;
            }

            SearchTreeEntry entry = new SearchTreeEntry(content);
            entry.level = 1;
            entry.userData = type;
            return entry;

        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            biome?.InitializeModule((Type)SearchTreeEntry.userData);
            return true;
        }
    }

}
#endif