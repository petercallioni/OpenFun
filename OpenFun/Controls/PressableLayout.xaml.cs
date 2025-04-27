using CommunityToolkit.Mvvm.Input;

namespace OpenFun.Controls
{
    public partial class PressableLayout : ContentView
    {
        // Bindable command property so that the caller can pass a command
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command),
                typeof(IRelayCommand),
                typeof(PressableLayout),
                null);

        // Bindable CommandParameter property
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                nameof(CommandParameter),
                typeof(object),
                typeof(PressableLayout),
                null);

        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public PressableLayout()
        {
            InitializeComponent();

            // Add a tap gesture recognizer that calls our async handler
            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnTappedAsync;
            GestureRecognizers.Add(tapGesture);
        }

        private async void OnTappedAsync(object? sender, TappedEventArgs e)
        {
            // Animate to a pressed state (scale down)
            await this.ScaleTo(0.95, 100, Easing.Linear);

            // Delay to allow the UI time to show the pressed state
            await Task.Delay(150);

            // Animate back to the normal state
            await this.ScaleTo(1, 100, Easing.Linear);

            // Execute the command if assigned and if it can execute.
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}