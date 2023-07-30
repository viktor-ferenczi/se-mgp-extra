using Sandbox.ModAPI;

namespace MgpExtra
{
    // It is a STIPPED DOWN version of the MultigridProjectorModAgent
    // from the Multigrid Projector plugin. Here we need only the Version
    // to disable the MgpExtra mod if MGP 0.5.0 or later is persent,
    // because such MGP versions implement all of this mod's functionality.

    // ReSharper disable once UnusedType.Global
    public class MultigridProjectorModAgent
    {
        private const long WorkshopId = 2415983416;
        private const long ModApiRequestId = WorkshopId * 1000 + 0;
        private const long ModApiResponseId = WorkshopId * 1000 + 1;

        private object[] api;

        // ReSharper disable once MemberCanBePrivate.Global
        public bool Available { get; private set; }
        public string Version { get; private set; }

        public MultigridProjectorModAgent()
        {
            MyAPIGateway.Utilities.RegisterMessageHandler(ModApiResponseId, HandleModMessage);
            MyAPIGateway.Utilities.SendModMessage(ModApiRequestId, null);
        }

        private void HandleModMessage(object obj)
        {
            api = obj as object[];
            if (api == null || api.Length < 1)
                return;

            Version = api[0] as string;
            if (Version == null)
                return;

            Available = true;
        }
    }
}