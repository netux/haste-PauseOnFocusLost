using Landfall.Haste;
using Landfall.Modding;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Core;
using Zorro.Settings;

namespace HastePauseOnUnfocusMod;

[LandfallPlugin]
public class PauseOnFocusLost : MonoBehaviour
{
    private PauseOnFocusLostSetting pauseOnFocusLostSetting;
    private UnpauseOnRegainFocusSetting? unpauseOnRegainFocusSetting;

    private bool LastPauseWasDueToFocusLoss = false;

    public void Start()
    {
        pauseOnFocusLostSetting = GameHandler.Instance.SettingsHandler.GetSetting<PauseOnFocusLostSetting>();
        unpauseOnRegainFocusSetting = GameHandler.Instance.SettingsHandler.GetSetting<UnpauseOnRegainFocusSetting>();
    }

    public void Update()
    {
        if (Singleton<EscapeMenu>.Instance == null)
        {
            return;
        }

        if (!Application.isFocused)
        {
            // Application lost focus
            if (
                pauseOnFocusLostSetting.Value == OffOnMode.ON &&
                !EscapeMenu.IsOpen
            )
            {
                Singleton<EscapeMenu>.Instance.ForceOpen();
                LastPauseWasDueToFocusLoss = true;
            }
        }
        else
        {
            // Application has focus
            if (
                unpauseOnRegainFocusSetting?.Value == OffOnMode.ON &&
                LastPauseWasDueToFocusLoss &&
                EscapeMenu.IsOpen
            )
            {
                Singleton<EscapeMenu>.Instance.Close();
            }
        }

        // Clear LastPauseWasDueToFocusLoss
        if (!EscapeMenu.IsOpen && LastPauseWasDueToFocusLoss)
        {
            LastPauseWasDueToFocusLoss = false;
        }
    }

    static PauseOnFocusLost()
    {
        On.GameHandler.Start += (original, self) =>
        {
            original(self);

            self.gameObject.AddComponent<PauseOnFocusLost>();
        };
    }
}

[HasteSetting]
public class PauseOnFocusLostSetting : OffOnSetting, IExposedSetting
{
    public override void ApplyValue() { /* no-op */ }

    public override List<LocalizedString> GetLocalizedChoices() => [
        // yoink
        new("Settings", "DisabledGraphicOption"),
        new("Settings", "EnabledGraphicOption")
    ];

    protected override OffOnMode GetDefaultValue() => OffOnMode.ON;

    public string GetCategory() => SettingCategory.General;

    public LocalizedString GetDisplayName() => new UnlocalizedString("Pause on focus lost");
}

[HasteSetting]
public class UnpauseOnRegainFocusSetting : OffOnSetting, IExposedSetting
{
    public override void ApplyValue() { /* no-op */ }

    public override List<LocalizedString> GetLocalizedChoices() => [
        // yoink
        new("Settings", "DisabledGraphicOption"),
        new("Settings", "EnabledGraphicOption")
    ];

    protected override OffOnMode GetDefaultValue() => OffOnMode.OFF;

    public string GetCategory() => SettingCategory.General;

    public LocalizedString GetDisplayName() => new UnlocalizedString("Unpause on regain focus");
}