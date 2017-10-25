using System;
using ICities;
using UnityEngine;
using ColossalFramework.Plugins;

namespace AutoEmpty
{
    public class AutoEmpty : IUserMod
    {
        public String Name
        {
            get { return "Automatic Empty"; }
        }

        public String Description
        {
            get { return "Automatically starts emptying and stops whenever it finishes. Works for landfills and cemeteries"; }
        }
    }

    public class Threader : ThreadingExtensionBase
    {
        private BuildingManager _buildingManager;
        private CemeteryAI cemeteryAI;
        FastList<ushort> garbageBuildings, healthBuildings;

        public override void OnAfterSimulationTick()
        {
            if (threadingManager.simulationTick % 1024 == 0 && !threadingManager.simulationPaused)
            {
                _buildingManager = ColossalFramework.Singleton<BuildingManager>.instance;

                garbageBuildings = _buildingManager.GetServiceBuildings(ItemClass.Service.Garbage);
                healthBuildings = _buildingManager.GetServiceBuildings(ItemClass.Service.HealthCare);

                //OutputLogger.PrintMessage("Quantidade de Garbage buildings: " + garbageBuildings.m_size.ToString());

                //OutputLogger.PrintMessage("Start Scanning " + threadingManager.simulationTick);
                

                var buffer = _buildingManager.m_buildings.m_buffer;
                int amount = 0;

                foreach (ushort serviceGarbage in garbageBuildings)
                {
                    var _buildingAi = buffer[serviceGarbage].Info.m_buildingAI;
                    if (_buildingAi is LandfillSiteAI)
                    {
                        amount = _buildingAi.GetGarbageAmount(serviceGarbage, ref buffer[serviceGarbage]);
                        if (amount > 7600000)
                        {
                            _buildingAi.SetEmptying(serviceGarbage, ref buffer[serviceGarbage], true);
                            //OutputLogger.PrintMessage("Start Emptying Landfill: " + serviceGarbage);
                        }
                        else if (amount < 400000)
                        {
                            _buildingAi.SetEmptying(serviceGarbage, ref buffer[serviceGarbage], false);
                            //OutputLogger.PrintMessage("Stop Emptying Landfill: " + serviceGarbage);
                        }
                    }
                }

                foreach (ushort deathCare in healthBuildings)
                {
                    var _buildingAi = buffer[deathCare].Info.m_buildingAI;
                    if (_buildingAi is CemeteryAI)
                    {
                        cemeteryAI = (CemeteryAI)_buildingAi;
                        //amount = cemeteryAI.m_graveCount;
                        if (cemeteryAI.IsFull(deathCare, ref buffer[deathCare]))
                        {
                            cemeteryAI.SetEmptying(deathCare, ref buffer[deathCare], true);
                            // OutputLogger.PrintMessage("Start Emptying Cemetary: " + amount);
                        }
                        else if (!cemeteryAI.IsFull(deathCare, ref buffer[deathCare]))
                        {
                            cemeteryAI.SetEmptying(deathCare, ref buffer[deathCare], false);
                            // OutputLogger.PrintMessage("Stop Emptying cemetary: " + amount);
                        }
                    }
                }
            }

            base.OnAfterSimulationTick();
        }
    }

    public class OutputLogger
    {
        const string prefix = "[Auto Empty] ";

        public static void PrintMessage(string message)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, prefix + message);
        }

        public static void PrintError(string message)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, prefix + message);
        }

        public static void PrintWarning(string message)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, prefix + message);
        }
    }
    
}
