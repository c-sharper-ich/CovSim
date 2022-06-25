using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace CovidSim
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        Data[] datas;

        DispatcherTimer dispatcherTimer;

        Random random = new Random();
        /// <summary>
        /// 感染者の動きを自由にする
        /// </summary>
        readonly bool AllowMove;
        /// <summary>
        /// 移動倍率用
        /// </summary>
        readonly int MoveMagnification;
        /// <summary>
        /// 感染期間
        /// </summary>
        readonly int InfectedSpan;

        readonly int InfectMagnification;

        readonly int MaskInfectMagnification;

        string outputcontents;

        Border[,] border;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startinfect">初期感染者</param>
        /// <param name="person">人間数</param>
        /// <param name="personmasked">人はマスクをするのか</param>
        /// <param name="allowmove">感染者の移動許可</param>
        /// <param name="movemagnification"></param>
        /// <param name="infectedspan"></param>
        /// <param name="infectmagnification"></param>
        /// <param name="maskinfectmagnification"></param>
        public Window1(int startinfect, int person, bool allowmove,int movemagnification,int infectedspan,int infectmagnification,int maskinfectmagnification)
        {
            InitializeComponent();
            for (int i = 0; i < 60; i++)
            {
                root.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
                root.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }
            border = new Border[60, 60];
            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 60; j++)
                {
                    border[i, j] = new Border
                    {
                        BorderBrush = new SolidColorBrush(Colors.Gray),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(4)
                    };
                    border[i, j].SetValue(Grid.ColumnProperty, i);
                    border[i, j].SetValue(Grid.RowProperty, j);
                    root.Children.Add(border[i, j]);
                }
            }
            datas = new Data[person];
            for(int i=0; i<datas.Length; i++)
            {
                int x=0, y=0;
                bool p=true;
                while (p)
                {
                    p = false;
                    x = random.Next(60);
                    y=random.Next(60);
                    if (i <= 1)
                    {

                    }
                    else
                    {
                        for (int j = 0; j < i - 1; j++)
                        {
                            if (x == datas[j].X && y == datas[j].Y)
                            {
                                p = true;
                            }
                            else
                            {
                                p = false;
                            }
                        }

                    }
                }
                if (i < startinfect)
                {
                    datas[i] = new Data
                    {
                        X = x,
                        Y = y,
                        State = State.Infected,
                    };
                }
                else
                {
                    datas[i] = new Data
                    {
                        X = x,
                        Y = y,
                        State = State.Normal,
                    };
                }
            }

            InfectedSpan = infectedspan;
            MaskInfectMagnification = maskinfectmagnification;
            InfectMagnification = infectmagnification;
            MoveMagnification = movemagnification;
            AllowMove = allowmove;
            dispatcherTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500)
            };
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();

        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            NextFrame();
            Output();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double length = ActualHeight * 0.90 / 60.0 > ActualWidth * 0.90 / 60.0 ? ActualWidth * 0.90 / 60.0 : ActualHeight * 0.90 / 60.0;
            root_width.Width = new GridLength(length * 60.0, GridUnitType.Pixel);
            root_width.MaxWidth = length * 60.0;
            root_height.Height = new GridLength(length * 60.0, GridUnitType.Pixel);
            root_height.MaxHeight = length * 60.0;
        }

        private void NextFrame()
        {
            for(int i = 0; i < datas.Length; i++)
            {
                if (random.Next(100) <= MoveMagnification)
                {
                    if (datas[i].State == State.Infected)
                    {
                        datas[i].TimeSinceInfected++;
                        if (datas[i].TimeSinceInfected > InfectedSpan)
                        {
                            datas[i].State = State.Saved;
                        }
                    }

                    if (datas[i].State == State.Infected && !AllowMove)
                    {
                        Debug.WriteLine(AllowMove);
                        continue;
                    }

                        if (random.Next(100) <= MoveMagnification)
                        {
                            int m = random.Next(4);
                            if (m == 0 && datas[i].X>0&&!Check(i,-1,0))
                            {
                                datas[i].X--;
                            }
                            if (m == 1 && datas[i].X<59 && !Check(i, +1, 0))
                            {
                                datas[i].X++;
                            }
                            if (m == 2 && datas[i].Y>0 && !Check(i, 0, -1))
                            {
                                datas[i].Y--;
                            }
                            if (m == 3 && datas[i].Y<59 && !Check(i, 0, +1))
                            {
                                datas[i].Y++;

                            }

                        }

                }

                for(int j = 0; j < datas.Length; j++)
                {
                    if (datas[j].State == State.Infected)
                    {

                        for(int k=0; k < datas.Length; k++)
                        {
                            if (k == j || datas[k].State == State.Saved || datas[k].State==State.Infected)
                            {
                                continue;
                            }
                            if ((datas[j].X - datas[k].X == 0 && Math.Abs(datas[j].Y - datas[k].Y) == 1) || (datas[j].Y - datas[k].Y == 0 && Math.Abs(datas[j].X - datas[k].X) == 1))
                            {
                                if (random.Next(100)<=InfectMagnification)
                                {
                                    datas[k].State = State.Infected;
                                }
                            }
                        }
                    }
                }

            }
        }

        private bool Check(int index,int vx,int vy)
        {
            bool returner=false;
            for(int i= 0; i < datas.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                if (datas[index].X + vx == datas[i].X && datas[index].Y + vy == datas[i].Y)
                {
                    returner = true;
                }
            }
            return returner;
        }
        private int outputtimes;
        private void Output()
        {
            outputtimes++;
            int infect = 0, saved = 0, normal = 0;
            for(int i = 0; i < 60; i++)
            {
                for(int j = 0; j < 60; j++)
                {
                    border[i, j].Background = null;
                }
            }
            for(int i =0; i < datas.Length; i++)
            {
                border[datas[i].X, datas[i].Y].Background = datas[i].State == State.Normal ? new SolidColorBrush(Colors.Green) : (datas[i].State == State.Infected ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Blue));
                    switch (datas[i].State)
                    {
                        case State.Normal:normal++;break;
                        case State.Infected:infect++;break;
                        case State.Saved:saved++;break;
                    }
            }
            outputcontents += outputtimes+"\t" + normal + "\t" + infect + "\t" + saved+"\n";
            if (outputtimes % 10 != 0)
            {
                return;
            }
            StreamWriter streamWriter=null;
            try
            {
                streamWriter = new StreamWriter(AppContext.BaseDirectory + "Log.txt",outputtimes!=0);
                streamWriter.Write(outputcontents);
                outputcontents = "";
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
        }

    }
    /// <summary>
    /// 感染状態を示す
    /// </summary>
    internal enum State
    {
        /// <summary>
        /// 通常
        /// </summary>
        Normal,
        /// <summary>
        /// 感染中
        /// </summary>
        Infected,
        /// <summary>
        /// 復帰
        /// </summary>
        Saved
    }
    internal class Data
    {
        internal State State;
        internal int X;
        internal int Y;
        /// <summary>
        /// 感染からの時間
        /// </summary>
        internal int TimeSinceInfected;
    }
}
