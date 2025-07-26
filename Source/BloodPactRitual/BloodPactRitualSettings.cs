using Verse;

namespace BloodPactRitual;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class BloodPactRitualSettings : ModSettings
{
    public bool AllowAnimal = true;
    public bool AllowPrisoner = true;
    public bool AllowRevolt = true;
    public bool SharedDamageNeedsTending;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref AllowRevolt, "AllowRevolt", true);
        Scribe_Values.Look(ref AllowAnimal, "AllowAnimal", true);
        Scribe_Values.Look(ref AllowPrisoner, "AllowAnimal", true);
        Scribe_Values.Look(ref SharedDamageNeedsTending, "SharedDamageNeedsTending");
    }
}