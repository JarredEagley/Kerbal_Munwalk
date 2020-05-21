using System;
using UnityEngine;

namespace nubeees_MunWalk
{
    //[KSPAddon(KSPAddon.Startup.Flight, false)]
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

        // Animation fields
        Animation _anim;
        AnimationClip stand;
        AnimationClip walkf;
        AnimationClip walkb;
        AnimationClip walkl;
        AnimationClip walkr;
        AnimationClip turnl;
        AnimationClip turnr;
        AnimationClip jump;

        // Line stuff.
        GameObject lineObj = new GameObject("Line");
        LineRenderer _line;


        // Weird thing I don't quite understand for raycasting.
        public int layerMask = 0;

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

            // If we've got a kerbal, get the kerbal's animations.
            if (_kerbal != null)
            {
                // Animation stuff
                stand = _kerbal.Animations.idle.State.clip;
                walkf = _kerbal.Animations.walkFwd.State.clip;
                walkb = _kerbal.Animations.walkBack.State.clip;
                walkl = _kerbal.Animations.walkLowGeeLeft.State.clip;
                walkr = _kerbal.Animations.walkLowGeeRight.State.clip;
                turnl = _kerbal.Animations.turnLeft.State.clip;
                turnr = _kerbal.Animations.turnRight.State.clip;
                jump = _kerbal.Animations.JumpStillStart.State.clip;
                _anim = _kerbal.GetComponent<Animation>();

                _vessel.rootPart.Modules.Add(new MunWalk_Part());
            }
            

            // Ignore me
            //try
            //{
                // Define feet.     this did not work.
                //rightfoot = new GameObject();
                //rightfoot.name = "rightfoot";
                //rightfoot.transform.position = transform.Find("footCollider_r").position;
                //rightfoot.transform.rotation = transform.Find("footCollider_r").rotation;
                //rightfoot.transform.parent = _kerbal.gameObject.transform;
                //leftfoot = new GameObject();
                //leftfoot.name = "leftfoot";
                //leftfoot.transform.position = transform.Find("footCollider_l").position;
                //leftfoot.transform.rotation = transform.Find("footCollider_l").rotation;
                //leftfoot.transform.parent = _kerbal.gameObject.transform;
            //}
            //catch
            //{
                //Debug.Log("Feet not found.");
            //}

        }


        int counter = 0; // Update counter
        bool active = false; // Keeps track of whether or not 'walking' is active or not.
        public void Update()
        {
            // Make sure these operations are being ran on kerbals only.
            if (_kerbal != null)
            {
                // TEST-- THIS WORKS!!!!!!!
                //_kerbal.vessel.rootPart.dragModel = Part.DragModel.NONE; 
                //Debug.Log(_kerbal.part.dragModel);
                //if (Input.GetKeyUp(KeyCode.C))
                //{
                //    _kerbal.isRagdoll = false;
                //}

                // Update slow:
                if (counter > 12)
                {
                    updateSlow();
                    counter = 0;
                }

                // Toggle with 'O' button.
                //if (Input.GetKeyUp(KeyCode.O))
                //{
                //    walktoggle = !walktoggle;
                //}

                // Toggle with context menu button.
                MunWalk_Part partmod = _kerbal.part.FindModuleImplementing<MunWalk_Part>();
                walktoggle = partmod.getActive_MW();
                airplanetoggle = partmod.getActive_AM();
                // Todo: Rename the airplanemode class to munwalkpart module or something like that...

                // drag handling:
                if (airplanetoggle)
                {
                    _kerbal.vessel.rootPart.dragModel = Part.DragModel.NONE;
                }
                else
                {
                    _kerbal.vessel.rootPart.dragModel = Part.DragModel.CUBE;
                }

                //Debug.Log(""+_vessel.situation.ToString());           log the vessel's situation

                // Handle force unragdoll button's action...
                if (partmod.forceunragdoll)
                {
                    
                    _kerbal.isRagdoll = false;
                    _anim.clip = jump;
                    _anim.Play();
                    _kerbal.fsm.StartFSM("Idle (Floating)");
                    //partmod.resetForceUnragdoll();            attempt at forcing unragdoll... It didn't work :(
                }

                // Force application frames:
                if (counter % 3 == 0)
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
                            active = true;
                            // Handle Movement.
                            try
                            {
                                if (counter == 6 && _vessel == FlightGlobals.ActiveVessel)
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

                        // If still active but walktoggle is off, set active to false, stop any animations, and set kerbal to be ragdolled.
                        if (active && !walktoggle)
                        {
                            active = false;
                            _anim.Stop();
                            _kerbal.fsm.StartFSM("Ragdoll");
                        }
                    }
                } // End of force application


                // Accel checking frames:
                if (counter % 3 == 2)
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
                } // End of accel checking.
                
                // End of kerbal checking
            }
            counter++;
        } // End of update method.

        // (update method helper)
        // Orient kerbal based on acceleration.
        public void orientKerbal ()
        {
            // Make kerbal 'float'
            _kerbal.fsm.StartFSM("Idle (Floating)");

            // Try to orient correctly if not in freefal.
            if (accel.magnitude > .5)
            {
                // Note: 8 is good for 1g
                Vector3 Angularvel = (Vector3.Cross(_kerbal.transform.up, AVGaccel.normalized) * ((float)AVGaccel.magnitude)) * 0.5f;
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
            // Update kerbal and vessel.    Moved to start
            //_vessel = FlightGlobals.ActiveVessel;
            //_kerbal = _vessel.evaController;

            // Line stuff
            //_line.useWorldSpace = false;
            //_line.transform.localPosition = FlightGlobals.ActiveVessel.CoM;
            //_line.material = new Material(Shader.Find("Particles/Additive"));
            //_line.startColor = Color.red;
            //_line.startWidth = .1f;
            //_line.endWidth = 0;
            //_line.SetPosition(1, accel);


            // Only using this for debug right now.
            double angle = VectorAngle(_kerbal.transform.up, AVGaccel.normalized);
            // Print to log //////////////////////////////////
            //Debug.Log("Kerbal: " + (_kerbal != null));
            //Debug.Log("Vessel: " + (_vessel != null));
            //Debug.Log("Walktoggle: " + walktoggle);
            //Debug.Log("***ANGLE*** : " + radToDeg(angle));
            //Debug.Log("*Accel* : " + AVGaccel.magnitude);
            //////////////////////////////////////////////////
            // Log the kerbal's state.
            //Debug.Log(_kerbal.fsm.currentStateName);
        }

        // (update method helper)
        // Move kerbal based on user input.
        private void moveKerbal ()
        {
            // real: 93.something
            float kerbalweight = 90f;

            // Jump!
            if (Input.GetKey(KeyCode.Space))
            {
                _vessel.rootPart.Rigidbody.AddForce(accel.normalized*4);
                _anim.clip = jump;
                _anim.Play();
                return;
            }
            // Kerbal clamber
            if (Input.GetKey(KeyCode.C))
            {
                // Raycast + teleport. Not yet implemented.
                
            }
            // Forward
            else if (Input.GetKey(KeyCode.W))
            {
                // I need to eventually have some sort of collision detection to make sure you've got something to push off of when taking a step. for now this timer will work okay.
                if (counter % 6 == 0)
                {
                    _vessel.rootPart.Rigidbody.AddForce(AVGaccel * .7f);
                    // Orthogonal porjection.
                    Vector3d _movevec = Vector3.ProjectOnPlane(_kerbal.transform.forward, AVGaccel);
                    _movevec *= AVGaccel.magnitude / 10;
                    _vessel.rootPart.Rigidbody.AddForceAtPosition(_movevec * 5f, _vessel.CoM - (_kerbal.transform.up * 0.1f));
                }

                //_vessel.rootPart.Rigidbody.angularVelocity = new Vector3d(0, 0, 0);
                //_vessel.rootPart.Rigidbody.AddRelativeTorque(-0.01f, 0, 0);
                _anim.clip = walkf;
                _anim.Play();
            }
            // Backward
            else if (Input.GetKey(KeyCode.S))
            {
                if (counter % 6 == 0)
                {
                    _vessel.rootPart.Rigidbody.AddForce(AVGaccel * .7f);
                    //_vessel.rootPart.Rigidbody.AddRelativeForce(0, (float)(accel.magnitude / kerbalweight), -.02f, ForceMode.Force);
                    // Orthogonal porjection.
                    Vector3d _movevec = Vector3.ProjectOnPlane(_kerbal.transform.forward * -1, AVGaccel);
                    _movevec *= AVGaccel.magnitude / 10;
                    _vessel.rootPart.Rigidbody.AddForceAtPosition(_movevec * 3f, _vessel.CoM - (_kerbal.transform.up * 0.1f));
                }


                //_vessel.rootPart.Rigidbody.angularVelocity = new Vector3d(0, 0, 0);
                //_vessel.rootPart.Rigidbody.AddRelativeTorque(0.01f, 0, 0);
                _anim.clip = walkb;
                _anim.Play();
            }
            // Right
            else if (Input.GetKey(KeyCode.D))
            {
                if (counter % 6 == 0)
                {
                    _vessel.rootPart.Rigidbody.AddForce(AVGaccel * .7f);
                    //_vessel.rootPart.Rigidbody.AddRelativeForce(.01f, (float)(accel.magnitude / kerbalweight), 0, ForceMode.Force);
                    // Orthogonal porjection.
                    Vector3d _movevec = Vector3.ProjectOnPlane(_kerbal.transform.right, AVGaccel);
                    _movevec *= AVGaccel.magnitude / 10;
                    _vessel.rootPart.Rigidbody.AddForceAtPosition(_movevec * 3f, _vessel.CoM - (_kerbal.transform.up * 0.1f));
                }


                //_vessel.rootPart.Rigidbody.angularVelocity = new Vector3d(0, 0, 0);
                //_vessel.rootPart.Rigidbody.AddRelativeTorque(0, 0, -0.01f);
                _anim.clip = walkr;
                _anim.Play();
            }
            // Left
            else if (Input.GetKey(KeyCode.A))
            {
                if (counter % 6 == 0)
                {
                    _vessel.rootPart.Rigidbody.AddForce(AVGaccel * .7f);
                    //_vessel.rootPart.Rigidbody.AddRelativeForce(-.01f, (float)(accel.magnitude / kerbalweight), 0, ForceMode.Force);
                    // Orthogonal porjection.
                    Vector3d _movevec = Vector3.ProjectOnPlane(_kerbal.transform.right * -1, AVGaccel);
                    _movevec *= AVGaccel.magnitude / 10;
                    _vessel.rootPart.Rigidbody.AddForceAtPosition(_movevec * 3f, _vessel.CoM - (_kerbal.transform.up * 0.1f));
                }


                //_vessel.rootPart.Rigidbody.angularVelocity = new Vector3d(0, 0, 0);
                //_vessel.rootPart.Rigidbody.AddRelativeTorque(0, 0, 0.01f);
                _anim.clip = walkl;
                _anim.Play();
            }

            // Rotate Left
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                _vessel.rootPart.Rigidbody.AddRelativeTorque(0, -1f, 0, ForceMode.Force);
                //_vessel.rootPart.Rigidbody.angularVelocity = new Vector3d(0, 0, 0);
                _anim.clip = turnl;
                _anim.Play();
            }
            // Rotate Right
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _vessel.rootPart.Rigidbody.AddRelativeTorque(0, 1f, 0, ForceMode.Force);
                
                //_vessel.rootPart.Rigidbody.angularVelocity = new Vector3d(0, 0, 0);
                _anim.clip = turnr;
                _anim.Play();
            }
            // Stand
            else
            {
                _anim.clip = stand;
                _anim.Play();
            }
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


        /// <summary>
        /// Raycast thing i dont entirely understand to be honest.
        /// </summary>
        /// <param name="rayLength"></param>
        /// <param name="rayObject"></param>
        /// <param name="direction"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool RayCast (float rayLength, GameObject rayObject, Vector3 direction, out RaycastHit hit)
        {
            var ray = new Ray(rayObject.transform.position, direction);
            int tempLayerMask = ~layerMask;
            return Physics.Raycast(ray, out hit, rayLength, tempLayerMask);
        }

        // Converts radians to degrees.
        private double radToDeg (double rad)
        {
            return rad * (180 / Math.PI);
        }

        // Gets angle between vecs.
        private double VectorAngle (Vector3d vec1, Vector3d vec2)
        {
            // Get the dot product. Dot product will be the numerator.
            double dot = 0;
            dot += (vec1.x * vec2.x);
            dot += (vec1.y * vec2.y);
            dot += (vec1.z * vec2.z);

            // Get the denominator.
            double denom = VectorNorm(vec1) * VectorNorm(vec2);

            return Math.Acos(dot/denom);
        }

        // Gets vector's norm.
        private double VectorNorm (Vector3d vec)
        {
            return Math.Sqrt((vec.x * vec.x) + (vec.y * vec.y) + (vec.z * vec.z));
        }

        private void calculateAverageAccel ()
        {
            // Get the mean for all elements of the accel array + the accel field.
            AVGaccel.Zero();
            for (int i = 0; i < accelarray.Length; i++)
            {
                AVGaccel += accelarray[i];
            }
            AVGaccel += accel;
            AVGaccel /= accelarray.Length+1;
        }
    }
}