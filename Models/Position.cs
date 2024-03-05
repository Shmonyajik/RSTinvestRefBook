using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSTinvestRefBook.Models
{
    public class Position
    {
        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged(nameof(Id)); }
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


        private bool isAcceptance;
        public bool IsAcceptance
        {
            get { return isAcceptance; }
            set { isAcceptance = value; OnPropertyChanged(nameof(IsAcceptance)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
    
}
