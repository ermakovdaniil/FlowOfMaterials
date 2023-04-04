using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using PlenkaAPI;
using PlenkaAPI.Data;
using PlenkaAPI.Models;

using PlenkaWpf.Utils;


namespace PlenkaWpf.VM
{
    /// <summary>
    ///     VM для окна исследователя
    /// </summary>
    internal class ResearcherControlVM : ViewModelBase

    {
        public List<int> ErrorList { get; set; }


    #region Functions

    #region Constructors

        public ResearcherControlVM()
        {
            Materials = DbContextSingleton.GetInstance().MembraneObjects.Where(o => o.Type.TypeName == "Материал").ToList();
            Material = DbContextSingleton.GetInstance().MembraneObjects.First(v => v.ObName == "Полистирол");

            Canals = DbContextSingleton.GetInstance().MembraneObjects.Where(o => o.Type.TypeName == "Канал").ToList();
            Canal = DbContextSingleton.GetInstance().MembraneObjects.First(v => v.ObName == "Канал");

            TempLineSerie = new LineSeries
                {Title = "Температура, °С",};

            TempLineSerie.Fill = Brushes.Transparent;

            TempSeries = new SeriesCollection
                {TempLineSerie,};

            NLineSerie = new LineSeries
                {Title = "Вязкость, Па·с",};

            NLineSerie.Fill = Brushes.Transparent;

            NSeries = new SeriesCollection
                {NLineSerie,};

            IsCalculated = false;
        }

    #endregion


        /// <summary>
        ///     Функция, обновляющая точки графика по словарю со значениями
        /// </summary>
        /// <param name="ls">Серия графика</param>
        private void UpdateLineSeriesByCordAndValue(LineSeries ls, List<double> x, List<double> y)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException("Количество значений x не совпадает с количеством значений y");
            }

            var newValues = new ChartValues<ObservablePoint>();

            for (var i = 0; i < x.Count; i++)
            {
                newValues.Add(new ObservablePoint(x[i], y[i]));
            }

            ls.Values = newValues;
        }

    #endregion


    #region Properties

        /// <summary>
        ///     Доступные материалы
        /// </summary>
        public List<MembraneObject> Materials { get; set; }
        public List<MembraneObject> Canals { get; set; }


        #region CanalProps

        /// <summary>
        ///     Текущий канал
        /// </summary>
        private MembraneObject _canal;

        public MembraneObject Canal
        {
            get
            {
                return _canal;
            }
            set
            {
                _canal = value;
                OnPropertyChanged(nameof(Length));
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(Depth));
            }
        }

        /// <summary>
        ///     Длина канала
        /// </summary>
        public double? Length
        {
            get
            {
                return Canal["Длина"];
            }
            set
            {
                Canal["Длина"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Ширина канала
        /// </summary>
        public double? Width
        {
            get
            {
                return Canal["Ширина"];
            }
            set
            {
                Canal["Ширина"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Глубина канала
        /// </summary>
        public double? Depth
        {
            get
            {
                return Canal["Глубина"];
            }
            set
            {
                Canal["Глубина"] = value;
                OnPropertyChanged();
            }
        }

    #endregion


    #region MaterialProps

        private MembraneObject _material;

        /// <summary>
        ///     Выбранный материал
        /// </summary>
        public MembraneObject Material
        {
            get
            {
                return _material;
            }
            set
            {
                _material = value;

                OnPropertyChanged(nameof(Density));
                OnPropertyChanged(nameof(SpecifiсHeatCapacity));
                OnPropertyChanged(nameof(MeltingTemperature));
                OnPropertyChanged(nameof(СonsСoef));
                OnPropertyChanged(nameof(TempСoef));
                OnPropertyChanged(nameof(RefTemp));
                OnPropertyChanged(nameof(MatFlowIndex));
                OnPropertyChanged(nameof(HeatCoef));
            }
        }

        private object _null;

        public object Null // Костыль для того чтобы ErrorStr в TextBox всегда был пустым
        {
            get
            {
                return _null;
            }
            set
            {
                _null = null;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Плоность материала
        /// </summary>
        public double? Density
        {
            get
            {
                return Material["Плотность"];
            }
            set
            {
                Material["Длина"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Удельная теплоемкость материала
        /// </summary>
        public double? SpecifiсHeatCapacity
        {
            get
            {
                return Material["Удельная теплоемкость"];
            }
            set
            {
                Material["Удельная теплоемкость"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Температуры плавления материала
        /// </summary>
        public double? MeltingTemperature
        {
            get
            {
                return Material["Температура плавления"];
            }
            set
            {
                Material["Температура плавления"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Коэффициент констистенции материала при температуре приведения
        /// </summary>
        public double? СonsСoef
        {
            get
            {
                return Material["Коэффициент констистенции материала при температуре приведения"];
            }
            set
            {
                Material["Коэффициент констистенции материала при температуре приведения"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Температурный коэффициент вязкости материала
        /// </summary>
        public double? TempСoef
        {
            get
            {
                return Material["Температурный коэффициент вязкости материала"];
            }
            set
            {
                Material["Температурный коэффициент вязкости материала"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Температура приведения
        /// </summary>
        public double? RefTemp
        {
            get
            {
                return Material["Температура приведения"];
            }
            set
            {
                Material["Температура приведения"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Индекс течения материала
        /// </summary>
        public double? MatFlowIndex
        {
            get
            {
                return Material["Индекс течения материала"];
            }
            set
            {
                Material["Индекс течения материала"] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Коэффициеент теплоотдачи от крышки канала к материалу
        /// </summary>
        public double? HeatCoef
        {
            get
            {
                return Material["Коэффициеент теплоотдачи от крышки канала к материалу"];
            }
            set
            {
                Material["Коэффициеент теплоотдачи от крышки канала к материалу"] = value;
                OnPropertyChanged();
            }
        }
        #endregion


        #region VarProps

        /// <summary>
        ///     Скорость крышки
        /// </summary>
        public double? CapSpeed { get; set; } = 1.5;

        /// <summary>
        ///     Температура крашки
        /// </summary>
        public double? CapTemperature { get; set; } = 210;

    #endregion


    #region Graphics

        /// <summary>
        ///     Список со значениями температуры и вязкости на протяжении канала
        /// </summary>
        public List<CordTempN> CordTempNs
        {
            get
            {
                if (MathClass != null)
                {
                    return MathClass.Results.CordTempNs;
                }

                return null;
            }
        }

        /// <summary>
        ///     Серия точек температуры
        /// </summary>
        private LineSeries TempLineSerie { get; }

        public SeriesCollection TempSeries { get; set; }

        /// <summary>
        ///     Серия точек вязкости
        /// </summary>
        private LineSeries NLineSerie { get; }

        public SeriesCollection NSeries { get; set; }

    #endregion


        /// <summary>
        ///     Текущая занаятая память
        /// </summary>
        public long TotalMemory
        {
            get
            {
                var currentProcess = Process.GetCurrentProcess();

                return currentProcess.WorkingSet64 / (1024 * 1024);
            }
        }

        private MathClass _mathClass;

        public MathClass MathClass
        {
            get
            {
                return _mathClass;
            }
            set
            {
                _mathClass = value;
                OnPropertyChanged();
            }
        }

        private void UpdateInterfaceElelemts()
        {
            var x = MathClass.Results.CordTempNs.Select(x => x.Cord).ToList();
            var n = MathClass.Results.CordTempNs.Select(x => x.N).ToList();
            var t = MathClass.Results.CordTempNs.Select(x => x.Temp).ToList();

            UpdateLineSeriesByCordAndValue(TempLineSerie, x, t);
            UpdateLineSeriesByCordAndValue(NLineSerie, x, n);

            OnPropertyChanged(nameof(TempSeries));
            OnPropertyChanged(nameof(NSeries));

            OnPropertyChanged(nameof(CordTempNs));
            OnPropertyChanged(nameof(TotalMemory));
        }

        /// <summary>
        ///     Шаг расчета
        /// </summary>
        public double? Step { get; set; } = 0.1;


        private bool _isCalculated;

        public bool IsCalculated
        {
            get
            {
                return _isCalculated;
            }
            set
            {
                _isCalculated = value;

                if (IsCalculated)
                {
                    TabControlVisibility = Visibility.Visible;
                }
                else
                {
                    TabControlVisibility = Visibility.Hidden;
                }
            }
        }

        private Visibility _tabControlVisibility;

        public Visibility TabControlVisibility
        {
            get
            {
                return _tabControlVisibility;
            }
            set
            {
                _tabControlVisibility = value;
                OnPropertyChanged();
            }
        }

    #endregion


    #region Commands

        private RelayCommand _calcCommand;

        /// <summary>
        ///     Команда, выполняющая расчет
        /// </summary>
        public RelayCommand CalcCommand
        {
            get
            {
                return _calcCommand ??= new RelayCommand(o =>
                {
                    IsCalculated = true;

                    var cp = new CalculationParameters
                    {
                        MaterialName = Material.ObName,
                        W = (double) Width,
                        H = (double) Depth,
                        L = (double) Length,
                        P = (double) Density,
                        C = (double) SpecifiсHeatCapacity,
                        T0 = (double) MeltingTemperature,
                        Vu = (double) CapSpeed,
                        Tu = (double) CapTemperature,
                        U0 = (double) СonsСoef,
                        B = (double) TempСoef,
                        Tr = (double) RefTemp,
                        N = (double) MatFlowIndex,
                        Au = (double) HeatCoef,
                        Step = (double) Step,
                    };

                    MathClass = new MathClass(cp);
                    MathClass.Calculate();
                    OnPropertyChanged(nameof(MathClass));
                    UpdateInterfaceElelemts();
                });
            }
        }

    #endregion
    }
}
