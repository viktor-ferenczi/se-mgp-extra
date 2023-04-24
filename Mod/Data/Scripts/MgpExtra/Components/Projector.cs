using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game;
using VRage.Utils;

// ReSharper disable once CheckNamespace
namespace MgpExtra
{
    // ReSharper disable once UnusedType.Global
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class Projector : MySessionComponentBase
    {
        private static bool IsWorkingButNotProjecting(IMyTerminalBlock block) => IsValid(block) && block.IsWorking && (block as IMyProjector)?.IsProjecting == false;
        private static bool IsProjecting(IMyTerminalBlock block) => IsWorking(block) && (block as IMyProjector)?.IsProjecting == true;
        private static bool IsWorking(IMyTerminalBlock block) => IsValid(block) && block.IsWorking;
        private static bool IsValid(IMyTerminalBlock block) => block.CubeGrid?.Physics != null;

        private readonly List<IMyTerminalControl> customControls = new List<IMyTerminalControl>();
        private readonly List<IMyTerminalAction> customActions = new List<IMyTerminalAction>();

        private bool initialized;
        
        public override void UpdateBeforeSimulation() {
            if (initialized)
                return;

            initialized = true;
            
            if (Comms.Role == Role.DedicatedServer)
                return;

            CreateCustomControls();
            
            MyAPIGateway.TerminalControls.CustomControlGetter += AddControlsToBlocks;
            MyAPIGateway.TerminalControls.CustomActionGetter += AddActionsToBlocks;
            MyAPIGateway.Utilities.InvokeOnGameThread(() => { SetUpdateOrder(MyUpdateOrder.NoUpdate); });
        }

        private void AddControlsToBlocks(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (block is IMyProjector)
            {
                controls.AddRange(customControls);
            }
        }

        private void AddActionsToBlocks(IMyTerminalBlock block, List<IMyTerminalAction> actions)
        {
            if (block is IMyProjector)
            {
                actions.AddRange(customActions);
            }
        }

        private void CreateCustomControls()
        {
            var checkbox = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCheckbox, IMyProjector>("ManualAlignment");
            checkbox.Visible = (_) => false;
            checkbox.Enabled = IsProjecting;
            checkbox.Getter = Aligner.Getter;
            checkbox.Setter = Aligner.Setter;
            checkbox.Title = MyStringId.GetOrCompute("Manual Alignment");
            checkbox.Tooltip = MyStringId.GetOrCompute("Allows the player to manually align the projection using keys familiar from block placement");
            checkbox.SupportsMultipleBlocks = false;
            customControls.Add(checkbox);

            var action = MyAPIGateway.TerminalControls.CreateAction<IMyProjector>("ToggleManualAlignment");
            action.Enabled = (_) => true;
            action.Action = Aligner.Toggle;
            action.ValidForGroups = false;
            action.Icon = ActionIcons.MOVING_OBJECT_TOGGLE;
            action.Name = new StringBuilder("Toggle Manual Alignment");
            action.Writer = (b, s) => s.Append(Aligner.Getter(b) ? "Aligning" : "Align");
            action.InvalidToolbarTypes = new List<MyToolbarType> {MyToolbarType.None, MyToolbarType.Character, MyToolbarType.Spectator};
            customActions.Add(action);
            
            var button = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyProjector>("LoadRepairProjection");
            button.Visible = IsWorking;
            button.Enabled = IsWorkingButNotProjecting;
            button.Action = Repair.LoadMechanicalGroup;
            button.Title = MyStringId.GetOrCompute("Load Repair Projection");
            button.Tooltip = MyStringId.GetOrCompute("Loads the projector's own grid as a repair projection.");
            button.SupportsMultipleBlocks = false;
            customControls.Add(button);
        }
    }
}