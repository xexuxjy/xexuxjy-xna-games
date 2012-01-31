using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet;

namespace MagicCarpet.com.xexuxjy.magiccarpet.util
{
    public static class FractalUtil
    {

        // translated from : http://gameprogrammer.com/fractal.html

        public static float AvgEndpoints(int i, int stride, float[] fa)
        {
            return ((float)(fa[i - stride] +
                     fa[i + stride]) * .5f);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public static float AvgDiamondVals(int i, int j, int stride, int size, int subSize, float[] fa)
        {
            /* In this diagram, our input stride is 1, the i,j location is
               indicated by "X", and the four value we want to average are
               "*"s:
                   .   *   .

                   *   X   *

                   .   *   .
               */

            /* In order to support tiled surfaces which meet seamless at the
               edges (that is, they "wrap"), We need to be careful how we
               calculate averages when the i,j diamond center lies on an edge
               of the array. The first four 'if' clauses handle these
               cases. The final 'else' clause handles the general case (in
               which i,j is not on an edge).
             */
            if (i == 0)
            {
                return ((float)(fa[(i * size) + j - stride] +
                         fa[(i * size) + j + stride] +
                         fa[((subSize - stride) * size) + j] +
                         fa[((i + stride) * size) + j]) * .25f);
            }
            else if (i == size - 1)
            {
                return ((float)(fa[(i * size) + j - stride] +
                         fa[(i * size) + j + stride] +
                         fa[((i - stride) * size) + j] +
                         fa[((0 + stride) * size) + j]) * .25f);
            }
            else if (j == 0)
            {
                return ((float)(fa[((i - stride) * size) + j] +
                         fa[((i + stride) * size) + j] +
                         fa[(i * size) + j + stride] +
                         fa[(i * size) + subSize - stride]) * .25f);
            }
            else if (j == size - 1)
            {
                return ((float)(fa[((i - stride) * size) + j] +
                         fa[((i + stride) * size) + j] +
                         fa[(i * size) + j - stride] +
                         fa[(i * size) + 0 + stride]) * .25f);
            }
            else
            {
                return ((float)(fa[((i - stride) * size) + j] +
                         fa[((i + stride) * size) + j] +
                         fa[(i * size) + j - stride] +
                         fa[(i * size) + j + stride]) * .25f);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        /*
         * avgSquareVals - Given the i,j location as the center of a square,
         * average the data values at the four corners of the square and return
         * it. "Stride" represents half the length of one side of the square.
         *
         * Called by fill2DFractArray.
         */
        public static float AvgSquareVals(int i, int j, int stride, int size, float[] fa)
        {
            /* In this diagram, our input stride is 1, the i,j location is
               indicated by "*", and the four value we want to average are
               "X"s:
                   X   .   X

                   .   *   .

                   X   .   X
               */
            return ((float)(fa[((i - stride) * size) + j - stride] +
                     fa[((i - stride) * size) + j + stride] +
                     fa[((i + stride) * size) + j - stride] +
                     fa[((i + stride) * size) + j + stride]) * .25f);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        /*
         * fill1DFractArray - Tessalate an array of values into an
         * approximation of fractal Brownian motion.
         */
        public void Fill1DFractArray(float[] fa, int size, int seedValue, float heightScale, float h)
        {
            int i;
            int stride;
            int subSize;
            float ratio, scale;

            if (!IsPowerOfTwo(size) || (size == 1))
            {
                /* We can't tesselate the array if it is not a power of 2. */
                return;
            }

            /* subSize is the dimension of the array in terms of connected line
               segments, while size is the dimension in terms of number of
               vertices. */
            subSize = size;
            size++;


            /* Set up our roughness constants.
               Random numbers are always generated in the range 0.0 to 1.0.
               'scale' is multiplied by the randum number.
               'ratio' is multiplied by 'scale' after each iteration
               to effectively reduce the randum number range.
               */
            ratio = (float)Math.Pow(2.0f, -h);
            scale = heightScale * ratio;

            /* Seed the endpoints of the array. To enable seamless wrapping,
               the endpoints need to be the same point. */
            stride = subSize / 2;
            fa[0] = fa[subSize] = 0.0f;


            while (stride != 0)
            {
                for (i = stride; i < subSize; i += stride)
                {
                    fa[i] = scale * FractRand(0.5f) +
                        AvgEndpoints(i, stride, fa);

                    /* reduce random number range */
                    scale *= ratio;

                    i += stride;
                }
                stride >>= 1;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void Fill2DFractArray(float[] fa, int size, int seedValue, float heightScale, float h)
        {
            int i, j;
            int stride;
            bool oddline;
            int subSize;
            float ratio, scale;

            if (!IsPowerOfTwo(size) || (size == 1))
            {
                /* We can't tesselate the array if it is not a power of 2. */
                return;
            }

            /* subSize is the dimension of the array in terms of connected line
               segments, while size is the dimension in terms of number of
               vertices. */
            subSize = size;
            size++;

            /* initialize random number generator */
            //srandom (seedValue);


            /* Set up our roughness constants.
               Random numbers are always generated in the range 0.0 to 1.0.
               'scale' is multiplied by the randum number.
               'ratio' is multiplied by 'scale' after each iteration
               to effectively reduce the randum number range.
               */
            ratio = (float)Math.Pow(2.0f, -h);
            scale = heightScale * ratio;

            /* Seed the first four values. For example, in a 4x4 array, we
               would initialize the data points indicated by '*':

                   *   .   .   .   *

                   .   .   .   .   .

                   .   .   .   .   .

                   .   .   .   .   .

                   *   .   .   .   *

               In terms of the "diamond-square" algorithm, this gives us
               "squares".

               We want the four corners of the array to have the same
               point. This will allow us to tile the arrays next to each other
               such that they join seemlessly. */

            stride = subSize / 2;
            fa[(0 * size) + 0] =
              fa[(subSize * size) + 0] =
                fa[(subSize * size) + subSize] =
                  fa[(0 * size) + subSize] = 0.0f;


            /* Now we add ever-increasing detail based on the "diamond" seeded
               values. We loop over stride, which gets cut in half at the
               bottom of the loop. Since it's an int, eventually division by 2
               will produce a zero result, terminating the loop. */
            while (stride != 0)
            {
                /* Take the existing "square" data and produce "diamond"
                   data. On the first pass through with a 4x4 matrix, the
                   existing data is shown as "X"s, and we need to generate the
                   "*" now:

                       X   .   .   .   X

                       .   .   .   .   .

                       .   .   *   .   .

                       .   .   .   .   .

                       X   .   .   .   X

                  It doesn't look like diamonds. What it actually is, for the
                  first pass, is the corners of four diamonds meeting at the
                  center of the array. */
                for (i = stride; i < subSize; i += stride)
                {
                    for (j = stride; j < subSize; j += stride)
                    {
                        fa[(i * size) + j] = scale * FractRand(0.5f) + AvgSquareVals(i, j, stride, size, fa);
                        j += stride;
                    }
                    i += stride;
                }

                /* Take the existing "diamond" data and make it into
                   "squares". Back to our 4X4 example: The first time we
                   encounter this code, the existing values are represented by
                   "X"s, and the values we want to generate here are "*"s:

                       X   .   *   .   X

                       .   .   .   .   .

                       *   .   X   .   *

                       .   .   .   .   .

                       X   .   *   .   X

                   i and j represent our (x,y) position in the array. The
                   first value we want to generate is at (i=2,j=0), and we use
                   "oddline" and "stride" to increment j to the desired value.
                   */
                oddline = false;
                for (i = 0; i < subSize; i += stride)
                {
                    oddline = !oddline;
                    for (j = 0; j < subSize; j += stride)
                    {
                        if ((oddline) && j == 0)
                        {
                            j += stride;
                        }

                        /* i and j are setup. Call avgDiamondVals with the
                           current position. It will return the average of the
                           surrounding diamond data points. */
                        fa[(i * size) + j] = scale * FractRand(0.5f) + AvgDiamondVals(i, j, stride, size, subSize, fa);

                        /* To wrap edges seamlessly, copy edge values around
                           to other side of array */
                        if (i == 0)
                        {
                            fa[(subSize * size) + j] = fa[(i * size) + j];
                        }
                        if (j == 0)
                        {
                            fa[(i * size) + subSize] = fa[(i * size) + j];
                        }

                        j += stride;
                    }
                }

                /* reduce random number range. */
                scale *= ratio;
                stride >>= 1;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public static float FractRand(float range)
        {
            float minRange = -range;
            float random = (float)Globals.s_random.NextDouble();
            return (random * (range - minRange) + minRange);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	
        ///////////////////////////////////////////////////////////////////////////////////////////////	
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        // The following fractal height functions were taken from a MineCraft clone (reference needed)
        // but are attributed else where.


        public static int[] GenerateFractalTerrainMap(int width, int maxHeight)
        {

            float[] heightData = GetRandomHeightData(width);
            int[] heights = FillHeights(heightData, width, maxHeight);

            return heights;
        }

        /// <summary>
        /// Converts a noise map into an integer based height map.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="width">Width in number of cubes</param>
        /// <param name="maxHeight">Maximum number of stacked cubes</param>
        /// <returns></returns>
        public static int[] FillHeights(float[] heightData, int width, int maxHeight)
        {
            float min = float.MaxValue;
            float max = float.MinValue;

            //heightData includes an extra row and column that we don't want. 
            //By using x and y instead of looping directly through the array, 
            //we can easily avoid that extra row and column.

            //Noise map won't be from 0.0 to 1.0, but we need it that way.  So,
            //start by finding the minimum and maximum value of the map.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int index = y * width + x;
                    if (heightData[index] > max)
                    {
                        max = heightData[index];
                    }
                    if (heightData[index] < min)
                    {
                        min = heightData[index];
                    }
                }
            }

            //To make sure the range is all positive and starts with 0, we will
            //use an adjustment value to shift all values by.
            float adjust = -min;

            //To make sure the highest value is 1.0, we will divide all values
            //by the difference between minimum and maximum.
            float spread = (max + adjust) - (min + adjust);

            int[] heights = new int[width * width];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int index = y * width + x;
                    float heightRatio = (heightData[index] + adjust) / spread;
                    heights[index] = (int)(heightRatio * maxHeight);
                }
            }

            return heights;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        /// <summary>
        /// Method uses the Diamond-Square algorithm to populate a floating
        /// point noise map.
        /// </summary>
        /// <param name="width">The width of the map in cubes - must be a power
        /// of 2</param>
        /// <returns></returns>
        public static float[] GetRandomHeightData(int width)
        {
            //The width needs to be a power of 2 + 1 for the fractal algorithm to work.
            //width = width + 1;

            float[] heightData = new float[width * width];

            //Later on there will be overlap with the algorithm.  Filling the
            //array with max value to start will help use identify which values
            //have already been set.
            for (int index = 0; index < heightData.Length; index++)
            {
                heightData[index] = float.MaxValue;
            }

            //Initialize the 4 corners to seed values.
            int x = 0;
            int y = 0;
            heightData[y * width + x] = 0.0f;
            x = width - 1;
            y = 0;
            heightData[y * width + x] = 0.0f;
            x = width - 1;
            y = width - 1;
            heightData[y * width + x] = 0.0f;
            x = 0;
            y = width - 1;
            heightData[y * width + x] = 0.0f;

            //Iterate through diamond and square passes until the entire array
            //is populated.
            for (int power = 1; power < width; power *= 2)
            {
                DiamondPass(heightData, power, width);
                SquarePass(heightData, power, width);
            }

            return heightData;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        /// <summary>
        /// Completes a diamond pass on the entire array at the given power.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="power">A power of 2 number that helps to adjust stride
        /// values appropriate to the pass number</param>
        /// <param name="width">Width of the map in number of cubes - must be a
        /// power of 2</param>
        public static void DiamondPass(float[] heightData, int power, int width)
        {
            int jump = (width - 1) / power;
            float heightMultiplier = 5.0f;


            for (int x = 0; x < width - jump; x += jump)
            {
                for (int y = 0; y < width - jump; y += jump)
                {
                    int centerX = x + jump / 2;
                    int centerY = y + jump / 2;

                    if (heightData[centerY * width + centerX] == float.MaxValue)
                    {
                        float corner1 = heightData[y * width + x];
                        float corner2 = heightData[y * width + x + jump];
                        float corner3 = heightData[(y + jump) * width + x];
                        float corner4 = heightData[(y + jump) * width + x + jump];

                        float average = (corner1 + corner2 + corner3 + corner4) / 4.0f;

                        float value = average + (float)((Globals.s_random.NextDouble() - 0.5) * heightMultiplier);

                        heightData[centerY * width + centerX] = value;
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        /// <summary>
        /// Completes a square pass on the entire array at the given power.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="power">A power of 2 number that helps to adjust stride
        /// values appropriate to the pass number</param>
        /// <param name="width">Width of the map in number of cubes - must be a
        /// power of 2</param>
        public static void SquarePass(float[] heightData, int power, int width)
        {
            int jump = (width - 1) / power;

            for (int x = 0; x < width - jump; x += jump)
            {
                for (int y = 0; y < width - jump; y += jump)
                {
                    int stride = jump / 2;

                    int midX;
                    int midY;

                    //Top
                    midX = x + jump / 2;
                    midY = y;
                    SquareFill(heightData, stride, midX, midY, width);

                    //Bottom
                    midX = x + stride;
                    midY = y + jump;
                    SquareFill(heightData, stride, midX, midY, width);

                    //Left
                    midX = x;
                    midY = y + stride;
                    SquareFill(heightData, stride, midX, midY, width);

                    //Right
                    midX = x + jump;
                    midY = y + stride;
                    SquareFill(heightData, stride, midX, midY, width);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        /// <summary>
        /// Helper method for the square pass that does the actual filling in
        /// of data.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="stride">Distance from the center to look for seed
        /// values</param>
        /// <param name="x">The x coordinate for the center value to be changed</param>
        /// <param name="y">The y coordinate for the center value to be changed</param>
        /// <param name="width">The width of the map in number of cubes</param>
        public static void SquareFill(float[] heightData, int stride, int x, int y, int width)
        {
            if (heightData[y * width + x] == float.MaxValue)
            {
                int topX = x;
                int topY = y + stride;

                int bottomX = x;
                int bottomY = y - stride;

                int leftX = x - stride;
                int leftY = y;

                int rightX = x + stride;
                int rightY = y;

                //There will be out of bounds issues at the borders, so make the values "wrap"
                if (topY >= width)
                {
                    topY = stride;
                }

                if (bottomY < 0)
                {
                    bottomY = (width - 1) - stride;
                }

                if (leftX < 0)
                {
                    leftX = (width - 1) - stride;
                }

                if (rightX >= width)
                {
                    rightX = 0;
                }

                float topValue = heightData[topY * width + topX];
                float bottomValue = heightData[bottomY * width + bottomX];
                float leftValue = heightData[leftY * width + leftX];
                float rightValue = heightData[rightY * width + rightX];

                float average = (topValue + bottomValue + leftValue + rightValue) / 4.0f;

                float value = average + (float)((Globals.s_random.NextDouble() - 0.5) * 2.0);
                heightData[y * width + x] = value;
            }
        }
    }
}
