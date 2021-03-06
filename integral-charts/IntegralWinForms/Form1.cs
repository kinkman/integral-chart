﻿using IntegralWinForms.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntegralWinForms
{  

    public partial class Form1 : Form
    {        

        double SumIntegral(double x)
        {            
            return 2 * x - Math.Log(2 * x) + 234;            
        }
        
        public Form1()
        {
            InitializeComponent();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }
                
        private void btCalc_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();

            double[] koefSin = new double[100];
            double[] koefPar = new double[100];
            double time, time1, time2, time8;
            double[] time3 = new double[4];
            double[] time4 = new double[100];
            double[] time5 = new double[100];
            double[] time6 = new double[100];
            double[] time7 = new double[100];

            double a = Convert.ToDouble(tbA.Text);
            double b = Convert.ToDouble(tbB.Text);
            long N = Convert.ToInt64(nudN.Text);
            Integral aInt = new Integral(a, b, N, SumIntegral);

            double result = aInt.Rectangle(out time, ref time4);
            double result1 = aInt.RectangleThreadParralel(out time1, ref time3, ref time5);            
            double result2 = aInt.RectangleForParralel(out time2);
            double result3 = aInt.RectangleTaskParralel(out time8);

            rtbLog.AppendText("Последовательный вариант" + Environment.NewLine);
            rtbLog.AppendText("Ответ: " + result.ToString() + Environment.NewLine);
            rtbLog.AppendText("Время: " + time.ToString() + Environment.NewLine);

            rtbLog.AppendText("Parallel Threads вариант" + Environment.NewLine);
            rtbLog.AppendText("Ответ: " + result1.ToString() + Environment.NewLine);
            rtbLog.AppendText("Время: " + time3[0].ToString() + Environment.NewLine);            

            rtbLog.AppendText("Parallel For вариант" + Environment.NewLine);
            rtbLog.AppendText("Ответ: " + result2.ToString() + Environment.NewLine);
            rtbLog.AppendText("Время: " + time2.ToString() + Environment.NewLine);

            rtbLog.AppendText("Parallel Tasks вариант" + Environment.NewLine);
            rtbLog.AppendText("Ответ: " + result3.ToString() + Environment.NewLine);
            rtbLog.AppendText("Время: " + time8.ToString() + Environment.NewLine);
            
            rtbLog.AppendText(Environment.NewLine);

            for (int i = 0; i < 3; i++)
            {
                chart3.Series[i].Points.Clear();                
            }

            chart2.Series[0].Points.Clear();
            chart1.Series[0].Points.Clear(); 
            chart1.Series[1].Points.Clear();
            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();

            chart3.Series[0].Points.AddXY(1, time);
            chart3.Series[1].Points.AddXY(2, time3[0]);
            chart3.Series[2].Points.AddXY(3, time2);            

            chart2.Series[0].Points.AddXY(1, time);
            chart2.Series[0].Points.AddXY(2, time3[1]); 
            chart2.Series[0].Points.AddXY(3, time3[2]);
            chart2.Series[0].Points.AddXY(4, time3[0]);

            time6[0] = time4[99];
            time7[0] = time5[99];

            for (int i = 0; i < 98; i++)
            {
                koefSin[97 - i] = time4[i + 2] / time4[i + 1];
                koefPar[97 - i] = time5[i + 2] / time5[i + 1];
                time6[i + 1] = time6[i] / koefSin[97 - i];
                time7[i + 1] = time7[i] / koefPar[97 - i];

                chart1.Series[0].Points.AddXY(i * N / 100, time4[i]);
                chart1.Series[1].Points.AddXY(i * N / 100, time5[i]);
                chart4.Series[0].Points.AddXY(i * (b - a) / 100, time6[i]);
                chart4.Series[1].Points.AddXY(i * (b - a) / 100, time7[i]);
            }

        }
    }
}
