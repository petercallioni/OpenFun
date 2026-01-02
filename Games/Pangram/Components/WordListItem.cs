using CommunityToolkit.Mvvm.ComponentModel;

namespace Pangram.Components
{
    public class WordListItem : ObservableObject
    {
        private string _word;
        private bool _enabled;
        private bool _isPangram;

        public WordListItem(string word, bool isPangram)
        {
            _word = word.ToUpper();
            _enabled = true;
            _isPangram = isPangram;
        }

        public string Word
        {
            get => _word;
            set
            {
                if (_word != value)
                {
                    _word = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEnabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPangram { get => _isPangram; }
    }
}
