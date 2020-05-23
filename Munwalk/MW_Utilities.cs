using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Munwalk
{
    /// <summary>
    /// Class which provides some useful methods used in Kerbal Munwalk
    /// </summary>
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
        // protected bool RayCast(float rayLength, GameObject rayObject, Vector3 direction, out RaycastHit hit)
        // {
        //     var ray = new Ray(rayObject.transform.position, direction);
        //     int tempLayerMask = ~layerMask;
        //     return Physics.Raycast(ray, out hit, rayLength, tempLayerMask);
        // }

        /// <summary>
        /// Given an angle in radians return the equivalent angle in degrees.
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double radToDeg(double rad)
        {
            return rad * (180 / Math.PI);
        }

        /// <summary>
        /// Return the angle between two input vectors.
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static double VectorAngle(Vector3d vec1, Vector3d vec2)
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

        /// <summary>
        /// Return the norm of a given vector.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static double VectorNorm(Vector3d vec)
        {
            return Math.Sqrt((vec.x * vec.x) + (vec.y * vec.y) + (vec.z * vec.z));
        }

        /// <summary>
        /// Given an array of vectors, calculate the vector which averages between them.
        /// </summary>
        /// <param name="vectorArray"></param>
        /// <returns></returns>
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
        public static List<Vessel> GetNearbyVessels(float range, bool includeSelf = false, Vessel thisVessel)
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
        public static Dictionary<Vessel, double> GetNearbyVesselRanges(float range, bool includeSelf = false, Vessel thisVessel)
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

        public static Vessel GetNearestVessel(float range, Vessel thisVessel)
        {
            // TO-DO
            try
            {
                GetNearestVessel = thisVessel;
            }
            catch (Exception ex)
            {

            }
        }

        public static void GetNearestPart()
        {
            // TO-DO
        }
    }


}
