using BepInEx;
using UnityEngine;
using System;
using System.Linq;
using BepInEx.Logging;
using System.Runtime.CompilerServices;
using System.Runtime;
using HUD;
using RWCustom;
using System.Collections.Generic;
using System.Globalization;
using static MonoMod.InlineRT.MonoModRule;
using JetBrains.Annotations;
using Unity.Mathematics;
using HarmonyLib;

using Menu;
using Menu.Remix.MixedUI;
using Expedition;
using JollyCoop.JollyMenu;
using MSCSceneID = MoreSlugcats.MoreSlugcatsEnums.MenuSceneID;
using IL.JollyCoop.JollyManual;

using SlugBase;
using On.MoreSlugcats;
using RainMeadow;
using DevConsole;
using DevConsole.Commands;
using On.Watcher;
using SBCameraScroll;

using Logger = UnityEngine.Logger;

// ReSharper disable SimplifyLinqExpressionUseAll

// ReSharper disable UseMethodAny.0

// ReSharper disable once CheckNamespace
namespace MothDiversity
{

    [BepInPlugin(MOD_ID, "MothDiversity", "0.0.1")]
    internal class Plugin : BaseUnityPlugin
    {
        public const string MOD_ID = "nassoc.mothdiversity";

        // thank you alphappy for logging help too
        internal static BepInEx.Logging.ManualLogSource logger;
        internal static void Log(LogLevel loglevel, object msg) => logger.Log(loglevel, msg);

        internal static Plugin instance;
        public Plugin()
        {
            logger = Logger;
            instance = this;
        }

        private bool weInitializedYet = false;
        public void OnEnable()
        {
            try
            {
                Logger.LogDebug("MothDiversity Plugin loading...");
                //On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

                if (!weInitializedYet)
                {
                    On.RainWorld.PostModsInit += PostModsInitt;

                    On.Watcher.BigMothGraphics.WingGraphics.DrawSprites += WingDrawSprites;
                }

                weInitializedYet = true;
                Logger.LogDebug("MothDiversity Plugin successfully loaded!");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        // i almost did a typo by forgetting the first l :<<< (its fine cause im one too lmao)
        public static Color[] flagFromColors(Color[] colors)
        {
            if (colors.Length == 3)
            {
                return [
                    colors[0],colors[0],
                    colors[0],colors[1],
                    colors[1],colors[1],
                    colors[2],colors[2],
                ];
            }
            if (colors.Length == 5)
            {
                return [
                    colors[0],colors[0],
                    colors[1],colors[2],
                    colors[3],colors[4],
                    colors[4],colors[4],
                ];
            }

            return [
                new Color(25,0,25),new Color(10,10,10),
                new Color(25,0,25),new Color(10,10,10),
                new Color(25,0,25),new Color(10,10,10),
                new Color(25,0,25),new Color(10,10,10),
            ];
        }
        
        Color[][] myColors = [
            [
                new Color(25,0,0),new Color(25,0,0),
                new Color(25,12,0),new Color(25,25,0),
                new Color(0,25,0),new Color(0,25,0),
                new Color(0,12,25),new Color(12,0,25)
            ],/*
            [
                new Color(10,20,25),new Color(10,20,25),
                new Color(25,15,15),new Color(25,25,25),
                new Color(25,15,15),new Color(10,20,25),
                new Color(10,20,25),new Color(10,20,25)
            ],
            [
                new Color(25,12,15),new Color(25,12,15),
                new Color(25,12,15),new Color(25,20,0),
                new Color(25,20,0),new Color(25,20,0),
                new Color(0,15,25),new Color(0,15,25)
            ],*/
            // trans
            flagFromColors([
                new Color(10,20,25),new Color(25,15,15),
                new Color(25,25,25),new Color(25,15,15),
                new Color(10,20,25),
            ]),
            // pan
            flagFromColors([
                new Color(25,12,15),new Color(25,20,0),
                new Color(0,15,25),
            ]),
            // less bien
            flagFromColors([
                new Color(25,0,0),new Color(25,12,0),
                new Color(25,25,25),new Color(20,10,15),
                new Color(15,0,10),
            ]),
        ];

        private void WingDrawSprites(BigMothGraphics.WingGraphics.orig_DrawSprites orig, Watcher.BigMothGraphics.WingGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            int num = (self.inFront ? self.owner.FirstInFrontWingSprite : self.owner.FirstBehindWingSprite) + self.firstSpriteOnLayer;
            float num2 = (float)((self.index == 1) ? -1 : 1);
            float a = 0.5f + self.spreadOutness * num2 * -1f * 0.5f;
            if (!self.owner.shadowMode)
            {
                UnityEngine.Random.State state = UnityEngine.Random.state;
                UnityEngine.Random.InitState(self.owner.bug.abstractCreature.ID.RandomSeed);
                int whichColShouldBe = (int)Math.Floor(UnityEngine.Random.value * myColors.Length);
                Logger.Log(LogLevel.Info, "g" + whichColShouldBe);
                for (int i = 0; i < self.segments.GetLength(0); i++)
                {
                    //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = self.SetColorAlpha(self.owner.BodyColorWithDepth(depth, num6 * self.spreadOutness * num2), a);
                    //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = new Color(0,0,0,a);
                    (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = new Color(myColors[whichColShouldBe][i].r/25,myColors[whichColShouldBe][i].g/25,myColors[whichColShouldBe][i].b/25, a);//[(int)Math.Floor(RXRandom.Int(0f,3f))]
                    //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2 + 1] = self.SetColorAlpha(myColors[whichColShouldBe][i], a);//[(int)Math.Floor(RXRandom.Int(0f,3f))]
                    //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2 + 1] = self.SetColorAlpha(self.owner.BodyColorWithDepth(depth, num6 * -self.spreadOutness * num2), a);
                    Logger.Log(LogLevel.Info,i+" "+self.SetColorAlpha(myColors[whichColShouldBe][i], a).r+" "+self.SetColorAlpha(myColors[whichColShouldBe][i], a).g+" "+self.SetColorAlpha(myColors[whichColShouldBe][i], a).b);
                }
            }

            //self.owner.bodyColor = Color.blue;
        }

        private void PostModsInitt(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
        }
        
        // MARKER: Utils
        private void Log(object text)
        {
            Logger.LogDebug("[MothDiversity] " + text);
        }
        
        public static string stroin(string input)
        {
            return string.Join("", input.ToUpper().ToCharArray());
        }

        public static string soogify(string input)
        {
            // be sure to stroin before soog
            if (input == "WHITE")
                return "SURVIVOR";
            if (input == "YELLOW")
                return "MONK";
            if (input == "RED")
                return "HUNTER";
            if (input == "NIGHT")
                return "WATCHER";

            return input;
        }

        public static bool isBetween(int input, int min, int max)
        {
            return (min < max ? input > min && input < max : input > max && input < min);
        }

        public static float floatConstr(float input, float lowest, float highest)
        {
            return Math.Min(Math.Max(input, lowest), highest);
        }

        public static int intConstr(int input, int lowest, int highest)
        {
            return Math.Min(Math.Max(input, lowest), highest);
        }
    }

    public class MothDiversityConfig : OptionInterface
    {
        public static MothDiversityConfig Instance { get; } = new MothDiversityConfig();

        public static void RegisterOI()
        {
            if (MachineConnector.GetRegisteredOI(Plugin.MOD_ID) != Instance)
                MachineConnector.SetRegisteredOI(Plugin.MOD_ID, Instance);
        }

        // All the configurables

        //   Page 1
        public static Configurable<bool> altScugSpecific = Instance.config.Bind("altScugSpecific", false,
            new ConfigurableInfo("Sets the Slugcat-Specific Creatures inputs to be an alternate set. Default false.")
        );
        // Menus and stuff
        public override void Initialize()
        {
            Plugin.Log(LogLevel.Info, "[MothDiversity] Attempting to initialize Intirator's config...");//!
            base.Initialize();
            Tabs = [
                new OpTab(this, "Main Page")
            ];

            Tabs[0].AddItems([
                new OpLabel(30f, 560f, "MothDiversity Config - Main Page", true),
                new OpLabel(20f, 500f, "Info", true),
                new OpLabel(10f, 470f, "MothDiversity is a mod that aaa"),
            ]);
            Plugin.Log(LogLevel.Info, "[MothDiversity] MothDiversity's config successfully initialized!");
        }
    }
}