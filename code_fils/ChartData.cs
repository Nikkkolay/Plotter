using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace Baturin.Plotter0_3_1
{
    class ChartData
    {
        public class ButtonWithMasters : Button
        {
            object master1, master2;
            public object Master1
            {
                get { return master1; }
                set { master1 = value; }
            }
            public object Master2
            {
                get { return master2; }
                set { master2 = value; }
            }
        }

        Expander expanderWithInfo;
        StackPanel stkForExp;
        TextBox TextBoxForFunc, leftBorderBox, rightBorderBox;
        ButtonWithMasters deletedButton, buildButton;
        WrapPanel wrpPanelForColors;
        Button[] enamColor;
        SolidColorBrush colorOfFunc;
        static uint numberChartData = 0;
        int numberOfColors;
        int indexOfChart;

        #region Свойства NumberChartData, StrFunc, LeftBorder, RightBorder, ColorOfFunc, IndexOfChart

        public static uint NumberChartData
        {
            get
            {
                return numberChartData;
            }

            set
            {
                if (value >= 0)
                    numberChartData = value;
                else throw new Exception("attempt to assign a number of graphs a negative value");
            }

        }

        public string StrFunc
        {
            get { return TextBoxForFunc.Text; }
        }

        public double LeftBorder
        {
            get
            {
                double L;
                int point = leftBorderBox.Text.IndexOf('.');
                char[] LeftBorder = leftBorderBox.Text.ToCharArray();
                if (point != -1)
                    LeftBorder[point] = ',';
                try
                {
                    L = Convert.ToDouble(new string(LeftBorder));
                }
                catch { throw new Exception("Некорректно введена левая граница"); }

                return L;
            }

            set
            { leftBorderBox.Text = Convert.ToString(value); }
        }

        public double RightBorder
        {
            get
            {
                double R;
                int point = rightBorderBox.Text.IndexOf('.');
                char[] RightBorder = rightBorderBox.Text.ToCharArray();
                if (point != -1)
                    RightBorder[point] = ',';
                try
                {
                    R = Convert.ToDouble(new string(RightBorder));
                }
                catch { throw new Exception("Некорректно введена правая граница"); }

                return R;
            }
            set
            { rightBorderBox.Text = Convert.ToString(value); }
        }

        public SolidColorBrush ColorOfFunc
        {
            get { return colorOfFunc; }
        }

        public int IndexOfChart
        {
            get { return indexOfChart; }
            set { indexOfChart = value; }
        }
        #endregion


        #region Свойства ExpanderWithInfo, DeletedButton
        public Expander ExpanderWithInfo
        {
            get { return expanderWithInfo; }
        }

        public ButtonWithMasters DeletedButton
        {
            get { return deletedButton; }
        }

        public ButtonWithMasters BuildButton
        {
            get { return buildButton; }
        }

        #endregion

        public ChartData()
        {
            expanderWithInfo = new Expander();
            ExpanderWithInfo.Background = Brushes.Moccasin;
            ExpanderWithInfo.Margin = new Thickness(5);
            indexOfChart = -1;

            //создание хедера для экспандера
            stkForExp = new StackPanel();
            stkForExp.Orientation = Orientation.Horizontal;
            stkForExp.Margin = new Thickness(5);
            colorOfFunc = Brushes.Green;


            stkForExp.Children.Add(new TextBlock
            {
                Text = ("Function #" + numberChartData),
                Background = colorOfFunc,
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 16
            });
            expanderWithInfo.Header = stkForExp;
            expanderWithInfo.Name = "blok" + (numberChartData++);

            //грид внутри которого размещена вся инфа
            Grid grid1 = new Grid();
            expanderWithInfo.Content = grid1;

            #region Создание 4 строк для grid1

            RowDefinition rowdef = new RowDefinition();
            rowdef.Height = new GridLength(0, GridUnitType.Auto);
            grid1.RowDefinitions.Add(rowdef);

            rowdef = new RowDefinition();
            rowdef.Height = new GridLength(0, GridUnitType.Auto);
            grid1.RowDefinitions.Add(rowdef);

            rowdef = new RowDefinition();
            rowdef.Height = new GridLength(0, GridUnitType.Auto);
            grid1.RowDefinitions.Add(rowdef);

            rowdef = new RowDefinition();
            rowdef.Height = new GridLength(0, GridUnitType.Auto);
            grid1.RowDefinitions.Add(rowdef);

            #endregion

            #region создание строчки y(x) = _______

            //создание нового грида для этой строки
            Grid grd1String = new Grid();
            grd1String.Margin = new Thickness(5);
            grid1.Children.Add(grd1String);
            Grid.SetRow(grd1String, 0);

            //добавление колоннок для этого грида
            ColumnDefinition coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(0, GridUnitType.Auto);
            grd1String.ColumnDefinitions.Add(coldef1);
            coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(1, GridUnitType.Star);
            grd1String.ColumnDefinitions.Add(coldef1);

            //надпись y(x) ="
            TextBlock firstString1 = new TextBlock();
            firstString1.Margin = new Thickness(5);
            firstString1.Padding = new Thickness(5);
            firstString1.Text = "y(x) =";
            firstString1.FontSize = 20;
            firstString1.HorizontalAlignment = HorizontalAlignment.Left;
            grd1String.Children.Add(firstString1);
            Grid.SetColumn(firstString1, 0);

            //поле для ввода функции
            TextBoxForFunc = new TextBox();
            TextBoxForFunc.Padding = new Thickness(5);
            TextBoxForFunc.VerticalAlignment = VerticalAlignment.Center;
            TextBoxForFunc.HorizontalAlignment = HorizontalAlignment.Stretch;
            grd1String.Children.Add(TextBoxForFunc);
            Grid.SetColumn(TextBoxForFunc, 1);

            #endregion

            #region создание строчки Interval: [ ___ , ____ ]

            //создание нового грида для этой строки
            Grid grd2String = new Grid();
            grd2String.Margin = new Thickness(5);
            grid1.Children.Add(grd2String);
            Grid.SetRow(grd2String, 1);

            #region Создания 5 колонок для второй строки
            ColumnDefinition coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(0, GridUnitType.Auto);
            grd2String.ColumnDefinitions.Add(coldef2);

            coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(50, GridUnitType.Star);
            grd2String.ColumnDefinitions.Add(coldef2);

            coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(0, GridUnitType.Auto);
            grd2String.ColumnDefinitions.Add(coldef2);

            coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(50, GridUnitType.Star);
            grd2String.ColumnDefinitions.Add(coldef2);

            coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(0, GridUnitType.Auto);
            grd2String.ColumnDefinitions.Add(coldef2);

            #endregion

            //след 5 блоков кода воспроизводят Interval: [ ___ , ____ ]
            TextBlock secondstr1 = new TextBlock();
            secondstr1.Margin = new Thickness(5);
            secondstr1.Padding = new Thickness(5);
            secondstr1.Text = "Interval : [";
            secondstr1.FontSize = 20;
            secondstr1.HorizontalAlignment = HorizontalAlignment.Left;
            grd2String.Children.Add(secondstr1);
            Grid.SetColumn(secondstr1, 0);

            leftBorderBox = new TextBox();
            leftBorderBox.Padding = new Thickness(5);
            //leftBorderBox.Margin = new Thickness(5);
            leftBorderBox.VerticalAlignment = VerticalAlignment.Center;
            leftBorderBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            grd2String.Children.Add(leftBorderBox);
            Grid.SetColumn(leftBorderBox, 1);

            TextBlock secondstr2 = new TextBlock();
            secondstr2.Margin = new Thickness(5);
            secondstr2.Padding = new Thickness(5);
            secondstr2.Text = ", ";
            secondstr2.FontSize = 20;
            secondstr2.HorizontalAlignment = HorizontalAlignment.Center;
            grd2String.Children.Add(secondstr2);
            Grid.SetColumn(secondstr2, 2);

            rightBorderBox = new TextBox();
            rightBorderBox.Padding = new Thickness(5);
            //rightBorderBox.Margin = new Thickness(5);
            rightBorderBox.VerticalAlignment = VerticalAlignment.Center;
            rightBorderBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            grd2String.Children.Add(rightBorderBox);
            Grid.SetColumn(rightBorderBox, 3);

            TextBlock secondstr3 = new TextBlock();
            secondstr3.Margin = new Thickness(5);
            secondstr3.Padding = new Thickness(5);
            secondstr3.Text = "]";
            secondstr3.FontSize = 20;
            secondstr3.HorizontalAlignment = HorizontalAlignment.Right;
            grd2String.Children.Add(secondstr3);
            Grid.SetColumn(secondstr3, 4);

            #endregion

            #region создание строчки c выбором цвета

            //доступные цыета Grin, Red,  Pink, Gold, Orange, Yellow, Violet, SkyBlue;
            numberOfColors = 8;
            enamColor = new Button[numberOfColors];
            #region заполнение массива кнопок с цветами
            for (int i = 0; i < numberOfColors; i++)
            {
                //enamColor[i].Padding = new Thickness(5);
                enamColor[i] = new Button();
                enamColor[i].Click += ChartData_Click;
                enamColor[i].Margin = new Thickness(5);
                enamColor[i].Width = enamColor[i].Height = 20;

            }
            enamColor[0].Background = Brushes.Green;
            enamColor[1].Background = Brushes.Red;
            enamColor[2].Background = Brushes.Pink;
            enamColor[3].Background = Brushes.Gold;
            enamColor[4].Background = Brushes.Orange;
            enamColor[5].Background = Brushes.Yellow;
            enamColor[6].Background = Brushes.Violet;
            enamColor[7].Background = Brushes.SkyBlue;

            #endregion

            wrpPanelForColors = new WrapPanel();
            wrpPanelForColors.Orientation = Orientation.Horizontal;
            for (int i = 0; i < numberOfColors; i++)
                wrpPanelForColors.Children.Add(enamColor[i]);

            grid1.Children.Add(wrpPanelForColors);
            Grid.SetRow(wrpPanelForColors, 2);

            #endregion создание строчки c выбором цвета

            #region создание строчки Build     Deleted

            Grid grd4String = new Grid();
            grd4String.Margin = new Thickness(5);
            grid1.Children.Add(grd4String);
            Grid.SetRow(grd4String, 3);

            #region Создание колонок для этой строки
            ColumnDefinition coldef4 = new ColumnDefinition();
            coldef4.Width = new GridLength(1, GridUnitType.Star);
            grd4String.ColumnDefinitions.Add(coldef4);

            coldef4 = new ColumnDefinition();
            coldef4.Width = new GridLength(1, GridUnitType.Star);
            grd4String.ColumnDefinitions.Add(coldef4);
            #endregion

            buildButton = new ButtonWithMasters();
            buildButton.Content = "Build";
            buildButton.Padding = new Thickness(5);
            buildButton.Margin = new Thickness(5);
            buildButton.HorizontalAlignment = HorizontalAlignment.Left;
            grd4String.Children.Add(buildButton);
            Grid.SetColumn(buildButton, 0);

            deletedButton = new ButtonWithMasters();
            deletedButton.Content = "Deleted";
            deletedButton.Name = "blok" + (numberChartData - 1);
            deletedButton.Padding = new Thickness(5);
            deletedButton.Margin = new Thickness(5);
            deletedButton.HorizontalAlignment = HorizontalAlignment.Right;
            grd4String.Children.Add(deletedButton);
            Grid.SetColumn(deletedButton, 1);

            #endregion
        }

        private void ChartData_Click(object sender, RoutedEventArgs e)
        {
            colorOfFunc = (SolidColorBrush)(sender as Button).Background;
            (stkForExp.Children[0] as TextBlock).Background = colorOfFunc;
        }
    }
}