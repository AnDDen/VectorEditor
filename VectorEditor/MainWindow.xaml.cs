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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VectorEditor.Classes;
using VectorEditor.Classes.Figures;

namespace VectorEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Mode currentMode;
        private Type currentFigureType;
        private IFigure addFigure;

        private bool isCanvasMouseDown;
        private bool isSelectedMouseDown;

        private Classes.Image image;

        private Color currentFillColor;
        private Color currentStrokeColor;
        private double currentStrokeThickness;

        private IFigure selectedFigure = null; 

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            currentMode = Mode.SELECT;
            image = new Classes.Image(canvas, 400, 250);

            // temp ( TODO : save settings in file and load them here )
            currentFillColor = Colors.White;
            currentStrokeColor = Colors.Black;
            currentStrokeThickness = 3;
        }

        private void SelectModeBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.SELECT;
        }

        private void AddRectBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.ADD_FIGURE;
            currentFigureType = typeof(Classes.Figures.Rectangle);
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isCanvasMouseDown = true;

            switch (currentMode)
            {
                case Mode.SELECT:
                    IFigure figure = null;
                    isSelectedMouseDown = false;

                    for (int i = image.Figures.Count - 1; i >= 0; i--)
                    {
                        if (image.Figures[i].Element.IsMouseOver)
                        {
                            figure = image.Figures[i];
                            break;
                        }
                    }

                    if (figure != null)
                    {                   
                        if (figure != selectedFigure)
                        {
                            if (selectedFigure != null)
                                selectedFigure.Selected = false;
                            selectedFigure = figure;
                            selectedFigure.Selected = true;
                        }
                        selectedFigure.SelectedMouseDown(e.GetPosition(canvas));
                        isSelectedMouseDown = true;
                        canvas.CaptureMouse();
                    }
                    else
                    {
                        if (selectedFigure != null)
                        {
                            selectedFigure.Selected = false;
                            selectedFigure = null;
                        }
                    }
                    break;

                case Mode.ADD_FIGURE:
                    addFigure = FigureFactory.GetFigure(currentFigureType);
                    addFigure.AddMouseDown(e.GetPosition(canvas));
                    addFigure.FillColor = currentFillColor;
                    addFigure.StrokeColor = currentStrokeColor;
                    addFigure.StrokeThickness = currentStrokeThickness;
                    image.AddFigure(addFigure);
                    canvas.CaptureMouse();
                    break;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            switch (currentMode)
            {
                case Mode.SELECT:
                    if (isCanvasMouseDown)
                    {
                        if (selectedFigure != null && isSelectedMouseDown)
                        {
                            selectedFigure.SelectedMouseMove(e.GetPosition(canvas));
                        }
                    }
                    break;

                case Mode.ADD_FIGURE:
                    if (isCanvasMouseDown)
                    {
                        if (addFigure != null)
                            addFigure.AddMouseMove(e.GetPosition(canvas));
                    }
                    break;
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (currentMode)
            {
                case Mode.SELECT:
                    if (selectedFigure != null && isSelectedMouseDown)
                    {
                        selectedFigure.SelectedMouseUp(e.GetPosition(canvas));
                    }
                    break;

                case Mode.ADD_FIGURE:
                    if (addFigure != null)
                        addFigure.AddMouseUp(e.GetPosition(canvas));
                    break;
            }

            canvas.ReleaseMouseCapture();
            isCanvasMouseDown = false;
        }
    }
}
