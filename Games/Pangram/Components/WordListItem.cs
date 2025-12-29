using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangram.Components
{
    public class WordListItem : ObservableObject
    {
        private string _word;
        private bool _enabled;

        public WordListItem(string word)
        {
            _word = word.ToUpper();
            _enabled = true;
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
    }
}
