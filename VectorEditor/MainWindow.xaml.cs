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

        public Color CurrentFillColor
        {
            get { return currentFillColor; }
            set
            {
                currentFillColor = value;
                if (currentFillColor == Colors.Transparent)
                    fillBtn.Background = Brushes.White;
                else
                    fillBtn.Background = new SolidColorBrush(currentFillColor);
            }
        }
        public Color CurrentStrokeColor
        {
            get { return currentStrokeColor; }
            set
            {
                currentStrokeColor = value;
                if (currentStrokeColor == Colors.Transparent)
                    strokeBtn.Background = Brushes.White;
                else
                    strokeBtn.Background = new SolidColorBrush(currentStrokeColor);
            }
        }
        public double CurrentStrokeThickness
        {
            get { return currentStrokeThickness; }
            set
            {
                currentStrokeThickness = value;
                thicknessBox.Text = currentStrokeThickness.ToString();
            }
        }

        public double ImageHeight
        {
            get { return image.Height; }
            set
            {
                image.Height = value;
                heightTextBox.Text = value.ToString();
            }
        }
        public double ImageWidth
        {
            get { return image.Width; }
            set
            {
                image.Width = value;
                widthTextBox.Text = value.ToString();
            }
        }

        public IFigure SelectedFigure
        {
            get
            {
                return selectedFigure;
            }

            set
            {
                selectedFigure = value;
                if (selectedFigure != null)
                {
                    SelectedFigureProperties.Visibility = Visibility.Visible;
                    UpdateSelectedFigureInfo();
                }
                else
                {
                    SelectedFigureProperties.Visibility = Visibility.Collapsed;
                }
            }
        }

        private IFigure selectedFigure = null;

        private System.Windows.Forms.ColorDialog colorDlg;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            currentMode = Mode.SELECT;
            SelectActiveOnToolbar(SelectModeBtn);
            image = new Classes.Image(canvas, canvasBorder, 400, 250);
            ImageHeight = 250;
            ImageWidth = 400;

            colorDlg = new System.Windows.Forms.ColorDialog();


            // temp ( TODO : save settings in file and load them here )
            CurrentFillColor = Colors.Transparent;
            CurrentStrokeColor = Colors.Black;
            CurrentStrokeThickness = 3;
        }

        private void UpdateSelectedFigureInfo()
        {
            selectedFigureFill.Background = new SolidColorBrush(selectedFigure.FillColor);
            selectedFigureStroke.Background = new SolidColorBrush(selectedFigure.StrokeColor);
            selectedFigureThickness.Text = selectedFigure.StrokeThickness.ToString();
        }

        private void SelectActiveOnToolbar(UIElement activeButton)
        {
            foreach (UIElement element in Toolbar.Children)
            {
                if (element is Button)
                {
                    if (element != activeButton)
                    {
                        (element as Button).Tag = "NotActive";
                    }
                    else
                        (element as Button).Tag = "Active";
                }
            }
        }

        private void SelectModeBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.SELECT;
            SelectActiveOnToolbar(sender as Button);
        }

        private void AddRectBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.ADD_FIGURE;
            currentFigureType = typeof(Classes.Figures.Rectangle);
            SelectActiveOnToolbar(sender as Button);
        }

        private void AddEllipseBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.ADD_FIGURE;
            currentFigureType = typeof(Classes.Figures.Rectangle);
            SelectActiveOnToolbar(sender as Button);
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
                        if (figure != SelectedFigure)
                        {
                            if (SelectedFigure != null)
                                SelectedFigure.Selected = false;
                            SelectedFigure = figure;
                            SelectedFigure.Selected = true;
                        }
                        SelectedFigure.SelectedMouseDown(e.GetPosition(canvas));
                        isSelectedMouseDown = true;
                        canvas.CaptureMouse();
                    }
                    else
                    {
                        if (SelectedFigure != null)
                        {
                            SelectedFigure.Selected = false;
                            SelectedFigure = null;
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
                        if (SelectedFigure != null && isSelectedMouseDown)
                        {
                            SelectedFigure.SelectedMouseMove(e.GetPosition(canvas));
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
                    if (SelectedFigure != null && isSelectedMouseDown)
                    {
                        SelectedFigure.SelectedMouseUp(e.GetPosition(canvas));
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

        private void fillBtn_Click(object sender, RoutedEventArgs e)
        {
            colorDlg.Color = System.Drawing.Color.FromArgb(CurrentFillColor.A, CurrentFillColor.R, CurrentFillColor.G, CurrentFillColor.B);
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentFillColor = Color.FromArgb(colorDlg.Color.A, colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B);
            }
        }

        private void strokeBtn_Click(object sender, RoutedEventArgs e)
        {
            colorDlg.Color = System.Drawing.Color.FromArgb(CurrentStrokeColor.A, CurrentStrokeColor.R, CurrentStrokeColor.G, CurrentStrokeColor.B);
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentStrokeColor = Color.FromArgb(colorDlg.Color.A, colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B);
            }
        }

        private void thicknessBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double t = 0;
            if (double.TryParse(thicknessBox.Text, out t) && t >= 0)
            {
                CurrentStrokeThickness = t;
            }
            else
            {
                MessageBox.Show(this, "Thickness must be positive number");
                CurrentStrokeThickness = 1;
            }            
        }

        private void widthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double w;
                if (double.TryParse(widthTextBox.Text, out w) && w > 0)
                {
                    ImageWidth = w;
                }
                else
                {
                    MessageBox.Show("Width must be positive number");
                    widthTextBox.Text = image.Width.ToString();
                }
            }
        }

        private void heightTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double h;
                if (double.TryParse(heightTextBox.Text, out h) && h > 0)
                {
                    ImageHeight = h;
                }
                else
                {
                    MessageBox.Show("Width must be positive number");
                    heightTextBox.Text = image.Height.ToString();
                }
            }
        }

        private void selectedFigureFill_Click(object sender, RoutedEventArgs e)
        {
            colorDlg.Color = System.Drawing.Color.FromArgb(selectedFigure.FillColor.A, selectedFigure.FillColor.R, selectedFigure.FillColor.G, selectedFigure.FillColor.B);
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFigure.FillColor = Color.FromArgb(colorDlg.Color.A, colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B);
                UpdateSelectedFigureInfo();
            }
        }

        private void selectedFigureStroke_Click(object sender, RoutedEventArgs e)
        {
            colorDlg.Color = System.Drawing.Color.FromArgb(selectedFigure.StrokeColor.A, selectedFigure.StrokeColor.R, selectedFigure.StrokeColor.G, selectedFigure.StrokeColor.B);
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFigure.StrokeColor = Color.FromArgb(colorDlg.Color.A, colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B);
                UpdateSelectedFigureInfo();
            }
        }

        private void selectedFigureThickness_KeyDown(object sender, KeyEventArgs e)
        {
            double t = 0;
            if (double.TryParse(selectedFigureThickness.Text, out t) && t >= 0)
            {
                selectedFigure.StrokeThickness = t;
                UpdateSelectedFigureInfo();
            }
            else
            {
                MessageBox.Show(this, "Thickness must be positive number");
                selectedFigureThickness.Text = selectedFigure.StrokeThickness.ToString();
            }
        }
    }
}
