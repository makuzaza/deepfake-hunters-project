// AudioManager.cs  -  Assets/_Project/Scripts/Managers
// STUB. Day 2 (Dev A): UI SFX, ambient office loop, notification stings.
using UnityEngine;
public class AudioManager : Singleton<AudioManager>
{
    private void OnEnable()  => GameEvents.TaskLaunched += OnLaunch;
    private void OnDisable() => GameEvents.TaskLaunched -= OnLaunch;
    private void OnLaunch(TaskSO task) { /* TODO Day 2: play notification sting. */ }
    public void PlayNotification() { /* TODO Day 2: SFX. */ }
}
