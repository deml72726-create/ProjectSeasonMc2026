using UnityEngine;
using TMPro;

using UnityEngine;
using TMPro;

public class TMP_Wobbler : MonoBehaviour
{
    public TMP_Text TextComponent;
    public bool isWobbling = true; // Control this instead of enabling the script

    [SerializeField] public float AngleMultiplier = 1.0f;
    [SerializeField] public float SpeedMultiplier = 4.0f;
    [SerializeField] public float CurveScale = 2.0f;

    void Start() { TextComponent = GetComponent<TMP_Text>(); }

    void LateUpdate()
    {
        if (TextComponent == null || !isWobbling) return;

        TextComponent.ForceMeshUpdate();
        var mesh = TextComponent.mesh;
        var vertices = mesh.vertices;

        for (int i = 0; i < TextComponent.textInfo.characterCount; i++)
        {
            var charInfo = TextComponent.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            Vector3 offset = Wobble(Time.time + i * CurveScale);
            int vIndex = charInfo.vertexIndex;
            for(int j = 0; j < 4; j++) vertices[vIndex + j] += offset;
        }

        mesh.vertices = vertices;
        TextComponent.canvasRenderer.SetMesh(mesh);
    }

    Vector3 Wobble(float time) => new Vector3(Mathf.Sin(time * 3.3f * SpeedMultiplier) * AngleMultiplier, Mathf.Cos(time * 2.5f * SpeedMultiplier) * AngleMultiplier, 0);
}