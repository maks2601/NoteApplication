namespace QualiumSystems
{
    public class NoteData
    {
        public string noteId;
        public string userId;
        public string message;
        public string fileLink;
        public string username;
        public int counter;

        public NoteData(string userId, string message, string username, int counter)
        {
            this.userId = userId;
            this.message = message;
            fileLink = default;
            this.username = username;
            this.counter = counter;
            noteId = userId + counter;
        }

        public NoteData(string userId, string fileLink, string message, string username, int counter)
        {
            this.userId = userId;
            this.fileLink = fileLink;
            this.message = message;
            this.username = username;
            this.counter = counter;
            noteId = userId + counter;
        }
    }
}
