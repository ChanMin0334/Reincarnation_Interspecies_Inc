using DG.Tweening.Plugins;
using System.Drawing;
using UnityEngine;

public static class BackGroundData
{
    #region Path
    public static readonly string[] PlanePath = 
    {
        "BGs/Sky",
        "BGs/Clouds",
        "BGs/RockMountains",
        "BGs/Grass Mountains",
        "BGs/Void"
    };

    public static readonly string[]  Cave01Path =
    {
        "BGs/cave_0",
        "BGs/cave_1",
        "BGs/cave_2",
        "BGs/cave_3",
        "BGs/Void"
    };

    public static readonly string[] Cave02Path =
    {
        "BGs/cave_0",
        "BGs/cave_1",
        "BGs/cave_2",
        "BGs/cave_4",
        "BGs/Void"
    };

    public static readonly string[] OakForestPath =
    {
        "BGs/Forest_0",
        "BGs/Forest_1",
        "BGs/Forest_2",
        "BGs/Void",
        "BGs/Void"
    };

    public static readonly string[] Cemetary01Path =
    {
        "BGs/Cemetary_0",
        "BGs/Cemetary_1",
        "BGs/Cemetary_2",
        "BGs/Void",
        "BGs/Cemetary_4"
    };

    public static readonly string[] Cemetary02Path =
    {
        "BGs/Cemetary_0",
        "BGs/Cemetary_1",
        "BGs/Void",
        "BGs/Cemetary_3",
        "BGs/Void"
    };


    public static readonly string[] StellarPath =
    {
        "BGs/Stellar_0",
        "BGs/Stellar_1",
        "BGs/Stellar_2",
        "BGs/Void",
        "BGs/Void"
    };
    #endregion
    #region speeds

    public static readonly float[] PlaneConstants =
    {
        1f, 0.8f, 0.6f, 0.4f, 1f
    };

    public static readonly float[] CaveConstants =
    {
        0.95f, 0.95f, 0.95f, 0.4f, 1f
    };

    public static readonly float[] ForestConstants =
    {
        1f, 0.8f, 0.6f, 1f, 1f
    };

    public static readonly float[] Cemetary01Constants =
    {
        1f, 0.8f, 0.6f, 1f, 0.2f
    };

    public static readonly float[] Cemetary02Constants =
    {
        1f, 0.8f, 1f, 0.4f, 1f
    };

    public static readonly float[] StellarConstants =
    {
        1f, 0.8f, 0.6f, 1f, 1f
    };
    #endregion
    #region amplifiers
    public static readonly float PlaneAmplifier = 1f;

    public static readonly float CaveAmplifier = 0.67f;

    public static readonly float ForestAmplifier = 1.78f;

    public static readonly float CemetaryAmplifier = 1f;

    public static readonly float StellarAmplifier = 1.78f;
    #endregion
}