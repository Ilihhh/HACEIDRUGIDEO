﻿
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public List<string> ComboBoxItems { get; set; } = Data.ComboBoxItems.entityTypes.Keys.ToList();
        public ObservableCollection<Entity> entitiesToShow;
        public ObservableCollection<Entity> Entities { get; set; }

        public MyICommand AddEntityCommand { get; set; }
        public MyICommand RemoveEntityCommand { get; set; }
        public ICommand ShowKeyboardCommand { get; set; }
        public MyICommand HideKeyboardCommand { get; set; }
        public ICommand KeyboardButtonCommand { get; set; }


        private Entity currentEntity = new Entity();
        private Entity selectedEntity = new Entity();

        private string searchText;
        private bool searchByName = true;           //search by name is default

        //tastatura things
        private Visibility isKeyboardOpen = Visibility.Hidden;
        private string textBoxText;
        private string currentTextBox;



        public NetworkEntitiesViewModel()
        {
            Entities = new ObservableCollection<Entity>();
            Entities.CollectionChanged += (sender, e) =>
            {
                Console.WriteLine("CollectionChanged event triggered");
            };
            //Entities.Add(new Entity { Id = 1, Name = "Majmubn", Type ="Digital Manometer", Value = 0 });
            //EntitiesToShow = Entities;
            FilterData();
            AddEntityCommand = new MyICommand(OnAdd);
            RemoveEntityCommand = new MyICommand(OnRemove, CanRemove);
            ShowKeyboardCommand = new RelayCommand(OnShowKeyboard);
            HideKeyboardCommand = new MyICommand(OnHideKeyboard);
            KeyboardButtonCommand = new RelayCommand(OnKeyboardButtonClicked);
        }


        public void RaiseCollectionChanged(ObservableCollection<Entity> collection)
        {
            // Kreirajte args za Reset akciju (možete kreirati različite args za različite akcije)
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            // Pristupite privatnom CollectionChanged događaju pomoću reflection-a
            var eventInfo = typeof(ObservableCollection<Entity>).GetField("CollectionChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (eventInfo != null)
            {
                var eventDelegate = (MulticastDelegate)eventInfo.GetValue(collection);
                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                    {
                        handler.Method.Invoke(handler.Target, new object[] { collection, args });
                    }
                }
            }
        }

        private void SaveState()
        {
            MainWindowViewModel.UndoStack.Push(new SaveState<CommandType, object>
                (CommandType.EntityManipulation,
                new ObservableCollection<Entity>(Entities)));
        }


        private void OnKeyboardButtonClicked(object parameter)
        {
            string buttonContent = parameter as string;

            if (CurrentTextBox.Equals("IdTextBox"))
            {
                if (buttonContent != null)
                {
                    if (buttonContent == "Backspace")
                    {
                        if (!string.IsNullOrEmpty(CurrentEntity.IdField))
                            CurrentEntity.IdField = CurrentEntity.IdField.Substring(0, CurrentEntity.IdField.Length - 1);
                    }
                    else if (buttonContent == "Space")
                    {
                        CurrentEntity.IdField += " ";
                    }
                    else
                    {
                        CurrentEntity.IdField += buttonContent;
                    }
                }
            }
            else if(CurrentTextBox.Equals("NameTextBox"))
            {
                if (buttonContent != null)
                {
                    if (buttonContent == "Backspace")
                    {
                        if (!string.IsNullOrEmpty(CurrentEntity.Name))
                            CurrentEntity.Name = CurrentEntity.Name.Substring(0, CurrentEntity.Name.Length - 1);
                    }
                    else if (buttonContent == "Space")
                    {
                        CurrentEntity.Name += " ";
                    }
                    else
                    {
                        CurrentEntity.Name += buttonContent;
                    }
                }
            }
            else if(CurrentTextBox.Equals("FilterTextBox"))
            {
                if (buttonContent != null)
                {
                    if (buttonContent == "Backspace")
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                            SearchText = SearchText.Substring(0, SearchText.Length - 1);
                    }
                    else if (buttonContent == "Space")
                    {
                        SearchText += " ";
                    }
                    else
                    {
                        SearchText += buttonContent;
                    }
                }
            }
        }

        public ObservableCollection<Entity> EntitiesToShow
        {
            get { return entitiesToShow; }
            set
            {
                entitiesToShow = new ObservableCollection<Entity>(value);
                OnPropertyChanged(nameof(EntitiesToShow));
            }
        }

        public string CurrentTextBox
        {
            get { return currentTextBox; }
            set
            {
                currentTextBox = value;
                OnPropertyChanged(nameof(CurrentTextBox));
            }
        }

        public string TextBoxText
        {
            get { return textBoxText; }
            set
            {
                textBoxText = value;
                OnPropertyChanged(nameof(TextBoxText));
            }
        }

        public Visibility IsKeyboardOpen
        {
            get { return isKeyboardOpen; }
            set
            {
                isKeyboardOpen = value;
                OnPropertyChanged(nameof(IsKeyboardOpen));
            }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FilterData();
                }
            }
        }

        public bool SearchByName
        {
            get { return searchByName; }
            set
            {
                searchByName = value;
                OnPropertyChanged(nameof(SearchText));
                FilterData();
            }
        }


        public void FilterData()
        {
            if(string.IsNullOrWhiteSpace(SearchText))
            {
                EntitiesToShow = new ObservableCollection<Entity>(Entities);
                return;
            }
            
            if(SearchByName)
            {
                //EntitiesToShow.Clear();
                EntitiesToShow = new ObservableCollection<Entity>(Entities.Where(d => d.Name.ToLower().Contains(SearchText.ToLower())));
            }
            else
            {
                EntitiesToShow = new ObservableCollection<Entity>(Entities.Where(d => d.Type.ToString().ToLower().Contains(SearchText.ToLower())));
            }
        }
        public Entity CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                currentEntity = value;
                OnPropertyChanged(nameof(CurrentEntity));
            }
        }

        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                RemoveEntityCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanRemove()
        {
            return SelectedEntity != null;
        }
        public void OnRemove()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete selected entity?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveState();
                Entities.Remove(SelectedEntity);
                ToastNotifications.RaiseToast(
                    "Success",
                    $"Selected entity deleted",
                    Notification.Wpf.NotificationType.Information);
                FilterData();
            }
        }
        public void OnAdd()
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want do add entity?", "Add confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    int parseId;
                    bool canParse = int.TryParse(CurrentEntity.IdField, out parseId);
                    CurrentEntity.DoesIdAlreadyExist = false;

                    if (canParse)
                    {
                        foreach (Entity e in Entities)
                        {
                            if (e.Id == parseId)
                            {
                                CurrentEntity.DoesIdAlreadyExist = true;
                                break;
                            }
                        }
                    }

                    CurrentEntity.Validate();

                    if (CurrentEntity.IsValid)
                    {
                        SaveState();
                        Entities.Add(new Entity() { Id = parseId, Name = CurrentEntity.Name, ImagePath = CurrentEntity.ImagePath, Type = CurrentEntity.Type, Value = 0 });
                        CurrentEntity.IdField = String.Empty;
                        CurrentEntity.Name = String.Empty;

                        ToastNotifications.RaiseToast(
                    "Success!",
                    "New entity added.",
                    Notification.Wpf.NotificationType.Success);
                        
                    }
                    else
                    {
                        ToastNotifications.RaiseToast(
                    "Error",
                    $"Invalid filed values",
                    Notification.Wpf.NotificationType.Error);
                    }

                    // Using filtering function so the changes apply to 'EntitiesToShow'
                    FilterData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }


        private void OnShowKeyboard(object parameter)
        {
            CurrentTextBox = parameter as string;
            IsKeyboardOpen = Visibility.Visible;
        }
        private void OnHideKeyboard()
        {
            IsKeyboardOpen = Visibility.Hidden;
        }
    }
}
