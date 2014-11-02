using System;
using System.Collections.Generic;
using System.Linq;
using CheckCrew;
using CheckCrew.Activators;

namespace CrewCheckerSpace
{
    public enum ModuleStatus
    {
        Enabled,
        Disabled
    }

    public class CrewChecker : PartModule
    {
        private readonly List<Part> _activeParts = new List<Part>();
        private readonly List<Part> _inactiveParts = new List<Part>();


        [KSPField(guiActive = true, guiName = "CrewChecker status", isPersistant = false)]
        public ModuleStatus Status = ModuleStatus.Enabled;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (state == StartState.Editor) return;

            CheckVessel();

            GameEvents.onCrewBoardVessel.Add(CrewBoarded);
        }

        public void OnDestroy()
        {
            GameEvents.onCrewBoardVessel.Remove(CrewBoarded);
        }


        private void CrewBoarded(GameEvents.FromToAction<Part, Part> data)
        {
            CheckVessel();
        }

        private void CheckVessel()
        {
            if (Status == ModuleStatus.Disabled) return;

            _activeParts.Clear();
            _inactiveParts.Clear();


            var crewedParts = GetPartsWithAvaibleCrew();

            foreach (var crewedPart in crewedParts)
            {
                if (crewedPart.protoModuleCrew.Count == crewedPart.CrewCapacity)
                {
                    FillActive(crewedPart);
                }
                else
                {
                    FillInactive(crewedPart);
                }
            }

            _inactiveParts.ForEach(x => x.DeactivatePart());
            _activeParts.ForEach(x => x.ActivatePart());

        }

        [KSPEvent(active = true, guiActive = true, guiName = "Toggle CrewChecker")]
        public void ToggleModule()
        {
            switch (Status)
            {
                case ModuleStatus.Enabled:
                    _inactiveParts.ForEach(x => x.ActivatePart());
                    Status = ModuleStatus.Disabled;
                    break;
                case ModuleStatus.Disabled:
                    Status = ModuleStatus.Enabled;
                    CheckVessel();
                    break;
            }
        }

        private IEnumerable<Part> GetPartsWithAvaibleCrew()
        {
            return vessel.parts.Where(x => x.CrewCapacity > 0).ToList();
        }

        private void FillActive(Part parent)
        {
            if (_activeParts.Any(x => x.uid == parent.uid)) return;

            _activeParts.Add(parent);

            if (parent.children == null) return;

            foreach (var child in parent.children)
            {
                if (child.Modules.OfType<VesselSeparator>().Any()) continue;

                FillActive(child);
            }
        }

        private void FillInactive(Part parent)
        {
            if (_inactiveParts.Any(x => x.uid == parent.uid)) return;

            _inactiveParts.Add(parent);

            if (parent.children == null) return;

            foreach (var child in parent.children)
            {
                if (child.Modules.OfType<VesselSeparator>().Any()) continue;

                FillInactive(child);
            }
        }

    }
}
