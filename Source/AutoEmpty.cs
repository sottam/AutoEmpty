using ICities;
using ColossalFramework.Plugins;

namespace AutoEmpty
{
    public class Monitor : ThreadingExtensionBase
    {
        private BuildingManager _buildingManager;
        //private CemeteryAI cemeteryAI;
        FastList<ushort> garbageBuildings, healthBuildings;

        public override void OnAfterSimulationTick()
        {
            if (threadingManager.simulationTick % 1024 == 0 && !threadingManager.simulationPaused)
            {
                _buildingManager = ColossalFramework.Singleton<BuildingManager>.instance;

                garbageBuildings = _buildingManager.GetServiceBuildings(ItemClass.Service.Garbage);
                healthBuildings = _buildingManager.GetServiceBuildings(ItemClass.Service.HealthCare);

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
                        }
                        else if (amount < 400000)
                        {
                            _buildingAi.SetEmptying(serviceGarbage, ref buffer[serviceGarbage], false);
                        }
                    }
                }

                foreach (ushort deathCare in healthBuildings)
                {
                    var _buildingAi = buffer[deathCare].Info.m_buildingAI;
                    if (_buildingAi is CemeteryAI)
                    {
                        //cemeteryAI = (CemeteryAI)_buildingAi;
                        if (_buildingAi.IsFull(deathCare, ref buffer[deathCare]))
                        {
                            _buildingAi.SetEmptying(deathCare, ref buffer[deathCare], true);
                        }
                        else if (_buildingAi.CanBeRelocated(deathCare, ref buffer[deathCare]) )
                        {
                            _buildingAi.SetEmptying(deathCare, ref buffer[deathCare], false);
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
