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

    public class Loader : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            GameObject go = new GameObject("Test Object");
            go.AddComponent<UnityBehavior>();
        }
    }

    public class UnityBehavior : MonoBehaviour
    {
        private void Start()
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "ITS WORKING!! ITS WORKING!!!");
        }

        private void FixedUpdate()
        {
            
        }

        private void Update()
        {
            
        }

        private void LateUpdate()
        {
            
        }
    }
}
