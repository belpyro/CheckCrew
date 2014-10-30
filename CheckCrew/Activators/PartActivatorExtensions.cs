using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckCrew.Activators
{
    public static class PartActivatorExtensions
    {
        public static void ActivateModule(this ModuleEngines module)
        {
            module.Events["Activate"].active = module.Events["Activate"].guiActive = true;
        }

        public static void ActivateModule(this ModuleRCS module)
        {
            module.Events["Enable"].active = module.Events["Enable"].guiActive = true;
            module.Enable();
        }

        public static void ActivateModule(this ModuleReactionWheel module)
        {
            module.State = ModuleReactionWheel.WheelState.Active;
        }

        public static void ActivateModule(this ModuleLight module)
        {
            module.Events["LightsOn"].active = module.Events["LightsOn"].guiActive = true;
        }

        public static void ActivateResource(this PartResource resource)
        {
            resource.flowState = true;
        }

        public static void DeactivateModule(this ModuleEngines module)
        {
            module.Shutdown();
            module.Events["Activate"].active = module.Events["Activate"].guiActive = false;
        }

        public static void DeactivateModule(this ModuleRCS module)
        {
            module.Disable();
            module.Events["Enable"].active = module.Events["Enable"].guiActive = false;
        }

        public static void DeactivateModule(this ModuleReactionWheel module)
        {
            module.State = ModuleReactionWheel.WheelState.Disabled;
        }

        public static void DeactivateModule(this ModuleLight module)
        {
            module.LightsOff();
            module.Events["LightsOn"].active = module.Events["LightsOn"].guiActive = false;
        }

        public static void DeactivateResource(this PartResource resource)
        {
            resource.flowState = false;
        }

        public static void DeactivatePart(this Part part)
        {
            part.Modules.OfType<ModuleEngines>().ToList().ForEach(x => x.DeactivateModule());
            part.Modules.OfType<ModuleRCS>().ToList().ForEach(x => x.DeactivateModule());
            part.Modules.OfType<ModuleReactionWheel>().ToList().ForEach(x => x.DeactivateModule());
            part.Modules.OfType<ModuleLight>().ToList().ForEach(x => x.DeactivateModule());
            part.Resources.OfType<PartResource>().ToList().ForEach(x => x.DeactivateResource());
        }
        
        public static void ActivatePart(this Part part)
        {
            part.Modules.OfType<ModuleEngines>().ToList().ForEach(x => x.ActivateModule());
            part.Modules.OfType<ModuleRCS>().ToList().ForEach(x => x.ActivateModule());
            part.Modules.OfType<ModuleReactionWheel>().ToList().ForEach(x => x.ActivateModule());
            part.Modules.OfType<ModuleLight>().ToList().ForEach(x => x.ActivateModule());
            part.Resources.OfType<PartResource>().ToList().ForEach(x => x.ActivateResource());
        }

    }
}
