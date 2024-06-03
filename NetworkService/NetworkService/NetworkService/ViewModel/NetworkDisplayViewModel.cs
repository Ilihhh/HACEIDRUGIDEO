using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    class NetworkDisplayViewModel : BindableBase
    {
        public ObservableCollection<EntityTypeGroup> GroupedEntities { get; set; }
        public ObservableCollection<Canvas> CanvasCollection { get; set; }
        public ObservableCollection<Line> LineCollection { get; set; }
        public ObservableCollection<Entity> EntitiesOnCanvas { get; set; }
        public ObservableCollection<Brush> BorderBrushCollection { get; set; }

        public BrushConverter brushConverter { get; set; }

        private Entity selectedEntity;

        private Entity draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;

        public MyICommand<object> DropEntityOnCanvas { get; set; }
        public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
        public MyICommand MouseLeftButtonUp { get; set; }
        public MyICommand<object> SelectedItemChangedCommand { get; set; }
        public MyICommand<object> FreeUpCanvas { get; set; }
        public MyICommand<object> RightMouseButtonDownOnCanvas { get; set; }

        private bool isLineSourceSelected = false;
        private int sourceCanvasIndex = -1;
        private int destinationCanvasIndex = -1;
        private Line currentLine = new Line();
        private Point linePoint1 = new Point();
        private Point linePoint2 = new Point();

        public NetworkDisplayViewModel()
        {
            LineCollection = new ObservableCollection<Line>();
            brushConverter = new BrushConverter();
            InitializeCanvases();
            InitializeGroupedEntities();
            InitializeEntitiesOncanvas();
            InitializeBorderBrushes();

            DropEntityOnCanvas = new MyICommand<object>(OnDrop);
            LeftMouseButtonDownOnCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
            MouseLeftButtonUp = new MyICommand(OnMouseLeftButtonUp);
            SelectedItemChangedCommand = new MyICommand<object>(OnSelectedItemChanged);
            FreeUpCanvas = new MyICommand<object>(OnFreeUpCanvas);
            RightMouseButtonDownOnCanvas = new MyICommand<object>(OnRightMouseButtonDown);
        }

        private void InitializeBorderBrushes()
        {
            BorderBrushCollection = new ObservableCollection<Brush>();
            for (int i = 0; i < 12; i++)
            {
                BorderBrushCollection.Add((SolidColorBrush)(brushConverter.ConvertFrom("#BBBBBB")));
            }
        }
       private void InitializeEntitiesOncanvas()
        {
            EntitiesOnCanvas = new ObservableCollection<Entity>();
            for(int i = 0; i < 12; i++)
            {
                EntitiesOnCanvas.Add(new Entity());
            }

        }
       private void InitializeGroupedEntities()
        {
            GroupedEntities = new ObservableCollection<EntityTypeGroup>();

            EntityTypeGroup type1 = new EntityTypeGroup("Digital Manometer");
            EntityTypeGroup type2 = new EntityTypeGroup("Cable Sensor");

            GroupedEntities.Add(type1);
            GroupedEntities.Add(type2);
        }
        

        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                OnPropertyChanged("SelectedEntity");
            }
        }

        private void OnSelectedItemChanged(object selectedItem)
        {
            if (!dragging && selectedItem is Entity selectedEntity)
            {
                dragging = true;
                draggedItem = selectedEntity;
                DragDrop.DoDragDrop(Application.Current.MainWindow, draggedItem, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }
        private void OnDrop(object parameter)
        {
            if(draggedItem != null)
            {
                int index = Convert.ToInt32(parameter);

                if (CanvasCollection[index].Resources["taken"] == null)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(draggedItem.ImagePath, UriKind.RelativeOrAbsolute);
                    image.EndInit();

                    CanvasCollection[index].Background = new ImageBrush(image);
                    CanvasCollection[index].Resources.Add("taken", true);
                    CanvasCollection[index].Resources.Add("data", draggedItem);
                    EntitiesOnCanvas[index] = draggedItem;

                    if(draggingSourceIndex != -1)
                    {
                        CanvasCollection[draggingSourceIndex].Background = Brushes.LightGray;
                        CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
                        CanvasCollection[draggingSourceIndex].Resources.Remove("data");

                        UpdateLinesForCanvas(draggingSourceIndex, index);

                        if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            currentLine = new Line();
                        }

                        draggingSourceIndex = -1;

                        Application.Current.Dispatcher.Invoke(() => UpdateEntityOnCanvas(draggedItem));
                    }
                    else//
                    {
                        if(draggedItem.Type == GroupedEntities[0].Type)
                        {
                            GroupedEntities[0].Entities.Remove(draggedItem);
                        }
                        else
                        {
                            GroupedEntities[1].Entities.Remove(draggedItem);
                        }
                    }
                }
            }
        }
        private void UpdateLinesForCanvas(int sourceCanvas, int destinationCanvas)
        {
            for (int i = 0; i < LineCollection.Count; i++)
            {
                if (LineCollection[i].Source == sourceCanvas)
                {
                    Point newSourcePoint = GetPointForCanvasIndex(destinationCanvas);
                    LineCollection[i].X1 = newSourcePoint.X;
                    LineCollection[i].Y1 = newSourcePoint.Y;
                    LineCollection[i].Source = destinationCanvas;
                }
                else if (LineCollection[i].Destination == sourceCanvas)
                {
                    Point newDestinationPoint = GetPointForCanvasIndex(destinationCanvas);
                    LineCollection[i].X2 = newDestinationPoint.X;
                    LineCollection[i].Y2 = newDestinationPoint.Y;
                    LineCollection[i].Destination = destinationCanvas;
                }
            }
        }

        private Point GetPointForCanvasIndex(int index)
        {
            double x = 0;
            double y = 0;
            for(int row = 0; row < 4; row++)
            {
                for(int col = 0; col < 3; col++)
                {
                    int currentIndex = row * 3 + col;
                    if(index == currentIndex)
                    {
                        x = 74 + (col * 148);
                        y = 60 + (row * 125);
                        break;
                    }
                }
            }
            return new Point(x, y);
        }
        private void OnLeftMouseButtonDown(object parameter)
        {
            if (dragging)                                               //NISAM SIGURAN DAL OVO SME AL RADI :)
            {
                int index = Convert.ToInt32(parameter);
                if (CanvasCollection[index].Resources["taken"] != null)
                {
                    dragging = true;
                    draggedItem = (Entity)(CanvasCollection[index].Resources["data"]);
                    draggingSourceIndex = index;
                    DragDrop.DoDragDrop(CanvasCollection[index], draggedItem, DragDropEffects.Move);
                }
            }
        }
        private void OnRightMouseButtonDown(object parameter)
        {
            int index = Convert.ToInt32(parameter);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                if (!isLineSourceSelected)
                {
                    sourceCanvasIndex = index;

                    linePoint1 = GetPointForCanvasIndex(sourceCanvasIndex);

                    currentLine.X1 = linePoint1.X;
                    currentLine.Y1 = linePoint1.Y;
                    currentLine.Source = sourceCanvasIndex;

                    isLineSourceSelected = true;
                }
                else
                {
                    destinationCanvasIndex = index;

                    if ((sourceCanvasIndex != destinationCanvasIndex) && !DoesLineAlreadyExist(sourceCanvasIndex, destinationCanvasIndex))
                    {
                        linePoint2 = GetPointForCanvasIndex(destinationCanvasIndex);

                        currentLine.X2 = linePoint2.X;
                        currentLine.Y2 = linePoint2.Y;
                        currentLine.Destination = destinationCanvasIndex;

                        LineCollection.Add(new Line
                        {
                            X1 = currentLine.X1,
                            Y1 = currentLine.Y1,
                            X2 = currentLine.X2,
                            Y2 = currentLine.Y2,
                            Source = currentLine.Source,
                            Destination = currentLine.Destination
                        });

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new Line();
                    }
                    else
                    {
                        // Pocetak i kraj linije su u istom canvasu

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new Line();
                    }
                }
            }
        }

        private bool DoesLineAlreadyExist(int sourceIndex, int destinationIndex)
        {
            foreach (Line line in LineCollection)
            {
                if ((line.Source == sourceIndex) && (line.Destination == destinationIndex))
                {
                    return true;
                }
                if ((line.Source == destinationIndex) && (line.Destination == sourceIndex))
                {
                    return true;
                }
            }
            return false;
        }
        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            SelectedEntity = null;
            dragging = false;
            draggingSourceIndex = -1;
        }

        public void DeleteEntityFromCanvas(Entity e)
        {
            int canvasIndex = GetCanvasIndexForEntityId(e.Id);

            if (canvasIndex != -1)
            {
                CanvasCollection[canvasIndex].Background = Brushes.LightGray;
                CanvasCollection[canvasIndex].Resources.Remove("taken");
                CanvasCollection[canvasIndex].Resources.Remove("data");
                //BorderBrushCollection[canvasIndex] = Brushes.DarkGray;

               DeleteLinesForCanvas(canvasIndex);
            }

        }

        public void UpdateEntityOnCanvas(Entity entity)
        {
            int canvasIndex = GetCanvasIndexForEntityId(entity.Id);

            
            if (canvasIndex != -1)
            {
                if (entity.IsValueValid())
                {
                    BorderBrushCollection[canvasIndex] = (SolidColorBrush)(brushConverter.ConvertFrom("#BBBBBB"));
                }
                else
                {
                    BorderBrushCollection[canvasIndex] = Brushes.Red;
                }
            }

            for (int i = 0; i < CanvasCollection.Count; i++)
            {
                Entity e = (CanvasCollection[i].Resources["data"]) as Entity;

                if (e == null)
                {
                    BorderBrushCollection[i] = (SolidColorBrush)(brushConverter.ConvertFrom("#BBBBBB"));
                    EntitiesOnCanvas[i] = new Entity();
                }
            }

        }

        private int GetCanvasIndexForEntityId(int id)
        {
            for(int i = 0; i < CanvasCollection.Count; i++)
            {
                Entity entity = (CanvasCollection[i].Resources["data"]) as Entity;

                if((entity != null) && entity.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        private void OnFreeUpCanvas(object parameter)
        {
            int index = Convert.ToInt32(parameter);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                if (sourceCanvasIndex != -1)
                {
                    isLineSourceSelected = false;
                    sourceCanvasIndex = -1;
                    linePoint1 = new Point();
                    linePoint2 = new Point();
                    currentLine = new Line();
                }

                DeleteLinesForCanvas(index);

                Entity entityToReturn = (Entity)CanvasCollection[index].Resources["data"];
                if(entityToReturn.Type == GroupedEntities[0].Type)
                {
                    GroupedEntities[0].Entities.Add(entityToReturn);
                }
                else
                {
                    GroupedEntities[1].Entities.Add(entityToReturn);
                }
                CanvasCollection[index].Background = Brushes.LightGray;
                CanvasCollection[index].Resources.Remove("taken");
                CanvasCollection[index].Resources.Remove("data");
                EntitiesOnCanvas[index] = new Entity();
                BorderBrushCollection[index] = Application.Current.Dispatcher.Invoke(() => (SolidColorBrush)(brushConverter.ConvertFrom("#BBBBBB")));
            }
        }

        private void DeleteLinesForCanvas(int canvasIndex)
        {
            List<Line> linesToDelete = new List<Line>();

            for (int i = 0; i < LineCollection.Count; i++)
            {
                if ((LineCollection[i].Source == canvasIndex) || (LineCollection[i].Destination == canvasIndex))
                {
                    linesToDelete.Add(LineCollection[i]);
                }
            }

            foreach (Line line in linesToDelete)
            {
                LineCollection.Remove(line);
            }
        }
        private void InitializeCanvases()
        {
            CanvasCollection = new ObservableCollection<Canvas>();
            for (int i = 0; i < 12; i++)
            {
                CanvasCollection.Add(new Canvas
                {
                    Background = Brushes.LightGray,
                    AllowDrop = true
                });
            }
        }
        
    }
}
