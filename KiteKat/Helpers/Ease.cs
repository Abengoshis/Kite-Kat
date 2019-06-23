using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiteKat.Helpers
{
    static class Ease
    {
        public static float Linear(float t)
        {
            return t;
        }

        public static float PowIn(float t, float power)
        {
            return (float)Math.Pow(t, power);
        }

        public static float PowOut(float t, float power)
        {
            return 1f - (float)Math.Pow(1f - t, power);
        }

        public static float PowInOut(float t, float power)
        {
            t *= 2f;
            if (t < 1f)
            {
                return 0.5f * PowIn(t, power);
            }
            else
            {
                return 1f - 0.5f * Math.Abs(PowIn(2f - t, power));
            }
        }

        public static float SineIn(float t)
        {
            return 1f - (float)Math.Cos(t * Math.PI / 2f);
        }

        public static float SineOut(float t)
        {
            return (float)Math.Sin(t * Math.PI / 2f);
        }

        public static float SineInOut(float t)
        {
            return 0.5f * (1f - (float)Math.Cos(t * Math.PI));
        }
    }
}
