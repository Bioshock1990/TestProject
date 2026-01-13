// Place into: Assets/Editor/AutoSaveWindow.cs
using UnityEditor;
using UnityEngine;
using System;

public class AutoSaveWindow : EditorWindow
{
    float intervalSeconds;
    bool enabledFlag;
    bool onlyIfDirty;
    bool pauseWhenPlaying;

    [MenuItem("Tools/Auto Save Settings")]
    public static void OpenWindow()
    {
        var win = GetWindow<AutoSaveWindow>("Auto Save");
        win.minSize = new Vector2(320, 160);
    }

    void OnEnable()
    {
        // Загружаем из EditorPrefs
        enabledFlag = AutoSaveManager.IsEnabled();
        intervalSeconds = (float)AutoSaveManager.GetIntervalSeconds();
        onlyIfDirty = AutoSaveManager.GetOnlyIfDirty();
        pauseWhenPlaying = AutoSaveManager.IsPausedWhenPlaying();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Автосохранение проекта (Editor)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Вкл/Выкл
        bool newEnabled = EditorGUILayout.Toggle("Enabled", enabledFlag);
        if (newEnabled != enabledFlag)
        {
            enabledFlag = newEnabled;
            AutoSaveManager.SetEnabled(enabledFlag);
            if (enabledFlag)
                AutoSaveManager.ResetTimer();
        }

        // Интервал
        float newInterval = EditorGUILayout.FloatField("Интервал (сек)", intervalSeconds);
        if (newInterval != intervalSeconds)
        {
            intervalSeconds = Mathf.Max(1f, newInterval);
            AutoSaveManager.SetIntervalSeconds(intervalSeconds);
            // пересчитать таймер
            AutoSaveManager.ResetTimer();
        }

        onlyIfDirty = EditorGUILayout.Toggle(new GUIContent("Сохранять только если есть изменения", "Если включено — сцены сохраняются только когда они 'dirty'."), onlyIfDirty);
        AutoSaveManager.SetOnlyIfDirty(onlyIfDirty);

        pauseWhenPlaying = EditorGUILayout.Toggle(new GUIContent("Пауза в Play Mode", "Отключать автосохранение во время проигрывания в редакторе."), pauseWhenPlaying);
        AutoSaveManager.SetPausedWhenPlaying(pauseWhenPlaying);

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Сохранить сейчас"))
        {
            AutoSaveManager.SaveNow();
        }
        if (GUILayout.Button("Сброс таймера"))
        {
            AutoSaveManager.ResetTimer();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Покажем последнее сохранение
        var lastUtc = AutoSaveManager.GetLastSaveTimeUTC();
        string lastText = lastUtc.HasValue ? lastUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : "—";
        EditorGUILayout.LabelField("Последнее сохранение:", lastText);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Сцен и ассетов сохраняются через API редактора (без имитации нажатия клавиш). По умолчанию интервал 120 сек, сохранение сцен выполняется обычным вызовом SaveOpenScenes().", MessageType.Info);
    }
}
