using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SoraHub_CS2
{
    public static class Calculate
    {
        // thanks yallah for CHaTGpt T^T
        public static Vector2 WorldToScreen(float[] matrix , Vector3 pos , Vector2 windowSize)
        {
            // calculate screenW (Depth Variable)
            float screenW = (matrix[12] * pos.X) + (matrix[13] * pos.Y) + (matrix[14] * pos.Z) + matrix[15];

            if (screenW > 0.001f)
            {
                //calculate screen X & Y

                float screenX = (matrix[0] * pos.X) + (matrix[1] * pos.Y) + (matrix[2] * pos.Z) + matrix[3];
                float screenY = (matrix[4] * pos.X) + (matrix[5] * pos.Y) + (matrix[6] * pos.Z) + matrix[7];

                // find perspective division

                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                return new Vector2(X, Y);
            }
            else
            {
                // not in our camera thus return outof Bounces
                return new Vector2(-99, -99);
            }

        }
    }
}
