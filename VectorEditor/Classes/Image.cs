using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VectorEditor.Classes.Figures;

namespace VectorEditor.Classes
{
    public class Image
    {
        public IList<IFigure> Figures { get; set; }

        private double width, height;

        private Canvas canvas;
        public Canvas Canvas { get { return canvas; } }

        public Image(Canvas canvas, double width, double height)
        {
            Figures = new List<IFigure>();
            this.canvas = canvas;
            canvas.Children.Clear();
            canvas.Width = width;
            canvas.Height = height;
            this.width = width;
            this.height = height;
        }
        public Image(double width, double height)
        {
            this.width = width;
            this.height = height;
            canvas = new Canvas() { Width = width, Height = height };
            Figures = new List<IFigure>();
        }

        /* Перемещение фигуры по уровням */
        public void SwapFigures(int index1, int index2)
        {
            IFigure tmp = Figures[index1];
            Figures[index1] = Figures[index2];
            Figures[index2] = tmp;

            UIElement tmpElem = canvas.Children[index1];
            canvas.Children[index1] = canvas.Children[index2];
            canvas.Children[index2] = tmpElem;
        }
        public void LayoutUp(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            SwapFigures(k, k + 1);
        }
        public void LayoutDown(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            SwapFigures(k, k - 1);
        }
        public void LayoutTop(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            Figures.RemoveAt(k);
            Figures.Add(figure);
        }
        public void LayoutBottom(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            Figures.RemoveAt(k);
            Figures.Insert(0, figure);
        }

        /* Добавление и удаление фигур */
        public void AddFigure(IFigure figure)
        {
            Figures.Add(figure);
            canvas.Children.Add(figure.Element);
        }
        public void RemoveFigure(IFigure figure)
        {
            Figures.Remove(figure);
            canvas.Children.Remove(figure.Element);
        }

        public string ToSVG()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("<svg width='{0}' height='{1}'>", width, height));
            foreach (IFigure f in Figures)
                stringBuilder.Append(f.ToSVG());
            stringBuilder.Append("</svg>");
            return stringBuilder.ToString();
        }
    }
}
