using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CWExpert
{
    public class IQBalancer
    {
        #region structures

        [StructLayout(LayoutKind.Sequential)]
        public struct IQ
        {
            public float phase;
            public float gain;
            public float mu;
            public float leakage;
            public ComplexF[] w;
            public ComplexF[] b;
            public ComplexF[] save;
            public ComplexF[] del;
            public ComplexF[] y;
            public ComplexF[] dc;
            public int MASK;
            public int index;
        }

        #endregion

        #region variable

        delegate void CrossThreadSetText(string command, int channel_no, string out_txt);

        private const int FFTBins = 2048;
        private const float DcTimeConst = 0.001f;
        private const float BaseIncrement = 0.001f;

        private int _maxAutomaticPasses = 1;
        private bool _autoBalanceIQ = true;
        private float _averageI;
        private float _averageQ;
        private float _gain = 1.0f;
        private float _phase = 0.0f;
        private float[] window = new float[2048];
        private readonly Random _rng = new Random();
        public const float MaxPower = 0.0f;
        public const float MinPower = -130.0f;
        private const double TWOPI = 6.28318530717856;
        ComplexF[] fftPtr;
        ComplexF[] zero;
        float[] spectrumPtr;
        private CWExpert MainForm;
        Fourier fft;
        IQ iqfix = new IQ();

        #endregion

        #region properties

        public float Phase
        {
            get { return (float)Math.Asin(_phase); }
            set { _phase = 0.001f * value; }
        }

        public float Gain
        {
            get { return _gain; }
            set { _gain = 1.0f + 0.001f * value; }
        }

        public int MaxAutomaticPasses
        {
            get { return _maxAutomaticPasses; }
            set { _maxAutomaticPasses = value; }
        }

        public bool AutoBalanceIQ
        {
            get { return _autoBalanceIQ; }
            set { _autoBalanceIQ = value; }
        }

        #endregion

        #region constructor/destructor

        public IQBalancer(CWExpert form)
        {
            makewindow(2048, ref window);
            fft = new Fourier();
            fftPtr = new ComplexF[4096];
            spectrumPtr = new float[4096];
            zero = new ComplexF[2048];
            MainForm = form;
            iqfix.phase = 0.0f;
            iqfix.gain = 1.0f;
            iqfix.mu = 0.0f;
            iqfix.leakage = 0.000000f;
            iqfix.MASK = 2047;
            iqfix.index = 0;
            iqfix.w = new ComplexF[2048];
            iqfix.b = new ComplexF[2048];
            iqfix.y = new ComplexF[2048];
            iqfix.del = new ComplexF[2048];
            Reset(FFTBins);
        }

        ~IQBalancer()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region misc function

        public void Reset(int length)
        {
            try
            {
                iqfix.mu = 0.0f;

                for (int i = 0; i < 16; i++)
                {
                    iqfix.w[i].Re = 0.0f;
                    iqfix.w[i].Im = 0.0f;
                    iqfix.y[i].Re = 0.0f;
                    iqfix.y[i].Im = 0.0f;
                    iqfix.del[i].Re = 0.0f;
                    iqfix.del[i].Im = 0.0f;
                }

                for (int i = 0; i < length * 2; i++)
                {
                    spectrumPtr[i] = MinPower;
                    fftPtr[i].Re = 0.0f;
                    fftPtr[i].Im = 0.0f;
                }

                _gain = 1.0f;
                _phase = 0.0f;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public void Process(ref ComplexF[] iq, int length)
        {
            switch (MainForm.IQcorrection)
            {
                case IQ_correction.BALANCED:
                    RemoveDC(ref iq, length);

                    var currentUtility = Utility(ref iq, _phase, _gain);

                    for (var count = 0; count < _maxAutomaticPasses; count++)
                    {
                        var phaseIncrement = BaseIncrement * (float)(_rng.NextDouble() - 0.5) * 2.0f;
                        var gainIncrement = BaseIncrement * (float)(_rng.NextDouble() - 0.5) * 2.0f;

                        var candidatePhase = _phase + phaseIncrement;
                        var candidateGain = _gain + gainIncrement;
                        var candidateUtility = Utility(ref iq, candidatePhase, candidateGain);

                        if (candidateUtility > currentUtility)
                        {
                            currentUtility = candidateUtility;
                            _gain = candidateGain;
                            _phase = candidatePhase;
                        }
                    }

                    for (var i = 0; i < length; i++)
                    {
                        iq[i].Im += _phase * iq[i].Re;
                        iq[i].Re *= _gain;
                        //iq[i].Im = (float)(iq[i].Im * (1.0 + _gain) * Math.Cos(_phase));
                        //iq[i].Re = (float)(iq[i].Re - iq[i].Im * Math.Sin(_phase));
                    }
                    break;

                case IQ_correction.WBIR:
                    correctIQ(ref iq, length);
                    break;

                case IQ_correction.FIXED:
                    Utility(ref iq, _phase, _gain);

                    for (var i = 0; i < length; i++)
                    {
                        iq[i].Re += _phase * iq[i].Im;
                        iq[i].Im *= _gain;

                    }
                    break;
            }
        }

        private void RemoveDC(ref ComplexF[] iq, int length)
        {
            for (var i = 0; i < length; i++)
            {
                // I branch
                var temp = _averageI * (1 - DcTimeConst) + iq[i].Re * DcTimeConst;

                if (!float.IsNaN((float)temp))
                {
                    _averageI = (float)temp;
                }
                iq[i].Re = iq[i].Re - _averageI;

                // Q branch
                temp = _averageQ * (1 - DcTimeConst) + iq[i].Im * DcTimeConst;
                if (!float.IsNaN((float)temp))
                {
                    _averageQ = (float)temp;
                }
                iq[i].Im = iq[i].Im - _averageQ;
            }
        }

        private float Utility(ref ComplexF[] iq, float phase, float gain)
        {
            float[] spectrum = new float[4096];
            Array.Copy(iq, 0, fftPtr, 2048, 2048);
            //Array.Copy(zero, 0, fftPtr, 0, 2048);

            for (var i = 0; i < 2048; i++)
            {
                fftPtr[i].Im += phase * fftPtr[i].Re;
                fftPtr[i].Re *= gain;
                fftPtr[i].Re *= window[i];
                fftPtr[i].Im *= window[i];
            }

            fft.FFT_Quick(fftPtr, 4096, FourierDirection.Forward);
            SpectrumPower(ref fftPtr, ref spectrumPtr, 4096, 50.0f);
            Array.Copy(spectrumPtr, 0, spectrum, 2048, 2048);
            Array.Copy(spectrumPtr, 2048, spectrum, 0, 2048);
            /*Array.Copy(spectrumPtr, 0, DX.new_display_data, 2048, 2048);
            Array.Copy(spectrumPtr, 2048, DX.new_display_data, 0, 2048);
            Array.Copy(DX.new_display_data, DX.new_waterfall_data, 4096);*/

            var result = 0.0f;

            for (var i = 0; i < 4096 / 2; i++)
            {
                var distanceFromCenter = 4096 / 2 - i;

                if (distanceFromCenter > (0.05f * 4096 / 2))
                {
                    result += Math.Abs(spectrum[i] - spectrum[4096 - 2 - i]);
                }
            }

            Array.Copy(iq, 0, fftPtr, 0, 2048);

            return result;
        }

        public void SpectrumPower(ref ComplexF[] buffer, ref float[] power, int length, float offset)
        {
            for (var i = 0; i < length; i++)
            {
                var m = buffer[i].Re * buffer[i].Re + buffer[i].Im * buffer[i].Im;
                var strength = (float)(10 * Math.Log10(m));

                if (float.IsNaN(strength))
                {
                    strength = MinPower;
                }
                else if (strength > MaxPower)
                {
                    strength = MaxPower;
                }
                else if (strength < MinPower)
                {
                    strength =  MinPower;
                }

                power[i] = strength;
            }
        }

        #endregion

        #region Window

        private void makewindow(int size, ref float[] window)
        {
            int i, j, midn;
            float freq, angle;
            midn = size >> 1;

            freq = (float)(TWOPI / size);

            for (i = 0, j = size - 1, angle = 0.0f; i <= midn;
             i++, j--, angle += freq)
                window[j] = (window[i] = (float)(0.5 - 0.5 * Math.Cos(angle)));
        }

        #endregion

        #region COMPLEX calculations

        private ComplexF CsubF(ComplexF x, ComplexF y)
        {
            ComplexF z;
            z.Re = x.Re - y.Re;
            z.Im = x.Im - y.Im;
            return z;
        }

        private ComplexF ConjgF(ComplexF z)
        {
            return CmplxF(z.Re, -z.Im);
        }

        private ComplexF CaddF(ComplexF x, ComplexF y)
        {
            ComplexF z;
            z.Re = x.Re + y.Re;
            z.Im = x.Im + y.Im;
            return z;
        }

        private float Cabs(ComplexF z)
        {
            return (float)Math.Sqrt(z.Re * z.Re + z.Im * z.Im);
        }

        private ComplexF CsclF(ComplexF x, float a)
        {
            ComplexF z;
            z.Re = x.Re * a;
            z.Im = x.Im * a;
            return z;
        }

        private ComplexF CmulF(ComplexF x, ComplexF y)
        {
            ComplexF z;
            z.Re = x.Re * y.Re - x.Im * y.Im;
            z.Im = x.Im * y.Re + x.Re * y.Im;
            return z;
        }

        private float CmagF(ComplexF z)
        {
            return (float)Math.Sqrt(z.Re * z.Re + z.Im * z.Im);
        }

        private float CsqrmagF(ComplexF z)
        {
            return (float)(z.Re * z.Re + z.Im * z.Im);
        }

        private ComplexF CmplxF(float x, float y)
        {
            ComplexF z;
            z.Re = x;
            z.Im = y;
            return z;
        }

        private ComplexF Cclamp(ComplexF x)
        {
            ComplexF z;

            z = x;

            z.Re = (float)Math.Min(z.Re, 1.0);
            z.Re = (float)Math.Max(z.Re, -1.0);

            z.Im = (float)Math.Min(z.Im, 1.0);
            z.Im = (float)Math.Max(z.Im, -1.0);


            return z;
        }

        #endregion

        Random rnd = new Random(564561);
        private void correctIQ(ref ComplexF[] input_buf, int length)
        {
            int i = 0;
            ComplexF tmp = new ComplexF();
            iqfix.mu = (float)rnd.NextDouble();

            for (i = 0; i < length; i++)
            {
                iqfix.index = i;
                iqfix.del[iqfix.index] = Cclamp(input_buf[i]);
                iqfix.y[iqfix.index] = Cclamp(CaddF(iqfix.del[iqfix.index],
                    CmulF(CsclF(iqfix.w[iqfix.index], 100.0f), ConjgF(iqfix.del[iqfix.index]))));
                iqfix.y[iqfix.index] = Cclamp(CaddF(iqfix.y[iqfix.index],
                    CmulF(CsclF(iqfix.w[iqfix.index], 100.0f), ConjgF(iqfix.y[iqfix.index]))));
                tmp = Cclamp(CsubF(iqfix.w[iqfix.index], CsclF(CmulF(iqfix.y[iqfix.index],
                    iqfix.y[iqfix.index]), iqfix.mu)));  // this is where the adaption happens

                if (tmp.Re < 1.0f || tmp.Im < 1.0f)
                    iqfix.w[iqfix.index] = tmp;
                else
                {
                    iqfix.w[iqfix.index].Im = 0.0f;  // iqfix.b[0].Im;		// reset
                    iqfix.w[iqfix.index].Re = 0.0f;  // iqfix.b[0].Re;
                }

                input_buf[i] = iqfix.y[iqfix.index];

                /*j++;

                if (j == 128)
                {
                    iqfix.index = (iqfix.index + iqfix.MASK) & iqfix.MASK;
                    j = 0;
                }*/
            }

            //iqfix.index = (iqfix.index + iqfix.MASK) & iqfix.MASK;
        }
    }
}
