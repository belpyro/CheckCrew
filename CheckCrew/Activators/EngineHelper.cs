using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckCrew.Activators
{
    public static class EngineHelper
    {
        public static void Activate(this ModuleEngines module)
        {
            module.enabled = true;
            module.Events["Activate"].active = true;
            module.Events["Activate"].guiActive = true;
        }

        public static void Deactivate(this ModuleEngines module)
        {
            module.Shutdown();
            module.enabled = false;
            module.Events["Activate"].active = false;
            module.Events["Activate"].guiActive = false;
        }
    }
}
