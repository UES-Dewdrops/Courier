using System;

namespace HenryMod.Modules
{
    internal static class StaticValues
    {
        internal static string descriptionText = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

        internal const float swordDamageCoefficient = 2.8f;

        internal const float gunDamageCoefficient = 4.2f;

        internal const float bombDamageCoefficient = 16f;

        // Rename stuff here
        internal const string prefabBodyName = "Henry";
        internal const string MODUID = "com.rob.HenryMod";
        internal const string MODNAME = "HenryMod";
        internal const string MODVERSION = "1.0.0";
        internal const string DEVELOPER_PREFIX = "ROB";
        internal const string assetbundleName = "myassetbundle";
        internal const string soundBnkName = "HenryBank.bnk";
        internal const string characterBodyPrefix = "_HENRY_BODY_";
    }
}