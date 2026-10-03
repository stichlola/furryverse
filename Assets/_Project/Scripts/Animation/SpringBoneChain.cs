using System.Collections.Generic;
using UnityEngine;

namespace Project.Animation
{
    /// <summary>
    /// Fisica secondaria per catene di ossa (coda, orecchie, capelli, sciarpa).
    /// Assegna il primo osso della catena a <see cref="root"/>: le ossa figlie vengono trovate da sole.
    /// </summary>
    public class SpringBoneChain : MonoBehaviour
    {
        [System.Serializable]
        public struct SpringCollider
        {
            public Transform center;
            public float radius;
        }

        [Tooltip("Primo osso della catena (es. Tail_01).")]
        public Transform root;

        [Header("Comportamento")]
        [Range(0f, 1f), Tooltip("Quanto la catena torna alla posa originale.")]
        public float stiffness = 0.08f;
        [Range(0f, 1f), Tooltip("Smorzamento: alto = movimento morbido e lento.")]
        public float damping = 0.12f;
        public Vector3 gravity = new Vector3(0f, -2f, 0f);
        [Tooltip("Lunghezza dell'ultima punta se l'ultimo osso non ha figli.")]
        public float endLength = 0.1f;
        [Tooltip("Attiva se le ossa della catena hanno anche animazioni proprie.")]
        public bool bonesAreAnimated = false;

        [Header("Collisioni (opzionali)")]
        public List<SpringCollider> colliders = new List<SpringCollider>();

        class Node
        {
            public Transform bone;
            public Quaternion restLocalRot;
            public Vector3 tipLocal;   // punta dell'osso nello spazio locale dell'osso
            public float length;       // lunghezza in world
            public Vector3 tip, prevTip;
        }

        readonly List<Node> nodes = new List<Node>();

        void Awake() => Build();
        void OnEnable() => ResetChain();

        void Build()
        {
            nodes.Clear();
            if (root == null) return;

            Transform t = root;
            while (t != null)
            {
                Transform child = t.childCount > 0 ? t.GetChild(0) : null;
                var n = new Node { bone = t, restLocalRot = t.localRotation };

                if (child != null)
                {
                    n.tipLocal = child.localPosition;
                    n.length = Vector3.Distance(t.position, child.position);
                }
                else
                {
                    // Punta virtuale lungo la direzione genitore → osso
                    Vector3 dirWorld = t.parent != null ? (t.position - t.parent.position).normalized : t.up;
                    n.tipLocal = t.InverseTransformDirection(dirWorld) * endLength / Mathf.Max(0.0001f, t.lossyScale.x);
                    n.length = endLength;
                }

                nodes.Add(n);
                t = child;
            }
            ResetChain();
        }

        /// <summary>Chiamalo dopo un teletrasporto per evitare "frustate".</summary>
        public void ResetChain()
        {
            foreach (var n in nodes)
            {
                n.tip = n.bone.TransformPoint(n.tipLocal);
                n.prevTip = n.tip;
            }
        }

        void LateUpdate()
        {
            if (nodes.Count == 0) return;
            float dt = Mathf.Min(Time.deltaTime, 1f / 30f);
            if (dt <= 0f) return;

            // Fattori indipendenti dal framerate (tarati su 60 fps)
            float k = 1f - Mathf.Pow(1f - stiffness, dt * 60f);
            float keep = Mathf.Pow(1f - damping, dt * 60f);

            foreach (var n in nodes)
            {
                if (!bonesAreAnimated) n.bone.localRotation = n.restLocalRot;

                Vector3 origin = n.bone.position;
                Vector3 restTip = n.bone.TransformPoint(n.tipLocal);

                // Verlet: inerzia + ritorno alla posa + gravità
                Vector3 velocity = (n.tip - n.prevTip) * keep;
                Vector3 next = n.tip + velocity + (restTip - n.tip) * k + gravity * (dt * dt);

                // Collisioni con sfere (es. corpo, gambe)
                foreach (var c in colliders)
                {
                    if (c.center == null) continue;
                    Vector3 d = next - c.center.position;
                    float r = c.radius;
                    if (d.sqrMagnitude < r * r) next = c.center.position + d.normalized * r;
                }

                // Mantiene la lunghezza dell'osso
                Vector3 dir = next - origin;
                if (dir.sqrMagnitude < 1e-8f) dir = restTip - origin;
                next = origin + dir.normalized * n.length;

                n.prevTip = n.tip;
                n.tip = next;

                // Ruota l'osso perché punti verso la nuova punta
                Quaternion delta = Quaternion.FromToRotation(restTip - origin, next - origin);
                n.bone.rotation = delta * n.bone.rotation;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.6f, 0.1f);
            foreach (var c in colliders)
                if (c.center != null) Gizmos.DrawWireSphere(c.center.position, c.radius);
            foreach (var n in nodes)
                if (n.bone != null) Gizmos.DrawLine(n.bone.position, n.tip);
        }
#endif
    }
}
