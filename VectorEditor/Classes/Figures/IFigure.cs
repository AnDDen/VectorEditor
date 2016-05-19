using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VectorEditor.Classes.Figures
{
    public interface IFigure
    {
        Color FillColor { get; set; }
        Color StrokeColor { get; set; }
        double StrokeThickness { get; set; }
        Canvas Element { get; }
        bool Selected { get; set; }

        void AddMouseDown(Point p);
        void AddMouseMove(Point p);
        void AddMouseUp(Point p);

        void SelectedMouseDown(Point p);
        void SelectedMouseMove(Point p);
        void SelectedMouseUp(Point p);

        string ToSVG();
        IFigure Copy();
    }
}
