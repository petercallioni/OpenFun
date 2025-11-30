namespace OpenFun_Core.Services
{
    /// <summary>
    /// Service for displaying dialog boxes in MAUI applications.
    /// Uses Shell.Current to ensure proper UI thread handling and platform compatibility.
    /// </summary>
    public class DialogService
    {
        private SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        /// Displays a confirmation dialog with two action buttons.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="acceptButton">The text for the accept button.</param>
        /// <param name="cancelButton">The text for the cancel button.</param>
        /// <returns>True if the accept button was clicked; otherwise false.</returns>
        public async Task<bool> DisplayConfirmationAsync(
            string title,
            string message,
            string acceptButton = "Yes",
            string cancelButton = "No")
        {
            try
            {
                await _semaphore.WaitAsync();
                
                if (Shell.Current is Shell shell)
                {
                    return await shell.DisplayAlert(title, message, acceptButton, cancelButton);
                }

                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Displays an alert dialog with a single OK button.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="okButton">The text for the OK button. Default is "OK".</param>
        public async Task DisplayAlertAsync(
            string title,
            string message,
            string okButton = "OK")
        {
            try
            {
                await _semaphore.WaitAsync();
                
                if (Shell.Current is Shell shell)
                {
                    await shell.DisplayAlert(title, message, okButton);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
