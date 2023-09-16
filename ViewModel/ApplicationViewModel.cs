using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using WpfNoteApp.Model;


namespace WpfNoteApp.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        const int PORT = 1024;
        const string IP = "127.0.0.1";
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
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        try
                        {
                            Notes.Add(SelectedNote);
                            socket.Connect(IP, PORT);
                            byte[] data = new byte[256];
                            NoteResponse response1 = new NoteResponse();
                            Note temp = new Note();
                            temp = Notes.LastOrDefault();
                            response1.Notes.Add(temp);
                            var json = JsonConvert.SerializeObject(response1);
                            socket.Send(Encoding.Unicode.GetBytes(json));
                            
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                    //using (Model1 db = new Model1())
                    //{
                    //    //db.Notes.Add(selectedNote);
                    //    db.Entry(selectedNote).State = System.Data.Entity.EntityState.Modified;
                    //    db.SaveChanges();
                    //    Notes.Clear();
                    //    foreach (Note note in db.Notes)
                    //    {
                    //        Notes.Add(note);
                    //    }
                    //    //Notes.Insert(0, selectedNote);
                    //}
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
                        using (var db = new Model1())
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
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect(IP, PORT);
                    byte[] data = new byte[1024];
                    int bytes = socket.Receive(data);
                    NoteResponse response = new NoteResponse();
                    var pack = Encoding.UTF8.GetString(data, 0, bytes);
                    response = JsonConvert.DeserializeObject<NoteResponse>(pack);
                    foreach (var item in response.Notes)
                    {
                        Notes.Add(item);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }

            //using(Model1 db = new Model1())
            //{
            //    foreach (Note note in db.Notes)
            //    {
            //        Notes.Add(note);
            //    }
            //}
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
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
