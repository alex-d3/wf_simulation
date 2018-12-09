using System;
using System.Numerics;
using System.Linq;
using MathNet.Numerics;

namespace Algorithms
{
    class Random50A : IAlgorithm
    {
        private int[] m_state;
        private int[] m_state_opt;
        private Complex[] m_wf;
        private Random m_rnd;
        public Random50A(Complex[] wf)
        {
            m_rnd = new Random();
            m_wf = new Complex[wf.Length];
            for (int i = 0; i < m_wf.Length; i++)
                m_wf[i] = wf[i];
            m_state = new int[wf.Length];
            m_state_opt = new int[wf.Length];
            GenerateInitialState();
        }

        private void GenerateInitialState()
        {
            m_state = Enumerable.Repeat(1, m_state.Length).ToArray();
        }

        public double Next()
        {
            int[] selection = new int[m_state.Length / 2];

            // Select half of the modulator pixels
            for (int i = 0; i < selection.Length; i++)
            {

                int pos = m_rnd.Next(m_wf.Length);
                if (selection.Contains(pos))
                    i--;
                else
                    selection[i] = pos;
            }

            // Calculate intensity before the optimization cycle
            double int_before = GetIntensity();

            // Set new modulator state
            for (int i = 0; i < selection.Length; i++)
                m_state[selection[i]] = 1 - m_state[selection[i]];
            
            // Calculate intensity after the optimization cycle
            double int_after = GetIntensity();

            if (int_after < int_before)
            {
                // Set previous modulator state
                for (int i = 0; i < selection.Length; i++)
                    m_state[selection[i]] = 1 - m_state[selection[i]];
                
                return int_before;
            }
            else
            {
                return int_after;
            }
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

        public void SetState(int[] state)
        {
            for (int i = 0; i < m_state.Length; i++)
                m_state[i] = state[i];
        }
    }
}