using System;
using System.Numerics;
using System.Linq;
using MathNet.Numerics;

namespace Algorithms
{
    class ContineousSeqA : IAlgorithm
    {
        private int[] m_state;
        private int m_current_pos;
        private Complex[] m_wf;
        // private delegate double addNoiseHandler();
        private Func<double> m_addNoise;
        
        public ContineousSeqA(Complex[] wf)
        {
            m_wf = new Complex[wf.Length];
            for (int i = 0; i < m_wf.Length; i++)
                m_wf[i] = wf[i];
            m_state = new int[wf.Length];
            GenerateInitialState();

            m_current_pos = 0;
        }

        public ContineousSeqA(Complex[] wf, Func<double> add_noise) : this(wf)
        {
            m_addNoise = add_noise;
        }

        private void GenerateInitialState()
        {
            m_state = Enumerable.Repeat(1, m_state.Length).ToArray();
        }

        public double Next()
        {
            // Calculate intensity before the optimization cycle
            double int_before = GetIntensity();

            // Add noise to the measurement
            if (m_addNoise != null)
            {
                int_before += m_addNoise();
                if (int_before < 0.0)
                    int_before = 0.0;
            }
            
            m_state[m_current_pos] = 1 - m_state[m_current_pos];

            // Calculate intensity before the optimization cycle
            double int_after = GetIntensity();

            // Add noise to the measurement
            if (m_addNoise != null)
            {
                int_after += m_addNoise();
                if (int_after < 0.0)
                    int_after = 0.0;
            }

            if (int_after < int_before)
            {
                m_state[m_current_pos] = 1 - m_state[m_current_pos];
                int_after = int_before;
            }

            if (++m_current_pos >= m_state.Length)
                m_current_pos = 0;

            return int_after;
        }

        public double GetIntensity()
        {
            Complex field_sum = new Complex();

            for (int i = 0; i < m_wf.Length; i++)
            {
                if (m_state[i] == 0)
                    continue;
                else
                    field_sum += m_wf[i];
            }

            return field_sum.MagnitudeSquared();
        }

        public int CurrentPosition
        {
            get
            {
                return m_current_pos;
            }
            set
            {
                m_current_pos = value;
            }
        }
    }
}