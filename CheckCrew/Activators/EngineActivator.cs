namespace CheckCrew.Activators
{
    public class EngineActivator : PartActivator<ModuleEngines>
    {
        public EngineActivator(ModuleEngines item)
            : base(item)
        {
        }

        public override void Activate()
        {
            _module.enabled = true;
            _module.Events["Activate"].active = true;
            _module.Events["Activate"].guiActive = true;
        }

        public override void Deactivate()
        {
            _module.Shutdown();
            _module.enabled = false;
            _module.Events["Activate"].active = false;
            _module.Events["Activate"].guiActive = false;
        }
    }
}
