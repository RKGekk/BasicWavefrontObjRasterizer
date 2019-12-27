using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public static class GeneralVariables {

        public const int MAX_RAY_DEPTH = 5;
        public const float INFINITY = 100000000.0f;
        public const float M_PI = 3.141592653589793f;
        public const float EPSILON_E3 = 0.001f;
        public const float EPSILON_E4 = 0.0001f;
        public const float EPSILON_E5 = 0.00001f;
        public const float EPSILON_E6 = 0.000001f;

        public static float mix(float a, float b, float mix) {
            return b * mix + a * (1 - mix);
        }

        public static bool FCMP(float a, float b) {
            return ((float)Math.Abs(a - b) < GeneralVariables.EPSILON_E3) ? true : false;
        }
    }
}
