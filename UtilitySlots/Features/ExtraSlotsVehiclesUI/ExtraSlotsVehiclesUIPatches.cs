using System;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// Patch UI principal pour les véhicules.
    /// On hook uGUI_Equipment.Init (comme pour les chips) et,
    /// en fonction de Equipment.owner, on délègue à SeamothUI / ExosuitUI / CyclopsUI.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), "Init")]
    internal static class ExtraSlotsVehiclesUIPatches
    {
        private static readonly FieldInfo UguiEquipment_Equipment_FI =
            AccessTools.Field(typeof(uGUI_Equipment), "equipment");

        private static readonly PropertyInfo Equipment_Owner_PI =
            AccessTools.Property(typeof(Equipment), "owner");

        private static bool _loggedInit;
        private static bool _loggedNoEquipmentField;
        private static bool _loggedNoOwnerProperty;

        private static Equipment GetEquipment(uGUI_Equipment ui)
        {
            if (ui == null)
                return null;

            if (UguiEquipment_Equipment_FI == null)
            {
                if (!_loggedNoEquipmentField)
                {
                    _loggedNoEquipmentField = true;
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][UI] Field 'uGUI_Equipment.equipment' introuvable via reflection. " +
                                "Impossible d'identifier le propriétaire des slots véhicule.");
                }
                return null;
            }

            try
            {
                return UguiEquipment_Equipment_FI.GetValue(ui) as Equipment;
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][UI] Exception en lisant uGUI_Equipment.equipment : " + e);
                return null;
            }
        }

        private static object GetOwner(Equipment eq)
        {
            if (eq == null)
                return null;

            if (Equipment_Owner_PI == null)
            {
                if (!_loggedNoOwnerProperty)
                {
                    _loggedNoOwnerProperty = true;
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][UI] Propriété 'Equipment.owner' introuvable via reflection. " +
                                "Impossible d'identifier le véhicule (SeaMoth / Exosuit / Cyclops).");
                }
                return null;
            }

            try
            {
                return Equipment_Owner_PI.GetValue(eq, null);
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][UI] Exception en lisant Equipment.owner : " + e);
                return null;
            }
        }

        /// <summary>
        /// Postfix sur uGUI_Equipment.Init : on détecte le type de propriétaire et on applique l'UI correspondante.
        /// </summary>
        static void Postfix(uGUI_Equipment __instance)
        {
            try
            {
                if (!_loggedInit)
                {
                    _loggedInit = true;
                    Log.Info("[UtilitySlots][ExtraSlotsVehicles][UI] Vehicle UI helpers initialised.");
                }

                var equipment = GetEquipment(__instance);
                if (equipment == null)
                {
                    // Très probablement un écran de base / armure / autre, on ne touche pas.
                    return;
                }

                var owner = GetOwner(equipment);
                if (owner == null)
                {
                    // Pas de propriétaire identifiable, on ne fait rien.
                    return;
                }

                // On choisit la bonne UI en fonction du type de propriétaire.
                if (owner is SeaMoth)
                {
                    Log.Info("[UtilitySlots][ExtraSlotsVehicles][UI] Owner=SeaMoth → SeamothUI.Refresh");
                    ExtraSlotsVehiclesSeamothUI.Refresh(__instance);
                    return;
                }

                if (owner is Exosuit)
                {
                    Log.Info("[UtilitySlots][ExtraSlotsVehicles][UI] Owner=Exosuit → ExosuitUI.Refresh");
                    ExtraSlotsVehiclesExosuitUI.Refresh(__instance);
                    return;
                }

                if (owner is SubRoot subRoot && subRoot.isCyclops)
                {
                    Log.Info("[UtilitySlots][ExtraSlotsVehicles][UI] Owner=Cyclops(SubRoot) → CyclopsUI.Refresh");
                    ExtraSlotsVehiclesCyclopsUI.Refresh(__instance);
                    return;
                }

                // Autre chose (joueur, base, etc.) : on ne touche pas.
                // Log.Debug serait overkill ici, on laisse silencieux pour ne pas spammer.
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][UI] Exception in uGUI_Equipment.Init postfix: " + e);
            }
        }
    }
}
