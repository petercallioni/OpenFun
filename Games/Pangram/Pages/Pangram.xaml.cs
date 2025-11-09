using Pangram.Models;
using Pangram.PageModels;
using System.ComponentModel;

namespace Pangram.Pages;

public partial class Pangram : ContentPage
{
    public Pangram(GamePageModel gamePageModel)
    {
        InitializeComponent();
        BindingContextChanged += OnBindingContextChanged;
        BindingContext = gamePageModel;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is GamePageModel vm)
        {
            // detach first to avoid duplicate handlers
            vm.PropertyChanged -= Vm_PropertyChanged;
            vm.PropertyChanged += Vm_PropertyChanged;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is GamePageModel vm)
            vm.PropertyChanged -= Vm_PropertyChanged;
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e?.PropertyName != nameof(GamePageModel.LastGuessResult))
            return;

        if (sender is GamePageModel vm)
        {
            Models.GuessWordResults result = vm.LastGuessResult;
            // ensure animation runs on UI thread
            MainThread.BeginInvokeOnMainThread(async () => await AnimateForResultAsync(result));
        }
    }

    private async Task AnimateForResultAsync(GuessWordResults result)
    {
        try
        {
            if (CurrentWordBorder == null)
                return;

            switch (result)
            {
                case GuessWordResults.INVALID:
                case GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER:
                case GuessWordResults.ALREADY_GUESSED:
                    await ShakeAsync(CurrentWordBorder);
                    break;

                case GuessWordResults.VALID:
                    await CurrentWordBorder.ScaleTo(1.08, 120, Easing.CubicInOut);
                    await CurrentWordBorder.ScaleTo(1.0, 120, Easing.CubicInOut);
                    break;

                default:
                    // no animation for other values
                    break;
            }
        }
        catch
        {
            // swallow to avoid unhandled exceptions during animation
        }
    }

    private async Task ShakeAsync(VisualElement element,
        double initialMagnitude = 12,   // Initial shake strength in pixels
        int steps = 3,                  // Number of decreasing steps
        uint durationMs = 45)           // Duration per movement in milliseconds
    {
        if (element == null)
            return;

        double original = element.TranslationX;
        List<double> offsets = new List<double>();

        // Generate dampened shake pattern
        for (int i = 0; i < steps; i++)
        {
            double magnitude = initialMagnitude / (i + 1);
            offsets.Add(-magnitude);
            offsets.Add(magnitude);
        }
        offsets.Add(0); // Return to center

        foreach (double offset in offsets)
        {
            await element.TranslateTo(offset, 0, durationMs, Easing.Linear);
        }

        element.TranslationX = original;
    }
}