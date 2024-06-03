
namespace NetworkService
{
    public abstract class ValidationBase : BindableBase
    {
        public ValidationErrors ValidationErrors { get; set; }
        public ValidationColors ValidationColors { get; set; }

        public bool IsValid { get; set; }
        public bool DoesIdAlreadyExist { get; set; }

        protected ValidationBase()
        {
            this.ValidationErrors = new ValidationErrors();
            this.ValidationColors = new ValidationColors();
        }

        protected abstract void ValidateSelf();

        public void Validate()
        {
            this.ValidationErrors.Clear();

            this.ValidationColors.Clear();

            this.ValidateSelf();
            this.IsValid = this.ValidationErrors.IsValid;
            this.OnPropertyChanged(nameof(IsValid));
            this.OnPropertyChanged(nameof(ValidationErrors));
            this.OnPropertyChanged(nameof(ValidationColors));
        }
    }
}
