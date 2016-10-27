using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegralWinForms.Classes
{
    public class Integral
    {
        private Func<double, double> _F;
        private double a;
        private double b;
        private long n;

        public double A
        {
            get
            {
                return a;
            }
        }

        public double B
        {
            get
            {
                return b;
            }
        }

        public long N
        {
            get
            {
                return n;
            }
        }

        public void SetData(double a, double b, long N, Func<double, double> f)
        {
            this.a = a;
            this.b = b;
            this.n = N;
            _F = f;
        }
        public Integral(double a, double b, long N, Func<double, double> f)
        {
            SetData(a, b, N, f);
        }

        public double Rectangle(out double time, ref double[] time5)
        {
            Stopwatch sw1;
            sw1 = new Stopwatch();
            sw1.Start();
            for (int i = 0; i < 100; i++)
                time5[i] = 0;
            time = 0;
            double res = CalcRectangle(a,b, n,_F, ref time5); 
            sw1.Stop();
            time = sw1.ElapsedMilliseconds;            
            return res;
        }

        public double RectangleThreadParralel(out double time, ref double[] time4, ref double[] time6)
        {
            Stopwatch sw2;
            sw2 = new Stopwatch();
            sw2.Start(); 
            for (int i = 0; i < 100; i++)
                time6[i] = 0;
            for (int i = 0; i < 4; i++)
                time4[i] = 0;
            time = 0;
            double res2 = ParallelThreadCalcRectangle(a, b, n, _F, ref time4, ref time6);
            sw2.Stop();
            time = sw2.ElapsedMilliseconds;
            return res2;
        }                

        public double RectangleForParralel(out double time)
        {
            Stopwatch sw3;
            sw3 = new Stopwatch();
            sw3.Start();
            time = 0;
            double res3 = ParallelForCalcRectangle(a, b, n, _F);
            sw3.Stop();
            time = sw3.ElapsedMilliseconds;
            return res3;
        }

        private double CalcRectangle(double a, double b, long N, Func<double, double> f, ref double[] time5)
        {
            Stopwatch sw;
            sw = new Stopwatch();
            double h = (b - a) / n;
            double S = 0;            
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                S += _F(a + i * h);

                if ((i % (n / 100)) == 0)
                {
                    sw.Stop();
                    sw.Start();
                    time5[i / (n / 100)] = sw.ElapsedMilliseconds;
                }                 
            }
            S *= h;
            return S;
        }

        private double ParallelThreadCalcRectangle(double a, double b, long N, Func<double, double> f, ref double[] time4, ref double[] time6)
        {                       
            Stopwatch sw, sw1, sw2, sw3;
            sw = new Stopwatch();
            sw1 = new Stopwatch(); 
            sw2 = new Stopwatch(); 
            sw3 = new Stopwatch();            
            double h = (b - a) / n;
            double S = 0;           
            double S10 = 0;
            for (int i = 0; i < n / 4; i++)
            {                
                S10 += _F(a + i * h);
                if ((i*4 % (n / 100)) == 0)
                {
                    sw.Stop();
                    sw.Start();
                    time6[i*4 / (n / 100)] = sw.ElapsedMilliseconds;
                } 
            }            
            Thread[] threads = new Thread[9];
            for (int i = 0; i < 9; i++)
            {
                int k = 0;
                int i1 = i;
                if (i1 < 4) k = 4;
                if (i1 > 3 && i1 < 6) k = 2; 
                if (i1 > 5 && i1 < 9) k = 3;
                threads[i] = new Thread(() =>
                {                    
                    double S1 = 0;
                    for (int j = i1 * (int)(n / k); j < i1 * (int)(n / k) + (int)(n / k); j++)
                    {
                        S1 += _F(a + j * h);
                    }
                    if (i1 < 4) S += S1;
                });                
            }
            
            sw1.Start();
            for (int i = 0; i < 4; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < 4; i++)
            {
                threads[i].Join();
            }
            sw1.Stop();
            time4[0] = sw1.ElapsedMilliseconds;

            sw2.Start();
            for (int i = 4; i < 6; i++)
            {
                threads[i].Start();
            } 
            for (int i = 4; i < 6; i++)
            {
                threads[i].Join();
            }            
            sw2.Stop();
            time4[1] = sw2.ElapsedMilliseconds;

            sw3.Start();
            for (int i = 6; i < 9; i++)
            {
                threads[i].Start();
            }
            for (int i = 6; i < 9; i++)
            {
                threads[i].Join();
            } 
            sw3.Stop();
            time4[2] = sw3.ElapsedMilliseconds;

            S *= h;
            return S;
        }               

        private double ParallelForCalcRectangle(double a, double b, long N, Func<double, double> f)
        {
            object obj = new object();
            double h = (b - a) / n;
            double S = 0;

            Parallel.For(0, N, () => 0.0, (i, state, local) =>
            {
                local += _F(a + h * i);
                return local;
            }, local => { lock (obj) S += local; });
            
            S *= h;
            return S;
        }

    }
}
