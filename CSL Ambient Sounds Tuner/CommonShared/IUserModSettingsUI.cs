using ICities;

namespace AmbientSoundsTuner.CommonShared
{
    /// <summary>
    /// An interface that contains the methods for a settings UI.
    /// </summary>
    public interface IUserModSettingsUI
    {
        /// <summary>
        /// Called when the settings UI needs to be shown.
        /// </summary>
        /// <param name="helper">The UI helper object.</param>
        void OnSettingsUI(UIHelperBase helper);
    }
}
