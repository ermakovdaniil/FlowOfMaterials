using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using static System.Math;


namespace PlenkaAPI
{
    /// <summary>
    ///     Структура для отображения данных в таблице с результатами
    /// </summary>
    public struct CordTempN
    {
        /// <summary>
        ///     Координата канала по X
        /// </summary>
        public double Cord { get; set; }

        /// <summary>
        ///     Температура
        /// </summary>
        public double Temp { get; set; }

        /// <summary>
        ///     Вязкость
        /// </summary>
        public double N { get; set; }
    }


    public struct CalculationParameters
    {
        public string MaterialName { get; init; }
        public double W { get; init; }
        public double H { get; init; }
        public double L { get; init; }
        public double P { get; init; }
        public double C { get; init; }
        public double T0 { get; init; }
        public double Vu { get; init; }
        public double Tu { get; init; }
        public double U0 { get; init; }
        public double B { get; init; }
        public double Tr { get; init; }
        public double N { get; init; }
        public double Au { get; init; }
        public double Step { get; init; }
    }


    public struct CalculationResults
    {
        /// <summary>
        ///     Таймер для времени расчета
        /// </summary>
        public Stopwatch MathTimer { get; init; }

        /// <summary>
        ///     Список с результатами расчета по координате канала
        /// </summary>
        public List<CordTempN> CordTempNs { get; init; }

        /// <summary>
        ///     Производительность канала
        /// </summary>
        public double Q { get; init; }

        /// <summary>
        ///     Температура продукта
        /// </summary>
        public double T { get; init; }

        /// <summary>
        ///     Вязкость продукта
        /// </summary>
        public double N { get; init; }
    }


    public class MathClass // todo как-то красиво переписать все это
    {
        public MathClass(CalculationParameters cp)
        {
            Cp = cp;
        }


        /// <summary>
        ///     Результаты вычислений
        /// </summary>
        public CalculationResults Results { get; private set; }

        private int GetDecimalDigitsCount(double number)
        {
            var str = number.ToString(new NumberFormatInfo
                                          {NumberDecimalSeparator = ".",})
                            .Split('.');

            return str.Length == 2 ? str[1].Length : 0;
        }

        /// <summary>
        ///     Функция производит вычисления с заданными параметрами
        /// </summary>
        public void Calculate()
        {
            var sw = new Stopwatch();
            sw.Start();
            var f = 0.125 * Pow(H / W, 2) - 0.625 * (H / W) + 1;
            var gamma = Vu / H;
            var qGamma = H * W * U0 * Pow(gamma, N + 1);
            var qAlpha = W * Au * (1 / B - Tu + Tr);
            var qch = H * W * Vu / 2 * f;
            var cordTempNs = new List<CordTempN>();
            var digitsCount = GetDecimalDigitsCount(Step);

            for (double i = 0; i <= L; i += Step)
            {
                var z = Round(i, digitsCount);

                var t = Tr + 1 / B * Log((B * qGamma + W * Au) /
                                         (B * qAlpha) *
                                         (1 - Exp(-(z * B * qAlpha / (P * C * qch)))) +
                                         Exp(B * (T0 - Tr - z * qAlpha / (P * C * qch))));

                var ni = U0 * Exp(-B * (t - Tr)) * Pow(gamma, N - 1);
                t = Round(t, 2);
                ni = Round(ni, 2);
                cordTempNs.Add(new CordTempN {Cord = z, N = ni, Temp = t,});
            }

            var q = Round(P * qch * 3600, 2);
            var T = cordTempNs.Last().Temp;
            var n = cordTempNs.Last().N;
            sw.Stop();

            Results = new CalculationResults
                {Q = q, T = T, N = n, CordTempNs = cordTempNs, MathTimer = sw,};
        }


    #region Parameters

        public CalculationParameters Cp { get; init; }

        private double W
        {
            get
            {
                return Cp.W;
            }
        }

        private double H
        {
            get
            {
                return Cp.H;
            }
        }

        private double L
        {
            get
            {
                return Cp.L;
            }
        }

        private double P
        {
            get
            {
                return Cp.P;
            }
        }

        private double C
        {
            get
            {
                return Cp.C;
            }
        }

        private double T0
        {
            get
            {
                return Cp.T0;
            }
        }

        private double Vu
        {
            get
            {
                return Cp.Vu;
            }
        }

        private double Tu
        {
            get
            {
                return Cp.Tu;
            }
        }

        private double U0
        {
            get
            {
                return Cp.U0;
            }
        }

        private double B
        {
            get
            {
                return Cp.B;
            }
        }

        private double Tr
        {
            get
            {
                return Cp.Tr;
            }
        }

        private double N
        {
            get
            {
                return Cp.N;
            }
        }

        private double Au
        {
            get
            {
                return Cp.Au;
            }
        }

        private double Step
        {
            get
            {
                return Cp.Step;
            }
        }

    #endregion
    }
}
