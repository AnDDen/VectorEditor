using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VectorEditor.Classes.Figures
{
    public class Rectangle : IFigure
    {
        private Color fillColor, strokeColor;
        private double strokeThickness;

        private double x, y, width, height;
        private double mouseDownX, mouseDownY, prevX, prevY;

        private bool isSelected;

        private Canvas elementCanvas;
        private System.Windows.Shapes.Rectangle rect;

        private List<System.Windows.Shapes.Rectangle> points;
        private System.Windows.Shapes.Rectangle pointTopLeft, pointTopRight, pointBottomLeft, pointBottomRight;
        private System.Windows.Shapes.Rectangle selectPoint = null;

        /* Inherited from IFigure */
        public Color FillColor
        {
            get
            {
                return fillColor;
            }

            set
            {
                fillColor = value;
                rect.Fill = new SolidColorBrush(fillColor);
            }
        }
        public Color StrokeColor
        {
            get
            {
                return strokeColor;
            }

            set
            {
                strokeColor = value;
                if (!isSelected)
                    rect.Stroke = new SolidColorBrush(strokeColor);
            }
        }
        public double StrokeThickness
        {
            get
            {
                return strokeThickness;
            }

            set
            {
                strokeThickness = value;
                rect.StrokeThickness = strokeThickness;

                Canvas.SetLeft(pointTopLeft, (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);
                Canvas.SetLeft(pointBottomLeft, (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);

                Canvas.SetTop(pointTopLeft, (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);
                Canvas.SetTop(pointTopRight, (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);                
            }
        }
        public Canvas Element
        {
            get { return elementCanvas; }
        }
        public bool Selected
        {
            get
            {
                return isSelected;
            }

            set
            {
                if (isSelected != value)
                {
                    if (value)
                    {
                        // Select
                        rect.Stroke = GlobalParams.SELECTED_FIGURE_BRUSH;
                        points.ForEach((p) => { p.Visibility = Visibility.Visible; });
                    }
                    else
                    {
                        // Deselect
                        rect.Stroke = new SolidColorBrush(strokeColor);
                        points.ForEach((p) => { p.Visibility = Visibility.Hidden; });
                    }
                }
                isSelected = value;
            }
        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
                Canvas.SetLeft(elementCanvas, x);
            }
        }
        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
                Canvas.SetTop(elementCanvas, y);
            }
        }
        public double Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
                elementCanvas.Width = width;
                rect.Width = width;
                Canvas.SetLeft(pointTopRight, width - GlobalParams.SELECT_POINT_SIZE - (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);
                Canvas.SetLeft(pointBottomRight, width - GlobalParams.SELECT_POINT_SIZE - (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);
            }
        }
        public double Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
                elementCanvas.Height = height;
                rect.Height = height;
                Canvas.SetTop(pointBottomLeft, height - GlobalParams.SELECT_POINT_SIZE - (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);
                Canvas.SetTop(pointBottomRight, height - GlobalParams.SELECT_POINT_SIZE - (strokeThickness - GlobalParams.SELECT_POINT_SIZE) / 2);
            }
        }

        private void InitPoints()
        {
            pointTopLeft = new System.Windows.Shapes.Rectangle()
            {
                Height = GlobalParams.SELECT_POINT_SIZE,
                Width = GlobalParams.SELECT_POINT_SIZE,
                Fill = GlobalParams.SELECT_POINT_FILL,
                Stroke = GlobalParams.SELECT_POINT_STROKE,
                StrokeThickness = 1
            };
            pointTopRight = new System.Windows.Shapes.Rectangle()
            {
                Height = GlobalParams.SELECT_POINT_SIZE,
                Width = GlobalParams.SELECT_POINT_SIZE,
                Fill = GlobalParams.SELECT_POINT_FILL,
                Stroke = GlobalParams.SELECT_POINT_STROKE,
                StrokeThickness = 1
            };
            pointBottomLeft = new System.Windows.Shapes.Rectangle()
            {
                Height = GlobalParams.SELECT_POINT_SIZE,
                Width = GlobalParams.SELECT_POINT_SIZE,
                Fill = GlobalParams.SELECT_POINT_FILL,
                Stroke = GlobalParams.SELECT_POINT_STROKE,
                StrokeThickness = 1
            };
            pointBottomRight = new System.Windows.Shapes.Rectangle()
            {
                Height = GlobalParams.SELECT_POINT_SIZE,
                Width = GlobalParams.SELECT_POINT_SIZE,
                Fill = GlobalParams.SELECT_POINT_FILL,
                Stroke = GlobalParams.SELECT_POINT_STROKE,
                StrokeThickness = 1
            };

            points = new List<System.Windows.Shapes.Rectangle>();
            points.Add(pointTopLeft);
            points.Add(pointTopRight);
            points.Add(pointBottomLeft);
            points.Add(pointBottomRight);

            points.ForEach((p) => 
            {
                elementCanvas.Children.Add(p);
                p.Visibility = Visibility.Hidden;
            });
        }

        public Rectangle(double x, double y)
        {
            elementCanvas = new Canvas();
            rect = new System.Windows.Shapes.Rectangle();
            elementCanvas.Children.Add(rect);

            InitPoints();

            X = x;
            Y = y;
            Width = 0;
            Height = 0;
        }
        public Rectangle() : this(0, 0) { }

        public void AddMouseDown(Point p)
        {
            mouseDownX = p.X; mouseDownY = p.Y;
            X = p.X; Y = p.Y;
        }
        public void AddMouseMove(Point p)
        {
            Width = Math.Abs(p.X - mouseDownX);
            Height = Math.Abs(p.Y - mouseDownY);
            X = Math.Min(p.X, mouseDownX);
            Y = Math.Min(p.Y, mouseDownY);
        }
        public void AddMouseUp(Point p)
        {
            Width = Math.Abs(p.X - mouseDownX);
            Height = Math.Abs(p.Y - mouseDownY);
            X = Math.Min(p.X, mouseDownX);
            Y = Math.Min(p.Y, mouseDownY);
        }

        public void SelectedMouseDown(Point p)
        {
            double rx = p.X - X, ry = p.Y - Y;
            selectPoint = null;
            points.ForEach((pnt) => {
                if (rx >= Canvas.GetLeft(pnt) && rx <= Canvas.GetLeft(pnt) + pnt.ActualWidth &&
                    ry >= Canvas.GetTop(pnt) && ry <= Canvas.GetTop(pnt) + pnt.ActualHeight)
                    selectPoint = pnt;
            });
            if (selectPoint == null)
            {
                prevX = p.X; prevY = p.Y;
            }
            else
            {
                prevX = X; prevY = Y;
            }
        }
        public void SelectedMouseMove(Point p)
        {
            if (selectPoint == null)
            {
                X += p.X - prevX;
                Y += p.Y - prevY;
                prevX = p.X; prevY = p.Y;
            }
            else if (selectPoint == pointTopLeft)
            {
                if (Width + X - p.X >= 0)
                {
                    Width += X - p.X;
                    X = p.X;
                }
                if (Height + Y - p.Y >= 0)
                {
                    Height += Y - p.Y;
                    Y = p.Y;
                }  
            }
            else if (selectPoint == pointTopRight)
            {
                Width = Math.Abs(p.X - prevX);
                X = Math.Min(p.X, prevX);
                if (Height + Y - p.Y >= 0)
                {
                    Height += Y - p.Y;
                    Y = p.Y;
                }
            }
            else if (selectPoint == pointBottomLeft)
            {
                if (Width + X - p.X >= 0)
                {
                    Width += X - p.X;
                    X = p.X;
                }
                Height = Math.Abs(p.Y - prevY);
                Y = Math.Min(p.Y, prevY);
            }
            else if (selectPoint == pointBottomRight)
            {
                Width = Math.Abs(p.X - prevX);
                Height = Math.Abs(p.Y - prevY);
                X = Math.Min(p.X, prevX);
                Y = Math.Min(p.Y, prevY);
            }
        }
        public void SelectedMouseUp(Point p)
        {
            SelectedMouseMove(p);
        }

        public string ToSVG()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .Append(string.Format("<rect x='{0}' y='{1}' width='{2}' height='{3}' ", 
                        x.ToString(new CultureInfo("en-US")), y.ToString(new CultureInfo("en-US")), 
                        width.ToString(new CultureInfo("en-US")), height.ToString(new CultureInfo("en-US"))))
                .Append("style='")
                .Append(string.Format("fill: rgb({0}, {1}, {2}); ", fillColor.R, fillColor.G, fillColor.B))
                .Append(string.Format("stroke-width: {0}; ", strokeThickness.ToString(new CultureInfo("en-US"))))
                .Append(string.Format("stroke: rgb({0}, {1}, {2})'", strokeColor.R, strokeColor.G, strokeColor.B))
                .Append("/>");
            return stringBuilder.ToString();            
        }

        public IFigure Copy()
        {
            return new Rectangle()
            {
                Width = width,
                Height = height,
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = strokeThickness,
                X = x,
                Y = y
            };
        }
    }
}
