using AyaCoreMod.Core;
using AyaCoreMod.Features;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    internal class ExtraSlotsVehiclesFeature : IFeature
    {
        public string Name => "ExtraSlotsVehicles";

        public void Enable()
        {
            if (!ExtraSlotsVehiclesRuntime.IsEnabled())
                return;

            UtilitySlots.Features.ExtraSlotsCore.ExtraSlotsCompatibilityPatches.EnsureGlobalSlotMapping();

            Log.Info("[UtilitySlots][ExtraSlotsVehicles] Feature enabled.");
        }

        public void Disable() { }
    }
}
