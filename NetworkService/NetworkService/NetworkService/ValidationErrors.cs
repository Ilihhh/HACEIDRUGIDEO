
using System;
using System.Collections.Generic;
using System.Windows.Media;


namespace NetworkService
{
    public class ValidationErrors : BindableBase
    {
        private readonly Dictionary<string, string> validationErrors = new Dictionary<string, string>();
        
        public bool IsValid
        {
            get
            {
                return this.validationErrors.Values.Count < 1;
            }
        }

        public string this[string fieldName]
        {
            get
            {
                return this.validationErrors.ContainsKey(fieldName) ? this.validationErrors[fieldName] : string.Empty;
            }
            
            set
            {
                if(this.validationErrors.ContainsKey(fieldName))
                {
                    if(string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Remove(fieldName);
                    }
                    else
                    {
                        this.validationErrors[fieldName] = value;
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(value))
                    {
                        this.validationErrors.Add(fieldName, value);
                    }
                }
                this.OnPropertyChanged(nameof(IsValid));
            }

            
        }
        public void Clear()
        {
            validationErrors.Clear();
        }
    }
}
