using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;

namespace Baturin.Plotter0_3_1
{
    public class MainViewModel
    {
        PlotModel myModel;
        public PlotModel MyModel
        {
            get { return myModel; }
            set { myModel = value; }
        }

        public MainViewModel()
        {
            myModel = new PlotModel();
            myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MajorGridlineStyle = LineStyle.Automatic,

                MajorGridlineThickness = 1,
                MajorGridlineColor = OxyColor.FromArgb(50, 0, 0, 0),
                MinorGridlineStyle = LineStyle.Automatic,
                MinorGridlineThickness = 1,
                MinorGridlineColor = OxyColor.FromArgb(50, 127, 127, 127)
            });

            myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineThickness = 1,
                MajorGridlineColor = OxyColor.FromArgb(50, 0, 0, 0),
                MinorGridlineStyle = LineStyle.Automatic,
                MinorGridlineThickness = 1,
                MinorGridlineColor = OxyColor.FromArgb(50, 127, 127, 127)
            });

            LinearAxis a = new LinearAxis();
            a.MajorGridlineColor = OxyColor.FromRgb(0, 0, 0);


        }

    }
}