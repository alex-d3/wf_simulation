using System;
using System.Numerics;
using System.Linq;
using MathNet.Numerics;

namespace Algorithms
{
    class ContineousSeqP : IAlgorithm
    {
        private Complex[] m_state;
        private int m_phase_steps;
        private int m_current_pos;
        private Complex[] m_wf;
        private delegate double IntensityNoise();
        public ContineousSeqP(Complex[] wf, int phase_steps = 256)
        {
            m_wf = new Complex[wf.Length];
            for (int i = 0; i < m_wf.Length; i++)
                m_wf[i] = wf[i];
            m_state = new Complex[wf.Length];
            GenerateInitialState();
            m_phase_steps = phase_steps;

            m_current_pos = 0;
        }

        private void GenerateInitialState()
        {
            for (int i = 0; i < m_state.Length; i++)
                m_state[i] = new Complex(1.0, 0.0);
        }

        public double Next()
        {
            // Calculate intensity before the optimization cycle
            double int_before = GetIntensity();

            // Adjust phase
            Complex pix_state_initial = m_state[m_current_pos];
            double phase;
            int phase_max_int = 0;
            Complex phase_c;
            double int_after;
            for (int i = 1; i < m_phase_steps; i++)
            {
                phase = 2.0 * Math.PI * i / (m_phase_steps - 1);
                phase_c = new Complex(Math.Cos(phase), Math.Sin(phase));

                m_state[m_current_pos] = pix_state_initial * phase_c;

                int_after = GetIntensity();

                if (int_after > int_before)
                {
                    phase_max_int = i;
                    int_before = int_after;
                }
            }

            if (phase_max_int != 0 && phase_max_int != (m_phase_steps - 1))
            {
                phase = 2.0 * Math.PI * phase_max_int / (m_phase_steps - 1);
                phase_c = new Complex(Math.Cos(phase), Math.Sin(phase));
                m_state[m_current_pos] = pix_state_initial * phase_c;
            }
            else
            {
                m_state[m_current_pos] = pix_state_initial;
            }
            

            // Calculate intensity before the optimization cycle
            int_after = GetIntensity();

            if (++m_current_pos >= m_state.Length)
                m_current_pos = 0;

            return int_after;
        }

        public double GetIntensity()
        {
            Complex field_sum = new Complex();

            for (int i = 0; i < m_wf.Length; i++)
            {
                field_sum += m_state[i] * m_wf[i];
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