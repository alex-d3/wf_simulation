using System;
using System.Numerics;
using System.IO;

using Algorithms;

namespace WFSimulation
{
    public static class AlgorithmRunner
    {
        public static IAlgorithm Run(IAlgorithm alg, int iterations, bool verbose = false)
        {
            for (int i = 0; i < iterations; i++)
            {
                double intens = alg.Next();

                if (verbose)
                {
                    Console.WriteLine("i = {0}: intensity = {1}", i, intens);
                }
            }

            return alg;
        }

        public static IAlgorithm Run(IAlgorithm alg, int iterations, string out_file)
        {
            StreamWriter sw = new StreamWriter(out_file);
            for (int i = 0; i < iterations; i++)
            {
                double intens = alg.Next();
                sw.WriteLine("{0},{1:e}", i, intens);
            }
            sw.Close();

            return alg;
        }
    }
}