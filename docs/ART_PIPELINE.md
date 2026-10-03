# Pipeline Blender → Unity

## 1. Modello (Tripo → Blender)
1. Claude elenca i modelli da generare con prompt e vista di riferimento (T-pose o A-pose, frontale).
2. Davide li genera su Tripo Studio e li importa in Blender.
3. Con l'ok di Davide, Claude via Blender MCP:
   - controlla scala (altezza personaggio ~1.6-1.9 m), origine ai piedi, applica le trasformazioni;
   - pulisce la mesh (doppi, normali), riduce i poligoni se serve (personaggio: 15-40k tris);
   - controlla UV e texture.

## 2. Rig
- Scheletro umanoide standard (compatibile Mixamo/Humanoid) + catene extra:
  - `Tail_01..Tail_05`, `Ear_L_01..02`, `Ear_R_01..02`, eventuali `Scarf_01..`, `Hair_01..`.
- Gambe digitigrade: tieni lo scheletro umanoide standard e modella la forma digitigrada nella mesh. Il retarget resta semplice.
- Skinning: controlla le deformazioni su spalle, fianchi e base della coda.

## 3. Animazioni (~10 clip)
| Clip | Loop | Note |
|---|---|---|
| Idle | sì | respiro leggero |
| Run | sì | velocità di riferimento ~8 m/s |
| Jump_Up | no | breve |
| Jump_Apex | sì | posa sospesa |
| Fall | sì | |
| Land | no | molto breve, lo squash lo fa lo script |
| Dash | no | posa allungata |
| WallSlide | sì | |
| Hit | no | |
| Death | no | |

Fonti: mocap (es. Mixamo) retargettato, poi ritocco di pose e tempi in Blender. Coda e orecchie **non animarle**: le muove `SpringBoneChain`.

## 4. Export FBX da Blender
- Selezione: armature + mesh.
- *Apply Scalings*: **FBX All**. *Forward*: **-Z Forward**, *Up*: **Y Up**. *Apply Transform*: attivo.
- Armature: *Add Leaf Bones* **disattivato**, *Primary Bone Axis* Y, *Secondary* X.
- Animazioni: *Bake Animation* attivo, una action per clip (NLA o action separate).
- Cartella: `Assets/_Project/Art/Characters/<Nome>/`.

## 5. Import in Unity
- Rig: **Humanoid**, controlla il mapping in *Configure*.
- Clip: imposta loop come in tabella, *Root Transform* bloccato (il movimento lo fa `PlayerMotor`).
- Le ossa extra (coda, orecchie) non sono guidate dalle animazioni Humanoid: sono gestite da `SpringBoneChain`.
- Prefab: `Player` (root con `CharacterController` + `PlayerMotor`) → figlio `Visual` (modello + `Animator` + `PlayerVisuals`).

## 6. Parametri Animator (letti da `PlayerVisuals`)
- `Speed` (float 0-1), `VelocityY` (float), `Grounded` (bool), `WallSlide` (bool)
- Trigger: `Jump`, `Land`, `Dash`
I parametri mancanti vengono ignorati, quindi puoi aggiungerli man mano.
