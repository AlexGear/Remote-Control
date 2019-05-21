using Remote_Control.Resources;

namespace Remote_Control {
    /// <summary>
    /// Предоставляет доступ к строковым ресурсам.
    /// </summary>
    public class LocalizedStrings {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}