using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public static Stack<SaveState<CommandType, object>> UndoStack { get; set; }
        public MyICommand UndoCommand { get; set; }

        public MyICommand<string> NavCommand { get; private set; }
        

        private NetworkEntitiesViewModel networkEntitiesViewModel = new NetworkEntitiesViewModel();
        private NetworkDisplayViewModel networkDisplayViewModel = new NetworkDisplayViewModel();
        private MeasurementGraphViewModel measurementGraphViewModel = new MeasurementGraphViewModel();

        private BindableBase currentViewModel;

        //private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom

            NavCommand = new MyICommand<string>(OnNav);

            UndoStack = new Stack<SaveState<CommandType, object>>();

            UndoCommand = new MyICommand(OnUndo, CanUndo);

            CurrentViewModel = networkEntitiesViewModel;

            networkEntitiesViewModel.Entities.CollectionChanged += this.OnCollectionChanged;
        }

        public void OnUndo()
        {
            SaveState<CommandType, object> saveState = UndoStack.Pop();
            if (saveState.CommandType == CommandType.NavCommand)
            {
                Type viewModelType = saveState.SavedState as Type;

                if (viewModelType == typeof(NetworkEntitiesViewModel))
                {
                    CurrentViewModel = networkEntitiesViewModel;
                }
                else if (viewModelType == typeof(NetworkDisplayViewModel))
                {
                    CurrentViewModel = networkDisplayViewModel;
                }
                else if (viewModelType == typeof(MeasurementGraphViewModel))
                {
                    CurrentViewModel = measurementGraphViewModel;
                }
                else
                {
                    CurrentViewModel = networkEntitiesViewModel;
                }
            }
            else if (saveState.CommandType == CommandType.EntityManipulation)
            {
                ObservableCollection<Entity> savedEntities = saveState.SavedState as ObservableCollection<Entity>;
                if (savedEntities != null)
                {
                    // Očistite trenutne entitete
                    var entities = networkEntitiesViewModel.Entities.ToList(); // Kopiramo trenutne entitete u listu

                    // Uklanjamo entitete jedan po jedan
                    foreach (var entity in entities)
                    {
                        if(!savedEntities.Contains(entity))
                        {
                            networkEntitiesViewModel.Entities.Remove(entity);
                        }
                    }

                    // Dodajte nove entitete jedan po jedan
                    foreach (var entity in savedEntities)
                    {
                        if(!entities.Contains(entity))
                        {
                            networkEntitiesViewModel.Entities.Add(entity);
                        }
                    }
                }
                networkEntitiesViewModel.FilterData();
            }
            else if (saveState.CommandType == CommandType.CanvasManipulation)
            {
                List<object> state = saveState.SavedState as List<object>;
                if (state == null || state.Count < 2)
                {
                    Debug.WriteLine("Error: state is null or does not contain enough elements.");
                    return;
                }

                // Proverite stvarne tipove objekata u listi
                foreach (var item in state)
                {
                    Debug.WriteLine("Type of item in state: " + item.GetType());
                }

                // Direkto kastovanje elemenata
                List<Entity> list1 = state[0] as List<Entity>;
                List<Line> list2 = state[1] as List<Line>;

                if (list1 == null)
                {
                    Debug.WriteLine("Error: list1 is null.");
                }
                if (list2 == null)
                {
                    Debug.WriteLine("Error: list2 is null.");
                }

                // Dalji kod koji koristi list1 i list2
                networkDisplayViewModel.DeleteFromCanvas();

                for (int i = 0; i < 12; i++)
                {
                    networkDisplayViewModel.EntitiesOnCanvas[i] = list1[i];
                }
                networkDisplayViewModel.UpdateOnCanvas();

                if (list2 != null)
                {
                    foreach (Line l in list2)
                    {
                        networkDisplayViewModel.LineCollection.Add(l);
                    }
                }
            }
            

            GC.Collect();
            UndoCommand.RaiseCanExecuteChanged();
            
        }
        public bool CanUndo()
        {
            return UndoStack.Count != 0;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UndoCommand.RaiseCanExecuteChanged();
            if (e.NewItems != null)
            {
                foreach(Entity newEntity in e.NewItems)
                {
                    if(newEntity.Type == networkDisplayViewModel.GroupedEntities[0].Type)
                    {
                        if (!networkDisplayViewModel.GroupedEntities[0].Entities.Contains(newEntity))
                        {
                            networkDisplayViewModel.GroupedEntities[0].Entities.Add(newEntity);                            
                        }
                    }
                    else
                    {
                        if (!networkDisplayViewModel.GroupedEntities[1].Entities.Contains(newEntity))
                        {
                            networkDisplayViewModel.GroupedEntities[1].Entities.Add(newEntity);
                        }
                    }
                }
                //za grafcuge
                foreach (Entity newEntity in e.NewItems)
                {
                    if(!measurementGraphViewModel.Entities.Contains(newEntity))
                    {
                        measurementGraphViewModel.Entities.Add(newEntity);
                    }
                }
            }

            if(e.OldItems != null)
            {
                foreach(Entity oldEntity in e.OldItems)
                {

                    if (networkDisplayViewModel.GroupedEntities[0].Entities.Contains(oldEntity))
                    {
                        networkDisplayViewModel.GroupedEntities[0].Entities.Remove(oldEntity);
                    }
                    else if (networkDisplayViewModel.GroupedEntities[1].Entities.Contains(oldEntity))
                    {
                        networkDisplayViewModel.GroupedEntities[1].Entities.Remove(oldEntity);
                    }
                    else
                    {
                        networkDisplayViewModel.DeleteEntityFromCanvas(oldEntity);
                    }
                }
                //za grafcuge
                foreach(Entity oldEntity in e.OldItems)
                {
                    if(measurementGraphViewModel.Entities.Contains(oldEntity))
                    {
                        measurementGraphViewModel.Entities.Remove(oldEntity);
                    }
                }
            }
        }


        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
                UndoCommand.RaiseCanExecuteChanged();
            }
        }
        
        private void OnNav(string destination)
        {
            switch(destination)
            {
                case "NetworkEntities":
                    if(CurrentViewModel != networkEntitiesViewModel)
                    {
                        UndoStack.Push(new SaveState<CommandType, object>(CommandType.NavCommand, CurrentViewModel.GetType()));
                        CurrentViewModel = networkEntitiesViewModel;
                    }
                    break;
                case "NetworkDisplay":
                    if(CurrentViewModel != networkDisplayViewModel)
                    {
                        UndoStack.Push(new SaveState<CommandType, object>(CommandType.NavCommand, CurrentViewModel.GetType()));
                        CurrentViewModel = networkDisplayViewModel;
                    }
                    break;
                case "MeasurementGraph":
                    if(CurrentViewModel != measurementGraphViewModel)
                    {
                        UndoStack.Push(new SaveState<CommandType, object>(CommandType.NavCommand, CurrentViewModel.GetType()));
                        CurrentViewModel = measurementGraphViewModel;
                    }
                    break;
            }
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(networkEntitiesViewModel.Entities.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            string incommingEntityId = incomming.Substring(0, incomming.IndexOf(':'));
                            double newValue = double.Parse(incomming.Substring(incomming.IndexOf(':') + 1));

                            for (int idx = 0; idx < networkEntitiesViewModel.Entities.Count; idx++)
                            {
                                string currentEntityId = $"Entitet_{idx}";
                                if (currentEntityId == incommingEntityId)
                                {
                                    AddToLastFive(networkEntitiesViewModel.Entities[idx], newValue);
                                    networkEntitiesViewModel.Entities[idx].Value = newValue;


                                    
                                    using (StreamWriter writer = File.AppendText("Log.txt"))
                                    {
                                        DateTime dateTime = DateTime.Now;
                                        writer.WriteLine($"{dateTime}: {networkEntitiesViewModel.Entities[idx].Type}, {newValue}");
                                    }
                                    
                                    Application.Current.Dispatcher.Invoke(() => networkDisplayViewModel.UpdateEntityOnCanvas(networkEntitiesViewModel.Entities[idx]));
                                    //measurementGraphViewModel.OnShow();

                                    break;
                                }
                            }

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        private void AddToLastFive(Entity entity, double newValue)
        {
            if (entity.Last_5_Values.Count == 5)
            {
                entity.Last_5_Values.RemoveAt(0);
                entity.Last_5_Values.Add(new Pair<DateTime, double>(DateTime.Now, newValue));
            }
            else
            {
                entity.Last_5_Values.Add(new Pair<DateTime, double>(DateTime.Now, newValue));
            }
        }
    }
}
