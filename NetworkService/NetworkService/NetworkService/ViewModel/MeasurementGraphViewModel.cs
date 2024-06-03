using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        public ObservableCollection<Entity> Entities { get; set; }
        public ObservableCollection<double> RectHeights { get; set; }
        public ObservableCollection<string> GraphTimes { get; set; }
        public ObservableCollection<SolidColorBrush> GraphColors { get; set; }
        public MyICommand ComboBoxSelectionChanged { get; set; }

        private Entity selectedEntity;

        public MeasurementGraphViewModel()
        {
            Entities = new ObservableCollection<Entity>();
            ComboBoxSelectionChanged = new MyICommand(OnComboBoxSelectedItemChanged);
            Entities.CollectionChanged += Entities_CollectionChanged;
            InitializeRectangleHeights();
            InitializeGraphTimes();
            InitializeGraphColors();
        }

        private void OnComboBoxSelectedItemChanged()
        {
            UpdateGraph();
        }
        private void InitializeGraphColors()
        {
            GraphColors = new ObservableCollection<SolidColorBrush>();
            for (int i = 0; i < 5; i++)
            {
                GraphColors.Add(Brushes.Blue);
            }
        }

        private void Entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Entity entity in e.NewItems)
                {
                    entity.PropertyChanged += Entity_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (Entity entity in e.OldItems)
                {
                    entity.PropertyChanged -= Entity_PropertyChanged;
                }
            }
        }

        private void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Entity.Value))
            {
                // Koristi Dispatcher da bi osigurao da se UpdateGraph izvršava u UI niti
                Application.Current.Dispatcher.Invoke(() => UpdateGraph());
            }
        }

        //Namesti da se ova funkcija poziva odmah po promeni u comboboxu
        private void UpdateGraph()
        {
            if(SelectedEntity != null)
            {
                if (SelectedEntity.Last_5_Values.Count > 0)
                {
                    RectHeights[0] = SelectedEntity.Last_5_Values[0].Item2*10;

                    if (SelectedEntity.Last_5_Values[0].Item2 < 5)
                    {
                        GraphColors[0] = new SolidColorBrush(Colors.Red);
                    }
                    else if (SelectedEntity.Last_5_Values[0].Item2 > 16)
                    {
                        GraphColors[0] = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        GraphColors[0] = new SolidColorBrush(Colors.Blue);
                    }

                    DateTime dateTime = SelectedEntity.Last_5_Values[0].Item1;
                    GraphTimes[0] = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                }
                else
                {
                    RectHeights[0] = 0;
                    GraphColors[0] = new SolidColorBrush(Colors.Blue);
                    GraphTimes[0] = "00:00";
                }

                // Nastavi sa istom logikom za ostale vrednosti
                if (SelectedEntity.Last_5_Values.Count > 1)
                {
                    RectHeights[1] = SelectedEntity.Last_5_Values[1].Item2*10;

                    if (SelectedEntity.Last_5_Values[1].Item2 < 5)
                    {
                        GraphColors[1] = new SolidColorBrush(Colors.Red);
                    }
                    else if (SelectedEntity.Last_5_Values[1].Item2 > 16)
                    {
                        GraphColors[1] = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        GraphColors[1] = new SolidColorBrush(Colors.Blue);
                    }

                    DateTime dateTime = SelectedEntity.Last_5_Values[1].Item1;
                    GraphTimes[1] = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                }
                else
                {
                    RectHeights[1] = 0;
                    GraphColors[1] = new SolidColorBrush(Colors.Blue);
                    GraphTimes[1] = "00:00";
                }

                if (SelectedEntity.Last_5_Values.Count > 2)
                {
                    RectHeights[2] = SelectedEntity.Last_5_Values[2].Item2*10;

                    if (SelectedEntity.Last_5_Values[2].Item2 < 5)
                    {
                        GraphColors[2] = new SolidColorBrush(Colors.Red);
                    }
                    else if (SelectedEntity.Last_5_Values[2].Item2 > 16)
                    {
                        GraphColors[2] = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        GraphColors[2] = new SolidColorBrush(Colors.Blue);
                    }

                    DateTime dateTime = SelectedEntity.Last_5_Values[2].Item1;
                    GraphTimes[2] = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                }
                else
                {
                    RectHeights[2] = 0;
                    GraphColors[2] = new SolidColorBrush(Colors.Blue);
                    GraphTimes[2] = "00:00";
                }

                if (SelectedEntity.Last_5_Values.Count > 3)
                {
                    RectHeights[3] = SelectedEntity.Last_5_Values[3].Item2*10;

                    if (SelectedEntity.Last_5_Values[3].Item2 < 5)
                    {
                        GraphColors[3] = new SolidColorBrush(Colors.Red);
                    }
                    else if (SelectedEntity.Last_5_Values[3].Item2 > 16)
                    {
                        GraphColors[3] = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        GraphColors[3] = new SolidColorBrush(Colors.Blue);
                    }

                    DateTime dateTime = SelectedEntity.Last_5_Values[3].Item1;
                    GraphTimes[3] = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                }
                else
                {
                    RectHeights[3] = 0;
                    GraphColors[3] = new SolidColorBrush(Colors.Blue);
                    GraphTimes[3] = "00:00";
                }

                if (SelectedEntity.Last_5_Values.Count > 4)
                {
                    RectHeights[4] = SelectedEntity.Last_5_Values[4].Item2*10;

                    if (SelectedEntity.Last_5_Values[4].Item2 < 5)
                    {
                        GraphColors[4] = new SolidColorBrush(Colors.Red);
                    }
                    else if (SelectedEntity.Last_5_Values[4].Item2 > 16)
                    {
                        GraphColors[4] = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        GraphColors[4] = new SolidColorBrush(Colors.Blue);
                    }

                    DateTime dateTime = SelectedEntity.Last_5_Values[4].Item1;
                    GraphTimes[4] = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                }
                else
                {
                    RectHeights[4] = 0;
                    GraphColors[4] = new SolidColorBrush(Colors.Blue);
                    GraphTimes[4] = "00:00";
                }
            }
            
        }

        private void InitializeGraphTimes()
        {
            GraphTimes = new ObservableCollection<string>();
            for (int i = 0; i < 5; i++)
            {
                GraphTimes.Add("00:00");
            }
        }

        private void InitializeRectangleHeights()
        {
            RectHeights = new ObservableCollection<double>();
            for (int i = 0; i < 5; i++)
            {
                RectHeights.Add(0);
            }
        }

        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
            }
        }
    }
}
