using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
