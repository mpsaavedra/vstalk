using VSTalk.Engine.Settings;

namespace VSTalk.Engine.Core.Context
{
    public class ModelContextSaver : IModelContextSaver
    {
        public IAccountSerilizer Serilizer { get; set; }

        private IModelContext _context;

        public ModelContextSaver()
        {
            Serilizer = new FileAccountSerilizer();
        }

        public void Save(IModelContext context)
        {
            _context = context;
            SaveHistory();
            SaveAccount();
        }

        private void SaveAccount()
        {
            Serilizer.Serilize(_context.Account);
        }

        private void SaveHistory()
        {
            var messageRepository = new MessageRepository();
            messageRepository.SaveHistory(_context.Account);
        }
    }
}