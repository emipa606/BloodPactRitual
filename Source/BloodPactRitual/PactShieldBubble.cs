using UnityEngine;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual
{
    [StaticConstructorOnStartup]
    public class PactShieldBubble
    {
        private const int NumTick = 100;
        private static readonly Color FullColor = new Color(150, 0, 0);

        private static readonly Material BubbleMat =
            MaterialPool.MatFrom("Other/BloodPactShield", ShaderDatabase.Transparent, FullColor);

        private static readonly Material ArrowMat =
            MaterialPool.MatFrom("Other/BloodPactSource", ShaderDatabase.Transparent, FullColor);

        internal static void DrawWornExtras(Pawn pawn, DirectPawnRelationPact pact)
        {
            var bonded = pact.otherPawn;
            if (bonded == null || !pawn.Spawned || !bonded.Spawned || bonded.MapHeld != pawn.MapHeld)
            {
                return;
            }

            DrawFeedback(pawn, bonded, ArrowMat, pact.LastReceivedPactDamage);
            DrawFeedback(pawn, bonded, BubbleMat, pact.LastShieldActive);
        }

        private static void DrawFeedback(Pawn pawn, Pawn bonded, Material mat, int lastActive)
        {
            var alpha = CalculateAlpha(lastActive);
            if (alpha <= 0)
            {
                return;
            }

            var toTargetPos = bonded.DrawPos - pawn.DrawPos;
            var angle = toTargetPos.AngleFlat() - 90; //Vector2.Angle(toTargetPos.AngleFlat(), Vector2.right);

            var instanceColor = FullColor;
            instanceColor.a *= alpha;
            var material = mat;
            if (instanceColor != material.color)
            {
                material = MaterialPool.MatFrom((Texture2D) material.mainTexture, material.shader, instanceColor);
            }

            const float size = 1.2f;
            var position = pawn.Drawer.DrawPos;
            position.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            var s = new Vector3(size, 1f, size);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(position, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
        }

        private static float CalculateAlpha(int lastActive)
        {
            return Mathf.Clamp(1f - (1f * (Find.TickManager.TicksGame - lastActive) / NumTick), 0f, 1f);
        }
    }
}