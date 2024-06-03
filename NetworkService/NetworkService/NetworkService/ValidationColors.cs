
using System;
using System.Collections.Generic;
using System.Windows.Media;


namespace NetworkService
{
    public class ValidationColors : BindableBase
    {
        private readonly Dictionary<string, Brush> validationColors = new Dictionary<string, Brush>();

        public bool IsValid
        {
            get
            {
                return this.validationColors.Values.Count < 1;
            }
        }

        public Brush this[string fieldName]
        {
            get
            {
                return this.validationColors.ContainsKey(fieldName) ? this.validationColors[fieldName] : Brushes.Gray;
            }

            set
            {
                if (this.validationColors.ContainsKey(fieldName))
                {
                    if (value == Brushes.Gray)
                    {
                        this.validationColors.Remove(fieldName);
                    }
                    else
                    {
                        this.validationColors[fieldName] = value;
                    }
                }
                else
                {
                    if (value != Brushes.Gray)
                    {
                        this.validationColors.Add(fieldName, value);
                    }
                }
                this.OnPropertyChanged(nameof(IsValid));
            }


        }
        public void Clear()
        {
            validationColors.Clear();
        }
    }
}
