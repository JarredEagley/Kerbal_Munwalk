using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Munwalk
{
    /// <summary>
    /// Class which encapsilates the line game object and its renderer game object for convenience.
    /// The line renderer will be left public for further convenience. This class just boxes it up with the GameObject.
    /// </summary>
    public class MW_ArrowGenerator
    {
        private GameObject lineObj = new GameObject("Line");
        public LineRenderer lineRenderer;

        public MW_ArrowGenerator(Vector3d initialTransform, bool useWorldSpace = false, Vector3? endPosition = null, Color? startColor = null, Color? endColor = null, float startWidth = 1, float endWidth = 0)
        {
            // Set default values for colors because they don't like being given default values like literally anything else.
            if (startColor == null)
            {
                startColor = Color.red;
            }
            if (endColor == null)
            {
                endColor = Color.yellow;
            }
            // Vectors are also a special snowflake.
            if (endPosition == null)
            {
                endPosition = new Vector3(0, 0, 0);
            }

            // Define the line renderer.
            this.lineRenderer = lineObj.AddComponent<LineRenderer>();

            // Set the material. I don't forsee myself ever needing to change this so it'll stay hardcoded for now.
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

            // Apply initial line params.

            this.lineRenderer.transform.localPosition = initialTransform;
            this.lineRenderer.useWorldSpace = useWorldSpace;
            this.lineRenderer.SetPosition(1, (Vector3)endPosition);

            this.lineRenderer.startColor = (Color)startColor;
            this.lineRenderer.endColor = (Color)endColor;

            this.lineRenderer.startWidth = startWidth;
            this.lineRenderer.endWidth = endWidth;

        }

        // ORIGINAL LINE STUFF FOR REFERENCE
        //_line.useWorldSpace = false;
        //_line.transform.localPosition = FlightGlobals.ActiveVessel.CoM;
        //_line.material = new Material(Shader.Find("Particles/Additive"));
        //_line.startColor = Color.red;
        //_line.startWidth = .1f;
        //_line.endWidth = 0;
        //_line.SetPosition(1, accel);


    }
}
