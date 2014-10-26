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

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (state == StartState.Editor) return;

            GameEvents.onCrewBoardVessel.Add(CrewBoarded);

            CheckVessel();
            Disable();
        }

        private void CrewBoarded(GameEvents.FromToAction<Part, Part> data)
        {
            CheckVessel();

            Enable();
            Disable();
        }

        private void CheckVessel()
        {
            _activeParts.Clear();
            _inactiveParts.Clear();

            CheckPartsToCrew();
        }

        public override void OnUpdate()
        {
            //CheckVessel();
            
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

        [KSPEvent(active = true, guiActive = true, guiName = "Disable parts")]
        public void Disable()
        {
            //SetEngine(false);
            //SetRCS(false);
            //SetCrossFeed(false);
            //SetReactionWeel(false);
            //SetLights(false);
            SetResources(false);
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

        private void CheckPartsToCrew()
        {
            var crewParts = vessel.parts.Where(x => x.protoModuleCrew.Any()).ToList();

            foreach (var crewPart in crewParts)
            {
                CheckPart(crewPart);
            }

            _inactiveParts.AddRange(vessel.Parts.Where(x => !_activeParts.Contains(x)));
        }

        private void CheckPart(Part item)
        {
            if (!_activeParts.Contains(item))
            {
                _activeParts.Add(item);
            }

            foreach (var child in item.children.Where(child => !child.Modules.OfType<ModuleDockingNode>().Any()))
            {
                CheckPart(child);
            }
        }

        void SetEngine(bool state)
        {
            try
            {
                var modules = state ? ActiveParts.SelectMany(x => x.Modules.OfType<ModuleEngines>()).ToList() : InactiveParts.SelectMany(x => x.Modules.OfType<ModuleEngines>()).ToList();

                if (modules.Any())
                {
                    modules.ForEach(y =>
                    {
                        y.Shutdown();
                        //y.Events["Activate"].active = state;
                        //y.Events["Activate"].guiActive = state;
                        //y.Events["Shutdown"].active = state;
                        //y.Events["Shutdown"].guiActive = state;
                        y.enabled = state;
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error engine {0} {1}", ex.Message, ex.StackTrace));
            }
        }

        void SetRCS(bool state)
        {
            try
            {
                var modules = state ? ActiveParts.SelectMany(x => x.Modules.OfType<ModuleRCS>()).ToList() : InactiveParts.SelectMany(x => x.Modules.OfType<ModuleRCS>()).ToList();

                if (modules.Any())
                {
                    modules.ForEach(y =>
                    {
                        if (state)
                        {
                            y.Enable();
                        }
                        else
                        {
                            y.Disable();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error rcs {0} {1}", ex.Message, ex.StackTrace));
            }
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

        void SetReactionWeel(bool state)
        {
            try
            {
                var modules = state ? ActiveParts.SelectMany(x => x.Modules.OfType<ModuleReactionWheel>()).ToList() : InactiveParts.SelectMany(x => x.Modules.OfType<ModuleReactionWheel>()).ToList();

                if (modules.Any())
                {
                    modules.ForEach(y =>
                    {
                        y.State = state ? ModuleReactionWheel.WheelState.Active : ModuleReactionWheel.WheelState.Disabled; ;
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error reaction wheel {0} {1}", ex.Message, ex.StackTrace));
            }
        }

        void SetLights(bool state)
        {
            try
            {
                var modules = state ? ActiveParts.SelectMany(x => x.Modules.OfType<ModuleLight>()).ToList() : InactiveParts.SelectMany(x => x.Modules.OfType<ModuleLight>()).ToList();

                if (modules.Any())
                {
                    modules.ForEach(y =>
                    {
                        y.enabled = state;
                        y.Events["LightsOn"].active = state;
                        y.Events["LightsOn"].guiActive = state;
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error reaction wheel {0} {1}", ex.Message, ex.StackTrace));
            }
        }

        public List<Part> ActiveParts
        {
            get { return _activeParts; }
        }

        public List<Part> InactiveParts
        {
            get { return _inactiveParts; }
        }

    }
}
