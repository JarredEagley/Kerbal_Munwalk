using System;
using UnityEngine;

namespace nubeees_MunWalk
{
    class MunWalk_Part : PartModule
    {
        [KSPField(guiActive = true, guiName = "MunWalk")]
        public bool munwalk = false;

        [KSPField(guiActive = true, guiName = "Airplane Mode")]
        public bool airplanemode = false;

        [KSPField(guiActive = false, guiName = "Airplane Mode")]
        public bool forceunragdoll = false;

        //[KSPEvent(guiActive = true, guiName = "Toggle test module")]

        /*
         * This event is active when controlling the vessel with the part.
         */
        [KSPEvent(guiActive = true, guiName = "Activate MunWalk")]
        public void ActivateEvent_MW()
        {
            ScreenMessages.PostScreenMessage("Activated Munwalk", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            munwalk = true;

            // This will hide the Activate event, and show the Deactivate event.
            Events["ActivateEvent_MW"].active = false;
            Events["DeactivateEvent_MW"].active = true;
        }

        /*
         * This event is also active when controlling the vessel with the part. It starts disabled.
         */
        [KSPEvent(guiActive = true, guiName = "Deactivate MunWalk", active = false)]
        public void DeactivateEvent_MW()
        {
            ScreenMessages.PostScreenMessage("Deactivated Munwalk", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            munwalk = false;

            // This will hide the Deactivate event, and show the Activate event.
            Events["ActivateEvent_MW"].active = true;
            Events["DeactivateEvent_MW"].active = false;
        }
        
        // Airplane mode
        [KSPEvent(guiActive = true, guiName = "Activate Airplane Mode")]
        public void ActivateEvent_AM()
        {
            ScreenMessages.PostScreenMessage("Activated 'Airplane Mode'", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            airplanemode = true;

            // This will hide the Activate event, and show the Deactivate event.
            Events["ActivateEvent_AM"].active = false;
            Events["DeactivateEvent_AM"].active = true;
        }

        [KSPEvent(guiActive = true, guiName = "Deactivate Airplane Mode", active = false)]
        public void DeactivateEvent_AM()
        {
            ScreenMessages.PostScreenMessage("Deactivated 'Airplane Mode'", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            airplanemode = false;

            // This will hide the Deactivate event, and show the Activate event.
            Events["ActivateEvent_AM"].active = true;
            Events["DeactivateEvent_AM"].active = false;
        }

        // Force unragdoll
        /*
        [KSPEvent(guiActive = false, guiName = "Force un-ragdoll")]
        public void ActivateEvent_Unragdoll()
        {
            ScreenMessages.PostScreenMessage("Forcing kerbal to recover...", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            forceunragdoll = true;

            // This will hide the Activate event, and show the Deactivate event.
            Events["ActivateEvent_Unragdoll"].active = false;
            //Events["DeactivateEvent_AM"].active = true;
        }
        public void resetForceUnragdoll()
        {
            Events["ActivateEvent_Unragdoll"].active = true;
            forceunragdoll = false;
        }
        */

        /*
         * This event is active when controlling a nearby EVA. It can be activated from up to 25
         * meters away.
         */
        //[KSPEvent(guiActiveUnfocused = true, unfocusedRange = 25f, guiName = "Nearby EVA")]
        //public void NearbyEvaEvent()
        //{
        //    ScreenMessages.PostScreenMessage("Clicked NearbyEvaEvent", 5.0f, ScreenMessageStyle.UPPER_CENTER);
        //}

        /*
         * This event is active when controlling a nearby vessel. Note that the event will also be
         * active for nearby EVAs. There is not currently a way to make it only active for nearby
         * vessels.
         */
        //[KSPEvent(guiActiveUnfocused = true, externalToEVAOnly = false, unfocusedRange = 25f, guiName = "Nearby Vessel")]
        //public void NearbyVesselEvent()
        //{
        //    ScreenMessages.PostScreenMessage("Clicked NearbyVesselEvent", 5.0f, ScreenMessageStyle.UPPER_CENTER);
        //}

        /*
         * This event is always active, regardless of which vessel is being controlled.
         */
        //[KSPEvent(guiActive = true, guiActiveUnfocused = true, externalToEVAOnly = false, unfocusedRange = 25f, guiName = "Any Event")]
        //public void AnyEvent()
        //{
        //    ScreenMessages.PostScreenMessage("Clicked AnyEvent", 5.0f, ScreenMessageStyle.UPPER_CENTER);
        //}

        public bool getActive_MW()
        {
            return munwalk;
        }
        public void setActive_MW(bool newstate)
        {
            munwalk = newstate;
        }

        public bool getActive_AM()
        {
            return airplanemode;
        }
        public void setActive_AM(bool newstate)
        {
            airplanemode = newstate;
        }


    }
}
