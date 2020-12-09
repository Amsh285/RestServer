namespace RestServer.WebServer.CommunicationObjects
{
    public interface IActionResult
    {
        public void AddHeaderEntry(string key, string value);

        void Execute();
    }
}
