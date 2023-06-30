using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfNoteApp.Model;

namespace WpfNoteApp.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new RelayCommand(obj =>
                {
                    Note note = new Note();
                    //Notes.Insert(0, note);
                    SelectedNote = note;
                }));
            }
        }

        private RelayCommand saveCommand;

        public RelayCommand SaveCommand
        {
            get 
            { 
                return saveCommand ?? (saveCommand = new RelayCommand(obj =>
                {
                    using(Model1 db = new Model1())
                    {
                        //db.Notes.Add(selectedNote);
                        db.Entry(selectedNote).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Notes.Clear();
                        foreach (Note note in db.Notes)
                        {
                            Notes.Add(note);
                        }
                        //Notes.Insert(0, selectedNote);
                    }
                })); 
            }
            
        }


        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand(obj =>
                {
                    Note note = selectedNote;/*obj as Note;*/
                    if (note != null)
                    {
                        Notes.Remove(note);
                        using(var db = new Model1())
                        {
                            //db.Notes.Remove(note);
                            db.Notes.Attach(note);
                            db.Entry(note).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                },
                (obj) => Notes.Count > 0));
            }
        }

        private Note selectedNote;

        public ApplicationViewModel()
        {
            Notes = new ObservableCollection<Note>();
            using(Model1 db = new Model1())
            {
                foreach (Note note in db.Notes)
                {
                    Notes.Add(note);
                }
            }
            //{
            //    new Note {Title = "Title1", BodyNote = "Body1", DateNote = DateTime.Now},
            //    new Note {Title = "Title2", BodyNote = "Body2", DateNote = DateTime.Now},
            //    new Note {Title = "Title3", BodyNote = "Body3", DateNote = DateTime.Now},
            //};

        }

        public ObservableCollection<Note> Notes { get; set; }
        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                OnPropertyChanged("SelectedNote");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
