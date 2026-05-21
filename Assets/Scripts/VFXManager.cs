using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private GameObject doubleRingMarker;
    public GameObject DoubleRingMarker { get {  return doubleRingMarker; } }

    [SerializeField] private GameObject[] magicVFX;
    public GameObject[] MagicVFX { get { return magicVFX; } }

    [SerializeField] private MagicData[] magicData;
    public MagicData[] MagicData { get { return magicData; } }

    public static VFXManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        MyActions.onLoadMagic += LoadMagic;
        MyActions.onShootMagic += ShootMagic;
        MyActions.onCreateMagic += CreateMagic;
    }

    private void OnDisable()
    {
        MyActions.onLoadMagic -= LoadMagic;
        MyActions.onShootMagic -= ShootMagic;
        MyActions.onCreateMagic -= CreateMagic;
    }

    public Magic CreateMagic(int id)
    {
        return new Magic(MagicData[id]);
    }

    public void LoadMagic(int id, Vector3 posA, float time)
    {
        if (magicVFX[id] == null) return;

        GameObject objLoad = Instantiate(magicVFX[id], posA, Quaternion.identity);
        Destroy(objLoad, time);
    }

    public void ShootMagic(int id, Vector3 posA, Vector3 posB, float time)
    {
        if (magicVFX[id] == null) return;

        GameObject objShoot = Instantiate(magicVFX[id], posA, Quaternion.identity);
        objShoot.transform.position = Vector3.LerpUnclamped(posA, posB, time);
        Destroy(objShoot, time);
    }
}
