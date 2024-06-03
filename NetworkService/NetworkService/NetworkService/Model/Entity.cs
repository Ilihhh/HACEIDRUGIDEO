using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Media;

namespace NetworkService.Model
{
    public enum SensorType
    {
        CableSensor,
        DigitalManometer
    }


    public class Entity : ValidationBase
    {
        private string idField;
        private int id;
        private string name;
        private string type;
        private double value;
        private string imagePath = "pack://application:,,,/NetworkService;component/Images/no-image.jpg";
        List<Pair<DateTime, double>> last_5_values;

        public Entity()
        {
            Last_5_Values = new List<Pair<DateTime, double>>();
        }
        public string IdField
        {
            get { return idField; }
            set
            {
                if(idField != value)
                {
                    idField = value;
                    OnPropertyChanged(nameof(IdField));
                }
            }
        }

        public int Id
        {
            get { return id; }
            set
            {
                if(id!=value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                ChangeImage();
                OnPropertyChanged(nameof(Type));
            }
        }

        public double Value
        {
            get { return this.value; }
            set
            {
                //vazno je da nema if petlje jer grafovi zavise od ove promene
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        
        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                if(imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }
        public List<Pair<DateTime, double>> Last_5_Values
        {
            get
            {
                return last_5_values;
            }
            set
            {
                if (last_5_values != value)
                {
                    last_5_values = value;
                    OnPropertyChanged(nameof(Last_5_Values));
                }
            }
        }

        protected override void ValidateSelf()
        {
            int parsedId;
            bool parsingResult = int.TryParse(this.IdField, out parsedId);
            if(this.DoesIdAlreadyExist)
            {
                this.ValidationErrors["Id"] = "Id must be unique";
                this.ValidationColors["Id"] = Brushes.Red;
            }

            if(!parsingResult)
            {
                this.ValidationErrors["Id"] = "Id must be a number";
                this.ValidationColors["Id"] = Brushes.Red;
            }
            else if(parsedId < 0)
            {
                this.ValidationErrors["Id"] = "Id must be positive";
                this.ValidationColors["Id"] = Brushes.Red;
            }
            if(string.IsNullOrEmpty(this.IdField))
            {
                this.ValidationErrors["Id"] = "Id field is required";
                this.ValidationColors["Id"] = Brushes.Red;
            }
            if(string.IsNullOrEmpty(this.Name))
            {
                this.ValidationErrors["Name"] = "Name field is required";
                this.ValidationColors["Name"] = Brushes.Red;
            }
            if(this.Type==null)
            {
                this.ValidationErrors["Type"] = "Type must be selected";
                this.ValidationColors["Type"] = Brushes.Red;
            }
        }

        public bool IsValueValid()
        {
            return Value <= 16 && Value >= 5;
        }

        private void ChangeImage()
        {
            switch(Type)
            {
                case "Cable Sensor":
                    ImagePath = "pack://application:,,,/NetworkService;component/Images/Cable Sensor.png".Replace(" ", "%20");
                    break;
                case "Digital Manometer": 
                    ImagePath = "pack://application:,,,/NetworkService;component/Images/Digital Manometer.jpg".Replace(" ", "%20");
                    break;
            }
        }

    }
}
