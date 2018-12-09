using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;

namespace Algorithms
{
    public class LFSR_N
    {
        public static Dictionary<int, int[]> default_taps = new Dictionary<int, int[]>(){
            {10, new int[] {6, 4, 2}}
        };

        BitArray m_register;
        private int[] m_taps;

        public LFSR_N(int length)
        {
            m_register = new BitArray(length);
            m_register.SetAll(true);
            GenerateInitialState();
            SetDefaultTaps(length);
        }

        private void GenerateInitialState()
        {
            m_register.SetAll(true);
        }

        private void ResetRegister()
        {
            GenerateInitialState();
        }

        public void SetRegister(BitArray register)
        {
            //
        }

        private void SetDefaultTaps(int reg_length)
        {
            if (default_taps.ContainsKey(reg_length))
                m_taps = default_taps[reg_length];
            else
                throw new NotImplementedException(
                    String.Format("Theare are no taps for a register with length equals to {0}.", reg_length));
        }

        private bool Shift()
        {
            bool return_value = m_register[0];
            // int return_value = m_register[0] ? 1 : 0;

            bool next_bit = m_register[0];

            for (int i = 0; i < m_taps.Length; i++)
                next_bit ^= m_register[m_taps[i]];
            
            m_register = m_register.RightShift(1);

            m_register[m_register.Length - 1] = next_bit;

            return return_value;
        }

        public BitArray GenerateMLS()
        {
            int mls_length = (int)Math.Pow(2, m_register.Length) - 1;
            BitArray mls = new BitArray(mls_length);

            for (int i = 0; i < mls.Length; i++)
                mls[i] = Shift();

            return mls;
        }

    }
    public class MSequenceP : IAlgorithm
    {
        private Complex[] m_state;
        private Complex[] m_wf;

        public MSequenceP(Complex[] wf, int phase_steps = 256)
        {
            m_wf = new Complex[wf.Length];
            for (int i = 0; i < m_wf.Length; i++)
                m_wf[i] = wf[i];
            m_state = new Complex[wf.Length];

        }

        public double Next()
        {
            return 1.0;
        }

        public double GetIntensity()
        {
            return 1.0;
        }
    }
}