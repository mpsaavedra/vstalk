using VSTalk.Engine.Settings;

namespace VSTalk.Engine.Core.Context
{
    public class ModelInitilizer : IModelInitilizer
    {
        public IAccountDeserilizer Deserilizer { get; set; }

        public ModelInitilizer()
        {
            Deserilizer = new FileAccountSerilizer();
        }

        public IModelContext InitializeContext()
        {
            var modelContext = new ModelContext();
            modelContext.Account = Deserilizer.Deserilize();
            return modelContext;
        }
    }
}