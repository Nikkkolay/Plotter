using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Buturin.PolishEntry;
using static Baturin.Plotter0_3_1.ChartData;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System.Windows.Data;
using System.Collections.Generic;

namespace Baturin.Plotter0_3_1
{
    class MainWindow : Window
    {
        Grid mainGrid;
        ScrollViewer scrvwr;
        StackPanel stk;
        List<ChartData> chartDatas;
        PlotView plott;
        public PlotView Plott
        {
            get { return plott; }
        }

        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            MainWindow Window = new MainWindow();
            app.Run(Window);
        }

        public MainWindow()
        {
            Title = "Plotter 0.3.1";
            mainGrid = new Grid();
            this.Content = mainGrid;
            chartDatas = new List<ChartData>();

            #region создание 2 колонок основного Грида

            ColumnDefinition coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(25, GridUnitType.Star);
            mainGrid.ColumnDefinitions.Add(coldef1);

            coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(75, GridUnitType.Star);
            mainGrid.ColumnDefinitions.Add(coldef1);

            #endregion

            #region cоздания места для инфы о графиках слева, с прокруткой
            scrvwr = new ScrollViewer();
            mainGrid.Children.Add(scrvwr);
            Grid.SetColumn(scrvwr, 0);
            stk = new StackPanel();
            scrvwr.Content = stk;
            #endregion

            #region подготовка места для графиков
            MainViewModel test = new MainViewModel();
            plott = new PlotView();
            plott.Margin = new Thickness(5);
            plott.Model = test.MyModel;
            mainGrid.Children.Add(plott);
            Grid.SetColumn(plott, 1);
            #endregion

            #region cоздание кнопки для добавления новых графиков
            Button addNewFunc = new Button();
            addNewFunc.Content = "Add new Chart";
            addNewFunc.FontSize = 16;
            addNewFunc.Background = Brushes.BurlyWood;
            addNewFunc.Margin = new Thickness(5);
            addNewFunc.Padding = new Thickness(5);
            addNewFunc.Click += AddNewFunc_Click;
            stk.Children.Add(addNewFunc);
            #endregion

            //добавление инфы о графике
            AddNewFunc();
        }

        public void AddNewFunc_Click(object sender, RoutedEventArgs e)
        {
            plott.Focus();//для возможности возвращения исходных осей при нажатии A без предарительного щелчка по графику
            AddNewFunc();
        }
        void AddNewFunc()
        {
            ChartData tempChartData = new ChartData();
            tempChartData.ExpanderWithInfo.IsExpanded = true;

            tempChartData.DeletedButton.Click += DeletedButton_Click;
            tempChartData.DeletedButton.Master1 = tempChartData.ExpanderWithInfo;
            tempChartData.DeletedButton.Master2 = tempChartData;

            tempChartData.BuildButton.Click += BuildButton_Click;
            tempChartData.BuildButton.Master1 = tempChartData.ExpanderWithInfo;
            tempChartData.BuildButton.Master2 = tempChartData;

            stk.Children.Add(tempChartData.ExpanderWithInfo);
            chartDatas.Add(tempChartData);
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            plott.Focus();//для возможности возвращения исходных осей при нажатии A без предарительного щелчка по графику
            ChartData Cdat = (sender as ButtonWithMasters).Master2 as ChartData;
            double Left, Right;
            ReversePolishEntry polishEntry;

            try
            {
                Left = Cdat.LeftBorder;
                Right = Cdat.RightBorder;
                polishEntry = new ReversePolishEntry(Cdat.StrFunc);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            if (Left > Right)
            {
                Left += Right;
                Right = Left - Right;
                Left = Left - Right;
                Cdat.LeftBorder = Left;
                Cdat.RightBorder = Right;
            }
            FunctionSeries func = new FunctionSeries(polishEntry.FunctionValue, Left, Right, (Right - Left) / 10000);
            func.Color = Cdat.ColorOfFunc.ToOxyColor();

            if (Cdat.IndexOfChart == -1)
            {
                this.Plott.Model.Series.Add(func);
                Cdat.IndexOfChart = this.Plott.Model.Series.IndexOf(func);
            }
            else
                this.Plott.Model.Series[Cdat.IndexOfChart] = func;

            this.Plott.Model.InvalidatePlot(true);
        }

        private void DeletedButton_Click(object sender, RoutedEventArgs e)
        {
            plott.Focus();//для возможности возвращения исходных осей при нажатии A без предарительного щелчка по графику
            ChartData Cdat = (sender as ButtonWithMasters).Master2 as ChartData;
            int index = Cdat.IndexOfChart;
            if (index != -1)
            {
                plott.Model.Series.RemoveAt(index);
                for (int j = 0; j < chartDatas.Count; j++)
                    if (chartDatas[j].IndexOfChart >= index)
                        chartDatas[j].IndexOfChart--;
            }

            //уменьшение числа отвечающего за кол-во элементов ChartData, и переименование объектов ChartData идущих после удаленных
            Expander exp = (sender as ButtonWithMasters).Master1 as Expander;
            ChartData.NumberChartData--;
            int i = stk.Children.IndexOf(exp) + 1;
            for (; i < stk.Children.Count; i++)
            {
                TextBlock tmp = (((stk.Children[i] as Expander).Header as StackPanel).Children[0] as TextBlock);
                uint newNum = Convert.ToUInt32(tmp.Text.Remove(0, 10)) - 1;
                tmp.Text = "Function #" + newNum;
            }

            stk.Children.Remove(exp);
            chartDatas.Remove(Cdat);
            //обновляем экран
            this.Plott.Model.InvalidatePlot(true);
        }
    }
}