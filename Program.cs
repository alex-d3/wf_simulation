using System;
using System.Numerics;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using CommandLine;

using Algorithms;

namespace WFSimulation
{
    class Program
    {
        static Complex[] RandomNormalComplex(double mean, double stdev, int count)
        {
            Normal n_distr = new Normal(mean, stdev);
            Complex[] c_array = new Complex[count];

            for (int i = 0; i < count; i++)
            {
                c_array[i] = new Complex(n_distr.Sample(), n_distr.Sample());
            }

            return c_array;
        }

        public static double GenerateGaussianNoise()
        {
            Normal rnd = new Normal(0.0, 3000.0);
            return rnd.Sample();
        }
        static void Main(string[] args)
        {
            Console.Write("Channels number: ");
            string ch_num_string = Console.ReadLine();
            int ch_num = Convert.ToInt32(ch_num_string);

            Console.Write("Loops count: ");
            string loops_count_string = Console.ReadLine();
            int loops_c = Convert.ToInt32(loops_count_string);

            // Calculate reference intensity
            double ref_intensity = 0.0;
            for (int i = 0; i < 100000; i++)
            {
                Complex[] wavefront = RandomNormalComplex(0.0, 3.0, ch_num);
                Complex c_wave = new Complex();
                for (int j = 0; j < wavefront.Length; j++)
                    c_wave += wavefront[j];
                
                ref_intensity += c_wave.MagnitudeSquared();
            }
            ref_intensity /= 100000;
            Console.WriteLine("Reference intensity: {0}", ref_intensity);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();

            Complex[] wf = RandomNormalComplex(0.0, 3.0, ch_num);

            Console.WriteLine("Contineous sequential algorithm (binary amplitude) w/o noise...");
            ContineousSeqA csa_wo_noise = new ContineousSeqA(wf);
            csa_wo_noise = (ContineousSeqA)AlgorithmRunner.Run(csa_wo_noise, ch_num * loops_c, "/tmp/seq_wo_noise.txt");
            double intensity_wo_noise = csa_wo_noise.GetIntensity();
            Console.WriteLine("I_opt = {0:e}, enh. = {1}", intensity_wo_noise, 2 * intensity_wo_noise / ref_intensity);

            Console.WriteLine("Contineous sequential algorithm (binary amplitude) w/o noise...");
            ContineousSeqA csa_w_noise = new ContineousSeqA(wf, GenerateGaussianNoise);
            csa_w_noise = (ContineousSeqA)AlgorithmRunner.Run(csa_w_noise, ch_num * loops_c, "/tmp/seq_w_noise.txt");
            double intensity_w_noise = csa_w_noise.GetIntensity();
            Console.WriteLine("I_opt = {0:e}, enh. = {1}", intensity_w_noise, 2 * intensity_w_noise / ref_intensity);

            Console.WriteLine("End of run. Press ENTER to quit...");
            Console.ReadLine();
        }
    }
}
