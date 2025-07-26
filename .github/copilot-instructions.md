# GitHub Copilot Instructions for BloodPact Mod

## Mod Overview and Purpose
The BloodPact mod for RimWorld introduces a series of rituals, relationships, and mechanics that revolve around the concept of a blood pact, enhancing the storytelling and strategic depth of the game. It introduces new ways for pawns to interact, form relationships, and share certain abilities, conditioned on their participation in blood pacts.

## Key Features and Systems
- **BloodPact Damage System**: Custom damage interactions that occur between pawns involved in a blood pact. This includes new damage types and shared damage mechanics.
- **Ritual Integration**: The centerpiece of the mod, allowing players to initiate blood rituals that grant unique abilities and relations.
- **Pact Relationships**: Establishes new relationship types among pawns, impacting mood and gameplay dynamics.
- **Auto-Tending Injuries**: A mechanic by which injuries from pact activities are self-tended to, reflecting the magical nature of the bond.
- **Thought Mechanics**: Custom thoughts and moods that emerge from participating in blood pacts, affecting overall pawn behavior and social interactions.
- **Utilities and Settings**: A flexible settings system allowing players to customize the mod's behavior.

## Coding Patterns and Conventions
- **Namespace Conventions**: Ensure all classes are defined within relevant namespaces to organize code effectively and prevent naming conflicts.
- **Class Structure**: Use public classes for main features and internal classes for helper functions that should not be exposed directly to other components.
- **Method Naming**: Use PascalCase for method names, providing clear and descriptive names of the method's actions and purposes.

## XML Integration
- **Defs Usage**: Integrate XML to define new damage types (BloodPactDamageDefOf), thoughts (BloodPactThoughtDefOf), and other game data essential for mod functionality.
- **Consistency**: Ensure XML entries follow structures that are consistent with RimWorld's standard def syntax to guarantee compatibility.
- **Patch Design**: Incorporate placeholders in XML to allow for easy expansion or alterations by mod users.

## Harmony Patching
- **Patch Structure**: Utilize Harmony patches to modify existing game behavior safely and cleanly without modifying the core game files directly.
- **Modular Patches**: Develop patches in separate static classes (e.g., `DamageWorker_AddInjury_Patch` and `Pawn_RelationsTracker_Patch`) to keep functionality segmented and maintainable.
- **Conflict Resolution**: Apply conditional checks to ensure patches only run under relevant conditions to minimize potential conflicts with other mods.

## Suggestions for Copilot
- **Template Generation**: Utilize Copilot to generate code templates for new feature implementations, especially for defining new Defs or Harmony patches.
- **Pattern Recognition**: Use Copilot to identify and suggest consistent coding patterns across different functionalities of the mod.
- **Automated Refactoring**: Take advantage of Copilot's suggestions to refactor code for better performance and readability.
- **Error Checking**: Utilize Copilot for preliminary error checking and resolving common syntax issues during the development process.

By adhering to these guidelines and leveraging GitHub Copilot's capabilities, you can streamline the development process of the BloodPact mod, ensuring efficient and effective integration into RimWorld's ecosystem.
