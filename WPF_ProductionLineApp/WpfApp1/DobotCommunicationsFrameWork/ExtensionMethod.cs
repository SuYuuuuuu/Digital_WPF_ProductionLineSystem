using System.Collections.Generic;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork

{
    internal static class ExtensionMethod
    {
        public static T Get<T>(this Dictionary<int, object> instance, int id)
        {
            return (T)instance[id];
        }
    }
}
