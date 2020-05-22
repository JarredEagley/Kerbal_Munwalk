using System;
using UnityEngine;

namespace nubeees_MunWalk
{
    public class MW_Core : VesselModule
    {
        // Fields
        bool walktoggle = false;
        bool airplanetoggle = false;
        KerbalEVA _kerbal;
        Vessel _vessel;
        //Vector3d accelold2;
        //Vector3d accelold1;
        Vector3d accel;
        Vector3d AVGaccel;
        // Average across an array of vectors, to smooth out the target vector.
        Vector3d[] accelarray = new Vector3d[5];

        // Line stuff.
        GameObject lineObj = new GameObject("Line");
        LineRenderer _line;


        // Weird thing I don't quite understand for raycasting.
        public int layerMask = 0;

        /** Start method.
         * 
         * "Called once and is where any setup or “on-spawn” code should go. This method runs when your 
         * class spawns into the correct scene and runs before all other methods (except Awake()). Note 
         * that all Start() methods that need to run in a scene run sequentially, if you are trying to 
         * reference another object it may not exist yet and could require using a Coroutine to delay 
         * running some of your code."
         */
        public void Start()
        {
            // Set all vectors to zero to start with.
            for (int i = 0; i < accelarray.Length; i++)
            {
                accelarray[i] = new Vector3d(0, 0, 0);
            }

            // Define vessel and kerbal.
            _kerbal = this.GetComponent<KerbalEVA>();
            _vessel = this.GetComponent<Vessel>();

            // Define debug lines.
            //_line = lineObj.AddComponent<LineRenderer>();                     uncomment me for accel lines

            // If we've got a kerbal, Add the munwalk componenet
            if (_kerbal != null)
            { 
                _vessel.rootPart.Modules.Add(new MunWalk_Part());
            }
        }


    int updateCounter = 0; // Update counter
    bool MWActive = false; // Keeps track of whether or not 'walking' is active or not.

    /** FixedUpdate
    * "Called every physics frame and anything physics or game related should happen here."
    */
    public void FixedUpdate()
        {
            // Make sure these operations are being run on kerbals only.
            if (_kerbal != null)
            {
                // Update slow counter:
                if (updateCounter > 12)
                {
                    updateSlow();
                    updateCounter = 0;
                }

                handleButtonActions();

                // Force application frames:
                if (updateCounter % 3 == 0)
                {
                    handleForceApplicationFrame();
                }

                // Accel checking frames:
                if (updateCounter % 3 == 2)
                {
                    handleAccelerationCalculationFrame();
                } 
                
            }
            updateCounter++;
        }

        /** Handles the actions taken when the context menu buttons are pressed.
         */
        private void handleButtonActions()
        {
            // Toggle with context menu button.
            MunWalk_Part partmod = _kerbal.part.FindModuleImplementing<MunWalk_Part>();
            walktoggle = partmod.getActive_MW();
            airplanetoggle = partmod.getActive_AM();

            // drag handling:
            if (airplanetoggle)
            {
                _kerbal.vessel.rootPart.dragModel = Part.DragModel.NONE;
            }
            else
            {
                _kerbal.vessel.rootPart.dragModel = Part.DragModel.CUBE;
            }

            // Handle force unragdoll button's action...
            if (partmod.forceunragdoll)
            {

                _kerbal.isRagdoll = false;
                _kerbal.fsm.StartFSM("Idle (Floating)");
                //partmod.resetForceUnragdoll();            attempt at forcing unragdoll... It didn't work :(
            }
        }


        private void handleForceApplicationFrame() 
        {

                // It might not be able to find your kerbal, so nullcheck   note to self: remove this unneeded nullcheck!!!
                if (_kerbal != null)
                {
                    // "Oops I tripped"
                    if (_kerbal.isRagdoll)
                    {
                        walktoggle = false;
                    }
                    // No standing in >5g allowed
                    if (accel.magnitude > 49)
                    {
                        walktoggle = false;
                    }

                    // Stuff to do when you use the 'O' toggle.
                    if (walktoggle)
                    {
                        MWActive = true;
                        // Handle Movement.
                        try
                        {
                            if (updateCounter == 6 && _vessel == FlightGlobals.ActiveVessel)
                            {
                                moveKerbal();
                            }
                        }
                        catch
                        {
                            // Give orientation a chance to catch up!
                            Debug.LogWarning("input error while trying to move kerbal.");
                        }

                        // Try to orient Kerbal to accel
                        orientKerbal();
                    }

                    // If still active but walktoggle is off, set active to false, stop any animations.
                    if (MWActive && !walktoggle)
                    {
                        MWActive = false;
                        _anim.Stop()
                            //_kerbal.fsm.StartFSM("Ragdoll");
                        }
                }

        }

        private void handleAccelerationCalculationFrame()
        {
            // Nullcheck for the kerbal.
            if (_kerbal != null)
            {
                // Calculate accel for this frame.
                Vector3 trueaccel = _kerbal.vessel.acceleration_immediate;
                // When in freefall, we don't want to account for gravity.
                accel = trueaccel - _kerbal.vessel.graviticAcceleration;

                // Store this accel vector to be averaged next round.
                //accelold2 = accelold1;
                for (int i = accelarray.Length - 1; i > 0; i--)
                {
                    accelarray[i] = accelarray[i - 1];
                }
                //accelold1 = accel;
                accelarray[0] = accel;

                // Calculate averaged acceleration.
                calculateAverageAccel();
            }
        }


        /** Orients the kerbal to align with a supplied 'up' direction vector.
         */
        public void orientKerbal (Vector3d inputVector)
        {
            // Make kerbal 'float'
            // TO-DO: change this to simply be an un-ragdoll.
            _kerbal.fsm.StartFSM("Idle (Floating)");

            // Try to orient correctly if not in freefal.
            if (inputVector.magnitude > .5)
            {
                // Note: 8 is good for 1g
                Vector3 Angularvel = (Vector3.Cross(_kerbal.transform.up, inputVector.normalized) * ((float)inputVector.magnitude)) * 0.5f;
                if (Angularvel.magnitude > 50)
                {
                    Angularvel /= 10;
                }
                _vessel.rootPart.Rigidbody.angularVelocity = Angularvel;
                // Just a debug log
                //Debug.Log("********Angvel magnitude: " + Angularvel.magnitude);
            }
        }

        // Called every 10 frames of the update method!
        private void updateSlow ()
        {

        }


        //[KSPEvent(guiActive = true, guiName = "Toggle Munwalk")]
        KSPAction testaction = new KSPAction();
        [KSPAction("Test")]
        public void inputWalkToggle()
        {
            if (walktoggle == false)
                walktoggle = true;
            else
                walktoggle = false; 
        }



    }
}