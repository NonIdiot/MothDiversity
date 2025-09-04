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
using Menu.Remix;
using Menu.Remix.MixedUI.ValueTypes;
using On.Watcher;
using SBCameraScroll;

using Logger = UnityEngine.Logger;

// ReSharper disable SimplifyLinqExpressionUseAll

// ReSharper disable UseMethodAny.0

// ReSharper disable once CheckNamespace
namespace MothDiversity
{

    [BepInPlugin(MOD_ID, "MothDiversity", "1.0.1")]
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
                    On.RainWorld.OnModsInit += RainWorldOnModsInitHook;
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
            if (colors.Length == 4)
            {
                return [
                    colors[0],colors[0],
                    colors[1],colors[1],
                    colors[2],colors[2],
                    colors[3],colors[3],
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
        
        public static Color[][] myColors = [
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
            // gay people, gay people, gay people, gay people, oo oo o ooo ooo ooo ooo ooooo
            flagFromColors([
                new Color(0,20,15),new Color(15,25,20),
                new Color(25,25,25),new Color(12,15,25),
                new Color(5,5,20),
            ]),
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
            // bicycle
            flagFromColors([
                new Color(20,0,10),new Color(15,8,15),
                new Color(0,5,15),
            ]),
            // ace shrexual
            flagFromColors([
                new Color(0,0,0),new Color(15,15,15),
                new Color(25,25,25),new Color(12,0,12),
            ]),
            // arrow
            flagFromColors([
                new Color(5,15,5),new Color(15,20,15),
                new Color(25,25,25),new Color(15,15,15),
                new Color(0,0,0),
            ]),
            // sun set when sun rise walk in
            flagFromColors([
                new Color(25,15,0),new Color(25,20,0),
                new Color(25,25,25),new Color(5,20,25),
                new Color(0,5,10),
            ]),
            // at what temperature does it become a gendersolid though
            flagFromColors([
                new Color(25,10,15),new Color(25,25,25),
                new Color(20,0,20),new Color(0,0,0),
                new Color(5,5,20),
            ]),
            // splinter WHAT???
            flagFromColors([
                new Color(25,20,0),new Color(10,0,15),
                new Color(25,20,0),
            ]),
            // lemonade iced tea if it was a sexuality i think
            flagFromColors([
                new Color(0,0,0),new Color(25,25,25),
                new Color(10,0,10),new Color(20,20,20),
            ]),
            // demidude
            flagFromColors([
                new Color(10,10,10),new Color(25,15,15),
                new Color(25,25,25),new Color(25,15,15),
                new Color(10,10,10),
            ]),
            // demidude (but male) (see the joke is that i use the term dude gender-neutrally)
            flagFromColors([
                new Color(10,10,10),new Color(10,20,25),
                new Color(25,25,25),new Color(10,20,25),
                new Color(10,10,10),
            ]),
            // you ever wake up and want to be THE CREATURE? cause i do
            flagFromColors([
                new Color(0,10,0),new Color(25,25,25),
                new Color(8,0,12),
            ]),
            // GAMBLINGGGGG
            flagFromColors([
                new Color(25,25,0),new Color(25,0,10),
                new Color(0,10,0),new Color(25,0,10),
                new Color(25,25,0),
            ]),
        ];
        public static string[] myColorNames = [
            "Pride Flag",
            "Gay Flag",
            "Trans Flag",
            "Pan Flag",
            "Lesbian Flag",
            "Bi Flag",
            "Ace Flag",
            "Aromantic Flag",
            "Sunset Flag",
            "Genderfluid Flag",
            "Inter Flag",
            "Demi Flag",
            "Demigirl Flag",
            "Demiboy Flag",
            "Alterhuman Flag",
            "Gambling Flag",
        ];

        public static bool shouldBeGay(int myOption)
        {
            return myOption > 0 && (myOption == 3 || (System.DateTime.Today.Month == 6 && (myOption == 2 || System.DateTime.Today.Day == 1)));
        }

        public static bool onlyChaos = false;
        public static float BODY_UNCHANGED_PROBABILITY = (onlyChaos ? 0 : 0.29f);
        public static float BODY_SLIGHTLY_CHANGED_PROBABILITY = (onlyChaos ? 0 : 0.40f);
        public static float BODY_ROSY_MAPLE_PROBABILITY = (onlyChaos ? 0 : 0.29f);
        public static float BODY_RAINBOW_PROBABILITY = (onlyChaos ? 0 : 0.14f);
        public static float BODY_CHAOS_PROBABILITY = (onlyChaos ? 999f : 0.35f);

        public static bool onlyGay = false;
        public static float WING_PATTERN_GAY_PROBABILITY = (onlyGay ? 999f : 0.05f);
        public static float WING_PATTERN_GRADIENT_PROBABILITY = 0.4f;
        public static float WING_PATTERN_STRIPES_PROBABILITY = 0.15f;
        public static float WING_PATTERN_STRIPES_ONE_PROBABILITY = 0.10f;
        public static float WING_PATTERN_STRIPED_GRADIENT_PROBABILITY = 0.5f;
        public static float WING_PATTERN_STRIPED_ONE_GRADIENT_PROBABILITY = 0.10f;
        public static float WING_PATTERN_BACKWARDS_GRADIENT_PROBABILITY = 0.05f;
        private void WingDrawSprites(BigMothGraphics.WingGraphics.orig_DrawSprites orig, Watcher.BigMothGraphics.WingGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);

            string[] myStuffs = [
                "None",
                "During Certain Days",
                "During Pride Month",
                "Always",
            ];
            int whereIs = -1;
            for (int i=0;i< myStuffs.Length;i++)
            {
                if (stroin(MothDiversityConfig.prideMothSetting.Value) == stroin(myStuffs[i]))
                {
                    whereIs = i;
                }
            }

            if (shouldBeGay(whereIs))
            {
                onlyGay = true;
            }
            else
            {
                //Log(LogLevel.Info, "plue says grr "+whereIs+" "+MothDiversityConfig.prideMothSetting.Value);
                Plugin.onlyGay = false;
            }

            int num = (self.inFront ? self.owner.FirstInFrontWingSprite : self.owner.FirstBehindWingSprite) + self.firstSpriteOnLayer;
            float num2 = (float)((self.index == 1) ? -1 : 1);
            float a = 0.5f + self.spreadOutness * num2 * -1f * 0.5f;
            UnityEngine.Random.State state = UnityEngine.Random.state;
            if (!self.owner.shadowMode)
            {
                UnityEngine.Random.InitState(self.owner.bug.abstractCreature.ID.RandomSeed);
                float bodyPatternColor = UnityEngine.Random.value + 0;
                float wingPatternColor = UnityEngine.Random.value + 0;
                float eyePatternColor = UnityEngine.Random.value + 0;
                //float antennaPatternColor = UnityEngine.Random.value + 0;
                float personality = UnityEngine.Random.value + 0;
                bool isInvinTime = self.owner.owner.room != null && self.owner.owner.room.game.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel;

                if (bodyPatternColor > BODY_UNCHANGED_PROBABILITY || isInvinTime)
                {
                    Logger.Log(LogLevel.Info, "old color was" + self.owner.bug.iVars.bodyColor);
                    if (bodyPatternColor < BODY_UNCHANGED_PROBABILITY + BODY_SLIGHTLY_CHANGED_PROBABILITY)
                    {
                        float warmth = UnityEngine.Random.value-0.5f;
                        float secondaryOffset = UnityEngine.Random.value-0.5f;
                        self.owner.bug.iVars.bodyColor = new Color(0.75f+Mathf.Clamp01(warmth)/2, 0.75f+Mathf.Clamp01(warmth)/4,0.75f+Mathf.Clamp01(-warmth)/2);
                        self.owner.bug.iVars.secondaryColor = new Color(self.owner.bug.iVars.bodyColor.r+secondaryOffset/4f,self.owner.bug.iVars.bodyColor.g+secondaryOffset/4f,self.owner.bug.iVars.bodyColor.b+secondaryOffset/4f);
                    }
                    else if (bodyPatternColor < BODY_UNCHANGED_PROBABILITY + BODY_SLIGHTLY_CHANGED_PROBABILITY + BODY_ROSY_MAPLE_PROBABILITY)
                    {
                        float warmth = UnityEngine.Random.value-0.25f;
                        float warmth2 = UnityEngine.Random.value;
                        self.owner.bug.iVars.bodyColor = new Color(0.75f+Mathf.Abs(warmth/3f), (warmth >= 0.5f ? 0.75f+warmth/3f : 0.5f), 0.5f+Mathf.Clamp01(-warmth/3f));
                        self.owner.bug.iVars.secondaryColor = (self.owner.bug.iVars.bodyColor.g > 0.51f ? new Color(1, warmth2, warmth2) : (warmth2 < 0.5 ? new Color(1, warmth2*2, 0) : new Color(1, warmth2, Mathf.Clamp01(warmth2-0.5f)*2f)));
                        //Logger.LogInfo("o "+self.owner.bug.iVars.bodyColor.r+" "+self.owner.bug.iVars.bodyColor.g+" "+self.owner.bug.iVars.bodyColor.b);
                    }
                    else if (bodyPatternColor < BODY_UNCHANGED_PROBABILITY + BODY_SLIGHTLY_CHANGED_PROBABILITY + BODY_ROSY_MAPLE_PROBABILITY + BODY_RAINBOW_PROBABILITY)
                    {
                        float warmth = UnityEngine.Random.value;
                        float warmth2 = (UnityEngine.Random.value-0.5f)/4f;
                        float warmth3 = (UnityEngine.Random.value-0.5f)/4f;
                        float warmth4 = (UnityEngine.Random.value-0.5f)/4f;
                        self.owner.bug.iVars.bodyColor = Color.HSVToRGB(warmth, 0.5f+warmth3, 0.5f+warmth3);
                        self.owner.bug.iVars.secondaryColor = Color.HSVToRGB(warmth+warmth2, 0.5f+warmth4, 0.5f+warmth4);
                    }
                    else if (bodyPatternColor < BODY_UNCHANGED_PROBABILITY + BODY_SLIGHTLY_CHANGED_PROBABILITY + BODY_ROSY_MAPLE_PROBABILITY + BODY_RAINBOW_PROBABILITY + BODY_CHAOS_PROBABILITY)
                    {
                        float warmth = UnityEngine.Random.value;
                        float warmth2 = UnityEngine.Random.value;
                        float warmth3 = UnityEngine.Random.value;
                        float warmth4 = (UnityEngine.Random.value-0.5f)/2f;
                        float warmth5 = (UnityEngine.Random.value-0.5f)/2f;
                        float warmth6 = (UnityEngine.Random.value-0.5f)/2f;
                        self.owner.bug.iVars.bodyColor = new Color(warmth, warmth2, warmth3);
                        self.owner.bug.iVars.secondaryColor = new Color(warmth+warmth4, warmth2+warmth5, warmth3+warmth6);
                    }
                    if (isInvinTime)
                    {
                        self.owner.bug.iVars.bodyColor = new Color(0f,0f,0f);
                        self.owner.bug.iVars.secondaryColor = new Color(1f,0f,0f);
                    }
                    self.owner.bodyColor = self.owner.bug.iVars.bodyColor;
                    sLeaser.sprites[self.owner.HeadMesh].color = self.owner.bodyColor;
                    for (int antennae = 0; antennae < 2; ++antennae)
                    {
                        sLeaser.sprites[self.owner.AntennaSprites(antennae)].color = Color.Lerp(self.owner.bodyColor, self.owner.palette.blackColor, 0.1f);
                        sLeaser.sprites[self.owner.AntennaSprites(antennae) + 1].color = self.owner.bodyColor;
                    }
                    self.owner.tongueBaseColor = Color.Lerp(Color.Lerp(Color.red, self.owner.bodyColor, 0.7f), self.owner.palette.blackColor, 0.15f);
                    self.owner.tongueTipColor = Color.Lerp(self.owner.tongueBaseColor, self.owner.palette.blackColor, 0.44f);
                }
                
                if (onlyGay || wingPatternColor < WING_PATTERN_GAY_PROBABILITY)
                {
                    int whichColShouldBe = (int)Math.Floor(Math.Max(0,UnityEngine.Random.value * (myColors.Length + 1) - 1));
                    //Logger.Log(LogLevel.Info, "g" + whichColShouldBe);
                    for (int i = 0; i < self.segments.GetLength(0); i++)
                    {
                        //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = self.SetColorAlpha(self.owner.BodyColorWithDepth(depth, num6 * self.spreadOutness * num2), a);
                        //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = new Color(0,0,0,a);
                        (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = new Color(myColors[whichColShouldBe][i].r/25,myColors[whichColShouldBe][i].g/25,myColors[whichColShouldBe][i].b/25, a);//[(int)Math.Floor(RXRandom.Int(0f,3f))]
                        //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2 + 1] = self.SetColorAlpha(myColors[whichColShouldBe][i], a);//[(int)Math.Floor(RXRandom.Int(0f,3f))]
                        //(sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2 + 1] = self.SetColorAlpha(self.owner.BodyColorWithDepth(depth, num6 * -self.spreadOutness * num2), a);
                        //Logger.Log(LogLevel.Info,i+" "+self.SetColorAlpha(myColors[whichColShouldBe][i], a).r+" "+self.SetColorAlpha(myColors[whichColShouldBe][i], a).g+" "+self.SetColorAlpha(myColors[whichColShouldBe][i], a).b);
                    }
                }
                else if (wingPatternColor < WING_PATTERN_GAY_PROBABILITY + WING_PATTERN_GRADIENT_PROBABILITY)
                {
                    for (int i = 0; i < self.segments.GetLength(0); i++)
                    {
                        Logger.Log(LogLevel.Info, "olg" + (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2]);
                        (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = Color.Lerp(self.owner.bodyColor,self.owner.bug.iVars.secondaryColor,(i/(float)self.segments.GetLength(0)));
                        Logger.Log(LogLevel.Info, "nue" + (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2]);
                    }
                }
                else if (wingPatternColor < WING_PATTERN_GAY_PROBABILITY + WING_PATTERN_GRADIENT_PROBABILITY + WING_PATTERN_STRIPES_PROBABILITY)
                {
                    bool stripedOrNot = false;
                    for (int i = 0; i < self.segments.GetLength(0); i++)
                    {
                        (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = (stripedOrNot ? self.owner.bodyColor : self.owner.bug.iVars.secondaryColor);
                        stripedOrNot = !stripedOrNot;
                    }
                }
                else if (wingPatternColor < WING_PATTERN_GAY_PROBABILITY + WING_PATTERN_GRADIENT_PROBABILITY + WING_PATTERN_STRIPES_PROBABILITY + WING_PATTERN_STRIPES_ONE_PROBABILITY)
                {
                    for (int i = 0; i < self.segments.GetLength(0); i++)
                    {
                        (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = (i > self.segments.GetLength(0)/3 && i < self.segments.GetLength(0)/3*2 ? self.owner.bodyColor : self.owner.bug.iVars.secondaryColor);
                    }
                }
                else if (wingPatternColor < WING_PATTERN_GAY_PROBABILITY + WING_PATTERN_GRADIENT_PROBABILITY + WING_PATTERN_STRIPES_PROBABILITY + WING_PATTERN_STRIPES_ONE_PROBABILITY + WING_PATTERN_STRIPED_GRADIENT_PROBABILITY)
                {
                    bool stripedOrNot = false;
                    for (int i = 0; i < self.segments.GetLength(0); i++)
                    {
                        (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = (stripedOrNot ? Color.Lerp(self.owner.bodyColor,self.owner.bug.iVars.secondaryColor,(i/(float)self.segments.GetLength(0))) : self.owner.bug.iVars.secondaryColor);
                        stripedOrNot = !stripedOrNot;
                    }
                }
                else if (wingPatternColor < WING_PATTERN_GAY_PROBABILITY + WING_PATTERN_GRADIENT_PROBABILITY + WING_PATTERN_STRIPES_PROBABILITY + WING_PATTERN_STRIPES_ONE_PROBABILITY + WING_PATTERN_STRIPED_GRADIENT_PROBABILITY + WING_PATTERN_STRIPED_ONE_GRADIENT_PROBABILITY)
                {
                    for (int i = 0; i < self.segments.GetLength(0); i++)
                    {
                        (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = (i > self.segments.GetLength(0)/3 && i < self.segments.GetLength(0)/3*2 ? Color.Lerp(self.owner.bodyColor,self.owner.bug.iVars.secondaryColor,(i/(float)self.segments.GetLength(0))) : self.owner.bug.iVars.secondaryColor);
                    }
                }
                else if (wingPatternColor < WING_PATTERN_GAY_PROBABILITY + WING_PATTERN_GRADIENT_PROBABILITY + WING_PATTERN_STRIPES_PROBABILITY + WING_PATTERN_STRIPES_ONE_PROBABILITY + WING_PATTERN_STRIPED_GRADIENT_PROBABILITY + WING_PATTERN_STRIPED_ONE_GRADIENT_PROBABILITY + WING_PATTERN_BACKWARDS_GRADIENT_PROBABILITY)
                {
                    bool stripedOrNot = false;
                    for (int i = 0; i < self.segments.GetLength(0); i++)
                    {
                        (sLeaser.sprites[num + 1] as TriangleMesh).verticeColors[i * 2] = Color.Lerp(self.owner.bodyColor,self.owner.bug.iVars.secondaryColor,(((float)self.segments.GetLength(0)-i)/(float)self.segments.GetLength(0)));
                        stripedOrNot = !stripedOrNot;
                    }
                }

                if (eyePatternColor > 0.75)
                {
                    float warmth = Mathf.Clamp01(eyePatternColor - 0.625f)*2f;
                    sLeaser.sprites[self.owner.EyeSprite(0)].color = new Color(self.owner.bodyColor.r*warmth,self.owner.bodyColor.g*warmth,self.owner.bodyColor.b*warmth);
                    sLeaser.sprites[self.owner.EyeSprite(1)].color = new Color(self.owner.bodyColor.r*warmth,self.owner.bodyColor.g*warmth,self.owner.bodyColor.b*warmth);
                }

                if (isInvinTime)
                {
                    sLeaser.sprites[self.owner.EyeSprite(0)].color = new Color(1,0,0);
                    sLeaser.sprites[self.owner.EyeSprite(1)].color = new Color(1,0,0);
                }

                /*
                if (antennaPatternColor > 0.5)
                {
                    int index1 = self.owner.AntennaSprites((double) self.owner.faceFlip < 0.0 ? self.index : 1 - self.index);
                    if (antennaPatternColor < 0.75)
                    {
                        float warmth = 1f-Mathf.Clamp01(antennaPatternColor - 0.5f)*2f;
                        for (int i = 0; i < self.owner.antennas.Length; i++)
                        {
                            for (int j = 0; j < self.owner.antennas[i].segments.GetLength(0); j++)
                            {
                                (sLeaser.sprites[index1 + 1] as TriangleMesh).verticeColors[j * 2] =  new Color(self.owner.bodyColor.r*warmth,self.owner.bodyColor.g*warmth,self.owner.bodyColor.b*warmth);
                            }
                        }
                    }
                    else
                    {
                        float warmth = Mathf.Clamp01(antennaPatternColor - 0.5f)*2f;
                        for (int i = 0; i < self.owner.antennas.Length; i++)
                        {
                            for (int j = 0; j < self.owner.antennas[i].segments.GetLength(0); j++)
                            {
                                (sLeaser.sprites[index1 + 1] as TriangleMesh).verticeColors[j * 2] =  new Color(self.owner.bodyColor.r*warmth+self.owner.bug.iVars.secondaryColor.r*(1-warmth),self.owner.bodyColor.g*warmth+self.owner.bug.iVars.secondaryColor.g*(1-warmth),self.owner.bodyColor.b*warmth+self.owner.bug.iVars.secondaryColor.b*(1-warmth));
                            }
                        }
                    }
                }
                */
                
                if (personality < 0.005 || (self.owner.bodyColor.g > 0.5f && Math.Abs(self.owner.bodyColor.r - self.owner.bodyColor.b) < 0.1f && self.owner.bodyColor.r - self.owner.bodyColor.b < 0.25f))
                {
                    self.owner.blinkCounter = 999;
                }
            }
            UnityEngine.Random.state = state;

            //self.owner.bodyColor = Color.blue;
        }
        
        private void RainWorldOnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                MachineConnector.SetRegisteredOI(Plugin.MOD_ID, MothDiversityConfig.Instance);
            }
            catch (Exception ex)
            {
                Log("[TextReplacementTool] EXCEPTION! "+ex.ToString());
            }
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

        public static Configurable<string> prideMothSetting = Instance.config.Bind("prideMothSetting", "During Pride Month",
            new ConfigurableInfo("The setting for the Pride Moth easter egg. Default During Pride Month.\nNote that no matter what this is set to, 5% of moths will still have random pride flags.")
        );
        public static Configurable<int> mainColorLenience = Instance.config.Bind("mainColorLenience", 10,
            new ConfigurableInfo("How far away a moth's color can be from the searched Main Color to be a match, measured in percent. Default 100.")
        );
        public static Configurable<Color> mainColor = Instance.config.Bind("mainColor", new Color(1,0,0),
            new ConfigurableInfo("The main color to search for. Default FF0000.")
        );
        public static Configurable<int> altColorLenience = Instance.config.Bind("altColorLenience", 100,
            new ConfigurableInfo("How far away a moth's color can be from the searched Alt Color to be a match, measured in percent. Default 100.")
        );
        public static Configurable<Color> altColor = Instance.config.Bind("altColor", new Color(1,0,0),
            new ConfigurableInfo("The alternate color to search for. Default FF0000.")
        );
        public static Configurable<int> searchStartID = Instance.config.Bind("searchStartID", 0,
            new ConfigurableInfo("What ID to start searching from. Default 0.")
        );
        public static Configurable<string> whichStripePattern = Instance.config.Bind("whichStripePattern", "Any",
            new ConfigurableInfo("The pattern to search for. Default Any.")
        );
        // Menus and stuff
        public OpComboBox prideMothSettingBox;
        public OpSlider mainColorLenienceSlider;
        public OpColorPicker mainColorPicker;
        public OpSlider altColorLenienceSlider;
        public OpColorPicker altColorPicker;
        public OpTextBox searchStartIDBox;
        public OpComboBox whichStripePatternBox;
        public OpLabel label1;
        public OpLabel label2;
        public string[] myStringies = ["100","FF0000","100","FF0000","0","Any","GLUE"];//During Certain Days
        public int whereSearched = -1;
        public int[] mothMatches = [];
        public override void Initialize()
        {
            Plugin.Log(LogLevel.Info, "[MothDiversity] Attempting to initialize MothDiversity's config...");//!
            base.Initialize();
            Tabs = [
                new OpTab(this, "Main Page")
            ];

            prideMothSettingBox = new OpComboBox(prideMothSetting, new Vector2(150f, 20f), 200f, new List<ListItem>
            {
                new ListItem("None", 0),
                new ListItem("During Certain Days", 1),
                new ListItem("During Pride Month", 2),
                new ListItem("Always", 3),
            });
            mainColorLenienceSlider = new OpSlider(mainColorLenience, new Vector2(10f, 350f),100) { max = 100, cosmetic = true };
            mainColorPicker = new OpColorPicker(mainColor, new(120f, 300f)) { value = mainColor.defaultValue, cosmetic = true };
            altColorLenienceSlider = new OpSlider(altColorLenience, new Vector2(290f, 350f),100) { max = 100, cosmetic = true };
            altColorPicker = new OpColorPicker(altColor, new(400f, 300f)) { value = altColor.defaultValue, cosmetic = true };
            searchStartIDBox = new OpTextBox(searchStartID, new Vector2(500f, 220f), 70) { cosmetic = true };
            List<ListItem> whichStripePatternList = new List<ListItem>
            {
                new ListItem("Any",0),
                new ("Patternless",1),
                new ("Any Flag",2),
                new ("Gradient",3),
                new ("Stripes",4),
                new ("Single Stripe",5),
                new ("Striped Gradient",6),
                new ("Single-Striped Gradient",7),
                new ("Backwards Gradient",8),
            };
            for (int i=0;i<Plugin.myColorNames.Length;i++)
            {
                whichStripePatternList.Add(new ListItem(Plugin.myColorNames[i], whichStripePatternList.Count));
            }
            whichStripePatternBox = new OpComboBox(whichStripePattern, new Vector2(150, 220f), 200f, whichStripePatternList) { cosmetic = true};
            label1 = new OpLabel(50f, 180f, "Calculating...") {verticalAlignment = OpLabel.LabelVAlignment.Top};
            label2 = new OpLabel(50f, 160f, " ") {verticalAlignment = OpLabel.LabelVAlignment.Top};
            Tabs[0].AddItems([
                new OpLabel(30f, 560f, "Moth Diversity Search", true),
                new OpLabel(10f, 500f, "This Remix menu allows you to search for moths with specific colors!"),
                new OpLabel(10f, 50f, "Misc. Settings:"),
                new OpLabel(30f, 20f, "Pride Moth Happens"),
                prideMothSettingBox,
                new OpLabel(10f, 400f, "Main Color Search"),
                mainColorLenienceSlider,
                mainColorPicker,
                new OpLabel(290f, 400f, "Alt Color Search"),
                altColorLenienceSlider,
                altColorPicker,
                new OpLabel(370f, 220f, "ID Search Start Point"),
                searchStartIDBox,
                new OpLabel(10f, 220f, "Wing Design"),
                whichStripePatternBox,
                label1,
                label2,
            ]);
            Plugin.Log(LogLevel.Info, "[MothDiversity] MothDiversity's config successfully initialized!");
        }

        public override void Update()
        {
            string[] myStuffs = [
                "None",
                "During Certain Days",
                "During Pride Month",
                "Always",
            ];
            int whereIs = -1;
            for (int i=0;i<myStuffs.Length;i++)
            {
                if (Plugin.stroin(prideMothSettingBox.value) == Plugin.stroin(myStuffs[i]))
                {
                    whereIs = i;
                }
            }

            if (Plugin.shouldBeGay(whereIs))
            {
                Plugin.onlyGay = true;
            }
            else if (myStuffs.IndexOf(prideMothSettingBox.value) != -1)
            {
                Plugin.onlyGay = false;
            }
            
            if (myStringies[0] != mainColorLenienceSlider.value.ToString() 
                || myStringies[1] != mainColorPicker.value.ToString()
                || myStringies[2] != altColorLenienceSlider.value.ToString()
                || myStringies[3] != altColorPicker.value.ToString()
                || myStringies[4] != searchStartIDBox.value.ToString()
                || myStringies[5] != whichStripePatternBox.value.ToString()
                || myStringies[6] != prideMothSettingBox.value)
            {
                myStringies[0] = mainColorLenienceSlider.value.ToString();
                myStringies[1] = mainColorPicker.value.ToString();
                myStringies[2] = altColorLenienceSlider.value.ToString();
                myStringies[3] = altColorPicker.value.ToString();
                myStringies[4] = searchStartIDBox.value.ToString();
                myStringies[5] = whichStripePatternBox.value.ToString();
                myStringies[6] = prideMothSettingBox.value.ToString();
                mothMatches = [];
                whereSearched = searchStartIDBox.valueInt - 1; //-1;
                Plugin.Log(LogLevel.Info, "[MothDiversity] Values changed. Restarting search!");
            }

            if (whereSearched < searchStartIDBox.valueInt+10000 && mothMatches.Length < 10)
            {
                label1.text = "Calculating... (at "+whereSearched+"/"+(searchStartIDBox.valueInt+10000)+", "+mothMatches.Length+"/10 matches)";
                UnityEngine.Random.State state = UnityEngine.Random.state;
                for (int i = 0; i < 20; i++)
                {
                    whereSearched++;
                    UnityEngine.Random.InitState(whereSearched);
                    Color[] whichColors = new Color[2];
                    Color b = Color.Lerp(new Color(0.5f, 0.25f, 0.1f), new Color(0.6f, 0.5f, 0.4f), 0.5f);
                    whichColors[0]=Color.Lerp(Color.Lerp(Color.white, b, Mathf.Pow(0.5f, 2f) * 0.17f), new Color(1f, 0.7f, 0.0f), (float) (((double) 0f - (double) Mathf.Pow(0.5f, 2f)) * 0.10000000149011612));
                    whichColors[1]=Color.Lerp(Color.Lerp(whichColors[0], new HSLColor(0.5f * 0.17f, 1f, 0.5f).rgb, 0.5f * 0.4f), Color.black, 0.3f);
                    int whichWing = 1;
                    float bodyPatternColor = UnityEngine.Random.value + 0;
                    float wingPatternColor = UnityEngine.Random.value + 0;
                    float eyePatternColor = UnityEngine.Random.value + 0;
                    //float antennaPatternColor = UnityEngine.Random.value + 0;
                    float personality = UnityEngine.Random.value + 0;

                    if (bodyPatternColor > Plugin.BODY_UNCHANGED_PROBABILITY)
                    {
                        if (bodyPatternColor < Plugin.BODY_UNCHANGED_PROBABILITY + Plugin.BODY_SLIGHTLY_CHANGED_PROBABILITY)
                        {
                            float warmth = UnityEngine.Random.value-0.5f;
                            float secondaryOffset = UnityEngine.Random.value-0.5f;
                            whichColors[0] = new Color(0.75f+Mathf.Clamp01(warmth)/2, 0.75f+Mathf.Clamp01(warmth)/4,0.75f+Mathf.Clamp01(-warmth)/2);
                            whichColors[1] = new Color(whichColors[0].r+secondaryOffset/4f,whichColors[0].g+secondaryOffset/4f,whichColors[0].b+secondaryOffset/4f);
                        }
                        else if (bodyPatternColor < Plugin.BODY_UNCHANGED_PROBABILITY + Plugin.BODY_SLIGHTLY_CHANGED_PROBABILITY + Plugin.BODY_ROSY_MAPLE_PROBABILITY)
                        {
                            float warmth = UnityEngine.Random.value-0.25f;
                            float warmth2 = UnityEngine.Random.value;
                            whichColors[0] = new Color(0.75f+Mathf.Abs(warmth/3f), (warmth >= 0.5f ? 0.75f+warmth/3f : 0.5f), 0.5f+Mathf.Clamp01(-warmth/3f));
                            whichColors[1] = (whichColors[0].g > 0.51f ? new Color(1, warmth2, warmth2) : (warmth2 < 0.5 ? new Color(1, warmth2*2, 0) : new Color(1, warmth2, Mathf.Clamp01(warmth2-0.5f)*2f)));
                        }
                        else if (bodyPatternColor < Plugin.BODY_UNCHANGED_PROBABILITY + Plugin.BODY_SLIGHTLY_CHANGED_PROBABILITY + Plugin.BODY_ROSY_MAPLE_PROBABILITY + Plugin.BODY_RAINBOW_PROBABILITY)
                        {
                            float warmth = UnityEngine.Random.value;
                            float warmth2 = (UnityEngine.Random.value-0.5f)/4f;
                            float warmth3 = (UnityEngine.Random.value-0.5f)/4f;
                            float warmth4 = (UnityEngine.Random.value-0.5f)/4f;
                            whichColors[0] = Color.HSVToRGB(warmth, 0.5f+warmth3, 0.5f+warmth3);
                            whichColors[1] = Color.HSVToRGB(warmth+warmth2, 0.5f+warmth4, 0.5f+warmth4);
                        }
                        else if (bodyPatternColor < Plugin.BODY_UNCHANGED_PROBABILITY + Plugin.BODY_SLIGHTLY_CHANGED_PROBABILITY + Plugin.BODY_ROSY_MAPLE_PROBABILITY + Plugin.BODY_RAINBOW_PROBABILITY + Plugin.BODY_CHAOS_PROBABILITY)
                        {
                            float warmth = UnityEngine.Random.value;
                            float warmth2 = UnityEngine.Random.value;
                            float warmth3 = UnityEngine.Random.value;
                            float warmth4 = (UnityEngine.Random.value-0.5f)/2f;
                            float warmth5 = (UnityEngine.Random.value-0.5f)/2f;
                            float warmth6 = (UnityEngine.Random.value-0.5f)/2f;
                            whichColors[0] = new Color(warmth, warmth2, warmth3);
                            whichColors[1] = new Color(warmth+warmth4, warmth2+warmth5, warmth3+warmth6);
                        }
                    }
                    
                    if (Plugin.onlyGay || wingPatternColor < Plugin.WING_PATTERN_GAY_PROBABILITY)
                    {
                        int whichColShouldBe = (int)Math.Floor(Math.Max(0,UnityEngine.Random.value * (Plugin.myColors.Length + 1) - 1));
                        whichWing = 9 + whichColShouldBe;
                    }
                    else if (wingPatternColor < Plugin.WING_PATTERN_GAY_PROBABILITY + Plugin.WING_PATTERN_GRADIENT_PROBABILITY)
                    {
                        whichWing = 3;
                    }
                    else if (wingPatternColor < Plugin.WING_PATTERN_GAY_PROBABILITY + Plugin.WING_PATTERN_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_STRIPES_PROBABILITY)
                    {
                        whichWing = 4;
                    }
                    else if (wingPatternColor < Plugin.WING_PATTERN_GAY_PROBABILITY + Plugin.WING_PATTERN_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_STRIPES_PROBABILITY + Plugin.WING_PATTERN_STRIPES_ONE_PROBABILITY)
                    {
                        whichWing = 5;
                    }
                    else if (wingPatternColor < Plugin.WING_PATTERN_GAY_PROBABILITY + Plugin.WING_PATTERN_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_STRIPES_PROBABILITY + Plugin.WING_PATTERN_STRIPES_ONE_PROBABILITY + Plugin.WING_PATTERN_STRIPED_GRADIENT_PROBABILITY)
                    {
                        whichWing = 6;
                    }
                    else if (wingPatternColor < Plugin.WING_PATTERN_GAY_PROBABILITY + Plugin.WING_PATTERN_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_STRIPES_PROBABILITY + Plugin.WING_PATTERN_STRIPES_ONE_PROBABILITY + Plugin.WING_PATTERN_STRIPED_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_STRIPED_ONE_GRADIENT_PROBABILITY)
                    {
                        whichWing = 7;
                    }
                    else if (wingPatternColor < Plugin.WING_PATTERN_GAY_PROBABILITY + Plugin.WING_PATTERN_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_STRIPES_PROBABILITY + Plugin.WING_PATTERN_STRIPES_ONE_PROBABILITY + Plugin.WING_PATTERN_STRIPED_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_STRIPED_ONE_GRADIENT_PROBABILITY + Plugin.WING_PATTERN_BACKWARDS_GRADIENT_PROBABILITY)
                    {
                        whichWing = 8;
                    }

                    int woag = -1;
                    for (int j = 0; j < whichStripePatternBox._itemList.Length; j++)
                    {
                        if (Plugin.stroin(whichStripePatternBox._itemList[j].name) == Plugin.stroin(whichStripePatternBox.value))
                        {
                            woag = whichStripePatternBox._itemList[j].value;
                            break;
                        }
                    }

                    if (i == 0)
                    {
                        //Plugin.Log(LogLevel.Info, "woag is " + woag+" / "+whichStripePatternBox.value);
                    }

                    bool boogie = woag != -1;
                    if (boogie)
                    {
                        if (woag == 2)
                        {
                            boogie = whichWing > 8;
                        }
                        else if (woag != 0)
                        {
                            boogie = whichWing == woag;
                        }
                        if (boogie)
                        {
                            if (Math.Abs(whichColors[0].r - mainColorPicker.valueColor.r) > mainColorLenienceSlider.GetValueInt() / 100f 
                                || Math.Abs(whichColors[0].g-mainColorPicker.valueColor.g)>mainColorLenienceSlider.GetValueInt()/100f 
                                || Math.Abs(whichColors[0].b-mainColorPicker.valueColor.b)>mainColorLenienceSlider.GetValueInt()/100f 
                                || Math.Abs(whichColors[1].r - altColorPicker.valueColor.r) > altColorLenienceSlider.GetValueInt() / 100f 
                                || Math.Abs(whichColors[1].g-altColorPicker.valueColor.g)>altColorLenienceSlider.GetValueInt()/100f 
                                || Math.Abs(whichColors[1].b-altColorPicker.valueColor.b)>altColorLenienceSlider.GetValueInt()/100f)
                            {
                                /*
                                Plugin.Log(LogLevel.Info, "aaaaa woog "+mainColorLenienceSlider.GetValueInt()+" "+altColorLenienceSlider.GetValueInt());
                                Plugin.Log(LogLevel.Info, Math.Abs(whichColors[0].r));
                                Plugin.Log(LogLevel.Info, Math.Abs(whichColors[0].g));
                                Plugin.Log(LogLevel.Info, Math.Abs(whichColors[0].b));
                                Plugin.Log(LogLevel.Info, Math.Abs(mainColorPicker.valueColor.r));
                                Plugin.Log(LogLevel.Info, Math.Abs(mainColorPicker.valueColor.g));
                                Plugin.Log(LogLevel.Info, Math.Abs(mainColorPicker.valueColor.b));
                                Plugin.Log(LogLevel.Info, Math.Abs(whichColors[0].r - mainColorPicker.valueColor.r));
                                Plugin.Log(LogLevel.Info, Math.Abs(whichColors[0].g - mainColorPicker.valueColor.g));
                                Plugin.Log(LogLevel.Info, Math.Abs(whichColors[0].b - mainColorPicker.valueColor.b));
                                //Plugin.Log(LogLevel.Info, Math.Abs(whichColors[1].r - altColorPicker.valueColor.r));
                                //Plugin.Log(LogLevel.Info, Math.Abs(whichColors[1].g - altColorPicker.valueColor.g));
                                //Plugin.Log(LogLevel.Info, Math.Abs(whichColors[1].b - altColorPicker.valueColor.b));*/
                                boogie = false;
                            }
                            else
                            {
                                mothMatches = mothMatches.AddItem(whereSearched).ToArray();
                            }
                        }
                    }
                }
                UnityEngine.Random.state = state;
            }
            else
            {
                label1.text = "Search complete!";
            }

            if (mothMatches.Length == 0)
            {
                label2.text = (label1.text == "Search complete!" ? "No Moths found in this range." : "No Moths found yet." );
            }
            else
            {
                label2.text = "Moths found:";
                foreach (int i in mothMatches)
                {
                    label2.text += "\nMoth ID "+i;
                }
            }
            label1.Update();
            label1.GrafUpdate(0);
            label2.Update();
            label2.GrafUpdate(0);
            base.Update();
        }
    }
}