using System;
using System.ComponentModel;

namespace RSTinvestRefBook.Models
{
    public class Position
    {
        public string Id { get; private set; }

        public Position()
        {
            Id = Guid.NewGuid().ToString();
        }

        private string hexId;
        public string HexId
        {
            get { return hexId; }
            set { hexId = value; OnPropertyChanged(nameof(Id)); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
    
}
