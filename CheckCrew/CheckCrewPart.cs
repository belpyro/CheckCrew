using System;
using System.Collections.Generic;
using System.Linq;
using CheckCrew.Activators;
using UnityEngine;

namespace CheckCrew
{
    public class CheckCrewPart : PartModule
    {
        private readonly List<Part> _activeParts = new List<Part>();
        private readonly List<Part> _inactiveParts = new List<Part>();

        private readonly Dictionary<Part, List<Part>> _partsDictionary = new Dictionary<Part, List<Part>>();

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (state == StartState.Editor) return;

            GameEvents.onCrewBoardVessel.Add(CrewBoarded);

            CheckVessel();
        }

        private void CrewBoarded(GameEvents.FromToAction<Part, Part> data)
        {
            CheckVessel();

            //Enable();
            //Disable();
        }

        private void CheckVessel()
        {
            _activeParts.Clear();
            _inactiveParts.Clear();


            var crewedParts = GetPartsWithAvaibleCrew();

            foreach (var crewedPart in crewedParts)
            {
                var items = new List<Part>();

                FillParts(crewedPart, items);

                if (items.Any())
                {
                    _partsDictionary.Add(crewedPart, items);
                }
            }

            
        }

        private void SetResources(bool state)
        {
            //InactiveParts.ForEach(x =>
            //{
            //    foreach (PartResource resource in x.Resources)
            //    {
            //        resource.flowMode = PartResource.FlowMode.None;
            //        resource.flowState = state;
            //    }
            //});
        }

        [KSPEvent(active = true, guiActive = true, guiActiveUnfocused = true, isDefault = true, guiName = "Disable parts")]
        public void Disable()
        {
            foreach (var item in _partsDictionary.Where(item => !item.Key.protoModuleCrew.Any()))
            {
                item.Value.ForEach(x => x.DeactivatePart());
            }
        }

        [KSPEvent(active = false, guiActive = false, guiName = "Enable parts")]
        public void Enable()
        {
            //SetEngine(true);
            //SetRCS(true);
            //SetCrossFeed(true);
            //SetReactionWeel(true);
            //SetLights(true);
            SetResources(true);
        }

        private IEnumerable<Part> GetPartsWithAvaibleCrew()
        {
            return vessel.parts.Where(x => x.CrewCapacity > 0).ToList();
        }

        private void FillParts(Part parent, ICollection<Part> chain)
        {
            foreach (var node in parent.attachNodes)
            {
               if(node.attachedPart.Modules.OfType<VesselSeparator>().Any()) continue;

                if (chain.Contains(node.attachedPart)) continue;

                chain.Add(node.attachedPart);
                FillParts(node.attachedPart, chain);
            }
            
            //foreach (var child in parent.children.Where(child => !child.Modules.OfType<VesselSeparator>().Any()))
            //{
            //    chain.Add(child);
            //    FillParts(child, chain);
            //}
        }

        void SetCrossFeed(bool state)
        {
            try
            {
                var modules = state ? InactiveParts.SelectMany(x => x.Modules.OfType<ModuleDockingNode>()).ToList() : InactiveParts.SelectMany(x => x.Modules.OfType<ModuleDockingNode>()).ToList();

                if (modules.Any())
                {
                    modules.ForEach(y =>
                    {
                        y.part.fuelCrossFeed = state;
                        y.Events["EnableXFeed"].guiActive = state;
                        y.Events["EnableXFeed"].active = state;
                        y.Events["DisableXFeed"].guiActive = state;
                        y.Events["DisableXFeed"].active = state;
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error cross feed {0} {1}", ex.Message, ex.StackTrace));
            }
        }

        public List<Part> ActiveParts
        {
            get { return _partsDictionary.Where(x => x.Key.protoModuleCrew.Any()).SelectMany(x => x.Value).ToList(); }
        }

        public List<Part> InactiveParts
        {
            get { return _partsDictionary.Where(x => !x.Key.protoModuleCrew.Any()).SelectMany(x => x.Value).ToList(); }
        }

    }
}
