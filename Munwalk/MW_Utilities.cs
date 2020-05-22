using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Munwalk
{
    /**
     *  Provides some useful methods for utility.
     */
    public class MW_Utilities
    {
        /// <summary>
        /// Raycast thing i dont entirely understand to be honest.
        /// </summary>
        /// <param name="rayLength"></param>
        /// <param name="rayObject"></param>
        /// <param name="direction"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        protected bool RayCast(float rayLength, GameObject rayObject, Vector3 direction, out RaycastHit hit)
        {
            var ray = new Ray(rayObject.transform.position, direction);
            int tempLayerMask = ~layerMask;
            return Physics.Raycast(ray, out hit, rayLength, tempLayerMask);
        }


        /** Takes an angle in radians and returns the equivalent angle in degrees.
         */
        protected double radToDeg(double rad)
        {
            return rad * (180 / Math.PI);
        }


        /** Returns the angle between two given vectors.
         */
        protected double VectorAngle(Vector3d vec1, Vector3d vec2)
        {
            // Get the dot product. Dot product will be the numerator.
            double dot = 0;
            dot += (vec1.x * vec2.x);
            dot += (vec1.y * vec2.y);
            dot += (vec1.z * vec2.z);

            // Get the denominator.
            double denom = VectorNorm(vec1) * VectorNorm(vec2);

            return Math.Acos(dot / denom);
        }

        /** Returns the norm of a given vector.
         * 
         */
        protected double VectorNorm(Vector3d vec)
        {
            return Math.Sqrt((vec.x * vec.x) + (vec.y * vec.y) + (vec.z * vec.z));
        }

        //protected Vector3d calculateAverageAccel(Vector3d AVGaccel)
        //{
        //    // Get the mean for all elements of the accel array + the accel field.
        //    AVGaccel.Zero();
        //    for (int i = 0; i < accelarray.Length; i++)
        //    {
        //        AVGaccel += accelarray[i];
        //    }
        //    AVGaccel += accel;
        //    AVGaccel /= accelarray.Length + 1;
        // }


        /** Given an array of vectors, calculate the average vector.
         * 
         */
        protected Vector3d AverageVector(Vector3d[] vectorArray)
        {
            // Get the mean for all elements of the accel array + the accel field.
            Vector3d avgVector = Vector3d.zero;
            for (int i = 0; i < vectorArray.Length; i++)
            {
                avgVector += vectorArray[i];
            }
            avgVector /= vectorArray.Length;
            return avgVector;
        }

        /// <summary>
        /// Thank you to RoverDude and anyone else involved with writing LogisticsTools.cs for this function!
        /// </summary>
        /// <param name="a">A 'Vessel' object.</param>
        /// <param name="b">A 'Vessel' object.</param>
        /// <returns>The distance between the two input vessels.</returns>
        public static double GetRange(Vessel a, Vessel b)
        {
            var posCur = a.GetWorldPos3D();
            var posNext = b.GetWorldPos3D();
            return Vector3d.Distance(posCur, posNext);
        }

        /// <summary>
        /// Thank you to RoverDude for having this function in his USITools repo! It is exactly what I needed! ☺
        /// Returns a list of nearby vessels.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="includeSelf"></param>
        /// <param name="thisVessel">The current vessel to run the search from.</param>
        /// <returns>A list of nearby vessels.</returns>
        public static List<Vessel> GetNearbyVessels(float range, bool includeSelf, Vessel thisVessel)
        {
            try
            {
                var vesselList = new List<Vessel>();
                var vesselCount = FlightGlobals.Vessels.Count;
                for (int i = 0; i < vesselCount; ++i)
                {
                    var v = FlightGlobals.Vessels[i];
                    if (v == thisVessel && !includeSelf)
                    {
                        continue;
                    }
                    if (GetRange(thisVessel, v) < range)
                    {
                        vesselList.Add(v);
                    }
                }
                return vesselList;
            }
            catch (Exception ex)
            {
                Debug.Log(String.Format("[KMW] - ERROR in GetNearbyVessels ", ex.Message));
                return new List<Vessel>();
            }
        }

        /// <summary>
        /// An extension of GetNearbyVessels, but instead returns a dictionary with the vessels as key sand their distances from the current vessel as a value.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="includeSelf"></param>
        /// <param name="thisVessel"></param>
        /// <returns>Dictionary of form {Vessel, vessel's distance as a double}</returns>
        public static Dictionary<Vessel, double> GetNearbyVesselRanges(float range, bool includeSelf, Vessel thisVessel)
        {
            try
            {
                var vesselDict = new Dictionary<Vessel, double>();
                var vesselCount = FlightGlobals.Vessels.Count;
                for (int i = 0; i < vesselCount; ++i)
                {
                    var v = FlightGlobals.Vessels[i];
                    if (v == thisVessel && !includeSelf)
                    {
                        continue;
                    }
                    var _getrange = GetRange(thisVessel, v);
                    if (_getrange < range)
                    {
                        vesselDict.Add(v, _getrange);
                    }
                }
                return vesselDict;
            }
            catch (Exception ex)
            {
                Debug.Log(String.Format("[KMW] - ERROR in GetNearbyVesselRanges ", ex.Message));
                return new Dictionary<Vessel, double>();
            }
        }
    }
}
