using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace CWExpert
{
    public class FFTW
    {
        #region variables

        //pointer to main form
        private int fft_len = 128;
        private int spectrum_len = 2048;

        //pointers to unmanaged arrays
        public IntPtr pin, pout;
        public IntPtr spec_in, spec_out;

        //managed arrays
        public float[] fin, fout;
        public float[] spec_fin, spec_fout;

        //handles to managed arrays, keeps them pinned in memory
        //        public GCHandle hin, hout;

        //pointers to the FFTW plan objects
        IntPtr fplan1;
        IntPtr fplan2;

        #endregion

        // Initializes FFTW and all arrays
        // n: Logical size of the transform
        unsafe public void InitFFTW(int n, int spectrum)
        {
            byte[] wisdom = new byte[8192];
  
            fixed (byte* wisdom_string = &(wisdom[0]))
            {
/*                if (File.Exists(".\\wisdom"))
                {
                    FileStream file = File.Open(".\\wisdom", FileMode.Open, FileAccess.Read);
                    file.Read(wisdom, 0, (int)file.Length);
                    for (int i = 0; i < file.Length; i++)
                        wisdom_string[i] = wisdom[i];
                    t = fftwf.fftw_import(wisdom_string);
                }*/
            }

            fft_len = n;
            spectrum_len = spectrum;

            //create two unmanaged arrays, properly aligned
            pin = fftwf.malloc(n * 8);
            pout = fftwf.malloc(n * 8);
            spec_in = fftwf.malloc(spectrum * 8);
            spec_out = fftwf.malloc(spectrum * 8);

            //create two managed arrays, possibly misalinged
            //n*2 because we are dealing with complex numbers
            fin = new float[n * 2];
            fout = new float[n * 2];
            spec_fin = new float[spectrum_len * 2];
            spec_fout = new float[spectrum_len * 2];

            //get handles and pin arrays so the GC doesn't move them
            //            hin = GCHandle.Alloc(fin, GCHandleType.Pinned);
            //            hout = GCHandle.Alloc(fout, GCHandleType.Pinned);

            //create a few test transforms
            fplan1 = fftwf.dft_1d(n, pin, pout, fftw_direction.Forward, fftw_flags.Patient);
            fplan2 = fftwf.dft_1d(spectrum, spec_in, spec_out, fftw_direction.Forward, fftw_flags.Patient);
            /*            fplan2 = fftwf.dft_1d(n, hin.AddrOfPinnedObject(), hout.AddrOfPinnedObject(),
                            fftw_direction.Forward, fftw_flags.Estimate);
                        fplan3 = fftwf.dft_1d(n, hout.AddrOfPinnedObject(), pin,
                            fftw_direction.Backward, fftw_flags.Measure);*/
        }

        public bool ComputeFFT()
        {
            try
            {
                fftwf.execute(fplan1);
                Marshal.Copy(pout, fout, 0, fft_len * 2);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        unsafe public bool ComputeSpectrum()
        {
            try
            {
                fftwf.execute(fplan2);
                Marshal.Copy(spec_out, spec_fout, 0, spectrum_len * 2);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        // Releases all memory used by FFTW/C#
        public void FreeFFTW()
        {
            try
            {
                fftwf.fftw_forget();
                fftwf.free(pin);
                fftwf.free(pout);
                fftwf.free(spec_in);
                fftwf.free(spec_out);
                fftwf.destroy_plan(fplan1);
                fftwf.destroy_plan(fplan2);
                fftwf.cleanup();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }
}