using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        public HistoryService imageHistory;

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
                    deleteBtn.IsEnabled = true;
                    UpdateSelectedFigureInfo();
                }
                else
                {
                    SelectedFigureProperties.Visibility = Visibility.Collapsed;
                    deleteBtn.IsEnabled = false;
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

            CloseImage();
            NewImage(600, 400);

            colorDlg = new System.Windows.Forms.ColorDialog();

            // temp ( TODO : save settings in file and load them here )
            CurrentFillColor = Colors.Transparent;
            CurrentStrokeColor = Colors.Black;
            CurrentStrokeThickness = 3;
        }

        private void NewImage(double width, double height)
        {
            ImageProperties.Visibility = Visibility.Visible;
            canvasBorder.Visibility = Visibility.Visible;
            currentMode = Mode.SELECT;
            SelectActiveOnToolbar(SelectModeBtn);
            image = new Classes.Image(canvas, canvasBorder, width, height);
            ImageHeight = height;
            ImageWidth = width;
            imageHistory = new HistoryService();
            imageHistory.AddState(image);
            UpdateUndoRedoBtns();
        }

        private void CloseImage()
        {
            ImageProperties.Visibility = Visibility.Collapsed;
            canvasBorder.Visibility = Visibility.Collapsed;
            image = null;
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

        private void AddPathBtn_Click(object sender, RoutedEventArgs e)
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
                        SaveImageState();
                    }
                    break;

                case Mode.ADD_FIGURE:
                    if (addFigure != null)
                    {
                        addFigure.AddMouseUp(e.GetPosition(canvas));
                        SaveImageState();
                    }
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

        private void layerUpBtn_Click(object sender, RoutedEventArgs e)
        {
            image.LayerUp(selectedFigure);
        }

        private void layerDownBtn_Click(object sender, RoutedEventArgs e)
        {
            image.LayerDown(selectedFigure);
        }

        private void layerTopBtn_Click(object sender, RoutedEventArgs e)
        {
            image.LayerTop(selectedFigure);
        }

        private void layerBottomBtn_Click(object sender, RoutedEventArgs e)
        {
            image.LayerBottom(selectedFigure);
        }

        private void SaveImageState()
        {
            imageHistory.AddState(image.Copy());
            UpdateUndoRedoBtns();
        }

        private void LoadImageState(Classes.Image img)
        {
            image.Width = img.Width;
            image.Height = img.Height;
            image.Figures.Clear();
            image.Canvas.Children.Clear();
            foreach (IFigure figure in img.Figures)
            {
                image.AddFigure(figure.Copy());
            }
            UpdateUndoRedoBtns();
        }

        private void UpdateUndoRedoBtns()
        {
            undoBtn.IsEnabled = imageHistory.CanUndo();
            redoBtn.IsEnabled = imageHistory.CanRedo();
        }

        SaveFileDialog fileDlg = new SaveFileDialog()
        {
            AddExtension = true,
            Filter = "Scalable Vector Graphics Files (*.svg)|*.svg|All Files (*.*)|*.*"
        };

        private void UndoCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            if (imageHistory.CanUndo())
            {
                LoadImageState(imageHistory.Undo());
            }
        }

        private void RedoCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            if (imageHistory.CanRedo())
            {
                LoadImageState(imageHistory.Redo());
            }
        }

        private void NewCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            NewImageWindow newImgWnd = new NewImageWindow();
            newImgWnd.ShowDialog();
            bool? res = newImgWnd.DialogResult;
            if (res.HasValue && res.Value)
            {
                CloseImage();
                NewImage(newImgWnd.ImageWidth, newImgWnd.ImageHeight);
            }
        }

        private void CloseCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void DeleteCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            if (selectedFigure != null)
            {
                image.RemoveFigure(selectedFigure);
                SelectedFigure = null;
            }
        }

        OpenFileDialog openFileDlg = new OpenFileDialog()
        {
            AddExtension = true,
            Filter = "Vector Graphics Files (*.dat)|*.dat|All Files (*.*)|*.*"
        };

        private void OpenCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            bool? res = openFileDlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                //open here
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(openFileDlg.FileName, FileMode.OpenOrCreate))
                {
                    Classes.Image tmp = (Classes.Image)formatter.Deserialize(fs);
                    CloseImage();
                    NewImage(tmp.Width, tmp.Height);
                    foreach (IFigure f in tmp.Figures)
                    {
                        image.AddFigure(f);
                    }
                }
            }
        }

        SaveFileDialog saveFileDlg = new SaveFileDialog()
        {
            AddExtension = true,
            Filter = "Vector Graphics Files (*.dat)|*.dat|All Files (*.*)|*.*"
        };

        private void SaveCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            bool? res = saveFileDlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                // save here
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(saveFileDlg.FileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, image);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool? res = fileDlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                try
                {
                    image.SaveToSVG(fileDlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
