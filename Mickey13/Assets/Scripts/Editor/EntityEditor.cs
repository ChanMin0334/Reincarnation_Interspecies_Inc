using UnityEngine;
using UnityEditor;

[ExecuteAlways]
[CustomEditor(typeof(Character))]

public class EntityEditor : Editor
{
    private bool showStats = true;
    public override void OnInspectorGUI()
    {
        Debug.Log("EntityEditor Active!");

        base.OnInspectorGUI();

        var entity = (Entity)target;
        if (entity == null || entity.Data == null)
            return;

        EditorGUILayout.Space(10);
        showStats = EditorGUILayout.Foldout(showStats, "Runtime Stat Layers (Not Serialized)", true);
        if (!showStats) return;

        EditorGUI.indentLevel++;
        DrawStatModel("Base (from SO)", entity.BaseFromSO);
        DrawStatModel("Final (Result)", entity.FinalStat);

        if (entity.Data.MetaPermanent != null)
            DrawStatModel("MetaPermanent", entity.Data.MetaPermanent);
        if (entity.Data.RunProgress != null)
            DrawStatModel("RunProgress", entity.Data.RunProgress);
        if (entity.Data.Rune != null)
            DrawStatModel("Rune", entity.Data.Rune);
        if (entity.Data.Artifacts != null)
            DrawStatModel("Artifacts", entity.Data.Artifacts);
        if (entity.Data.Buffs != null)
            DrawStatModel("Buffs", entity.Data.Buffs);

        EditorGUI.indentLevel--;
    }

    private void DrawStatModel(string label, StatModel stat)
    {
        if (stat == null) return;

        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LabelField("HP", stat.HP.value.ToString());
        EditorGUILayout.LabelField("ATK", stat.Atk.value.ToString());
        EditorGUILayout.FloatField("ATK_CoolDown", stat.AtkCooldown);
        EditorGUILayout.FloatField("Skill_CoolDown", stat.SkillCooldown);
        EditorGUILayout.FloatField("CRIT", stat.CritChance);
        EditorGUILayout.FloatField("CRIT DMG", stat.CritMult);
        EditorGUILayout.FloatField("SPD", stat.MoveSpeed);
        EditorGUILayout.FloatField("ATK RANGE", stat.AtkRange);
        EditorGUILayout.FloatField("ATK Area", stat.AtkArea);
        EditorGUILayout.FloatField("DamageDealMult", stat.DamageDealMult);
        EditorGUILayout.FloatField("DamageTakenMult", stat.DamageTakenMult);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(5);
    }
}

