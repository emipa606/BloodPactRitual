# GitHub Copilot Instructions for Blood Pact Ritual (Continued) Mod

## Mod Overview and Purpose

**Mod Name:** Blood Pact Ritual (Continued)  
**Mod Description:** An update to Oopah's Blood Pact Ritual mod that introduces the ability for colonists to form powerful, double-edged blood pacts. This mod aims to add new combat possibilities and drama to the RimWorld experience.

**Key Features:**
- **Blood Pacts:** A ritual where one colonist attacks another with a special ritual dagger, creating an unbreakable bond between them.
- **Shared Damage:** Both pawns share injuries, reducing the total damage and increasing resilience in combat.
- **Shared Death:** If one pact member dies, the other dies as well.
- **Mood and Opinion Impact:** The effectiveness of the blood pact is influenced by the pawns' relationship and mood.
- **Options:** Mod options to allow blood pacts with animals, recruit prisoners, and enable pawn revolt.

## Coding Patterns and Conventions

This mod follows a structured approach to coding, adhering to the following patterns and conventions:

1. **Mod Structure:**
   - **C# Files:** Located in the `Source\BloodPactRitual` directory, containing the main logic for the mod.
   - **DefOf Classes:** Located in the `DefOf` directory, used to define constants for def types.

2. **Naming Conventions:**
   - **Class Names:** PascalCase, e.g., `BloodPactRitual`, `DirectPawnRelationPact`.
   - **Method Names:** PascalCase, e.g., `ApplyOpinionAndMoodFactor`, `CreatePact`.
   - **Variable Names:** camelCase, e.g., `baseAcceptChance`, `pawnReaction`.

3. **File Organization:**
   - **DefInjected:** Located in the `DefInjected` directory, containing XML files for defining game elements.
   - **Source:** Located in the `Source` directory, containing C# source code.

## XML Integration

The Blood Pact Ritual mod integrates with RimWorld's XML system to define new game elements. Key XML files include:

- **DamageDef:** `Damages_BloodPactInjury.xml` - Defines the damage effects of blood pact injuries.
- **HediffDef:** `Hediffs_Local_BloodPact.xml` - Defines hediffs related to blood pacts.
- **PawnRelationDef:** `PawnRelations_BloodPact.xml` - Defines the blood pact pawn relation.
- **ThingDef:** `Weapons_BloodPact.xml` - Defines the ritual dagger used in blood pacts.
- **ThoughtDef:** `Thoughts_Memory_Social_BloodPact.xml` - Defines thoughts related to blood pacts.

## Harmony Patching

The mod uses Harmony patching to modify vanilla RimWorld behavior. Key patched methods include:

- **Ritual Dagger:** Modifies the attack behavior of the ritual dagger.
- **Combat Mechanics:** Adjusts combat mechanics to include shared damage and death effects.
- **Pawn Relationships:** Adds new pawn relations for blood pacts.

## Suggestions for Copilot

To enhance your experience with GitHub Copilot in this mod project, consider the following suggestions:

1. **Code Generation:**
   - **Class Templates:** Use Copilot to generate new class templates for defining game elements.
   - **Method Skeletons:** Use Copilot to generate method skeletons for new behaviors.

2. **XML Defs:**
   - **Def Injection:** Use Copilot to generate XML definitions for new game elements.
   - **DefOf Classes:** Use Copilot to generate `DefOf` classes for defining constants.

3. **Harmony Patches:**
   - **Patch Generation:** Use Copilot to generate Harmony patch methods for modifying vanilla behavior.
   - **Patch Integration:** Use Copilot to integrate patches into the mod's code structure.

4. **Testing and Debugging:**
   - **Error Handling:** Use Copilot to generate error handling code for debugging and logging.
   - **Unit Tests:** Use Copilot to generate unit tests for validating mod behavior.

By following these guidelines and leveraging GitHub Copilot, you can efficiently develop and maintain the Blood Pact Ritual (Continued) mod, enhancing its features and ensuring compatibility with RimWorld.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

