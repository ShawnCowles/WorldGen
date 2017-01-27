using System;

namespace WorldGen.Generator
{
    /* From: https://gist.github.com/jstanden */

    /// <summary>
    /// A simplex noise generator, adapted from: https://gist.github.com/jstanden/1489447
    /// </summary>
    [Serializable]
    public class SimplexNoiseGenerator
    {
        private int[] A = new int[3];
        private float s, u, v, w;
        private int i, j, k;
        private float onethird = 0.333333333f;
        private float onesixth = 0.166666667f;
        private int[] T;

        /// <summary>
        /// Create a new SimplexNoiseGenerator.
        /// </summary>
        /// <param name="rngSeed">The seed for the noise generator.</param>
        public SimplexNoiseGenerator(int rngSeed)
        {
            if (T == null)
            {
                var rand = new Random(rngSeed);
                T = new int[8];
                for (int q = 0; q < 8; q++)
                {
                    T[q] = rand.Next();
                }
            }
        }

        /// <summary>
        /// Create a new SimplexNoiseGenerator.
        /// </summary>
        /// <param name="seed">The seed for the noise generator.</param>
        public SimplexNoiseGenerator(string seed)
        {
            T = new int[8];
            string[] seed_parts = seed.Split(new char[] { ' ' });

            for (int q = 0; q < 8; q++)
            {
                int b;
                try
                {
                    b = int.Parse(seed_parts[q]);
                }
                catch
                {
                    b = 0x0;
                }
                T[q] = b;
            }
        }

        /// <summary>
        /// Create a new SimplexNoiseGenerator.
        /// </summary>
        /// <param name="seed">The seed for the noise generator.</param>
        public SimplexNoiseGenerator(int[] seed)
        { // {0x16, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a}
            T = seed;
        }

        //public string GetSeed()
        //{
        //    string seed = "";

        //    for (int q = 0; q < 8; q++)
        //    {
        //        seed += T[q].ToString();
        //        if (q < 7)
        //            seed += " ";
        //    }

        //    return seed;
        //}

        /// <summary>
        /// Return the noise value at a position.
        /// </summary>
        /// <param name="x">The x position for the noise.</param>
        /// <param name="y">The y position for the noise.</param>
        /// <param name="z">The z position for the noise.</param>
        /// <param name="octaves">The number of passes to be summed, higher values are slower but add more detail.</param>
        /// <param name="multiplier">The scale of the noise generated, which results in larger or smaller features.</param>
        /// <param name="amplitude">The amplitude of the noise generated.</param>
        /// <param name="lacunarity">The scale multiple factor between each pass. Higher values add detail.</param>
        /// <param name="persistence">How much each additional pass adds to the result. Should be less than 1.</param>
        /// <returns>The noise value at a position.</returns>
        public float CoherentNoise(float x, float y, float z, int octaves = 1, int multiplier = 25, float amplitude = 0.5f, float lacunarity = 2, float persistence = 0.9f)
        {
            var v3x = x / multiplier;
            var v3y = y / multiplier;
            var v3z = z / multiplier;

            float val = 0;
            for (int n = 0; n < octaves; n++)
            {
                val += noise(v3x, v3y, v3z) * amplitude;

                v3x *= lacunarity;
                v3y *= lacunarity;
                v3z *= lacunarity;

                amplitude *= persistence;
            }
            return val;
        }
        
        private float noise(float x, float y, float z)
        {
            s = (x + y + z) * onethird;
            i = fastfloor(x + s);
            j = fastfloor(y + s);
            k = fastfloor(z + s);

            s = (i + j + k) * onesixth;
            u = x - i + s;
            v = y - j + s;
            w = z - k + s;

            A[0] = 0; A[1] = 0; A[2] = 0;

            int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
            int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;

            return kay(hi) + kay(3 - hi - lo) + kay(lo) + kay(0);
        }

        private float kay(int a)
        {
            s = (A[0] + A[1] + A[2]) * onesixth;
            float x = u - A[0] + s;
            float y = v - A[1] + s;
            float z = w - A[2] + s;
            float t = 0.6f - x * x - y * y - z * z;
            int h = shuffle(i + A[0], j + A[1], k + A[2]);
            A[a]++;
            if (t < 0) return 0;
            int b5 = h >> 5 & 1;
            int b4 = h >> 4 & 1;
            int b3 = h >> 3 & 1;
            int b2 = h >> 2 & 1;
            int b1 = h & 3;

            float p = b1 == 1 ? x : b1 == 2 ? y : z;
            float q = b1 == 1 ? y : b1 == 2 ? z : x;
            float r = b1 == 1 ? z : b1 == 2 ? x : y;

            p = b5 == b3 ? -p : p;
            q = b5 == b4 ? -q : q;
            r = b5 != (b4 ^ b3) ? -r : r;
            t *= t;
            return 8 * t * t * (p + (b1 == 0 ? q + r : b2 == 0 ? q : r));
        }

        private int shuffle(int i, int j, int k)
        {
            return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
        }

        private int b(int i, int j, int k, int B)
        {
            return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
        }

        private int b(int N, int B)
        {
            return N >> B & 1;
        }

        private int fastfloor(float n)
        {
            return n > 0 ? (int)n : (int)n - 1;
        }
    }
}
