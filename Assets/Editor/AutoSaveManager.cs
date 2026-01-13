// Place into: Assets/Editor/AutoSaveManager.cs
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSaveManager
{
    // EditorPrefs keys
    const string KEY_ENABLED = "AS_AutoSave_Enabled";
    const string KEY_INTERVAL = "AS_AutoSave_IntervalSeconds";
    const string KEY_ONLY_IF_DIRTY = "AS_AutoSave_OnlyIfDirty";
    const string KEY_PAUSE_WHEN_PLAYING = "AS_AutoSave_PauseWhenPlaying";
    const string KEY_LAST_SAVE = "AS_AutoSave_LastSaveTicks";

    static double nextTime = 0;

    static AutoSaveManager()
    {
        // Подпишемся на update редактора
        EditorApplication.update += Update;
        // и инициализируем nextTime так, чтобы ждать интервал после загрузки (если включено)
        if (IsEnabled())
            ScheduleNext();
    }

    static void Update()
    {
        if (!IsEnabled())
            return;

        if (IsPausedWhenPlaying() && EditorApplication.isPlaying)
            return;

        double now = EditorApplication.timeSinceStartup;
        if (now >= nextTime)
        {
            PerformSave();
            ScheduleNext();
        }
    }

    static void ScheduleNext()
    {
        double interval = Math.Max(1.0, GetIntervalSeconds());
        nextTime = EditorApplication.timeSinceStartup + interval;
    }

    static void PerformSave()
    {
        try
        {
            bool onlyIfDirty = GetOnlyIfDirty();

            // Сохраняем открытые сцены (EditorSceneManager.SaveOpenScenes сохраняет только изменённые сцены)
            if (!onlyIfDirty)
            {
                // Если пользователь хочет принудительно записать сцену даже если не "dirty" —
                // стандартные API не имеют "SaveEvenIfNotDirty", но SaveOpenScenes безопасно и недеградирует данные.
                EditorSceneManager.SaveOpenScenes();
            }
            else
            {
                // Только если есть изменённые сцены
                EditorSceneManager.SaveOpenScenes();
            }

            // Сохраняем все ассеты / проект
            AssetDatabase.SaveAssets();

            // Запомнить время
            EditorPrefs.SetString(KEY_LAST_SAVE, DateTime.UtcNow.Ticks.ToString());

            Debug.Log($"[AutoSave] Сохранено сцены и ассеты ({DateTime.Now:yyyy-MM-dd HH:mm:ss})");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AutoSave] Ошибка при сохранении: {ex}");
        }
    }

    #region EditorPrefs accessors

    public static bool IsEnabled() => EditorPrefs.GetBool(KEY_ENABLED, false);
    public static void SetEnabled(bool v) => EditorPrefs.SetBool(KEY_ENABLED, v);

    public static double GetIntervalSeconds() => EditorPrefs.GetFloat(KEY_INTERVAL, 120f); // по умолчанию 120 сек
    public static void SetIntervalSeconds(float s) => EditorPrefs.SetFloat(KEY_INTERVAL, Mathf.Max(1f, s));

    public static bool GetOnlyIfDirty() => EditorPrefs.GetBool(KEY_ONLY_IF_DIRTY, true);
    public static void SetOnlyIfDirty(bool v) => EditorPrefs.SetBool(KEY_ONLY_IF_DIRTY, v);

    public static bool IsPausedWhenPlaying() => EditorPrefs.GetBool(KEY_PAUSE_WHEN_PLAYING, true);
    public static void SetPausedWhenPlaying(bool v) => EditorPrefs.SetBool(KEY_PAUSE_WHEN_PLAYING, v);

    public static DateTime? GetLastSaveTimeUTC()
    {
        if (!EditorPrefs.HasKey(KEY_LAST_SAVE)) return null;
        if (long.TryParse(EditorPrefs.GetString(KEY_LAST_SAVE, "0"), out long ticks) && ticks > 0)
            return new DateTime(ticks, DateTimeKind.Utc);
        return null;
    }

    #endregion

    // Для удобства — публичные команды, чтобы окно могло вызвать
    public static void SaveNow() => PerformSave();
    public static void ResetTimer() => ScheduleNext();
}
