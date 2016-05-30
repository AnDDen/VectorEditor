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

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                canvasBorder.Width = width;
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                canvasBorder.Height = height;
            }
        }

        private Canvas canvas;
        public Canvas Canvas { get { return canvas; } }

        private Border canvasBorder;
        public Border CanvasBorder { get { return canvasBorder; } }

        public Image(Canvas canvas, Border canvasBorder, double width, double height)
        {
            Figures = new List<IFigure>();
            this.canvas = canvas;
            this.canvasBorder = canvasBorder;
            canvas.Children.Clear();
            canvasBorder.Width = width;
            canvasBorder.Height = height;
            this.width = width;
            this.height = height;
        }
        public Image(double width, double height)
        {
            this.width = width;
            this.height = height;
            canvasBorder = new Border() { Width = width, Height = height };
            canvas = new Canvas();
            canvasBorder.Child = canvas;
            Figures = new List<IFigure>();
        }

        public Image Copy()
        {
            Image res = new Image(width, height);
            foreach (IFigure f in Figures)
            {
                res.AddFigure(f.Copy());
            }
            return res;
        }

        /* Перемещение фигуры по уровням */
        public void SwapFigures(int index1, int index2)
        {
            IFigure figure1 = Figures[index1];
            IFigure figure2 = Figures[index2];

            Figures.RemoveAt(index1);
            Figures.Insert(index1, figure2);
            Figures.RemoveAt(index2);
            Figures.Insert(index2, figure1);
            canvas.Children.RemoveAt(index2);
            canvas.Children.Insert(index2, new UIElement());
            canvas.Children.RemoveAt(index1);
            canvas.Children.Insert(index1, figure2.Element);
            canvas.Children.RemoveAt(index2);
            canvas.Children.Insert(index2, figure1.Element);
        }
        public void LayerUp(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            if (k + 1 < Figures.Count)
                SwapFigures(k, k + 1);
        }
        public void LayerDown(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            if (k - 1 >= 0)
                SwapFigures(k, k - 1);
        }
        public void LayerTop(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            Figures.RemoveAt(k);
            Canvas.Children.RemoveAt(k);
            Figures.Add(figure);
            Canvas.Children.Add(figure.Element);
        }
        public void LayerBottom(IFigure figure)
        {
            int k = Figures.IndexOf(figure);
            Figures.RemoveAt(k);
            Canvas.Children.RemoveAt(k);
            Figures.Insert(0, figure);
            Canvas.Children.Insert(0, figure.Element);
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
